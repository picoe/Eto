using Eto.Forms;
using Eto.Drawing;
using ImageView = Eto.Test.Sections.Drawing.DrawableImageView;
using System;

namespace Eto.Test.Sections.Drawing
{
	/// <summary>
	/// We use this class instead of ImageView to
	/// hook into the rendering and supply a different
	/// drawing toolkit.
	/// </summary>
	public class DrawableImageView : Drawable
	{
		DrawingToolkit toolkit;

		Image image;
		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				MinimumSize = image.Size;
			}
		}

		public DrawableImageView(Func<Drawable, DrawingToolkit> createToolkit, Image image)
		{
			toolkit = createToolkit(this);
			Image = image;

			Paint += (s, e) => toolkit.Render(e.Graphics, g =>
			{
				if (Image != null)
					g.DrawImage(Image, PointF.Empty);
			});
		}
	}

	public class BitmapSection : Scrollable
	{
		readonly Func<Drawable, DrawingToolkit> createToolkit;

		public BitmapSection()
			: this(DrawingToolkit.Create)
		{
		}

		public BitmapSection(Func<Drawable, DrawingToolkit> createToolkit)
		{
			this.createToolkit = createToolkit;

			var layout = new DynamicLayout();

			layout.AddRow(new Label { Text = "Load from Stream" }, LoadFromStream());

			layout.AddRow(
				new Label { Text = "Custom 32-bit" }, CreateCustom32(),
				new Label { Text = "Custom 32-bit alpha" }, CreateCustom32Alpha(),
				null
			);

			layout.AddRow(
				new Label { Text = "Clone" }, Cloning(),
				new Label { Text = "Clone rectangle" }, TableLayout.AutoSized(CloningRectangle(), centered: true),
				null);

			layout.Add(null);

			Content = layout;
		}

		Control LoadFromStream()
		{
			var resourceStream = GetType().Assembly.GetManifestResourceStream("Eto.Test.TestImage.png");

			var image = new Bitmap(resourceStream);

			return new DrawableImageView(createToolkit, image);
		}

		Control CreateCustom32()
		{
			var image = new Bitmap(100, 100, PixelFormat.Format32bppRgb);

			// should always ensure .Dispose() is called when you are done with a Graphics object
			using (var graphics = new Graphics (image))
			{
				graphics.DrawLine(Pens.Blue(), Point.Empty, new Point(image.Size));
				graphics.DrawRectangle(Pens.Blue(), new Rectangle(image.Size - 1));
			}

			return new DrawableImageView(createToolkit, image);
		}

		Control CreateCustom32Alpha()
		{
			var image = new Bitmap(100, 100, PixelFormat.Format32bppRgba);

			// should always ensure .Dispose() is called when you are done with a Graphics object
			using (var graphics = new Graphics (image))
			{
				graphics.DrawLine(Pens.Blue(), Point.Empty, new Point(image.Size));
				graphics.DrawRectangle(Pens.Black(), new Rectangle(image.Size - 1));
			}

			return new DrawableImageView(createToolkit, image);
		}

		Control Cloning()
		{
			var image = TestIcons.TestImage;
			image = image.Clone();
			return new DrawableImageView(createToolkit, image);
		}

		Control CloningRectangle()
		{
			var image = TestIcons.TestImage;
			image = image.Clone(new Rectangle(32, 32, 64, 64));
			return new DrawableImageView(createToolkit, image);
		}
	}
}
