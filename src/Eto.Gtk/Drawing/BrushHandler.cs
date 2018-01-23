using Eto.Drawing;

namespace Eto.GtkSharp.Drawing
{
	/// <summary>
	/// Handler for the <see cref="Brush.IHandler"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class BrushHandler : Brush.IHandler
	{
		public abstract void Apply (object control, GraphicsHandler graphics);
	}
}

