using System;
using System.Collections.Generic;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;
using swm = Windows.UI.Xaml.Media;
using swmi = Windows.UI.Xaml.Media.Imaging;
using Eto.Forms;
using System.Diagnostics;
using Eto.Platform.Xaml.Forms.Controls;

namespace Eto.Platform.Direct2D.Drawing
{
	public partial class GraphicsHandler
	{
		DrawableHandler drawableHandler;
		public GraphicsHandler(DrawableHandler drawableHandler)
		{
			this.drawableHandler = drawableHandler;
			drawableHandler.Control.Loaded += (s, e) => CreateXamlRenderTarget(drawableHandler);
			drawableHandler.Control.SizeChanged += (s, e) => CreateXamlRenderTarget(drawableHandler);
		}

		public Bitmap Image { get { return image; } }

		private void CreateXamlRenderTarget(DrawableHandler drawable)
		{
			if (!double.IsNaN(drawable.Control.Width) &&
				!double.IsNaN(drawable.Control.Height) &&
				(image == null ||
				 image.Width != drawable.Control.Width ||
				 image.Height != drawable.Control.Height))
			{
				if (image != null)
					image.Dispose();

				var size = new Size((int)drawable.Control.Width, (int)drawable.Control.Height);
				image = new Bitmap(size, PixelFormat.Format32bppRgba);
				CreateWicTarget();
			}
		}
	}
}
