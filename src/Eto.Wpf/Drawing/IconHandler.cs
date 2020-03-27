using System;
using System.Collections.Generic;
using System.Globalization;
using Eto.Drawing;
using System.IO;
using sw = System.Windows;
using swi = System.Windows.Interop;
using swm = System.Windows.Media;
using swmi = System.Windows.Media.Imaging;
using sd = System.Drawing;
using System.Linq;

namespace Eto.Wpf.Drawing
{
	public interface IWpfImage
	{
		swmi.BitmapSource GetImageClosestToSize(float scale, Size? fittingSize);
	}

	public class IconHandler : WidgetHandler<swmi.BitmapFrame, Icon>, Icon.IHandler, IWpfImage
	{
		List<IconFrame> _frames;
		CachedBitmapFrame _cached;

		public IconHandler()
		{
		}

		public IconHandler(sd.Icon icon)
		{
			var rect = new sw.Int32Rect(0, 0, icon.Width, icon.Height);
			var img = swi.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, rect, swmi.BitmapSizeOptions.FromEmptyOptions());
			Control = swmi.BitmapFrame.Create(img);
		}

		public IconHandler(swmi.BitmapFrame control)
		{
			this.Control = control;
		}

		public void Create(Stream stream)
		{
			Control = swmi.BitmapFrame.Create(stream);
		}

		public void Create(string fileName)
		{
			using (var stream = File.OpenRead(fileName))
			{
				Control = swmi.BitmapFrame.Create(stream);
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			if (_frames == null)
				_frames = GetFrames().ToList();
		}

		IEnumerable<IconFrame> GetFrames()
		{
			var icons = Control?.Decoder?.Frames;
			if (icons == null)
			{
				yield return IconFrame.FromControlObject(1f, new Bitmap(new BitmapHandler(Control)));
				yield break;
			}
			foreach (var icon in icons)
			{
				// this is needed to actually load the icon so it can then be used in other threads.
				// even though we freeze it, it delays the creation until something like this is called..
				_ = icon.PixelWidth;
				yield return IconFrame.FromControlObject(1f, new Bitmap(new BitmapHandler(icon)));
			}
		}
		Size? size;
		public Size Size
		{
			get
			{
				if (size != null)
					return size.Value;
				var largest = GetLargestIcon();
				return new Size((int)largest.Bitmap.Width, (int)largest.Bitmap.Height);
			}
		}

		public IEnumerable<IconFrame> Frames => _frames ?? (_frames = GetFrames().ToList());

		IconFrame GetLargestIcon()
		{
			IconFrame curicon = null;
			foreach (var icon in Frames)
			{
				if (curicon == null || icon.PixelSize.Width > curicon.PixelSize.Width)
					curicon = icon;
			}
			return curicon;
		}


		public swmi.BitmapSource GetImageClosestToSize(float scale, Size? fittingSize)
		{
			var size = fittingSize ?? Size;
			var frame = Widget.GetFrame(scale, size);
			var wpfBitmap = frame.ToWpf(scale);
			if ((wpfBitmap.Width == size.Width && wpfBitmap.Height == size.Height)
				|| size.Height == 0
				|| size.Width == 0
				|| scale <= 0)
				return wpfBitmap;

			if (_cached == null)
				_cached = new CachedBitmapFrame();
			return _cached.Get(wpfBitmap, scale, size.Width, size.Height, swm.BitmapScalingMode.Linear) ?? wpfBitmap;
		}

		/*
		const int sICONDIR = 6;            // sizeof(ICONDIR) 
		const int sICONDIRENTRY = 16;      // sizeof(ICONDIRENTRY)

		public IEnumerable<swmi.BitmapFrame> SplitIcon(swmi.BitmapFrame icon, MemoryStream input)
		{
			if (icon == null)
			{
				throw new ArgumentNullException("icon");
			}

			// Get multiple .ico file image.
			byte[] srcBuf = input.ToArray();

			var splitIcons = new List<swmi.BitmapFrame>();
			int count = BitConverter.ToInt16(srcBuf, 4); // ICONDIR.idCount

			for (int i = 0; i < count; i++)
			{
				using (var destStream = new MemoryStream())
				using (var writer = new BinaryWriter(destStream))
				{
					// Copy ICONDIR and ICONDIRENTRY.
					int pos = 0;
					writer.Write(srcBuf, pos, sICONDIR - 2);
					writer.Write((short)1);    // ICONDIR.idCount == 1;

					pos += sICONDIR;
					pos += sICONDIRENTRY * i;

					writer.Write(srcBuf, pos, sICONDIRENTRY - 4); // write out icon info (minus old offset)
					writer.Write(sICONDIR + sICONDIRENTRY);    // write offset of icon data
					pos += 8;

					// Copy picture and mask data.
					int imgSize = BitConverter.ToInt32(srcBuf, pos);       // ICONDIRENTRY.dwBytesInRes
					pos += 4;
					int imgOffset = BitConverter.ToInt32(srcBuf, pos);    // ICONDIRENTRY.dwImageOffset
					if (imgOffset + imgSize > srcBuf.Length)
						throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, "Icon not a valid format"));
					writer.Write(srcBuf, imgOffset, imgSize);
					writer.Flush();

					// Create new icon.
					destStream.Seek(0, SeekOrigin.Begin);
					splitIcons.Add(swmi.BitmapFrame.Create(destStream));
				}
			}

			return splitIcons.ToArray();
		}
		*/

		public void Create(IEnumerable<IconFrame> frames)
		{
			_frames = new List<IconFrame>(frames);
			var scale = 1;
			var largest = _frames.OrderBy(r => r.PixelSize.Width * r.PixelSize.Height).LastOrDefault(r => r.Scale == scale);
			if (largest == null)
				largest = GetLargestIcon();
			size = Size.Ceiling((SizeF)largest.PixelSize / largest.Scale);
		}
	}
}
