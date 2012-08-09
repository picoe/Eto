using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	public class BitmapSection : Panel
	{
		public BitmapSection ()
		{
			var layout = new DynamicLayout (this);

			layout.AddRow (new Label { Text = "Load from Stream" }, LoadFromStream ());

			layout.AddRow (
				new Label { Text = "Custom 32-bit" }, CreateCustom32 (),
				new Label { Text = "Custom 32-bit alpha" }, CreateCustom32Alpha (),
				null
				);

			layout.Add (null);
		}

		Control LoadFromStream ()
		{
			var resourceStream = GetType().Assembly.GetManifestResourceStream ("Eto.Test.TestImage.png");

			var image = new Bitmap (resourceStream);

			return new ImageView { Image = image };
		}

		Control CreateCustom32 ()
		{
			var image = new Bitmap (100, 100, PixelFormat.Format32bppRgb);

			// should always ensure .Dispose() is called when you are done with a Graphics object
			using (var graphics = new Graphics (image)) {
				graphics.DrawLine (Colors.Blue, Point.Empty, new Point (image.Size));
				graphics.DrawRectangle (Colors.Blue, new Rectangle (image.Size));
			}

			return new ImageView { Image = image };
		}

		Control CreateCustom32Alpha ()
		{
			var image = new Bitmap (100, 100, PixelFormat.Format32bppRgba);

			// should always ensure .Dispose() is called when you are done with a Graphics object
			using (var graphics = new Graphics (image)) {
				graphics.DrawLine (Colors.Blue, Point.Empty, new Point (image.Size));
				graphics.DrawRectangle (Colors.Black, new Rectangle (image.Size));
			}

			return new ImageView { Image = image };
		}

	}
}
