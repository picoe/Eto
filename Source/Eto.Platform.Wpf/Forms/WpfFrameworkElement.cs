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
		sw.Size GetPreferredSize(sw.Size? constraint);
		sw.FrameworkElement ContainerControl { get; }
	}

	public static class ControlExtensions
	{
		public static IWpfFrameworkElement GetWpfFrameworkElement(this Control control)
		{
			if (control == null)
				return null;
			var handler = control.Handler as IWpfFrameworkElement;
			if (handler != null)
				return handler;
			var controlObject = control.ControlObject as Control;
			if (controlObject != null)
				return controlObject.GetWpfFrameworkElement();

			return null;
		}

		public static sw.FrameworkElement GetContainerControl(this Control control)
		{
			if (control == null)
				return null;
			var handler = control.Handler as IWpfFrameworkElement;
			if (handler != null)
				return handler.ContainerControl;

			var controlObject = control.ControlObject as Control;
			if (controlObject != null)
				return controlObject.GetContainerControl();

			return control.ControlObject as sw.FrameworkElement;
		}

		public static sw.Size GetPreferredSize(this Control control, sw.Size? available = null)
		{
			var handler = control.GetWpfFrameworkElement();
			if (handler != null)
				return handler.GetPreferredSize(available);
			return sw.Size.Empty;
		}
	}

	public static class WpfFrameworkElementHelper
	{
		public static bool ShouldCaptureMouse;
	}

	public abstract class WpfFrameworkElement<T, W> : WidgetHandler<T, W>, IControl, IWpfFrameworkElement
		where T : System.Windows.FrameworkElement
		where W : Control
	{
		Size? size;
		sw.Size preferredSize = new sw.Size(double.NaN, double.NaN);
		Size? newSize;
		Cursor cursor;
		bool loaded;
		bool isMouseOver;
		bool isMouseCaptured;

		public abstract Color BackgroundColor { get; set; }

		public virtual bool UseMousePreview { get { return false; } }

		public virtual sw.FrameworkElement ContainerControl
		{
			get { return this.Control; }
		}

		public virtual Size Size
		{
			get
			{
				var newSize = this.newSize;
				if (!Control.IsLoaded && size != null) return size.Value;
				else if (newSize != null) return newSize.Value;
				else return Conversions.GetSize(Control);
			}
			set
			{
				size = value;
				preferredSize.Width = (value.Width >= 0) ? value.Width : double.NaN;
				preferredSize.Height = (value.Height >= 0) ? value.Height : double.NaN;
				Conversions.SetSize(Control, value);
			}
		}

		public virtual sw.Size GetPreferredSize(sw.Size? constraint = null)
		{
			var size = preferredSize;
			if (double.IsNaN(size.Width) || double.IsNaN(size.Height) || constraint != null)
			{
				ContainerControl.Measure(constraint ?? new sw.Size(double.PositiveInfinity, double.PositiveInfinity));
				var desired = ContainerControl.DesiredSize;
				if (double.IsNaN(size.Width))
					size.Width = desired.Width;
				if (double.IsNaN(size.Height))
					size.Height = desired.Height;
			}
			return size;
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

		public virtual void Invalidate()
		{
			Control.InvalidateVisual();
		}

		public virtual void Invalidate(Rectangle rect)
		{
			Control.InvalidateVisual();
		}

		public void SuspendLayout()
		{

		}

		public void ResumeLayout()
		{
			//Control.UpdateLayout ();
		}

		public virtual void Focus()
		{
			if (Control.IsLoaded)
				Control.Focus();
			else
				Control.Loaded += (sender, e) =>
				{
					Control.Focus();
				};
		}

		protected virtual void EnsureLoaded()
		{
			if (!loaded)
			{
				Control.EnsureLoaded();
				loaded = true;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
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

		public override void AttachEvent(string handler)
		{
			var wpfcontrol = Control as swc.Control;
			switch (handler)
			{
				case Eto.Forms.Control.MouseMoveEvent:
					Control.MouseMove += (sender, e) =>
					{
						var args = e.ToEto(Control);
						Widget.OnMouseMove(args);
						e.Handled = args.Handled || isMouseCaptured;
					};
					break;
				case Eto.Forms.Control.MouseDownEvent:
					if (UseMousePreview)
						Control.PreviewMouseDown += HandleMouseDown;
					else
						Control.MouseDown += HandleMouseDown;
					HandleEvent(Eto.Forms.Control.MouseUpEvent);
					break;
				case Eto.Forms.Control.MouseDoubleClickEvent:
					if (wpfcontrol != null)
						if (UseMousePreview)
							wpfcontrol.PreviewMouseDoubleClick += HandleMouseDoubleClick;
						else
							wpfcontrol.MouseDoubleClick += HandleMouseDoubleClick;
					else
						HandleEvent(Eto.Forms.Control.MouseDownEvent);
					break;
				case Eto.Forms.Control.MouseUpEvent:
					if (UseMousePreview)
						Control.PreviewMouseUp += HandleMouseUp;
					else
						Control.MouseUp += HandleMouseUp;
					HandleEvent(Eto.Forms.Control.MouseDownEvent);
					break;
				case Eto.Forms.Control.MouseEnterEvent:
					Control.MouseEnter += (sender, e) =>
					{
						if (isMouseOver != Control.IsMouseOver)
						{
							var args = e.ToEto(Control);
							Widget.OnMouseEnter(args);
							e.Handled = args.Handled;
							isMouseOver = Control.IsMouseOver;
						}
					};
					break;
				case Eto.Forms.Control.MouseLeaveEvent:
					Control.MouseLeave += (sender, e) =>
					{
						if (isMouseOver != Control.IsMouseOver)
						{
							var args = e.ToEto(Control);
							Widget.OnMouseLeave(args);
							e.Handled = args.Handled;
							isMouseOver = Control.IsMouseOver;
						}
					};
					break;
				case Eto.Forms.Control.MouseWheelEvent:
					Control.PreviewMouseWheel += (sender, e) =>
					{
						var args = e.ToEto(Control);
						Widget.OnMouseLeave(args);
						e.Handled = args.Handled;
					};
					break;
				case Eto.Forms.Control.SizeChangedEvent:
					Control.SizeChanged += (sender, e) =>
					{
						this.newSize = e.NewSize.ToEtoSize(); // so we can report this back in Control.Size
						Widget.OnSizeChanged(EventArgs.Empty);
						this.newSize = null;
					};
					break;
				case Eto.Forms.Control.KeyDownEvent:
					Control.TextInput += (sender, e) =>
					{
						foreach (var keyChar in e.Text)
						{
							var key = Key.None;
							var args = new KeyEventArgs(key, KeyEventType.KeyDown, keyChar);
							Widget.OnKeyDown(args);
							e.Handled |= args.Handled;
						}
					};
					Control.KeyDown += (sender, e) =>
					{
						var args = e.ToEto(KeyEventType.KeyDown);
						Widget.OnKeyDown(args);
						e.Handled = args.Handled;
					};
					break;
				case Eto.Forms.Control.KeyUpEvent:
					Control.KeyUp += (sender, e) =>
					{
						var args = e.ToEto(KeyEventType.KeyUp);
						Widget.OnKeyUp(args);
						e.Handled = args.Handled;
					};
					break;
				case Eto.Forms.Control.ShownEvent:
					Control.IsVisibleChanged += (sender, e) =>
					{
						if ((bool)e.NewValue)
						{
							Widget.OnShown(EventArgs.Empty);
						}
					};
					break;
				case Eto.Forms.Control.GotFocusEvent:
					Control.GotFocus += (sender, e) =>
					{
						Widget.OnGotFocus(EventArgs.Empty);
					};
					break;
				case Eto.Forms.Control.LostFocusEvent:
					Control.LostFocus += (sender, e) =>
					{
						Widget.OnLostFocus(EventArgs.Empty);
					};
					break;
				default:
					base.AttachEvent(handler);
					break;
			}
		}

		void HandleMouseUp(object sender, swi.MouseButtonEventArgs e)
		{
			var args = e.ToEto(Control, swi.MouseButtonState.Released);
			Widget.OnMouseUp(args);
			e.Handled = args.Handled;
			if (Control.IsMouseCaptured && isMouseCaptured)
			{
				Control.ReleaseMouseCapture();
				isMouseCaptured = false;
			}
		}

		void HandleMouseDoubleClick(object sender, swi.MouseButtonEventArgs e)
		{
			var args = e.ToEto(Control);
			Widget.OnMouseDoubleClick(args);
			e.Handled = args.Handled;
		}

		void HandleMouseDown(object sender, swi.MouseButtonEventArgs e)
		{
			WpfFrameworkElementHelper.ShouldCaptureMouse = true;
			isMouseCaptured = false;
			var args = e.ToEto(Control);
			if (!(Control is swc.Control) && e.ClickCount == 2)
				Widget.OnMouseDoubleClick(args);
			if (!args.Handled)
				Widget.OnMouseDown(args);
			e.Handled = args.Handled || !WpfFrameworkElementHelper.ShouldCaptureMouse;
			if (WpfFrameworkElementHelper.ShouldCaptureMouse && (!UseMousePreview || e.Handled))
			{
				e.Handled = true;
				isMouseCaptured = true;
				Control.CaptureMouse();
			}
		}

		public virtual void OnLoad(EventArgs e)
		{
			Control.Tag = this;
			HandleEvent(Eto.Forms.Control.MouseDownEvent);
			HandleEvent(Eto.Forms.Control.MouseUpEvent);
		}

		public virtual void OnPreLoad(EventArgs e)
		{
		}

		public virtual void OnLoadComplete(EventArgs e)
		{
		}

		public virtual void OnUnLoad(EventArgs e)
		{
		}

		public virtual void SetParent(Container parent)
		{
			if (parent == null && Widget.Parent != null)
			{
				var currentParent = Widget.Parent.Handler as IWpfContainer;
				if (currentParent != null)
					currentParent.Remove(this.ContainerControl);
			}
		}

		public void MapPlatformAction(string systemAction, BaseAction action)
		{
		}

		public PointF PointFromScreen(PointF point)
		{
			if (Control.IsLoaded)
				return Control.PointFromScreen(point.ToWpf()).ToEto();
			else
				return point;
		}

		public PointF PointToScreen(PointF point)
		{
			if (Control.IsLoaded)
				return Control.PointToScreen(point.ToWpf()).ToEto();
			else
				return point;
		}

		public Point Location
		{
			get
			{
				if (Widget.Parent == null)
					return Point.Empty;
				else
					return Control.TranslatePoint(new sw.Point(0, 0), Widget.Parent.GetContainerControl()).ToEtoPoint();
			}
		}
	}
}
