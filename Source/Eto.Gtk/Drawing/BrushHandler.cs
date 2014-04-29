using Eto.Drawing;

namespace Eto.GtkSharp.Drawing
{
	/// <summary>
	/// Handler for the <see cref="IBrush"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class BrushHandler : IBrush
	{
		public abstract void Apply (object control, GraphicsHandler graphics);
	}
}

