using Eto.Drawing;

#if OSX

namespace Eto.Mac.Drawing
#elif IOS

namespace Eto.iOS.Drawing
#endif
{
	/// <summary>
	/// Handler for <see cref="Brush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class BrushHandler : Brush.IHandler
	{
		public abstract void Draw(object control, GraphicsHandler graphics, bool stroke, FillMode fillMode, bool clip);
	}
}

