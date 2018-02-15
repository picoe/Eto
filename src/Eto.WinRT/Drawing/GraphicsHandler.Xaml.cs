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
using Eto.WinRT.Forms.Controls;
using System.Threading.Tasks;

namespace Eto.Direct2D.Drawing
{
	public partial class GraphicsHandler
	{
		DrawableHandler drawableHandler;
		public GraphicsHandler(DrawableHandler drawableHandler)
		{
			this.drawableHandler = drawableHandler;
			drawableHandler.Widget.LoadComplete += (s, e) => CreateXamlRenderTarget(); 
			drawableHandler.Control.Loaded += (s, e) => CreateXamlRenderTarget();
			//drawableHandler.Control.LayoutUpdated += (s, e) => CreateXamlRenderTarget();
			drawableHandler.Widget.SizeChanged += (s, e) => CreateXamlRenderTarget();
		}

		public Bitmap Image { get { return image; } }

		private void CreateXamlRenderTarget()
		{
			InitializeControl();
			if(drawableHandler != null)
				drawableHandler.Render();
		}

		public sd.RenderTarget InitializeControl()
		{
			var sizeF = new SizeF((float)drawableHandler.Control.Width, (float)drawableHandler.Control.Height);
			CreateXamlRenderTarget(sizeF);
			return Control;
		}

		private void CreateXamlRenderTarget(SizeF size)
		{
			if (!double.IsNaN(size.Width) &&
				!double.IsNaN(size.Height) &&
				!size.IsEmpty &&
				(image == null ||
				 image.Width != size.Width ||
				 image.Height != size.Height))
			{
				if (image != null)
					image.Dispose();

				image = new Bitmap(new Size((int)size.Width, (int)size.Height), PixelFormat.Format32bppRgba);
				CreateWicTarget();
			}
		}
	}
}
