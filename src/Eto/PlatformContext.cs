using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto
{
	/// <summary>
	/// Sets the current generator for a block of code
	/// </summary>
	class PlatformContext : IDisposable
	{
		readonly Platform previous;

		public PlatformContext(Platform platform)
		{
			previous = Platform.Instance;
			Platform.Initialize(platform);
		}

		public void Dispose()
		{
			Platform.Initialize(previous);
		}
	}
}
