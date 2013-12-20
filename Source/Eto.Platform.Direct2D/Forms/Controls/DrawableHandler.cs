using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.Direct2D.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using swf = System.Windows.Forms;

namespace Eto.Platform.Direct2D.Forms.Controls
{
	public class DrawableHandler : Eto.Platform.Windows.DrawableHandler
	{
		Graphics graphics;
		GraphicsHandler graphicsHandler;

		protected override void Initialize()
		{
			base.Initialize();
			Control.SetStyle(swf.ControlStyles.SupportsTransparentBackColor | swf.ControlStyles.DoubleBuffer, false);
			Control.SetStyle(swf.ControlStyles.AllPaintingInWmPaint | swf.ControlStyles.Opaque, true);
			Control.HandleCreated += (sender, e) =>
			{
				graphics = new Graphics(Widget, Generator);
				graphicsHandler = (GraphicsHandler)graphics.Handler;
			};
		}

		public override Graphics CreateGraphics()
		{
			if (graphics == null)
				return null;
			var handler = new GraphicsHandler((GraphicsHandler)graphics.Handler);
			handler.BeginDrawing();
			return new Graphics(Generator, handler);
		}

		public override void Update(Eto.Drawing.Rectangle rect)
		{
			if (graphics == null)
				return;
			graphicsHandler.BeginDrawing();
			Widget.OnPaint(new PaintEventArgs(graphics, rect));
			graphicsHandler.EndDrawing();
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			if (graphics == null)
				return;
			graphicsHandler.BeginDrawing();
			Widget.OnPaint(new PaintEventArgs(graphics, e.ClipRectangle.ToEto()));
			graphicsHandler.EndDrawing();
		}
	}
}
