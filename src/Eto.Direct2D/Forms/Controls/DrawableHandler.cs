using Eto.Drawing;
using Eto.Forms;
using Eto.Direct2D.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using swf = System.Windows.Forms;

namespace Eto.Direct2D.Forms.Controls
{
	public class DrawableHandler : Eto.WinForms.Forms.Controls.DrawableHandler
	{
		Graphics graphics;
		GraphicsHandler graphicsHandler;
		SolidBrush backgroundColor;

		public override Color BackgroundColor
		{
			get { return backgroundColor != null ? backgroundColor.Color : base.BackgroundColor; }
			set
			{
				backgroundColor = value.A > 0 ? new SolidBrush(value) : null;
				if (Widget.Loaded)
					Invalidate(false);
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.SetStyle(swf.ControlStyles.SupportsTransparentBackColor | swf.ControlStyles.DoubleBuffer, false);
			Control.SetStyle(swf.ControlStyles.AllPaintingInWmPaint | swf.ControlStyles.Opaque, true);
			Control.HandleCreated += (sender, e) =>
			{
				graphics = new Graphics(new GraphicsHandler(this));
				graphicsHandler = (GraphicsHandler)graphics.Handler;
			};
		}

		public override Graphics CreateGraphics()
		{
			if (graphics == null)
				return null;
			var handler = new GraphicsHandler((GraphicsHandler)graphics.Handler);
			handler.BeginDrawing();
			return new Graphics(handler);
		}

		public override void Update(Eto.Drawing.Rectangle rect)
		{
			if (graphics == null)
				return;
			graphicsHandler.PerformDrawing(null, () =>
			{
				Callback.OnPaint(Widget, new PaintEventArgs(graphics, rect));
			});
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			if (graphics == null)
				return;
			var clipRect = e.ClipRectangle.ToEto();
			graphicsHandler.PerformDrawing(clipRect, () =>
			{
				// clear to control's background color
				if (backgroundColor == null)
					backgroundColor = new SolidBrush(base.BackgroundColor);
				graphics.Clear(backgroundColor);

				// perform user painting
				Callback.OnPaint(Widget, new PaintEventArgs(graphics, clipRect));
			});
		}
	}
}
