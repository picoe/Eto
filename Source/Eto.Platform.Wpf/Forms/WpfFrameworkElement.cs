using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;
using sw = System.Windows;
using swi = System.Windows.Input;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;

namespace Eto.Platform.Wpf.Forms
{
	public interface IWpfFrameworkElement
	{
		sw.Size GetPreferredSize (sw.Size? constraint);
		sw.FrameworkElement ContainerControl { get; }
	}

	public static class ControlExtensions
	{
		public static sw.FrameworkElement GetContainerControl (this Control control)
		{
			var handler = control.Handler as IWpfFrameworkElement;
			if (handler != null)
				return handler.ContainerControl;
			else
				return control.ControlObject as sw.FrameworkElement;
		}

        public static sw.Size GetPreferredSize (this Control control, sw.Size? available = null)
        {
			if (control != null) {
				var handler = control.Handler as IWpfFrameworkElement;
				if (handler != null)
					return handler.GetPreferredSize (available);
			}
			return sw.Size.Empty;
        }
	}

	public abstract class WpfFrameworkElement<T, W> : WidgetHandler<T, W>, IControl, IWpfFrameworkElement
		where T : System.Windows.FrameworkElement
		where W : Control
	{
		Size? size;
		double preferredWidth = double.NaN;
		double preferredHeight = double.NaN;
		Size? newSize;
		Cursor cursor;
		bool loaded;

		public abstract Color BackgroundColor
		{
			get;
			set;
		}

		public virtual sw.FrameworkElement ContainerControl
		{
			get { return this.Control; }
		}

		public virtual Size Size
		{
			get {
				var newSize = this.newSize;
				if (!Widget.Loaded && size != null) return size.Value;
				else if (newSize != null) return newSize.Value;
				else return Conversions.GetSize (Control); 
			}
			set {
				size = value;
				preferredWidth = value.Width == -1 ? double.NaN : (double)value.Width;
				preferredHeight = value.Height == -1 ? double.NaN : (double)value.Height;
				Conversions.SetSize (Control, value); 
			}
		}

		public virtual sw.Size GetPreferredSize (sw.Size? constraint = null)
		{
			if (double.IsNaN(preferredWidth) || double.IsNaN(preferredHeight) || constraint != null) {
				ContainerControl.Measure (constraint ?? new sw.Size (double.PositiveInfinity, double.PositiveInfinity));
					if (double.IsNaN (preferredWidth))
						preferredWidth = ContainerControl.DesiredSize.Width;
					if (double.IsNaN (preferredHeight))
						preferredHeight = ContainerControl.DesiredSize.Height;
				}
				return new sw.Size (preferredWidth, preferredHeight);
			}

		public bool Enabled
		{
			get { return Control.IsEnabled; }
			set { Control.IsEnabled = value; }
		}

		public virtual Cursor Cursor
		{
			get { return cursor; }
			set
			{
				cursor = value;
				if (cursor != null)
					Control.Cursor = ((CursorHandler)cursor.Handler).Control;
				else
					Control.Cursor = null;
			}
		}

		public string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
		}

		public virtual void Invalidate ()
		{
			Control.InvalidateVisual ();
		}

		public virtual void Invalidate (Rectangle rect)
		{
			Control.InvalidateVisual ();
		}

		public void SuspendLayout ()
		{

		}

		public void ResumeLayout ()
		{
			//Control.UpdateLayout ();
		}

		public virtual void Focus ()
		{
			Control.Focus ();
		}

		protected virtual void EnsureLoaded ()
		{
			if (!loaded) {
				Control.EnsureLoaded ();
				loaded = true;
			}
		}

		public virtual bool HasFocus
		{
			get { return Control.IsFocused; }
		}

		public bool Visible
		{
			get { return Control.IsVisible; }
			set
			{
				Control.Visibility = (value) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
			}
		}

		public override void AttachEvent (string handler)
		{
			var wpfcontrol = Control as swc.Control;
			switch (handler) {
				case Eto.Forms.Control.MouseMoveEvent:
					Control.MouseMove += (sender, e) => {
						var args = e.ToEto (Control);
						Widget.OnMouseMove (args);
						e.Handled = args.Handled;
					};
					break;
				case Eto.Forms.Control.MouseDownEvent:
					Control.MouseDown += (sender, e) => {
						var args = e.ToEto (Control);
						Widget.OnMouseDown (args);
						e.Handled = args.Handled;
					};
					break;
				case Eto.Forms.Control.MouseDoubleClickEvent:
					if (wpfcontrol != null)
						wpfcontrol.MouseDoubleClick += (sender, e) => {
							var args = e.ToEto (Control);
							Widget.OnMouseDoubleClick (args);
							e.Handled = args.Handled;
						};
					else
						Control.MouseDown += (sender, e) => {
							if (e.ClickCount == 2) {
								var args = e.ToEto (Control);
								Widget.OnMouseDoubleClick (args);
								e.Handled = args.Handled;
							}
						};
					break;
				case Eto.Forms.Control.MouseUpEvent:
					Control.MouseUp += (sender, e) => {
						var args = e.ToEto (Control);
						Widget.OnMouseUp (args);
						e.Handled = args.Handled;
					};
					break;
				case Eto.Forms.Control.MouseEnterEvent:
					Control.MouseEnter += (sender, e) => {
						var args = e.ToEto (Control);
						Widget.OnMouseEnter (args);
						e.Handled = args.Handled;
					};
					break;
				case Eto.Forms.Control.MouseLeaveEvent:
					Control.MouseLeave += (sender, e) => {
						var args = e.ToEto (Control);
						Widget.OnMouseLeave (args);
						e.Handled = args.Handled;
					};
					break;
				case Eto.Forms.Control.SizeChangedEvent:
					Control.SizeChanged += (sender, e) => {
						this.newSize = e.NewSize.ToEto (); // so we can report this back in Control.Size
						Widget.OnSizeChanged (EventArgs.Empty);
						this.newSize = null;
					};
					break;
				case Eto.Forms.Control.KeyDownEvent:
					Control.TextInput += (sender, e) => {
						foreach (var keyChar in e.Text) {
							var key = Key.None;
                            var args = new KeyPressEventArgs(key, KeyType.KeyDown, keyChar);
							Widget.OnKeyDown (args);
							e.Handled |= args.Handled;
						}
					};
					Control.KeyDown += (sender, e) => {
						var args = e.ToEto (KeyType.KeyDown);
						Widget.OnKeyDown (args);
						e.Handled = args.Handled;
					};
					break;
				case Eto.Forms.Control.ShownEvent:
					Control.IsVisibleChanged += (sender, e) => {
						if ((bool)e.NewValue) {
							Widget.OnShown (EventArgs.Empty);
						}
					};
					break;
				case Eto.Forms.Control.HiddenEvent:
					Control.IsVisibleChanged += (sender, e) => {
						if (!(bool)e.NewValue) {
							Widget.OnHidden (EventArgs.Empty);
						}
					};
					break;
				case Eto.Forms.Control.GotFocusEvent:
					Control.GotFocus += (sender, e) => {
						Widget.OnGotFocus (EventArgs.Empty);
					};
					break;
				case Eto.Forms.Control.LostFocusEvent:
					Control.LostFocus += (sender, e) => {
						Widget.OnLostFocus (EventArgs.Empty);
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

		public void MapPlatformAction (string systemAction, BaseAction action)
		{
			
		}
		public Point ScreenToWorld (Point p)
		{
			return Control.PointFromScreen (p.ToWpf ()).ToEto ();
		}

		public Point WorldToScreen (Point p)
		{
			return Control.PointToScreen (p.ToWpf ()).ToEto ();
		}


        public DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects)
        {
            throw new NotImplementedException();
        }


        public bool Capture
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Point MousePosition
        {
			get { return swi.Mouse.GetPosition (Control).ToEto (); }
        }

        public Point Location
        {
			get
			{
				if (Widget.Parent == null)
					return Point.Empty;
				else
					return Control.TranslatePoint (new sw.Point (0, 0), Widget.Parent.GetContainerControl ()).ToEto ();
			}
        }

        public void SetControl(object control)
        {
            throw new NotImplementedException();
        }
    }
}
