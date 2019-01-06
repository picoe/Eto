#if !NETSTANDARD1_0
using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Resources;

public class GenerateVersion : Task
{
	public GenerateVersion() : base() { }
	public GenerateVersion(ResourceManager taskResources) : base(taskResources) { }
	public GenerateVersion(ResourceManager taskResources, string helpKeywordPrefix) : base(taskResources, helpKeywordPrefix) { }

	[Output]
	public string Version { get; set; }

	public override bool Execute()
	{
		var currentDate = DateTime.Now;
		var timeSpanDays = currentDate - new DateTime(2000, 01, 01);
		var timeSpanSeconds = currentDate - DateTime.Today;

		var numberOfDays = (int)timeSpanDays.TotalDays;
		var numberOfSeconds = (int)(timeSpanSeconds.TotalSeconds / 2);
		var versionString = Version.Replace(".*", "");
			
		var version = System.Version.Parse(versionString);
			
		Version = FormattableString.Invariant(
			$"{version.Major}.{version.Minor}.{numberOfDays}.{numberOfSeconds}"
			);
		
		return true;
	}
}
#endif // !NETSTANDARD1_0