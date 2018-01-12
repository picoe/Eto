using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", "Clip Transforms")]
	public class ClipTransformSection : Panel
	{
		public ClipTransformSection()
		{
			Padding = new Padding(10);
			var drawable = new Drawable();

			bool showInvalid = false;
			var showInvalidCheckBox = new CheckBox { Text = "Show Invalid Regions" };
			showInvalidCheckBox.CheckedBinding.Bind(() => showInvalid, v => { showInvalid = v ?? false; drawable.Invalidate(); });

			drawable.Paint += (sender, e) =>
			{
				var g = e.Graphics;

				var rect = new RectangleF(drawable.ClientSize);
				Func<RectangleF> boundsRect = () => g.CurrentTransform.Inverse().TransformRectangle(rect);

				g.SaveTransform();

				g.SetClip(new RectangleF(0, 0, 50, 50));
				g.FillRectangle(Colors.Red, rect);

				g.TranslateTransform(50, 0);
				if (!showInvalid)
					g.FillRectangle(Colors.Green, boundsRect());

				g.TranslateTransform(0, 50);
				g.SetClip(new RectangleF(0, 0, 50, 50));
				g.FillRectangle(Colors.Red, boundsRect());

				g.ScaleTransform(2, 2);
				g.TranslateTransform(25, 25);
				if (!showInvalid)
					g.FillRectangle(Colors.Green, boundsRect());

				g.SetClip(GraphicsPath.GetRoundRect(new RectangleF(0, 0, 25, 25), 4));
				g.FillRectangle(Colors.Red, boundsRect());

				g.ScaleTransform(2, 2);
				g.TranslateTransform(25, 25);
				if (!showInvalid)
					g.FillRectangle(Colors.Green, boundsRect());

				g.SetClip(new RectangleF(-25/2f, -25/2f, 25/2f, 25/2f));
				g.FillRectangle(Colors.Red, boundsRect());

				g.RestoreTransform();

				if (!showInvalid)
					g.FillRectangle(Colors.Green, boundsRect());
			};

			var options = new StackLayout
			{
				Orientation = Orientation.Horizontal,
				VerticalContentAlignment = VerticalAlignment.Center,
				Spacing = 5,
				Items = { showInvalidCheckBox }
			};

			Content = new StackLayout {
				Spacing = 5,
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Items = { options, new StackLayoutItem(drawable, expand: true) }
			};

		}
	}
}

