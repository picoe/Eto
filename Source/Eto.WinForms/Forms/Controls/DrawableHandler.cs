using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using Eto.WinForms.Drawing;

namespace Eto.WinForms.Forms.Controls
{
	public class DrawableHandler : WindowsPanel<DrawableHandler.EtoDrawable, Drawable, Drawable.ICallback>, Drawable.IHandler
	{
		public virtual bool SupportsCreateGraphics { get { return true; } }

		public class EtoDrawable : swf.Control
		{
			bool canFocus;

			public DrawableHandler Handler { get; set; }

			public EtoDrawable ()
			{
				this.SetStyle (swf.ControlStyles.AllPaintingInWmPaint, true);
				this.SetStyle (swf.ControlStyles.StandardClick, true);
				this.SetStyle (swf.ControlStyles.StandardDoubleClick, true);
				this.SetStyle (swf.ControlStyles.ContainerControl, true);
				this.SetStyle (swf.ControlStyles.UserPaint, true);
				this.SetStyle (swf.ControlStyles.DoubleBuffer, true);
				this.SetStyle (swf.ControlStyles.ResizeRedraw, true);
				this.SetStyle (swf.ControlStyles.SupportsTransparentBackColor, true);
			}

			public new void SetStyle(swf.ControlStyles flag, bool value)
			{
				base.SetStyle(flag, value);
			}

			public bool CanFocusMe
			{
				get { return canFocus; }
				set { canFocus = value; SetStyle(swf.ControlStyles.Selectable, value); }
			}

			protected override void OnGotFocus (EventArgs e)
			{
				base.OnGotFocus (e);
				Invalidate ();
			}

			protected override void OnLostFocus (EventArgs e)
			{
				base.OnLostFocus (e);
				Invalidate ();
			}

			protected override bool ProcessDialogKey (swf.Keys keyData)
			{
				var e = new swf.KeyEventArgs (keyData);
				OnKeyDown(e);
				if (!e.Handled) {
					// Prevent firing the keydown event twice for the same key
					if (CanFocusMe && keyData == swf.Keys.Tab || keyData == (swf.Keys.Tab | swf.Keys.Shift))
						return base.ProcessDialogKey(keyData);
					Handler.LastKeyDown = e.KeyData.ToEto ();
				}
				return e.Handled;
			}

			protected override bool IsInputKey (swf.Keys keyData)
			{
				switch (keyData) {
				case swf.Keys.Up:
				case swf.Keys.Down:
				case swf.Keys.Left:
				case swf.Keys.Right:
				case swf.Keys.Back:
					return true;
				default:
					return base.IsInputKey (keyData);
				}
			}

			protected override void OnPaint (swf.PaintEventArgs e)
			{
				base.OnPaint (e);

				Handler.OnPaint(e);
			}

			protected override void OnClick (EventArgs e)
			{
				base.OnClick (e);
				if (CanFocusMe) Focus ();
			}
		}

        public DrawableHandler()
        {
        }

        /// <summary>
        /// This is to allow instantiating a DrawableHandler
        /// from an existing control.
        /// </summary>
        public DrawableHandler(EtoDrawable control) 
        {
            Control = control;
			Control.Handler = this;
            Control.TabStop = true;
        }

		public void Create ()
		{
			Control = new EtoDrawable { Handler = this };
			Control.TabStop = true;
		}

		public void Create(bool largeCanvas)
		{
			Create();
		}

		public virtual Graphics CreateGraphics()
		{
			return new Graphics(new GraphicsHandler(Control.CreateGraphics()));
		}

		public bool CanFocus {
			get { return Control.CanFocusMe; }
			set { Control.CanFocusMe = value; }
		}

		public virtual void Update(Rectangle rect)
		{
			using (var g = Control.CreateGraphics ()) {
				var graphics = new Graphics(new GraphicsHandler(g));

				Callback.OnPaint(Widget, new PaintEventArgs(graphics, rect));
			}
		}

		protected virtual void OnPaint(swf.PaintEventArgs e)
		{
			Callback.OnPaint(Widget, e.ToEto());
		}
	}
}
