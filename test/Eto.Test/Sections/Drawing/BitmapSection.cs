using Eto.Forms;
using Eto.Drawing;
using System;
using System.Collections.Generic;

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

		public bool ScaleImage { get; set; }

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (Image != null)
			{
				if (ScaleImage)
					e.Graphics.DrawImage(Image, 0, 0, ClientSize.Width, ClientSize.Height);
				else
					e.Graphics.DrawImage(Image, new PointF((ClientSize - Image.Size) / 2));
			}
		}
	}

	[Section("Drawing", "Bitmap")]
	public class BitmapSection : Scrollable
	{
		public BitmapSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow(
				"Load from Stream", LoadFromStream(), 
				"Custom 24-bit", CreateCustom24(),
				null
			);

			layout.AddRow(
				"Custom 32-bit", CreateCustom32(),
				"Custom 32-bit alpha", CreateCustom32Alpha(),
				null
			);

			layout.AddRow(
				"Clone", Cloning(),
				"Clone rectangle", TableLayout.AutoSized(CloneRectangle(), centered: true),
				null);

			layout.AddRow(
				"Draw to a rect", TableLayout.AutoSized(DrawImageToRect(), centered: true)
			);

			layout.AddRow(
				"Scaled", TableLayout.AutoSized(ScaleImage(), centered: true),
				"Scaled ImageView", TableLayout.AutoSized(ScaleImageView(), centered: true)
				);

			layout.Add(null);

			Content = layout;
		}

		Control LoadFromStream()
		{
			var image = TestIcons.TestImage;

			return new DrawableImageView { Image = image };
		}

		void DrawTest(Bitmap image)
		{
			// should always ensure .Dispose() is called when you are done with a Graphics or BitmapData object.

			// Test setting pixels directly
			using (var bd = image.Lock())
			{
				var sz = image.Size / 5;
				for (int x = sz.Width; x < sz.Width * 2; x++)
					for (int y = sz.Height; y < sz.Height * 2; y++)
						bd.SetPixel(x, y, Colors.Green);
			}

			// Test using Graphics object
			using (var graphics = new Graphics(image))
			{
				graphics.DrawLine(Pens.Blue, Point.Empty, new Point(image.Size));
				graphics.DrawRectangle(Pens.Blue, new Rectangle(image.Size - 1));
			}

			// should be able to set pixels after using graphics object
			using (var bd = image.Lock())
			{
				var sz = image.Size / 5;
				for (int x = sz.Width * 3; x < sz.Width * 4; x++)
					for (int y = sz.Height * 3; y < sz.Height * 4; y++)
						bd.SetPixel(x, y, Colors.Red);
			}

		}

		Control CreateCustom24()
		{
			var size = new Size(100, 100) * (int)Screen.PrimaryScreen.LogicalPixelSize;
			var image = new Bitmap(size, PixelFormat.Format24bppRgb);

			DrawTest(image);

			return new DrawableImageView { Image = new Icon(Screen.PrimaryScreen.LogicalPixelSize, image) };
		}

		Control CreateCustom32()
		{
			var size = new Size(100, 100) * (int)Screen.PrimaryScreen.LogicalPixelSize;
			var image = new Bitmap(size, PixelFormat.Format32bppRgb);

			DrawTest(image);

			return new DrawableImageView { Image = new Icon(Screen.PrimaryScreen.LogicalPixelSize, image) };
		}

		Control CreateCustom32Alpha()
		{
			var size = new Size(100, 100) * (int)Screen.PrimaryScreen.LogicalPixelSize;
			var image = new Bitmap(size, PixelFormat.Format32bppRgba);

			DrawTest(image);
			return new DrawableImageView { Image = new Icon(Screen.PrimaryScreen.LogicalPixelSize, image) };
		}

		Control Cloning()
		{
			var image = TestIcons.TestImage;
			image = image.Clone();
			return new DrawableImageView { Image = new Icon(Screen.PrimaryScreen.LogicalPixelSize, image) };
		}

		IEnumerable<Rectangle> GetTiles(Image image)
		{
			yield return new Rectangle(0, 0, 32, image.Height);
			yield return new Rectangle(32, 0, image.Width - 32, 32);
			yield return new Rectangle(32, 32, 64, 64);
			yield return new Rectangle(image.Width - 32, 32, 32, image.Height - 64);
			yield return new Rectangle(32, 96, 64, image.Height - 96);
			yield return new Rectangle(image.Width - 32, 96, 32, image.Height - 96);
		}

		Control CloneRectangle()
		{
			var image = TestIcons.TestImage;
			var bitmap = new Bitmap(image.Size, PixelFormat.Format32bppRgba);
			using (var g = new Graphics(bitmap))
			{
				foreach (var tile in GetTiles(image))
				{
					using (var clone = image.Clone(tile))
						g.DrawImage(clone, tile.X, tile.Y);
				}
			}
			return new DrawableImageView { Image = new Icon(Screen.PrimaryScreen.LogicalPixelSize, bitmap) };
		}

		Control DrawImageToRect()
		{
			var image64 = TestIcons.Textures;

			var size = new Size(105, 105) * (int)Screen.PrimaryScreen.LogicalPixelSize;
			var bitmap = new Bitmap(size, PixelFormat.Format32bppRgba);
			using (var g = new Graphics(bitmap))
			{
				g.PixelOffsetMode = PixelOffsetMode.Half;
				g.ScaleTransform(Screen.PrimaryScreen.LogicalPixelSize, Screen.PrimaryScreen.LogicalPixelSize);

				// Draw the "5" portion of the texture at a smaller size at the origin.
				g.DrawImage(image64, new RectangleF(80, 80, 80, 80), new RectangleF(0, 0, 32, 32));
				// draw two rulers to indicate how big the green image should be
				g.SaveTransform();
				g.MultiplyTransform(Matrix.Create(1, 0, 0, 1, 0.5f, 70.5f));
				DrawRuler(32, g, Colors.Blue);
				g.RestoreTransform();

				g.SaveTransform();
				g.MultiplyTransform(Matrix.Create(0, 1, 1, 0, 70.5f, 0.5f));
				DrawRuler(32, g, Colors.Blue);
				g.RestoreTransform();
			}
			return new DrawableImageView { Image = new Icon(Screen.PrimaryScreen.LogicalPixelSize, bitmap) };
		}

		Control ScaleImage()
		{
			var image = TestIcons.TestImage;
			image = new Bitmap(image, 32, 32);
			return new DrawableImageView { Image = image };
		}

		Control ScaleImageView()
		{
			var image = TestIcons.TestImage;
			image = new Bitmap(image, 32, 32);
			return new ImageView { Image = image };
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