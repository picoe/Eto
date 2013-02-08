using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows
{
	public class DrawableHandler : WindowsControl<DrawableHandler.EtoDrawable, Drawable>, IDrawable
	{
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
			}

			public bool CanFocusMe
			{
				get { return canFocus; }
				set { canFocus = value; this.SetStyle (swf.ControlStyles.Selectable, value); }
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
				base.OnKeyDown (e);
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

				Handler.Widget.OnPaint (e.ToEto (Handler.Generator));
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
		
		public bool CanFocus {
			get { return Control.CanFocusMe; }
			set { Control.CanFocusMe = value; }
		}

		public void Update(Rectangle rect)
		{
			using (var g = Control.CreateGraphics ()) {
				var graphics = new Graphics (Widget.Generator, new GraphicsHandler (g));

				Widget.OnPaint (new PaintEventArgs (graphics, rect));
			}
		}
	}
}
