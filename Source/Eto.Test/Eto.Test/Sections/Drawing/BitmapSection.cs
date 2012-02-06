using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Test.Controls;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	public class BitmapSection : SectionBase
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
			var resourceStream = Resources.GetResource("Eto.Test.TestImage.png");

			var image = new Bitmap (resourceStream);

			return new ImageView { Image = image };
		}

		Control CreateCustom32 ()
		{
			var image = new Bitmap (100, 100, PixelFormat.Format32bppRgb);

			var graphics = new Graphics (image);
			graphics.DrawLine (Color.Blue, Point.Empty, new Point (image.Size));
			graphics.DrawRectangle (Color.Blue, new Rectangle (image.Size));
			graphics.Flush ();

			return new ImageView { Image = image };
		}

		Control CreateCustom32Alpha ()
		{
			var image = new Bitmap (100, 100, PixelFormat.Format32bppRgba);

			var graphics = new Graphics (image);
			graphics.DrawLine (Color.Blue, Point.Empty, new Point (image.Size));
			graphics.DrawRectangle (Color.Black, new Rectangle (image.Size));
			graphics.Flush ();

			return new ImageView { Image = image };
		}

	}
}
