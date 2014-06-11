using Eto.Forms;
using Eto.Drawing;
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
					MinimumSize = image.Size;
				if (Loaded)
					Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (Image != null)
				e.Graphics.DrawImage(Image, PointF.Empty);
		}
	}

	[Section("Drawing", "Bitmap")]
	public class BitmapSection : Scrollable
	{
		public BitmapSection()
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

			layout.AddRow(
				new Label { Text = "Clone using tiles" }, TableLayout.AutoSized(CloneTiles(), centered: true),
				new Label { Text = "Draw to a rect" }, TableLayout.AutoSized(DrawImageToRect(), centered: true),
				null);

			layout.Add(null);

			Content = layout;
		}

		Control LoadFromStream()
		{
			var image = TestIcons.TestImage;

			return new DrawableImageView { Image = image };
		}

		Control CreateCustom32()
		{
			var image = new Bitmap(100, 100, PixelFormat.Format32bppRgb);

			// should always ensure .Dispose() is called when you are done with a Graphics object
			using (var graphics = new Graphics(image))
			{
				graphics.DrawLine(Pens.Blue, Point.Empty, new Point(image.Size));
				graphics.DrawRectangle(Pens.Blue, new Rectangle(image.Size - 1));
			}

			return new DrawableImageView { Image = image };
		}

		Control CreateCustom32Alpha()
		{
			var image = new Bitmap(100, 100, PixelFormat.Format32bppRgba);

			// should always ensure .Dispose() is called when you are done with a Graphics object
			using (var graphics = new Graphics(image))
			{
				graphics.DrawLine(Pens.Blue, Point.Empty, new Point(image.Size));
				graphics.DrawRectangle(Pens.Black, new Rectangle(image.Size - 1));
			}
			return new DrawableImageView { Image = image };
		}

		Control Cloning()
		{
			var image = TestIcons.TestImage;
			image = image.Clone();
			return new DrawableImageView { Image = image };
		}

		Control CloningRectangle()
		{
			var image = TestIcons.TestImage;
			image = image.Clone(new Rectangle(32, 32, 64, 64));
			return new DrawableImageView { Image = image };
		}

		Control CloneTiles()
		{
			// Creates a duplicate of the bitmap by cloning tiles of it
			// and drawing them in the same location in the duplicate.
			var image = TestIcons.TestImage;
			var bitmap = new Bitmap(new Size(image.Size), PixelFormat.Format32bppRgba);
			var tile = 64; // the test image is 128x128 so this produces 4 tiles.
			using (var g = new Graphics(bitmap))
			{
				for (var x = 0; x < image.Width; x += tile)
					for (var y = 0; y < image.Height; y += tile)
					{
						var clone = image.Clone(new Rectangle(x, y, tile, tile));
						g.DrawImage(clone, x, y);
					}
			}
			return new DrawableImageView { Image = bitmap };
		}

		Control DrawImageToRect()
		{
			var image64 = TestIcons.Textures;

			var bitmap = new Bitmap(new Size(105, 105), PixelFormat.Format32bppRgba);
			using (var g = new Graphics(bitmap))
			{
				// Draw the "5" portion of the texture at a smaller size at the origin.
				g.DrawImage(image64, new RectangleF(80, 80, 80, 80), new RectangleF(0, 0, 32, 32));
				// draw two rulers to indicate how big the green image should be
				g.SaveTransform();
				g.MultiplyTransform(Matrix.Create(1, 0, 0, 1, 0, 70));
				DrawRuler(32, g, Colors.Blue);
				g.RestoreTransform();

				g.SaveTransform();
				g.MultiplyTransform(Matrix.Create(0, 1, 1, 0, 70, 0));
				DrawRuler(32, g, Colors.Blue);
				g.RestoreTransform();
			}
			return new DrawableImageView { Image = bitmap };
		}

		/// <summary>
		/// Draws a unit-length horizontal ruler from (0, 0) to (1, 0) into the graphics context.
		/// Scale and rotate as needed before calling.
		/// </summary>
		/// <param name="g"></param>
		static void DrawRuler(float length, Graphics g, Color color)
		{
			g.DrawLine(color, 0, 0, length, 0);
			var capHeight = 2;
			g.DrawLine(color, 0, -capHeight, 0, capHeight);
			g.DrawLine(color, length, -capHeight, length, capHeight);
		}
	}
}