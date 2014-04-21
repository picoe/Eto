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
			drawableHandler.Widget.LoadComplete += (s, e) => CreateXamlRenderTarget(); 
			drawableHandler.Control.Loaded += (s, e) => CreateXamlRenderTarget();
			drawableHandler.Control.SizeChanged += (s, e) => CreateXamlRenderTarget();

			// initialize to some default
			// CreateXamlRenderTarget(new SizeF(1000, 1000));
		}

		public Bitmap Image { get { return image; } }

		private void CreateXamlRenderTarget()
		{
			var sizeF = new SizeF((float)drawableHandler.Control.Width, (float)drawableHandler.Control.Height);
			CreateXamlRenderTarget(sizeF);
		}

		private void CreateXamlRenderTarget(SizeF sizeF)
		{
			if (!double.IsNaN(sizeF.Width) &&
				!double.IsNaN(sizeF.Height) &&
				(image == null ||
				 image.Width != sizeF.Width ||
				 image.Height != sizeF.Height))
			{
				if (image != null)
					image.Dispose();

				var size = new Size((int)sizeF.Width, (int)sizeF.Height);
				image = new Bitmap(size, PixelFormat.Format32bppRgba);
				CreateWicTarget();
			}
		}
	}
}
