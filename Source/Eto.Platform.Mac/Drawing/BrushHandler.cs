using System;
using Eto.Drawing;

#if OSX
using MonoMac.CoreGraphics;

namespace Eto.Platform.Mac.Drawing
#elif IOS
using MonoTouch.CoreGraphics;

namespace Eto.Platform.iOS.Drawing
#endif
{
	public abstract class BrushHandler : IBrush
	{
		public abstract void Apply (GraphicsHandler graphics);

		public object ControlObject { get { return this; } }

		public void Dispose ()
		{
		}
	}
}

