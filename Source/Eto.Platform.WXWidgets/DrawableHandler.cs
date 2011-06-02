using System;

namespace Eto.Forms.WXWidgets
{
	internal class DrawableHandler : WXControl, IDrawable
	{
		wx.Control control = null;

		public DrawableHandler(Widget widget) : base(widget)
		{
			//control.EVT_PAINT(
			//control = new DrawableInternal();
			//control.Paint += new System.Windows.Forms.PaintEventHandler(control_Paint);
		}

		public override object ControlObject
		{
			get { return control; }
		}

		private void control_Paint(object sender, wx.Event e)
		{
			wx.PaintDC dc = new wx.PaintDC((wx.Window)e.EventObject);
			
			Graphics graphics = new Graphics(Widget.Generator, new GraphicsHandler(dc));
			//e.EventObject
			//e.Graphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.Red), e.ClipRectangle);
			//Graphics graphics = new Graphics(Widget.Generator, new GraphicsHandler(e.Graphics));

			((Drawable)Widget).OnPaint(new PaintEventArgs(graphics, control.ClientRect));
			dc.Dispose();
		}

		public override object CreateControl(Control parent, object container)
		{
			control = new wx.Control((wx.Window)container, ((WXGenerator)Generator).GetNextButtonID(), Location, Size, 0, string.Empty);
			control.EVT_PAINT(new wx.EventListener(control_Paint));

			return control;
		}

		
	}
}
