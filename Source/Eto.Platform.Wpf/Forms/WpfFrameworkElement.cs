using System;
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
		sw.Size GetPreferredSize(sw.Size constraint);
		sw.FrameworkElement ContainerControl { get; }
		void SetScale(bool xscale, bool yscale);
		sw.Size ParentMinimumSize { get; set; }
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
			return controlObject != null ? controlObject.GetWpfFrameworkElement() : null;

		}

		public static IWpfContainer GetWpfContainer(this Container control)
		{
			if (control == null)
				return null;
			var handler = control.Handler as IWpfContainer;
			if (handler != null)
				return handler;
			var controlObject = control.ControlObject as Container;
			return controlObject != null ? controlObject.GetWpfContainer() : null;

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

		public static sw.Size GetPreferredSize(this Control control, sw.Size available)
		{
			var handler = control.GetWpfFrameworkElement();
			if (handler != null)
				return handler.GetPreferredSize(available);
			return Conversions.ZeroSize;
		}
	}

	public static class WpfFrameworkElementHelper
	{
		public static bool ShouldCaptureMouse;
	}

	public abstract class WpfFrameworkElement<TControl, TWidget> : WidgetHandler<TControl, TWidget>, IControl, IWpfFrameworkElement
		where TControl : System.Windows.FrameworkElement
		where TWidget : Control
	{
		sw.Size preferredSize = new sw.Size(double.NaN, double.NaN);
		Size? newSize;
		Cursor cursor;
		sw.Size parentMinimumSize;
		bool isMouseOver;
		bool isMouseCaptured;
		public bool XScale { get; private set; }
		public bool YScale { get; private set; }

		protected sw.Size PreferredSize { get { return preferredSize; } set { preferredSize = value; } }

		protected virtual Size DefaultSize { get { return Size.Empty; } }

		public abstract Color BackgroundColor { get; set; }

		public virtual bool UseMousePreview { get { return false; } }

		public virtual bool UseKeyPreview { get { return false; } }

		public sw.Size ParentMinimumSize
		{
			get { return parentMinimumSize; }
			set
			{
				parentMinimumSize = value;
				SetSize();
			}
		}

		public virtual sw.FrameworkElement ContainerControl { get { return Control; } }

		public virtual Size Size
		{
			get
			{
				if (newSize != null)
					return newSize.Value;
				if (!Control.IsLoaded)
					return preferredSize.ToEtoSize();
				return Control.GetSize();
			}
			set
			{
				preferredSize = value.ToWpf();
				SetSize();
			}
		}

		public virtual void SetScale(bool xscale, bool yscale)
		{
			XScale = xscale;
			YScale = yscale;
			SetSize();
		}

		protected virtual void SetSize()
		{
			ContainerControl.Width = XScale && Control.IsLoaded ? double.NaN : Math.Max(preferredSize.Width, parentMinimumSize.Width);
			ContainerControl.Height = YScale && Control.IsLoaded ? double.NaN : Math.Max(preferredSize.Height, parentMinimumSize.Height);
			var defaultSize = DefaultSize;
			ContainerControl.MinWidth = XScale && Control.IsLoaded ? 0 : Math.Max(0, double.IsNaN(preferredSize.Width) ? defaultSize.Width : preferredSize.Width);
			ContainerControl.MinHeight = YScale && Control.IsLoaded ? 0 : Math.Max(0, double.IsNaN(preferredSize.Height) ? defaultSize.Height : preferredSize.Height);
		}

		public virtual sw.Size GetPreferredSize(sw.Size constraint)
		{
			var size = preferredSize;
			if (double.IsNaN(size.Width) || double.IsNaN(size.Height))
			{
				ContainerControl.Measure(constraint);
				var desired = ContainerControl.DesiredSize;
				if (double.IsNaN(size.Width))
					size.Width = Math.Max(desired.Width, DefaultSize.Width);
				if (double.IsNaN(size.Height))
					size.Height = Math.Max(desired.Height, DefaultSize.Height);
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
				Control.Cursor = cursor != null ? ((CursorHandler)cursor.Handler).Control : null;
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
		}

		public virtual void Focus()
		{
			if (Control.IsLoaded)
				Control.Focus();
			else
				Control.Loaded += HandleFocus;
		}

		void HandleFocus(object sender, sw.RoutedEventArgs e)
		{
			Control.Focus();
			Control.Loaded -= HandleFocus;
		}

		protected virtual void EnsureLoaded()
		{
			Control.EnsureLoaded();
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

		public override void AttachEvent(string id)
		{
			var wpfcontrol = Control as swc.Control;
			switch (id)
			{
				case Eto.Forms.Control.MouseMoveEvent:
					if (UseMousePreview)
						ContainerControl.PreviewMouseMove += HandleMouseMove;
					else
						ContainerControl.MouseMove += HandleMouseMove;
					break;
				case Eto.Forms.Control.MouseDownEvent:
					if (UseMousePreview)
						ContainerControl.PreviewMouseDown += HandleMouseDown;
					else
						ContainerControl.MouseDown += HandleMouseDown;
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
						ContainerControl.PreviewMouseUp += HandleMouseUp;
					else
						ContainerControl.MouseUp += HandleMouseUp;
					HandleEvent(Eto.Forms.Control.MouseDownEvent);
					break;
				case Eto.Forms.Control.MouseEnterEvent:
					ContainerControl.MouseEnter += (sender, e) =>
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
					ContainerControl.MouseLeave += (sender, e) =>
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
					ContainerControl.PreviewMouseWheel += (sender, e) =>
					{
						var args = e.ToEto(Control);
						Widget.OnMouseLeave(args);
						e.Handled = args.Handled;
					};
					break;
				case Eto.Forms.Control.SizeChangedEvent:
					ContainerControl.SizeChanged += (sender, e) =>
					{
						newSize = e.NewSize.ToEtoSize(); // so we can report this back in Control.Size
						Widget.OnSizeChanged(EventArgs.Empty);
						newSize = null;
					};
					break;
				case Eto.Forms.Control.KeyDownEvent:
					if (UseKeyPreview)
					{
						Control.PreviewKeyDown += HandleKeyDown;
						Control.PreviewTextInput += HandleTextInput;
					}
					else
					{
						Control.KeyDown += HandleKeyDown;
						Control.TextInput += HandleTextInput;
					}
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
					ContainerControl.IsVisibleChanged += (sender, e) =>
					{
						if ((bool)e.NewValue)
						{
							Widget.OnShown(EventArgs.Empty);
						}
					};
					break;
				case Eto.Forms.Control.GotFocusEvent:
					Control.GotFocus += (sender, e) => Widget.OnGotFocus(EventArgs.Empty);
					break;
				case Eto.Forms.Control.LostFocusEvent:
					Control.LostFocus += (sender, e) => Widget.OnLostFocus(EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void HandleTextInput(object sender, swi.TextCompositionEventArgs e)
		{
			foreach (var keyChar in e.Text)
			{
				var key = Keys.None;
				var args = new KeyEventArgs(key, KeyEventType.KeyDown, keyChar);
				Widget.OnKeyDown(args);
				e.Handled |= args.Handled;
			}
		}

		void HandleKeyDown(object sender, swi.KeyEventArgs e)
		{
			var args = e.ToEto(KeyEventType.KeyDown);
			Widget.OnKeyDown(args);
			e.Handled = args.Handled;
		}

		void HandleMouseMove(object sender, swi.MouseEventArgs e)
		{
			var args = e.ToEto(Control);
			Widget.OnMouseMove(args);
			e.Handled = args.Handled || isMouseCaptured;
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
			isMouseCaptured = false;
			var args = e.ToEto(Control);
			if (!(Control is swc.Control) && e.ClickCount == 2)
				Widget.OnMouseDoubleClick(args);
			if (!args.Handled)
			{
				WpfFrameworkElementHelper.ShouldCaptureMouse = true;
				Widget.OnMouseDown(args);
			}
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
			Control.Loaded += Control_Loaded;
		}

		void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			SetSize();
		}

		public virtual void OnPreLoad(EventArgs e)
		{
		}

		public virtual void OnLoadComplete(EventArgs e)
		{
		}

		public virtual void OnUnLoad(EventArgs e)
		{
			SetScale(false, false);
		}

		public virtual void SetParent(Container parent)
		{
			if (parent == null && Widget.Parent != null)
			{
				var currentParent = Widget.Parent.Handler as IWpfContainer;
				if (currentParent != null)
					currentParent.Remove(ContainerControl);
			}
		}

		public void MapPlatformAction(string systemAction, BaseAction action)
		{
		}

		public PointF PointFromScreen(PointF point)
		{
			return Control.IsLoaded ? Control.PointFromScreen(point.ToWpf()).ToEto() : point;
		}

		public PointF PointToScreen(PointF point)
		{
			return Control.IsLoaded ? Control.PointToScreen(point.ToWpf()).ToEto() : point;
		}

		public Point Location
		{
			get
			{
				if (Widget.Parent == null)
					return Point.Empty;
				return Control.TranslatePoint(new sw.Point(0, 0), Widget.Parent.GetContainerControl()).ToEtoPoint();
			}
		}
	}
}
