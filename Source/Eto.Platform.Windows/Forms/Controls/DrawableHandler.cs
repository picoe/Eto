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
		public class DrawableInternal : SWF.UserControl
		{
			bool canFocus;

            public Drawable Drawable { get; set; }

            public DrawableHandler Handler { get; set; }

			public DrawableInternal(
                DrawableHandler handler)
			{
				this.Handler = handler;
                this.SetStyle(SWF.ControlStyles.AllPaintingInWmPaint, true);
                //this.SetStyle(SWF.ControlStyles.SupportsTransparentBackColor, true);
                //this.SetStyle(SWF.ControlStyles.Selectable, true);
                this.SetStyle(SWF.ControlStyles.StandardClick, true);
                this.SetStyle(SWF.ControlStyles.StandardDoubleClick, true);
                this.SetStyle(SWF.ControlStyles.ContainerControl, true);
                this.SetStyle(SWF.ControlStyles.UserPaint, true);
                this.SetStyle(SWF.ControlStyles.DoubleBuffer, true);
                this.SetStyle(SWF.ControlStyles.ResizeRedraw, true);
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
			
			protected override bool IsInputKey(SWF.Keys keyData)
			{
				switch (keyData)
				{
					case SWF.Keys.Up:
					case SWF.Keys.Down:
					case SWF.Keys.Left:
					case SWF.Keys.Right:
                    case SWF.Keys.Back:
						return true;
					default:
						return base.IsInputKey(keyData);
				}
			}

			protected override void OnPaint(SWF.PaintEventArgs e)
			{
				base.OnPaint(e);

                if (Handler != null &&
                    Handler.Widget != null)
                    Handler.Widget.OnPaint(
                        new PaintEventArgs(graphics, e.ClipRectangle.ToEto ()));
			}
			protected override void OnClick(EventArgs e)
			{
				base.OnClick (e);
				if (CanFocusMe) Focus();
			}

            protected override void OnSizeChanged(EventArgs e)
            {
                base.OnSizeChanged(e);
                Invalidate();
            }

		}

        public DrawableHandler()
        {
        }

        /// <summary>
        /// This is to allow instantiating a DrawableHandler
        /// from an existing control.
        /// </summary>
        public DrawableHandler(
            DrawableInternal control) 
        {
            Control = control;
            control.Handler = this; 

            Control.Drawable =
                new Eto.Forms.Drawable(
                        Generator.Current,
                        this);

            Control.TabStop = true;
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
			get	{ return Control.Size.ToEto (); }
			set { Control.Size = value.ToSD (); }
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
