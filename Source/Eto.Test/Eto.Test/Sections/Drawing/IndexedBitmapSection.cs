using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", "IndexedBitmap")]
	public class IndexedBitmapSection : Scrollable
	{
		public IndexedBitmapSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow(
				new Label { Text = "Indexed Bitmap on ImageView" }, CreateIndexedImageView(),
				new Label { Text = "Indexed Bitmap on Drawable" }, CreateIndexedDrawable(),
				null
			);

			layout.Add(null);

			Content = layout;
		}

		IndexedBitmap CreateImage()
		{
			var image = new IndexedBitmap(100, 100, 8);
			var ega = Palette.GetEgaPalette();
			var pal = new Palette(ega);

			// must have at least 256 colors for an 8-bit bitmap
			while (pal.Count < 256)
				pal.Add(Colors.Black);
			image.Palette = pal;
			using (var bd = image.Lock())
			{
				unsafe
				{
					var brow = (byte*)bd.Data;
					for (int y = 0; y < image.Size.Height; y++)
					{
						byte* b = brow;
						var col = -y;
						for (int x = 0; x < image.Size.Width; x++)
						{
							while (col < 0)
								col = ega.Count + col;
							while (col >= ega.Count)
								col -= ega.Count;
							*b = (byte)col++;
							b++;
						}
						brow += bd.ScanWidth;
					}
				}
			}
			return image;

		}

		Control CreateIndexedImageView()
		{
			return new DrawableImageView { Image = CreateImage() };
		}

		Control CreateIndexedDrawable()
		{
			var control = new Drawable { Size = new Size(100, 100) };
			var image = CreateImage();
			control.Paint += (sender, pe) => pe.Graphics.DrawImage(image, 0, 0);
			return control;
		}
	}
}
