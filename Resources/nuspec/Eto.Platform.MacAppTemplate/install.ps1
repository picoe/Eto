param($installPath, $toolsPath, $package, $project)

$project.Properties.Item("PostBuildEvent").Value = '$(ProjectDir)\Mac\buildapp.cmd "$(TargetDir)" "$(TargetFileName)" "$(TargetName)"'