using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "OperatingSystemPlatform")]
    public class OperatingSystemPlatformSection : Scrollable
    {
        public OperatingSystemPlatformSection()
		{
			var platform = EtoEnvironment.Platform;
			
			var layout = new DynamicLayout
			{
				DefaultSpacing = new Size(10, 10)
			};
			layout.BeginCentered(yscale: true, xscale: true);

			layout.Add(new Label { Text = "OperatingSystemPlatform", Font = SystemFonts.Bold(SystemFonts.Bold().Size + 2) });
			
			layout.BeginVertical();
			layout.AddRow("IsNetCore:", platform.IsNetCore.ToString());
			layout.AddRow("IsNetFramework:", platform.IsNetFramework.ToString());
			layout.AddRow("IsMono:", platform.IsMono.ToString());
			layout.AddRow("IsWinRT:", platform.IsWinRT.ToString());
			layout.AddRow("IsWindows:", platform.IsWindows.ToString());
			layout.AddRow("IsMac:", platform.IsMac.ToString());
			layout.AddRow("IsUnix:", platform.IsUnix.ToString());
			layout.AddRow("IsLinux:", platform.IsLinux.ToString());
			layout.EndVertical();

			layout.Add(new Label { Text = "RuntimeInformation", Font = SystemFonts.Bold(SystemFonts.Bold().Size + 2) });
			layout.BeginVertical();
			layout.AddRow("FrameworkDescription: ", RuntimeInformation.FrameworkDescription);
			layout.AddRow("OSArchitecture: ", RuntimeInformation.OSArchitecture.ToString());
			layout.AddRow("OSDescription: ", RuntimeInformation.OSDescription);
			layout.AddRow("ProcessArchitecture: ", RuntimeInformation.ProcessArchitecture.ToString());
#if NET
			layout.AddRow("RuntimeIdentifier: ", RuntimeInformation.RuntimeIdentifier);
#endif
			layout.EndVertical();

			layout.EndCentered();
			Content = layout;
		}
    }
}