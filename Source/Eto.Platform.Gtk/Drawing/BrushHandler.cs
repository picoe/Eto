using System;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Drawing
{
	public abstract class BrushHandler : IBrush
	{
		public object ControlObject { get { return this; } }

		public abstract void Apply (GraphicsHandler graphics);

		public void Dispose ()
		{
		}
	}
}

