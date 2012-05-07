﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	public class IndexedBitmapSection : Panel
	{
		public IndexedBitmapSection ()
		{
			var layout = new DynamicLayout (this);

			layout.AddRow (
				new Label { Text = "Indexed Bitmap on ImageView" }, CreateIndexedImageView (),
				new Label { Text = "Indexed Bitmap on Drawable" }, CreateIndexedDrawable (),
				null
				);

			layout.Add (null);
		}
		
		IndexedBitmap CreateImage()
		{
			var image = new IndexedBitmap (100, 100, 8);
			var pal = new Palette (Palette.GetEgaPalette ());
			
			// must have at least 256 colors for an 8-bit bitmap
			while (pal.Count < 256)
				pal.Add (Color.Black);
			image.Palette = pal;
			var bd = image.Lock ();
			
			unsafe {
				int col = 0;
				byte* brow = (byte*)bd.Data;
				for (int y = 0; y < image.Size.Height; y++) {
					byte* b = brow;
					for (int x = 0; x < image.Size.Width; x++) {
						if (col >= pal.Count) 
							col = 0;
						*b = (byte)col++;
						b++;
					}
					brow += bd.ScanWidth;
				}
			}
			image.Unlock (bd);
			return image;
			
		}

		Control CreateIndexedImageView ()
		{
			return new ImageView { Image = CreateImage () };
		}
		Control CreateIndexedDrawable ()
		{
			var control = new Drawable { Size = new Size(100, 100) };
			var image = CreateImage();
			control.Paint += delegate(object sender, PaintEventArgs pe) {
				pe.Graphics.DrawImage (image, 0, 0);
			};
			return control;
		}

	}
}
