using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	public class InterpolationSection : Scrollable
	{
		public InterpolationSection ()
		{
			var layout = new DynamicLayout (this);

			layout.AddRow (
				new Label { Text = "Default" }, CreateImage (ImageInterpolation.Default),
				new Label { Text = "None" }, CreateImage (ImageInterpolation.None),
				null
				);
			layout.AddRow (
				new Label { Text = "Low" }, CreateImage (ImageInterpolation.Low),
				new Label { Text = "Medium" }, CreateImage (ImageInterpolation.Medium),
				null
				);
			layout.AddRow (
				new Label { Text = "High" }, CreateImage (ImageInterpolation.High)
				);

			layout.Add (null);
		}

		Control CreateImage (ImageInterpolation interpolation)
		{
			var resourceStream = GetType().Assembly.GetManifestResourceStream ("Eto.Test.TestImage.png");

			var image = new Bitmap (resourceStream);
			var drawable = new Drawable { Size = new Size(250, 160) };

			drawable.Paint += (sender, pe) => {
				pe.Graphics.ImageInterpolation = interpolation;
				pe.Graphics.DrawImage (image, 0, 0, 20, 20);
				pe.Graphics.DrawImage (image, 0, 20, 50, 50);
				pe.Graphics.DrawImage (image, 0, 70, 100, 100);
				pe.Graphics.DrawImage (image, 120, 0, 300, 300);
			};

			return drawable;
		}
	}
}
