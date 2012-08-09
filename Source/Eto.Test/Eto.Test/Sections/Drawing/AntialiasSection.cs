using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	public class AntialiasSection : Panel
	{
		public AntialiasSection ()
		{
			var layout = new DynamicLayout (this);

			layout.AddRow (
				new Label { Text = "Antialias On" }, AntialiasOn (),
				new Label { Text = "Antialias Off" }, AntialiasOff (), 
				null
			);

			layout.Add (null);
		}

		Control AntialiasOn ()
		{
			var control = new Drawable { Size = new Size (100, 100), BackgroundColor = Colors.Black };

			var path = CreatePath ();
			control.Paint += (sender, e) => {
				e.Graphics.Antialias = true;
				e.Graphics.DrawPath (Colors.White, path);
			};

			return control;
		}

		Control AntialiasOff ()
		{
			var control = new Drawable { Size = new Size (100, 100), BackgroundColor = Colors.Black };

			var path = CreatePath ();
			control.Paint += (sender, e) => {
				e.Graphics.Antialias = false;
				e.Graphics.DrawPath (Colors.White, path);
			};

			return control;
		}

		GraphicsPath CreatePath ()
		{
			var path = new GraphicsPath ();
			path.MoveTo (new Point (10, 10));
			path.LineTo (new Point (20, 90));
			path.LineTo (new Point (10, 60));
			path.LineTo (new Point (90, 80));
			path.LineTo (new Point (60, 30));
			return path;
		}

	}
}
