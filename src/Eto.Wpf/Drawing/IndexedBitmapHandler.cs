using System;
using System.Collections.Generic;
using System.Globalization;
using sw = System.Windows;
using swm = System.Windows.Media;
using swmi = System.Windows.Media.Imaging;
using Eto.Drawing;
using Eto.Wpf.Forms;

namespace Eto.Wpf.Drawing
{
	public class IndexedBitmapHandler : WidgetHandler<swmi.WriteableBitmap, IndexedBitmap>, IndexedBitmap.IHandler, IWpfImage
	{
		Palette palette;
		int numColors;
		bool isLocked;
		bool paletteSetInLocked;

		public void Create (int width, int height, int bitsPerPixel)
		{
			Size = new Size(width, height);
			var format = swm.PixelFormats.Indexed8;
			numColors = (int)Math.Pow(2, bitsPerPixel);
			var colors = new List<swm.Color> (numColors);
			while (colors.Count < numColors) {
				colors.Add (swm.Colors.Black);
			}
			ApplicationHandler.InvokeIfNecessary (() => {
				Control = new swmi.WriteableBitmap (width, height, 96, 96, format, new swmi.BitmapPalette (colors));
			});
		}

		public void Resize (int width, int height)
		{
			throw new NotImplementedException ();
		}

		public BitmapData Lock ()
		{
			if (isLocked)
				throw new InvalidOperationException ();
			BitmapDataHandler bd = null;
			ApplicationHandler.InvokeIfNecessary (() => {
				Control.Lock ();
				bd = new BitmapDataHandler (Widget, Control.BackBuffer, Size.Width, Control.Format.BitsPerPixel, Control);
			});
			isLocked = true;
			return bd;
		}

		public void Unlock (BitmapData bitmapData)
		{
			if (!isLocked)
				throw new InvalidOperationException ();
			ApplicationHandler.InvokeIfNecessary (() => {
				Control.AddDirtyRect (new sw.Int32Rect (0, 0, Size.Width, Size.Height));
				Control.Unlock ();
				if (paletteSetInLocked) {
					SetPalette ();
					paletteSetInLocked = false;
				}
			});
			isLocked = false;
		}

		public Palette Palette
		{
			get
			{
				if (palette == null) {
					palette = new Palette ();
					foreach (var col in Control.Palette.Colors) {
						palette.Add (col.ToEto ());
					}
				}
				return palette;
			}
			set
			{
				if (value.Count != numColors)
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Palette must have the same number of colors as the image"));
				palette = value;
				if (isLocked)
					paletteSetInLocked = true;
				else {
					ApplicationHandler.InvokeIfNecessary(SetPalette);
				}
			}
		}

		void SetPalette ()
		{
			var old = Control;
			// re-create with new palette
			var colors = new List<swm.Color> (numColors);
			for (int i = 0; i < numColors; i++) {
				colors.Add (palette[i].ToWpf ());
			}
			var bufferSize = old.BackBufferStride * old.PixelHeight;
			var pal = new swmi.BitmapPalette (colors);
			var bs = swmi.BitmapSource.Create (old.PixelWidth, old.PixelHeight, 96, 96, old.Format, pal, old.BackBuffer, bufferSize, old.BackBufferStride);
			Control = new swmi.WriteableBitmap (bs);
		}

		public Size Size
		{
			get; private set;
		}

		public swmi.BitmapSource GetImageClosestToSize(float scale, Size? fittingSize)
		{
			return Control;
		}
	}
}
