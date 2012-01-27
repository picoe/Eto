using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;
using sw = System.Windows;
using swi = System.Windows.Input;
using swc = System.Windows.Controls;

namespace Eto.Platform.Wpf.Forms
{
	public abstract class WpfFrameworkElement<T, W> : WidgetHandler<T, W>, IControl
		where T : System.Windows.FrameworkElement
		where W : Control
	{
		public abstract Color BackgroundColor
		{
			get;
			set;
		}

		public virtual Size Size
		{
			get {
				return new Size ((int)Control.Width, (int)Control.Height);
			}
			set {
				Control.Width = value.Width; Control.Height = value.Height;
			}
		}

		public bool Enabled
		{
			get { return Control.IsEnabled; }
			set { Control.IsEnabled = value; }
		}

		public Cursor Cursor
		{
			get;
			set;
		}

		public string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
		}

		public void Invalidate ()
		{
			Control.InvalidateVisual ();
		}

		public void Invalidate (Rectangle rect)
		{
			Control.InvalidateVisual ();
		}

		public Graphics CreateGraphics ()
		{
			throw new NotImplementedException ();
		}

		public void SuspendLayout ()
		{

		}

		public void ResumeLayout ()
		{
			Control.UpdateLayout ();
		}

		public void Focus ()
		{
			Control.Focus ();
		}

		public bool HasFocus
		{
			get { return Control.IsFocused; }
		}

		public bool Visible
		{
			get { return Control.IsVisible; }
			set
			{
				Control.Visibility = (value) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
			}
		}

		public override void AttachEvent (string handler)
		{
			var wpfcontrol = Control as swc.Control;
			switch (handler) {
				case Eto.Forms.Control.MouseMoveEvent:
					Control.MouseMove += (sender, e) => {
						var args = Generator.ConvertMouseEvent (Control, e);
						Widget.OnMouseMove (args);
						e.Handled = args.Handled;
					};
					break;
				case Eto.Forms.Control.MouseDownEvent:
					Control.MouseDown += (sender, e) => {
						var args = Generator.ConvertMouseEvent (Control, e);
						Widget.OnMouseDown (args);
						e.Handled = args.Handled;
					};
					break;
				case Eto.Forms.Control.MouseDoubleClickEvent:
					if (wpfcontrol != null)
						wpfcontrol.MouseDoubleClick += (sender, e) => {
							var args = Generator.ConvertMouseEvent (Control, e);
							Widget.OnMouseDoubleClick (args);
							e.Handled = args.Handled;
						};
					else
						Control.MouseDown += (sender, e) => {
							if (e.ClickCount == 2) {
								var args = Generator.ConvertMouseEvent (Control, e);
								Widget.OnMouseDoubleClick (args);
								e.Handled = args.Handled;
							}
						};
					break;
				case Eto.Forms.Control.MouseUpEvent:
					Control.MouseUp += (sender, e) => {
						var args = Generator.ConvertMouseEvent (Control, e);
						Widget.OnMouseUp (args);
						e.Handled = args.Handled;
					};
					break;
				case Eto.Forms.Control.MouseEnterEvent:
					Control.MouseEnter += (sender, e) => {
						var args = Generator.ConvertMouseEvent (Control, e);
						Widget.OnMouseEnter (args);
						e.Handled = args.Handled;
					};
					break;
				case Eto.Forms.Control.MouseLeaveEvent:
					Control.MouseEnter += (sender, e) => {
						var args = Generator.ConvertMouseEvent (Control, e);
						Widget.OnMouseLeave (args);
						e.Handled = args.Handled;
					};
					break;
				default:
					base.AttachEvent (handler);
					break;
			}
		}

		public virtual void OnLoad (EventArgs e)
		{
		}

		public virtual void OnPreLoad (EventArgs e)
		{
		}

		public virtual void OnLoadComplete (EventArgs e)
		{
		}

		public virtual void SetParent (Control parent)
		{
		}

		public virtual void SetParentLayout (Layout layout)
		{
		}

		public IAsyncResult BeginInvoke (Delegate method, object[] args)
		{
			throw new NotImplementedException ();
		}

		public object EndInvoke (IAsyncResult result)
		{
			throw new NotImplementedException ();
		}

		public object Invoke (Delegate method, object[] args)
		{
			throw new NotImplementedException ();
		}

		public bool InvokeRequired
		{
			get { throw new NotImplementedException (); }
		}
	}
}
