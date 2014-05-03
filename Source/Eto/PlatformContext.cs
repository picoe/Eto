using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto
{
	/// <summary>
	/// Sets the current generator for a block of code,
	/// and verifies that all objects created in that 
	/// block were created using that generator.
	/// </summary>
	internal class PlatformContext : IDisposable
	{
		readonly Platform previous;
		readonly Platform previousValidate;

		public PlatformContext(Platform platform)
		{
			previous = Platform.Instance;
			previousValidate = Platform.ValidatePlatform;
			Platform.Initialize(platform);
			Eto.Platform.ValidatePlatform = platform;
		}

		public void Dispose()
		{
			Platform.Initialize(previous);
			Platform.ValidatePlatform = previousValidate;
		}
	}
}
