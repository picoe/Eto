using Eto.Forms;
using Eto.Drawing;
using ImageView = Eto.Test.Sections.Drawing.DrawableImageView;
using System;

namespace Eto.Test.Sections.Drawing
{
	/// <summary>
	/// We use this class instead of ImageView to test showing the image using the graphics context only
	/// </summary>
	public class DrawableImageView : Drawable
	{
		Image image;
		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				if (image != null)
					this.MinimumSize = image.Size;
			}
		}

		public override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (Image != null)
				e.Graphics.DrawImage(Image, PointF.Empty);
		}

		public DrawableImageView(Generator generator, Image image)
			: base(generator)
		{
			Image = image;
		}
	}

	public class BitmapSection : Scrollable
	{
		public BitmapSection()
			: this(null)
		{
		}

		public BitmapSection(Generator generator)
			: base(generator)
		{
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
			var image = TestIcons.TestImage(Generator);

			return new DrawableImageView(Generator, image);
		}

		Control CreateCustom32()
		{
			var image = new Bitmap(100, 100, PixelFormat.Format32bppRgb, Generator);

			// should always ensure .Dispose() is called when you are done with a Graphics object
			using (var graphics = new Graphics(image))
			{
				graphics.DrawLine(Pens.Blue(Generator), Point.Empty, new Point(image.Size));
				graphics.DrawRectangle(Pens.Blue(Generator), new Rectangle(image.Size - 1));
			}

			return new DrawableImageView(Generator, image);
		}

		Control CreateCustom32Alpha()
		{
			var image = new Bitmap(100, 100, PixelFormat.Format32bppRgba, Generator);

			// should always ensure .Dispose() is called when you are done with a Graphics object
			using (var graphics = new Graphics(image))
			{
				graphics.DrawLine(Pens.Blue(Generator), Point.Empty, new Point(image.Size));
				graphics.DrawRectangle(Pens.Black(Generator), new Rectangle(image.Size - 1));
			}
			return new DrawableImageView(Generator, image);
		}

		Control Cloning()
		{
			var image = TestIcons.TestImage(Generator);
			image = image.Clone();
			return new DrawableImageView(Generator, image);
		}

		Control CloningRectangle()
		{
			var image = TestIcons.TestImage(Generator);
			image = image.Clone(new Rectangle(32, 32, 64, 64));
			return new DrawableImageView(Generator, image);
		}
	}
}
