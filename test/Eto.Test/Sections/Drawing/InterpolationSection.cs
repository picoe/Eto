using System.Reflection;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", "ImageInterpolation")]
	public class InterpolationSection : Scrollable
	{
		public InterpolationSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow(
				new Label { Text = "Default" }, CreateImage(ImageInterpolation.Default),
				new Label { Text = "None" }, CreateImage(ImageInterpolation.None),
				null
			);
			layout.AddRow(
				new Label { Text = "Low" }, CreateImage(ImageInterpolation.Low),
				new Label { Text = "Medium" }, CreateImage(ImageInterpolation.Medium),
				null
			);
			layout.AddRow(
				new Label { Text = "High" }, CreateImage(ImageInterpolation.High)
			);

			layout.Add(null);

			Content = layout;
		}

		Control CreateImage(ImageInterpolation interpolation)
		{
			var image = TestIcons.TestImage;
			var drawable = new Drawable { Size = new Size(250, 160) };

			drawable.Paint += (sender, pe) =>
			{
				pe.Graphics.ImageInterpolation = interpolation;
				pe.Graphics.DrawImage(image, 0, 0, 20, 20);
				pe.Graphics.DrawImage(image, 0, 20, 50, 50);
				pe.Graphics.DrawImage(image, 0, 70, 100, 100);
				pe.Graphics.DrawImage(image, 120, 0, 300, 300);
			};

			return drawable;
		}
	}
}
