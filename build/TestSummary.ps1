<#
.SYNOPSIS
    Renders trx test results into a GitHub Actions job summary.

.DESCRIPTION
    Reads the trx report(s) written by the unit test run and appends a markdown summary to
    $env:GITHUB_STEP_SUMMARY - a totals table plus a collapsible entry per failed test - so
    results can be read from the run page without digging through the raw job log. Failures are
    also emitted as workflow error annotations, which surface them on the PR diff when the trx
    stack trace points at a file in the repository.

    A summary is always written, even when a report is missing or unreadable (a test host killed
    mid-write leaves an empty or truncated trx) - the reason is reported in place of the results.

    Writes to stdout when run outside of Actions, so it can be tested locally:
        pwsh build/TestSummary.ps1 -Title "Unit tests (Mac)"
#>
[CmdletBinding()]
param(
	# Heading for this run, e.g. "Unit tests (Wpf)".
	[Parameter(Mandatory)][string]$Title,
	# Directory searched (recursively) for trx reports.
	[string]$ResultsRoot = 'artifacts/test',
	# Name/pattern of the trx report(s) to read.
	[string]$FileName = '*.trx',
	# Cap the per-test detail so a badly broken run can't blow the 1MB summary limit.
	[int]$MaxFailures = 50,
	# GitHub only displays the first handful of annotations per step, so don't bother emitting more.
	[int]$MaxAnnotations = 10
)

$ErrorActionPreference = 'Stop'

$summary = [System.Text.StringBuilder]::new()
function Add-Line([string]$text = '') { [void]$summary.AppendLine($text) }

# Fenced code blocks are used for failure details, so make sure the content can't end one early.
function Format-Block([string]$text)
{
	if ([string]::IsNullOrWhiteSpace($text)) { return $null }
	return $text.Replace('```', "'''").TrimEnd()
}

$files = @(Get-ChildItem -Path $ResultsRoot -Recurse -Filter $FileName -File -ErrorAction SilentlyContinue)

Add-Line "## $Title"
Add-Line

# Everything is buffered and only written to $GITHUB_STEP_SUMMARY at the very end, so any error in
# between would otherwise produce no summary at all - the one case where the reader most needs one.
# Reporting the error into the summary (and still failing the step) keeps it self-diagnosing.
try
{

if ($files.Count -eq 0)
{
	# No report at all means the host never wrote one: it crashed, was killed by the step timeout, or
	# was terminated during shutdown before the trx was flushed (a run whose tests all completed can
	# still end up here). Worth calling out - the totals table would otherwise be missing with no reason.
	Add-Line '> [!WARNING]'
	Add-Line "> No test results found under ``$ResultsRoot`` - the test host didn't write a report. It may have crashed, hit the step timeout, or been terminated during shutdown before flushing the trx. See the job log."
	Write-Output "::warning title=$Title::No test results found, the test host didn't write a report (crash, timeout, or terminated during shutdown)"
}
else
{
	$ns = @{ t = 'http://microsoft.com/schemas/VisualStudio/TeamTest/2010' }
	$total = 0; $passed = 0; $failed = 0; $skipped = 0
	$seconds = 0.0
	$failures = [System.Collections.Generic.List[object]]::new()
	$unreadable = [System.Collections.Generic.List[object]]::new()

	foreach ($file in $files)
	{
		# A trx from a host that died mid-write is empty or truncated, so it can't be parsed. Record it
		# and carry on rather than letting the run's totals disappear along with it.
		try
		{
			# An empty file reads back as $null rather than throwing, so check before casting.
			$content = Get-Content -LiteralPath $file.FullName -Raw
			if ([string]::IsNullOrWhiteSpace($content)) { throw 'The file is empty.' }
			$xml = [xml]$content
		}
		catch
		{
			# The parse error quotes the file content, so flatten it - it goes in a single-line blockquote.
			$unreadable.Add([pscustomobject]@{ Name = $file.Name; Reason = ($_.Exception.Message -replace '\s+', ' ').Trim() })
			continue
		}

		$counters = Select-Xml -Xml $xml -Namespace $ns -XPath '/t:TestRun/t:ResultSummary/t:Counters' | Select-Object -First 1
		if ($counters)
		{
			$c = $counters.Node
			$total += [int]$c.total
			$passed += [int]$c.passed
			$failed += [int]$c.failed
			# trx splits "didn't run" across several counters; for a summary they're all just skipped.
			$skipped += [int]$c.notExecuted + [int]$c.inconclusive + [int]$c.notRunnable
		}

		$times = Select-Xml -Xml $xml -Namespace $ns -XPath '/t:TestRun/t:Times' | Select-Object -First 1
		if ($times -and $times.Node.start -and $times.Node.finish)
		{
			$seconds += ([datetime]$times.Node.finish - [datetime]$times.Node.start).TotalSeconds
		}

		# testName in the results is only the method (with its test case arguments), so pull the
		# class name from the definitions to make each failure identifiable on its own.
		$classNames = @{}
		foreach ($def in (Select-Xml -Xml $xml -Namespace $ns -XPath '//t:TestDefinitions/t:UnitTest'))
		{
			$method = $def.Node.TestMethod
			if ($method) { $classNames[$def.Node.id] = $method.className }
		}

		foreach ($result in (Select-Xml -Xml $xml -Namespace $ns -XPath "//t:UnitTestResult[@outcome='Failed']"))
		{
			$node = $result.Node
			$class = $classNames[$node.testId]
			$failures.Add([pscustomobject]@{
					Name       = if ($class) { "$class.$($node.testName)" } else { $node.testName }
					Message    = Format-Block $node.Output.ErrorInfo.Message
					StackTrace = Format-Block $node.Output.ErrorInfo.StackTrace
				})
		}
	}

	$outcome = if ($failed -gt 0) { "❌ $failed failed" } elseif ($unreadable.Count -gt 0) { '⚠️ incomplete' } elseif ($total -eq 0) { '⚠️ no tests ran' } else { '✅ passed' }

	foreach ($bad in $unreadable)
	{
		Add-Line '> [!WARNING]'
		Add-Line "> ``$($bad.Name)`` could not be read, so its results are missing from the totals below - the test host was likely terminated while writing it. $($bad.Reason)"
		Add-Line
		Write-Output "::warning title=$Title::$($bad.Name) could not be read, the test host was likely terminated while writing it"
	}

	Add-Line '| Result | Total | Passed | Failed | Skipped | Duration |'
	Add-Line '|:---|---:|---:|---:|---:|---:|'
	Add-Line "| $outcome | $total | $passed | $failed | $skipped | $([math]::Round($seconds, 1))s |"
	Add-Line

	$shown = 0
	foreach ($failure in $failures)
	{
		if ($shown -ge $MaxFailures)
		{
			Add-Line "_...and $($failures.Count - $MaxFailures) more, see the job log._"
			break
		}
		$shown++

		Add-Line "<details><summary>❌ <code>$($failure.Name)</code></summary>"
		Add-Line
		Add-Line '```'
		if ($failure.Message) { Add-Line $failure.Message }
		if ($failure.StackTrace) { Add-Line $failure.StackTrace }
		Add-Line '```'
		Add-Line '</details>'
		Add-Line

		if ($shown -le $MaxAnnotations)
		{
			# Point the annotation at the failing line when the stack trace has one for a file that
			# is still in the workspace, so it lands on the PR diff rather than just the job log.
			$location = ''
			if ($failure.StackTrace -match '\sin\s(?<file>.+):line\s(?<line>\d+)')
			{
				$path = $Matches.file
				$root = if ($env:GITHUB_WORKSPACE) { $env:GITHUB_WORKSPACE } else { (Get-Location).Path }
				if ($path.StartsWith($root, [StringComparison]::OrdinalIgnoreCase))
				{
					$relative = $path.Substring($root.Length).TrimStart('/', '\').Replace('\', '/')
					$location = "file=$relative,line=$($Matches.line),"
				}
			}
			# Annotations are single line, and '::' would be read as another command token.
			$message = ($failure.Message -replace '\s+', ' ').Replace('::', ':').Trim()
			Write-Output "::error $($location)title=$Title::$($failure.Name): $message"
		}
	}
}

}
catch
{
	Add-Line '> [!CAUTION]'
	Add-Line "> The test summary could not be generated: $($_.Exception.Message)"
	Write-Output "::error title=$Title::Test summary failed: $(($_.Exception.Message -replace '\s+', ' ').Replace('::', ':'))"
	throw
}
finally
{
	if ($env:GITHUB_STEP_SUMMARY)
	{
		Add-Content -LiteralPath $env:GITHUB_STEP_SUMMARY -Value $summary.ToString()
	}
	else
	{
		Write-Output $summary.ToString()
	}
}
