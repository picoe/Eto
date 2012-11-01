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
	public interface IWpfFrameworkElement
	{
		Size? PreferredSize { get; }
		sw.FrameworkElement ContainerControl { get; }
	}

	public static class ControlExtensions
	{
		public static sw.FrameworkElement GetContainerControl (this Control control)
		{
			var handler = control as IWpfFrameworkElement;
			if (handler != null)
				return handler.ContainerControl;
			else
				return control.ControlObject as sw.FrameworkElement;
		}
	}

	public abstract class WpfFrameworkElement<T, W> : WidgetHandler<T, W>, IControl, IWpfFrameworkElement
		where T : System.Windows.FrameworkElement
		where W : Control
	{
		Size? size;
		Size? newSize;
		Cursor cursor;

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
				else return Generator.GetSize (Control); 
			}
			set {
				size = value;
				Generator.SetSize (Control, value); 
			}
		}

		public Size? PreferredSize
		{
			get { return size; }
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

		public Graphics CreateGraphics ()
		{
			throw new NotImplementedException ();
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
							var args = new KeyPressEventArgs (key, keyChar);
							Widget.OnKeyDown (args);
							e.Handled |= args.Handled;
						}
					};
					Control.KeyDown += (sender, e) => {
						var args = e.ToEto ();
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

		public override void Initialize ()
		{
			base.Initialize ();
			
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
	}
}
