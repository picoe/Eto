using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sw = System.Windows;
using swm = System.Windows.Media;
using swmi = System.Windows.Media.Imaging;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Drawing
{
	public class IndexedBitmapHandler : WidgetHandler<swmi.WriteableBitmap, IndexedBitmap>, IIndexedBitmap, IWpfImage
	{
		Palette palette;
		int numColors;

		public void Create (int width, int height, int bitsPerPixel)
		{
			var format = swm.PixelFormats.Indexed8;
			numColors = (int)Math.Pow(2, bitsPerPixel);
			var colors = new List<swm.Color> (numColors);
			while (colors.Count < numColors) {
				colors.Add (swm.Colors.Black);
			}
			Control = new swmi.WriteableBitmap (width, height, 96, 96, format, new swmi.BitmapPalette (colors));
		}

		public void Resize (int width, int height)
		{
			throw new NotImplementedException ();
		}

		public BitmapData Lock ()
		{
			var wb = Control as swm.Imaging.WriteableBitmap;
			if (wb != null) {
				wb.Lock ();
				return new BitmapDataHandler (Widget, wb.BackBuffer, (int)Control.PixelWidth, Control.Format.BitsPerPixel, Control);
			}
			else
				throw new InvalidOperationException ();
		}

		public void Unlock (BitmapData bitmapData)
		{
			var wb = Control as swm.Imaging.WriteableBitmap;
			if (wb != null) {

				wb.AddDirtyRect (new sw.Int32Rect (0, 0, Size.Width, Size.Height));
				wb.Unlock ();
			}
		}

		public Palette Palette
		{
			get
			{
				if (palette == null) {
					palette = new Eto.Drawing.Palette ();
					foreach (var col in Control.Palette.Colors) {
						palette.Add (col.ToEto ());
					}
				}
				return palette;
			}
			set
			{
				if (value.Count != numColors)
					throw new ArgumentOutOfRangeException ("Palette must have the same number of colors as the image");
				palette = value;
				var old = Control;
				// re-create with new palette
				var colors = new List<swm.Color> (numColors);
				for (int i = 0; i < numColors; i++) {
					colors.Add(palette[i].ToWpf ());
				}
				Control = new swmi.WriteableBitmap (old.PixelWidth, old.PixelHeight, 96, 96, old.Format, new swmi.BitmapPalette (colors));
				Control.CopyPixels (sw.Int32Rect.Empty, old.BackBuffer, old.BackBufferStride * old.PixelHeight, old.BackBufferStride);
			}
		}

		public Size Size
		{
			get { return new Size (Control.PixelWidth, Control.PixelHeight); }
		}

		public swmi.BitmapSource GetImageClosestToSize (int? width)
		{
			return Control;
		}
	}
}
