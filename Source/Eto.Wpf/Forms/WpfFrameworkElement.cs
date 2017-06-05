using System;
using Eto.Forms;
using Eto.Drawing;
using sw = System.Windows;
using swi = System.Windows.Input;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Wpf.Forms
{
	public interface IEtoWpfControl
	{
		IWpfFrameworkElement Handler { get; set; }
	}
	public interface IWpfFrameworkElement
	{
		sw.Size MeasureOverride(sw.Size constraint, Func<sw.Size, sw.Size> measure);
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
			return WpfConversions.ZeroSize;
		}
	}

	public static class WpfFrameworkElementHelper
	{
		public static bool ShouldCaptureMouse;
	}

	public abstract class WpfFrameworkElement<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Control.IHandler, IWpfFrameworkElement
		where TControl : System.Windows.FrameworkElement
		where TWidget : Control
		where TCallback : Control.ICallback
	{
		Size? newSize;
		Cursor cursor;
		sw.Size parentMinimumSize;
		bool isMouseOver;
		bool isMouseCaptured;
		public bool XScale { get; private set; }
		public bool YScale { get; private set; }

		public override IntPtr NativeHandle
		{
			get
			{
				var hwnd = sw.PresentationSource.FromVisual(Control) as sw.Interop.HwndSource;
				return hwnd != null ? hwnd.Handle : IntPtr.Zero;
			}
		}

		public virtual sw.Size MeasureOverride(sw.Size constraint, Func<sw.Size, sw.Size> measure)
		{
			// enforce eto-style sizing to wpf controls
			var size = UserPreferredSize;
			var control = ContainerControl;

			// Constrain content to the preferred size of this control, if specified.
			var desired = measure(constraint.IfInfinity(size.InfinityIfNan()));
			// Desired size should not be smaller than default (minimum) size.
			// ensures buttons, text box, etc have a minimum size
			var defaultSize = DefaultSize.ZeroIfNan();
			desired = desired.Max(defaultSize);

			// Desired should also not be bigger than default size if we have no constraint.
			// Without it, controls like TextArea, GridView, etc will grow to their content.
			if (double.IsInfinity(constraint.Width) && defaultSize.Width > 0)
				desired.Width = PreventUserResize ? defaultSize.Width : Math.Max(defaultSize.Width, desired.Width);
			if (double.IsInfinity(constraint.Height) && defaultSize.Height > 0)
				desired.Height = PreventUserResize ? defaultSize.Height : Math.Max(defaultSize.Height, desired.Height);

			// use the user preferred size, and ensure it's not larger than available size
			size = size.IfNaN(desired);
			size = size.Min(constraint);

			// restrict to the min/max sizes
			size = size.Max(control.GetMinSize());
			size = size.Min(control.GetMaxSize());
			return size;
		}

		protected sw.Size UserPreferredSize { get; set; } = new sw.Size(double.NaN, double.NaN);

		protected virtual sw.Size DefaultSize => new sw.Size(double.NaN, double.NaN);

		/// <summary>
		/// This property, when set to true, will prevent the control from growing/shrinking based on user input.
		/// Typically, this will be accompanied by overriding the <see cref="DefaultSize"/> as well.
		/// 
		/// For example, when the user types into a text box, it will grow to fit the content if it is auto sized.
		/// This doesn't happen on any other platform, so we need to disable this behaviour on WPF.
		/// </summary>
		protected virtual bool PreventUserResize { get { return false; } }

		public abstract Color BackgroundColor { get; set; }

		public virtual bool UseMousePreview { get { return false; } }

		public virtual bool UseKeyPreview { get { return false; } }

		public sw.Size ParentMinimumSize
		{
			get { return parentMinimumSize; }
			set
			{
				if (parentMinimumSize != value)
				{
					parentMinimumSize = value;
					SetSize();
				}
			}
		}

		public virtual sw.FrameworkElement ContainerControl { get { return Control; } }

		public virtual Size Size
		{
			get
			{
				if (newSize != null)
					return newSize.Value;
				if (!Widget.Loaded)
					return UserPreferredSize.ToEtoSize();
				return Control.GetSize();
			}
			set
			{
				UserPreferredSize = value.ToWpf();
				SetSize();
				ContainerControl.InvalidateMeasure();
				UpdatePreferredSize();
			}
		}

        public virtual void UpdatePreferredSize()
        {
			if (Widget.Loaded)
			{
				Widget.VisualParent.GetWpfContainer()?.UpdatePreferredSize();
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
			// this is needed so that the control doesn't actually grow when its content is too large.
			// For some reason, MeasureOverride is not sufficient alone to tell WPF what size the control should be.
			// for example, when a TextBox has a large amount of text, we do not grow the text box to fit that content in Eto's sizing model.
			// ideally, this should be removed, but may require overriding ArrangeOverride on all controls as well.
			// see the ControlTests.ControlsShouldHaveSaneDefaultWidths unit test for repro of the issue.
			var defaultSize = DefaultSize.ZeroIfNan();
			if (XScale && Control.IsLoaded)
			{
				ContainerControl.Width = double.NaN;
				ContainerControl.MinWidth = 0;
			}
			else
			{
				var containerWidth = PreventUserResize && double.IsNaN(UserPreferredSize.Width)
					? defaultSize.Width <= 0
						? double.NaN
						: defaultSize.Width
					: UserPreferredSize.Width;
				ContainerControl.Width = Math.Max(containerWidth, parentMinimumSize.Width);
				ContainerControl.MinWidth = Math.Max(0, double.IsNaN(UserPreferredSize.Width) ? defaultSize.Width : UserPreferredSize.Width);
			}

			if (YScale && Control.IsLoaded)
			{
				ContainerControl.Height = double.NaN;
				ContainerControl.MinHeight = 0;
			}
			else
			{
				var containerHeight = PreventUserResize && double.IsNaN(UserPreferredSize.Height)
					? defaultSize.Height <= 0
						? double.NaN
						: defaultSize.Height
					: UserPreferredSize.Height;
				ContainerControl.Height = Math.Max(containerHeight, parentMinimumSize.Height);
				ContainerControl.MinHeight = Math.Max(0, double.IsNaN(UserPreferredSize.Height) ? defaultSize.Height : UserPreferredSize.Height);
			}
		}

		public virtual sw.Size GetPreferredSize(sw.Size constraint)
		{
			ContainerControl.Measure(constraint);
			return ContainerControl.DesiredSize;
		}

		public virtual bool Enabled
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

		public virtual void Invalidate(bool invalidateChildren)
		{
			Control.InvalidateVisual();
		}

		public virtual void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			Control.InvalidateVisual();
		}

		public virtual void SuspendLayout()
		{

		}

		public virtual void ResumeLayout()
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
			get { return Control.IsKeyboardFocusWithin; }
		}

		public bool Visible
		{
			get { return ContainerControl.Visibility != sw.Visibility.Collapsed; }
			set
			{
				ContainerControl.Visibility = (value) ? sw.Visibility.Visible : sw.Visibility.Collapsed;
                UpdatePreferredSize();
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
							Callback.OnMouseEnter(Widget, args);
							e.Handled = args.Handled;
							isMouseOver = Control.IsMouseOver;
						}
					};
					break;
				case Eto.Forms.Control.MouseLeaveEvent:
					HandleEvent(Eto.Forms.Control.MouseEnterEvent);
					ContainerControl.MouseLeave += (sender, e) =>
					{
						if (isMouseOver != Control.IsMouseOver)
						{
							var args = e.ToEto(Control);
							Callback.OnMouseLeave(Widget, args);
							e.Handled = args.Handled;
							isMouseOver = Control.IsMouseOver;
						}
					};
					break;
				case Eto.Forms.Control.MouseWheelEvent:
					ContainerControl.PreviewMouseWheel += (sender, e) =>
					{
						var args = e.ToEto(Control);
						Callback.OnMouseWheel(Widget, args);
						e.Handled = args.Handled;
					};
					break;
				case Eto.Forms.Control.SizeChangedEvent:
					ContainerControl.SizeChanged += (sender, e) =>
					{
						newSize = e.NewSize.ToEtoSize(); // so we can report this back in Control.Size
						Callback.OnSizeChanged(Widget, EventArgs.Empty);
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
				case Eto.Forms.Control.TextInputEvent:
					HandleEvent(Eto.Forms.Control.KeyDownEvent);
					break;
				case Eto.Forms.Control.KeyUpEvent:
					Control.KeyUp += (sender, e) =>
					{
						var args = e.ToEto(KeyEventType.KeyUp);
						if (args.Key != Keys.None)
						{
							Callback.OnKeyUp(Widget, args);
							e.Handled = args.Handled;
						}
					};
					break;
				case Eto.Forms.Control.ShownEvent:
					ContainerControl.IsVisibleChanged += (sender, e) =>
					{
						if ((bool)e.NewValue)
						{
							Callback.OnShown(Widget, EventArgs.Empty);
						}
					};
					break;
				case Eto.Forms.Control.GotFocusEvent:
					Control.IsKeyboardFocusWithinChanged += (sender, e) =>
					{
						if (HasFocus)
							Callback.OnGotFocus(Widget, EventArgs.Empty);
						else
							Callback.OnLostFocus(Widget, EventArgs.Empty);
					};
					break;
				case Eto.Forms.Control.LostFocusEvent:
					HandleEvent(Eto.Forms.Control.GotFocusEvent);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void HandleTextInput(object sender, swi.TextCompositionEventArgs e)
		{
			var tiargs = new TextInputEventArgs(e.Text);
			Callback.OnTextInput(Widget, tiargs);
			if (tiargs.Cancel)
			{
				e.Handled = true;
				return;
			}

			foreach (var keyChar in e.Text)
			{
				var key = Keys.None;
				var args = new KeyEventArgs(key, KeyEventType.KeyDown, keyChar);
				Callback.OnKeyDown(Widget, args);
				e.Handled |= args.Handled;
			}
		}

		void HandleKeyDown(object sender, swi.KeyEventArgs e)
		{
			var args = e.ToEto(KeyEventType.KeyDown);
			if (args.Key != Keys.None)
			{
				Callback.OnKeyDown(Widget, args);
				e.Handled = args.Handled;
			}
		}

		void HandleMouseMove(object sender, swi.MouseEventArgs e)
		{
			var args = e.ToEto(Control);
			Callback.OnMouseMove(Widget, args);
			e.Handled = args.Handled;
		}

		void HandleMouseUp(object sender, swi.MouseButtonEventArgs e)
		{
			var args = e.ToEto(Control, swi.MouseButtonState.Released);
			Callback.OnMouseUp(Widget, args);
			e.Handled = args.Handled;
			if (isMouseCaptured && Control.IsMouseCaptured)
			{
				Control.ReleaseMouseCapture();
				isMouseCaptured = false;
			}
		}

		void HandleMouseDoubleClick(object sender, swi.MouseButtonEventArgs e)
		{
			var args = e.ToEto(Control);
			Callback.OnMouseDoubleClick(Widget, args);
			e.Handled = args.Handled;
		}

		void HandleMouseDown(object sender, swi.MouseButtonEventArgs e)
		{
			var args = e.ToEto(Control);
			if (!(Control is swc.Control) && e.ClickCount == 2)
				Callback.OnMouseDoubleClick(Widget, args);
			if (!args.Handled)
			{
				WpfFrameworkElementHelper.ShouldCaptureMouse = true;
				Callback.OnMouseDown(Widget, args);
			}
			e.Handled = args.Handled || !WpfFrameworkElementHelper.ShouldCaptureMouse;
			if (WpfFrameworkElementHelper.ShouldCaptureMouse
				&& (
					// capture mouse automatically so mouse moves outside control are captured until released
					// but only if the control that was clicked is this control
					(!UseMousePreview && (e.OriginalSource == ContainerControl || e.OriginalSource == Control))
					|| e.Handled
				))
			{
				isMouseCaptured = true;
				Control.CaptureMouse();
			}
			else
			{
				isMouseCaptured = false;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Tag = this;
			HandleEvent(Eto.Forms.Control.MouseDownEvent);
			HandleEvent(Eto.Forms.Control.MouseUpEvent);
			Control.Loaded += Control_Loaded;
		}

		public virtual void OnLoad(EventArgs e)
		{
		}

		void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			SetSize();
			if (NeedsPixelSizeNotifications && Win32.PerMonitorDpiSupported)
				OnLogicalPixelSizeChanged();
		}

		public virtual void OnPreLoad(EventArgs e)
		{
		}

		public virtual void OnLoadComplete(EventArgs e)
		{
			if (NeedsPixelSizeNotifications && Win32.PerMonitorDpiSupported)
			{
				var parent = Widget.ParentWindow;
				if (parent != null)
				{
					parent.LogicalPixelSizeChanged += Parent_PixelSizeChanged;
				}
			}
		}

		protected float ParentScale
		{
			get { return Widget.ParentWindow?.LogicalPixelSize ?? Screen.PrimaryScreen.LogicalPixelSize; }
		}

		void Parent_PixelSizeChanged(object sender, EventArgs e)
		{
			OnLogicalPixelSizeChanged();
		}

		protected virtual bool NeedsPixelSizeNotifications
		{
			get { return false; }
		}

		protected virtual void OnLogicalPixelSizeChanged()
		{
		}


		public virtual void OnUnLoad(EventArgs e)
		{
			SetScale(false, false);

			if (NeedsPixelSizeNotifications && Win32.PerMonitorDpiSupported)
			{
				var parent = Widget.ParentWindow;
				if (parent != null)
					parent.LogicalPixelSizeChanged -= Parent_PixelSizeChanged;
			}
		}

		public virtual void SetParent(Container parent)
		{
			if (parent == null && Widget.VisualParent != null)
			{
				var currentParent = Widget.VisualParent.Handler as IWpfContainer;
				if (currentParent != null)
					currentParent.Remove(ContainerControl);
			}
		}

		public IEnumerable<string> SupportedPlatformCommands
		{
			get { return Enumerable.Empty<string>(); }
		}

		public void MapPlatformCommand(string systemAction, Command command)
		{
		}

		public PointF PointFromScreen(PointF point)
		{
			if (!ContainerControl.IsLoaded)
				return point;

			point = point.LogicalToScreen();
			return ContainerControl.PointFromScreen(point.ToWpf()).ToEto();
		}

		public PointF PointToScreen(PointF point)
		{
			if (!ContainerControl.IsLoaded)
				return point;
			return ContainerControl.PointToScreen(point.ToWpf()).ToEtoPoint().ScreenToLogical();
		}

		public Point Location
		{
			get
			{
				if (Widget.VisualParent == null)
					return Point.Empty;
				return Control.TranslatePoint(new sw.Point(0, 0), Widget.VisualParent.GetContainerControl()).ToEtoPoint();
			}
		}

		public virtual sw.FrameworkElement TabControl => ContainerControl;
		public virtual int TabIndex
		{
			get { return swi.KeyboardNavigation.GetTabIndex(TabControl); }
			set { swi.KeyboardNavigation.SetTabIndex(TabControl, value); }
		}

		public virtual IEnumerable<Control> VisualControls => Enumerable.Empty<Control>();
	}
}
