using System;
using System.Linq;

namespace Eto.Platform.Android
{
	public class EtoEnvironmentHandler : WidgetHandler<Widget>, IEtoEnvironment
	{
		public string GetFolderPath (EtoSpecialFolder folder)
		{
			throw new NotImplementedException();
		}

		public OperatingSystemPlatform GetPlatform()
		{
			return new OperatingSystemPlatform
			{
				IsAndroid = true,
				IsLinux = true,
				IsUnix = true,
				IsMono = true,				
			};
		}
	}
}

