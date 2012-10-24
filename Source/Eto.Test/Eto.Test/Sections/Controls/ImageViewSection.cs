using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Test.Sections.Controls
{
	public class ImageViewSection : Scrollable
	{
		public ImageViewSection ()
		{
			var layout = new DynamicLayout (this);

			layout.BeginHorizontal ();
			layout.Add (null, xscale: false);
			layout.Add (new Label { Text = "Bitmap", HorizontalAlign = HorizontalAlign.Center }, xscale: true);
			layout.Add (new Label { Text = "Icon", HorizontalAlign = HorizontalAlign.Center }, xscale: true);
			layout.EndHorizontal ();
			layout.AddRow (new Label { Text = "Auto Sized" }, AutoSized (GetBitmap ()), AutoSized (GetIcon ()));
			layout.AddRow (new Label { Text = "Small Size" }, SmallSize (GetBitmap ()), SmallSize (GetIcon ()));
			layout.AddRow (new Label { Text = "Large Size" }, LargeSize (GetBitmap ()), LargeSize (GetIcon ()));
			layout.AddRow (new Label { Text = "Scaled Size" }, Scaled (GetBitmap ()), Scaled (GetIcon ()));
		}

		Icon GetIcon ()
		{
			return Icon.FromResource ("Eto.Test.TestIcon.ico");
		}

		Bitmap GetBitmap ()
		{
			return Bitmap.FromResource ("Eto.Test.TestImage.png");
		}

		Control AutoSized (Image image)
		{
			return TableLayout.AutoSized(new ImageView { Image = image }, centered: true);
		}

		Control LargeSize (Image image)
		{
			return new ImageView { Image = image, Size = new Size (200, 200) };
		}

		Control SmallSize (Image image)
		{
			return new ImageView { Image = image, Size = new Size (64, 64) };
		}

		Control Scaled (Image image)
		{
			return new ImageView { Image = image };
		}

	}
}
