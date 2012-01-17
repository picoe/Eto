using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

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
