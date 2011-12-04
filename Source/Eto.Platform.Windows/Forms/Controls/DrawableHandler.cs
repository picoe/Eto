using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows
{
	public class DrawableHandler : WindowsControl<DrawableHandler.DrawableInternal, Drawable>, IDrawable
	{

		public class DrawableInternal : SWF.Control
		{
			bool canFocus;

			DrawableHandler handler;
			public DrawableInternal(DrawableHandler handler)
			{
				this.handler = handler;
				this.SetStyle(SWF.ControlStyles.AllPaintingInWmPaint, true);
				//this.SetStyle(SWF.ControlStyles.SupportsTransparentBackColor, true);
				//this.SetStyle(SWF.ControlStyles.Selectable, true);
				this.SetStyle(SWF.ControlStyles.StandardClick, true);
				this.SetStyle(SWF.ControlStyles.StandardDoubleClick, true);
				this.SetStyle(SWF.ControlStyles.ContainerControl, true);
				this.SetStyle(SWF.ControlStyles.UserPaint, true);
				this.SetStyle(SWF.ControlStyles.DoubleBuffer, true);
				//this.SetStyle(SWF.ControlStyles.Opaque, true);
				//this.SetStyle(SWF.ControlStyles., true);
				//this.BackColor = SD.Color.Transparent;
				//this.BackColor = SD.Color.Black;
			}
			
			public bool CanFocusMe
			{
				get { return canFocus; }
				set { canFocus = value; this.SetStyle(SWF.ControlStyles.Selectable, value);  }
			}

			protected override void OnGotFocus(EventArgs e)
			{
				base.OnGotFocus(e);
				Invalidate();
			}

			protected override void OnLostFocus(EventArgs e)
			{
				base.OnLostFocus(e);
				Invalidate();
			}
			
			protected override bool ProcessDialogKey(SWF.Keys keyData)
			{
				var e = new SWF.KeyEventArgs(keyData);
				base.OnKeyDown(e);
				return e.Handled;
			}
			
			protected override bool IsInputKey(System.Windows.Forms.Keys keyData)
			{
				switch (keyData)
				{
					case SWF.Keys.Up:
					case SWF.Keys.Down:
					case SWF.Keys.Left:
					case SWF.Keys.Right:
						return true;
					default:
						return base.IsInputKey(keyData);
				}
			}

			protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
			{
				//base.OnPaint(e);
				Graphics graphics = new Graphics(handler.Widget.Generator, new GraphicsHandler(e.Graphics));

				handler.Widget.OnPaint(new PaintEventArgs(graphics, Generator.Convert(e.ClipRectangle)));
			}
			protected override void OnClick(EventArgs e)
			{
				base.OnClick (e);
				if (CanFocusMe) Focus();
			}

		}
		
		public void Create ()
		{
			Control = new DrawableInternal(this);
			Control.TabStop = true;
		}
		
		public bool CanFocus {
			get {
				return Control.CanFocusMe;
			}
			set {
				Control.CanFocusMe = value;
			}
		}

		public override Size Size
		{
			get	{ return Generator.Convert(Control.Size); }
			set { Control.Size = Generator.Convert(value); }
		}

		public void Update(Rectangle rect)
		{
			SD.Graphics g = Control.CreateGraphics();
			Graphics graphics = new Graphics(Widget.Generator, new GraphicsHandler(g));

			Widget.OnPaint(new PaintEventArgs(graphics, rect));
			g.Dispose();
		}

	}
}
