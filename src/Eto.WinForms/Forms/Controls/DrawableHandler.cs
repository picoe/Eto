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

		public class EtoDrawable : PanelBase<DrawableHandler>
		{
			bool canFocus;

			public EtoDrawable(DrawableHandler handler)
				: base(handler)
			{
				base.SetStyle
					( swf.ControlStyles.AllPaintingInWmPaint
					| swf.ControlStyles.StandardClick
					| swf.ControlStyles.StandardDoubleClick
					| swf.ControlStyles.ContainerControl
					| swf.ControlStyles.UserPaint
					| swf.ControlStyles.DoubleBuffer
					| swf.ControlStyles.ResizeRedraw
					| swf.ControlStyles.SupportsTransparentBackColor
					, true);
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

			protected override bool ProcessDialogKey(swf.Keys keyData)
			{
				var e = new swf.KeyEventArgs (keyData);
				OnKeyDown(e);
				if (!e.Handled)
				{
					// Prevent firing the keydown event twice for the same key
					if (CanFocusMe && keyData == swf.Keys.Tab || keyData == ( swf.Keys.Tab | swf.Keys.Shift ))
						return base.ProcessDialogKey(keyData);
					Handler.LastKeyDown = e.KeyData.ToEto();
				}
				return e.Handled;
			}

			protected override void OnPaint(swf.PaintEventArgs e)
			{
				Handler.OnPaint(e);
				// base.OnPaint(e); --- don't really need to call Paint event (it does nothing else)
			}

			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				if (CanFocusMe) Focus();
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
		}

		public void Create()
		{
			Control = new EtoDrawable(this);
		}

		public void Create(bool largeCanvas)
		{
			Create();
		}

		public virtual Graphics CreateGraphics()
		{
			return new Graphics(new GraphicsHandler(Control.CreateGraphics(), true));
		}

		public bool CanFocus
		{
			get { return Control.CanFocusMe; }
			set { Control.CanFocusMe = value; }
		}

		public virtual void Update(Rectangle rect)
		{
			using (var g = CreateGraphics())
				Callback.OnPaint(Widget, new PaintEventArgs(g, rect));
		}

		protected virtual void OnPaint(swf.PaintEventArgs e)
		{
			using (var g = e.Graphics.ToEto(false))
				Callback.OnPaint(Widget, new PaintEventArgs(g, e.ClipRectangle.ToEto()));
		}

		protected override void SetContent(Control control, swf.Control contentControl)
		{
			var handler = control.Handler as IWindowsControl;
			if (handler != null && !handler.BackgroundColorSet)
			{
				// there is no direct way to ask the control (except reflection),
				// so, we rather catch the exception and ignore it,
				// if the control does not support transparent background
				try
				{
					contentControl.BackColor = sd.Color.Transparent;
				}
				catch (ArgumentException) { }
			}
			base.SetContent(control, contentControl);
		}
	}
}
