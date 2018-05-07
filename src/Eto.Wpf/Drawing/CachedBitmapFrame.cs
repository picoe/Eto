using System;
using sw = System.Windows;
using swm = System.Windows.Media;
using swmi = System.Windows.Media.Imaging;

namespace Eto.Wpf.Drawing
{
	/// <summary>
	/// Used to cache a single resized bitmap with specified width/height/scaling
	/// </summary>
	class CachedBitmapFrame
	{
		swm.BitmapScalingMode _scalingMode;
		float _scale;
		int _width;
		int _height;
		WeakReference _cachedFrameReference;

		public swmi.BitmapFrame Get(swmi.BitmapSource image, float scale, int width, int height, swm.BitmapScalingMode scalingMode)
		{
			if (width <= 0 || height <= 0 || scale <= 0)
				return null;

			var _cachedFrame = _cachedFrameReference?.Target as swmi.BitmapFrame;
			// if parameters are the same, return cached bitmap
			if (_cachedFrame != null && scale == _scale && width == _width && height == _height && scalingMode == _scalingMode)
				return _cachedFrame;

			// generate a new bitmap with the desired size & scale.
			var scaledwidth = (int)Math.Round(width * scale);
			var scaledheight = (int)Math.Round(height * scale);
			if (scaledwidth <= 0 || scaledheight <= 0)
				return null;
			var group = new swm.DrawingGroup();
			swm.RenderOptions.SetBitmapScalingMode(group, scalingMode);
			group.Children.Add(new swm.ImageDrawing(image, new sw.Rect(0, 0, width, height)));

			var targetVisual = new swm.DrawingVisual();
			using (var targetContext = targetVisual.RenderOpen())
				targetContext.DrawDrawing(group);

			// note, this uses a GDI handle, which are limited (only 5000 or so can be created).  
			// There's no way to get around it other than just not creating that many and using GC.Collect/WaitForPendingFinalizers.
			// we can't do it in Eto as it'd be a serious performance hit.
			var target = new swmi.RenderTargetBitmap(scaledwidth, scaledheight, 96 * scale, 96 * scale, swm.PixelFormats.Default);
			target.Render(targetVisual);
			target.Freeze();

			_cachedFrame = swmi.BitmapFrame.Create(target);
			_cachedFrame.Freeze();
			_scale = scale;
			_width = width;
			_height = height;
			_scalingMode = scalingMode;
			_cachedFrameReference = new WeakReference(_cachedFrame);
			return _cachedFrame;
		}
	}
}
