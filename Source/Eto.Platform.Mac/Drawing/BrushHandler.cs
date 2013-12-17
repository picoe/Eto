using Eto.Drawing;

#if OSX

namespace Eto.Platform.Mac.Drawing
#elif IOS

namespace Eto.Platform.iOS.Drawing
#endif
{
	/// <summary>
	/// Handler for <see cref="IBrush"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class BrushHandler : IBrush
	{
		public abstract void Apply (object control, GraphicsHandler graphics);
	}
}

