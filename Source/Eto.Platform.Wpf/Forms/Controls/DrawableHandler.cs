using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class DrawableHandler : WpfPanel<swc.Canvas, Drawable>, IDrawable
	{
		class MyCanvas : swc.Canvas
		{
			public DrawableHandler Handler { get; set; }

			protected override void OnRender (swm.DrawingContext dc)
			{
				var graphics = new Eto.Drawing.Graphics (Handler.Widget.Generator, new Drawing.GraphicsHandler (dc));
				var clip = Rectangle.Empty;
				Handler.Widget.OnPaint (new PaintEventArgs (graphics, clip));
			}
		}

		public void Create ()
		{
			Control = new MyCanvas{ Handler = this };
		}

		public void Update (Eto.Drawing.Rectangle rect)
		{
			Control.InvalidateVisual ();
		}

		public bool CanFocus
		{
			get { return Control.Focusable; }
			set { Control.Focusable = value; }
		}
	}
}
