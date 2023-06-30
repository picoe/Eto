using System.Windows.Interop;
using static System.Windows.WpfDataObjectExtensions;

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
		void DragLeave(sw.DragEventArgs e);

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

		[Obsolete("Use Control.GetPreferredSize instead")]
		public static sw.Size GetPreferredSize(Control control, sw.Size available)
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

	class WpfFrameworkElement
	{
		internal static readonly object Cursor_Key = new object();

		// source Eto control for drag operations
		internal static Control DragSourceControl { get; set; }

		internal const string CustomCursor_DataKey = "Eto.CustomCursor";

		internal static IWpfFrameworkElement LastDragTarget;
		
		internal static readonly object LoadActionList_Key = new object();
	}

	public abstract partial class WpfFrameworkElement<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Control.IHandler, IWpfFrameworkElement
		where TControl : System.Windows.FrameworkElement
		where TWidget : Control
		where TCallback : Control.ICallback
	{
		Size? newSize;
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

		sw.Size _userPreferredSize = new sw.Size(double.NaN, double.NaN);
		protected sw.Size UserPreferredSize
		{
			get => _userPreferredSize;
			set
			{
				_userPreferredSize = value;
				SetSize();
				ContainerControl.InvalidateMeasure();
				UpdatePreferredSize();
			}
		}

		protected virtual sw.Size DefaultSize => new sw.Size(double.NaN, double.NaN);

		/// <summary>
		/// This property, when set to true, will prevent the control from growing/shrinking based on user input.
		/// Typically, this will be accompanied by overriding the <see cref="DefaultSize"/> as well.
		/// 
		/// For example, when the user types into a text box, it will grow to fit the content if it is auto sized.
		/// This doesn't happen on any other platform, so we need to disable this behaviour on WPF.
		/// </summary>
		protected virtual bool PreventUserResize => false;

		public abstract Color BackgroundColor { get; set; }

		public virtual bool UseMousePreview => false;

		public virtual bool UseKeyPreview => false;

		public virtual bool UseDragDropPreview => UseMousePreview;

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

		public virtual sw.FrameworkElement ContainerControl => Control;

		public virtual Size Size
		{
			get
			{
				if (newSize != null)
					return newSize.Value;
				if (!Widget.Loaded)
					return UserPreferredSize.ToEtoSize();
				// if (Win32.IsSystemDpiAware && !double.IsNaN(Control.ActualWidth) && !double.IsNaN(Control.ActualHeight))
				// {
				// 	// convert system dpi to logical
				// 	var sizef = new SizeF((float)Control.ActualWidth, (float)Control.ActualHeight) / (Win32.GetDpiForSystem() / 96f) * SwfScreen.GetLogicalPixelSize();
				// 	return Size.Round(sizef);
				// }

				return Control.GetSize();
			}
			set
			{
				var newSize = value.ToWpf();
				if (UserPreferredSize == newSize)
					return;
				UserPreferredSize = newSize;
			}
		}

		public virtual int Width
		{
			get => Size.Width;
			set
			{
				var newWidth = value == -1 ? double.NaN : value;
				var userPreferredSize = UserPreferredSize;
				if (userPreferredSize.Width == newWidth)
					return;
				UserPreferredSize = new sw.Size(newWidth, userPreferredSize.Height);
			}
		}

		public virtual int Height
		{
			get => Size.Height;
			set
			{
				var newHeight = value == -1 ? double.NaN : value;
				var userPreferredSize = UserPreferredSize;
				if (userPreferredSize.Height == newHeight)
					return;
				UserPreferredSize = new sw.Size(userPreferredSize.Width, newHeight);
			}
		}

		public virtual void UpdatePreferredSize()
		{
			if (Widget.Loaded)
			{
				Widget.VisualParent.GetWpfContainer()?.OnChildPreferredSizeUpdated();
			}
		}

		public virtual void OnChildPreferredSizeUpdated()
		{
			if (double.IsNaN(UserPreferredSize.Width) || double.IsNaN(UserPreferredSize.Height))
				UpdatePreferredSize();
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
			if (XScale && Widget.Loaded)
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

			if (YScale && Widget.Loaded)
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

		public SizeF GetPreferredSize(SizeF availableSize)
		{
			var size = availableSize.ToWpf();
			if (ContainerControl.Parent == null && !(ContainerControl is swc.Panel) && !(ContainerControl is swc.Border))
			{
				// what the?!? some controls don't measure correctly when not in a container..
				var p = new swc.Border();
				p.Child = ContainerControl;
				p.Measure(size);
				var result = p.DesiredSize.ToEto();
				p.Child = null;
				return result;
			}
			ContainerControl.Measure(size);
			return ContainerControl.DesiredSize.ToEto();
		}

		public virtual bool Enabled
		{
			get { return ContainerControl.IsEnabled; }
			set { ContainerControl.IsEnabled = value; }
		}

		public virtual Cursor Cursor
		{
			get => Widget.Properties.Get<Cursor>(WpfFrameworkElement.Cursor_Key);
			set
			{
				if (Widget.Properties.TrySet(WpfFrameworkElement.Cursor_Key, value))
				{
					ContainerControl.Cursor = (value?.Handler as CursorHandler)?.Control;
				}
			}
		}

		public string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
		}

		public virtual bool AllowDrop
		{
			get { return Control.AllowDrop; }
			set { Control.AllowDrop = value; }
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

		protected virtual sw.FrameworkElement FocusControl => Control;

		public virtual void Focus()
		{
			if (FocusControl.IsLoaded)
				FocusControl.Focus();
			else
				FocusControl.Loaded += HandleFocus;
		}

		void HandleFocus(object sender, sw.RoutedEventArgs e)
		{
			FocusControl.Focus();
			FocusControl.Loaded -= HandleFocus;
		}

		protected virtual void EnsureLoaded()
		{
			Control.EnsureLoaded();
		}

		public virtual bool HasFocus
		{
			get { return Control.IsKeyboardFocusWithin; }
		}

		public virtual bool Visible
		{
			get { return ContainerControl.Visibility == sw.Visibility.Visible; }
			set
			{
				if (value != Visible)
				{
					ContainerControl.Visibility = (value) ? sw.Visibility.Visible : sw.Visibility.Collapsed;
					UpdatePreferredSize();
				}
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
					ContainerControl.LostMouseCapture += HandleLostMouseCapture;
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
					ContainerControl.MouseEnter += HandleMouseEnter;
					break;
				case Eto.Forms.Control.MouseLeaveEvent:
					HandleEvent(Eto.Forms.Control.MouseEnterEvent);
					ContainerControl.MouseLeave += HandleMouseLeave;
					break;
				case Eto.Forms.Control.MouseWheelEvent:
					ContainerControl.PreviewMouseWheel += HandlePreviewMouseWheel;
					break;
				case Eto.Forms.Control.SizeChangedEvent:
					ContainerControl.SizeChanged += HandleSizeChanged;
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
					Control.KeyUp += HandleKeyUp;
					break;
				case Eto.Forms.Control.ShownEvent:
					ContainerControl.IsVisibleChanged += HandleIsVisibleChanged;
					break;
				case Eto.Forms.Control.GotFocusEvent:
					Control.IsKeyboardFocusWithinChanged += HandleIsKeyboardFocusWithinChanged;
					break;
				case Eto.Forms.Control.LostFocusEvent:
					HandleEvent(Eto.Forms.Control.GotFocusEvent);
					break;
				case Eto.Forms.Control.DragDropEvent:
					if (UseDragDropPreview)
						Control.PreviewDrop += Control_DragDrop;
					else
						Control.Drop += Control_DragDrop;
					break;
				case Eto.Forms.Control.DragOverEvent:
					if (UseDragDropPreview)
						Control.PreviewDragOver += Control_DragOver;
					else
						Control.DragOver += Control_DragOver;
					HandleEvent(Eto.Forms.Control.DragLeaveEvent);
					break;
				case Eto.Forms.Control.DragEnterEvent:
					if (UseDragDropPreview)
						Control.PreviewDragEnter += Control_DragEnter;
					else
						Control.DragEnter += Control_DragEnter;
					HandleEvent(Eto.Forms.Control.DragOverEvent);
					break;
				case Eto.Forms.Control.DragLeaveEvent:
					if (UseDragDropPreview)
						Control.PreviewDragLeave += Control_DragLeave;
					else
						Control.DragLeave += Control_DragLeave;
					HandleEvent(Eto.Forms.Control.DragEnterEvent); // need DragEnter so it doesn't get called when going over children
					break;
				case Eto.Forms.Control.DragEndEvent:
					// handled in DoDragDrop, as it is blocking on Windows
					break;
				case Eto.Forms.Control.EnabledChangedEvent:
					Control.IsEnabledChanged += Control_IsEnabledChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void HandleIsKeyboardFocusWithinChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
		{
			// sometimes this gets called after disposed? Happens with designer in VS 2017
			if (Control == null)
				return;
			if (HasFocus)
				Callback.OnGotFocus(Widget, EventArgs.Empty);
			else
				Callback.OnLostFocus(Widget, EventArgs.Empty);
		}

		private void HandlePreviewMouseWheel(object sender, swi.MouseWheelEventArgs e)
		{
			if (Control == null)
				return;
			var args = e.ToEto(Control);
			Callback.OnMouseWheel(Widget, args);
			e.Handled = args.Handled;
		}

		private void HandleIsVisibleChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				Callback.OnShown(Widget, EventArgs.Empty);
			}
		}

		private void HandleSizeChanged(object sender, sw.SizeChangedEventArgs e)
		{
			if (Control == null)
				return;
			var size = e.NewSize.ToEtoSize();
			var prev = e.PreviousSize.ToEtoSize();
			if (size != prev) // WPF calls this event even if it hasn't changed
			{
				newSize = size; // so we can report this back in Control.Size
				Callback.OnSizeChanged(Widget, EventArgs.Empty);
				newSize = null;
			}
		}

		private void HandleMouseLeave(object sender, swi.MouseEventArgs e)
		{
			if (Control == null || isMouseOver == Control.IsMouseOver)
				return;

			var args = e.ToEto(Control);
			Callback.OnMouseLeave(Widget, args);
			e.Handled = args.Handled;
			isMouseOver = Control.IsMouseOver;
		}

		private void HandleMouseEnter(object sender, swi.MouseEventArgs e)
		{
			if (Control == null || isMouseOver == Control.IsMouseOver)
				return;

			var args = e.ToEto(Control);
			Callback.OnMouseEnter(Widget, args);
			e.Handled = args.Handled;
			isMouseOver = Control.IsMouseOver;
		}

		private void Control_DragDrop(object sender, sw.DragEventArgs e)
		{
			var args = GetDragEventArgs(e, null);
			HandleDrop(e, args);
		}

		private void Control_DragOver(object sender, sw.DragEventArgs e)
		{
			var args = GetDragEventArgs(e, null);
			HandleDragOver(e, args);
		}

		private void Control_IsEnabledChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
		{
			if (Control == null)
				return;
			Callback.OnEnabledChanged(Widget, EventArgs.Empty);
		}

		private void Control_DragEnter(object sender, sw.DragEventArgs e)
		{
			IsDragLeaving = false;
			var args = GetDragEventArgs(e, null);

			if (!IsDragEntered)
			{
				IsDragEntered = true;
				WpfFrameworkElement.LastDragTarget?.DragLeave(e);
				WpfFrameworkElement.LastDragTarget = null;
				HandleDragEnter(e, args);
			}
			else
			{
				// we re-entered, treat as another drag over so we can set the proper drag effect again
				HandleDragOver(e, args);
			}
		}

		void IWpfFrameworkElement.DragLeave(sw.DragEventArgs e)
		{
			if (IsDragLeaving)
			{
				IsDragLeaving = false;
				IsDragEntered = false;
				if (_lastDragArgs != null)
				{
					HandleDragLeave(e, GetDragEventArgs(_lastDragArgs, null));
					_lastDragArgs = null;
				}
			}
		}

		sw.DragEventArgs _lastDragArgs;

		void Control_DragLeave(object sender, sw.DragEventArgs e)
		{
			if (IsDragEntered)
			{
				IsDragLeaving = true;
				_lastDragArgs = e;
				WpfFrameworkElement.LastDragTarget = this;
				Control.Dispatcher.BeginInvoke(new Action(() =>
				{
					if (IsDragLeaving)
					{
						IsDragEntered = false;
						IsDragLeaving = false;
						WpfFrameworkElement.LastDragTarget = null;
						_lastDragArgs = null;
						HandleDragLeave(e, GetDragEventArgs(e, null));
					}
				}));
				//e.Handled = true;
			}
		}

		static readonly object IsDragEntered_Key = new object();

		bool IsDragEntered
		{
			get { return Widget.Properties.Get<bool>(IsDragEntered_Key); }
			set { Widget.Properties.Set(IsDragEntered_Key, value); }
		}

		static readonly object IsDragLeaving_Key = new object();

		bool IsDragLeaving
		{
			get { return Widget.Properties.Get<bool>(IsDragLeaving_Key); }
			set { Widget.Properties.Set(IsDragLeaving_Key, value); }
		}


		protected virtual void HandleDrop(sw.DragEventArgs e, DragEventArgs args)
		{
			if (e.Data.GetDataPresent(WpfFrameworkElement.CustomCursor_DataKey))
				e.Data.SetDataEx(WpfFrameworkElement.CustomCursor_DataKey, false);
			IsDragEntered = false;
			if (dragEnterEffects != null)
				args.Effects = dragEnterEffects.Value;
			Callback.OnDragLeave(Widget, args);
			Callback.OnDragDrop(Widget, args);
			e.Effects = args.Effects.ToWpf();
			e.Handled = true;
		}

		DragEffects? dragEnterEffects;

		protected virtual void HandleDragEnter(sw.DragEventArgs e, DragEventArgs args)
		{
			var lastCursor = MouseHandler.s_CursorSetCount;
			Callback.OnDragEnter(Widget, args);
			e.Effects = args.Effects.ToWpf();
			dragEnterEffects = args.Effects;
			e.Handled = true;

			if (lastCursor != MouseHandler.s_CursorSetCount)
				e.Data.SetDataEx(WpfFrameworkElement.CustomCursor_DataKey, true);
		}

		protected virtual void HandleDragLeave(sw.DragEventArgs e, DragEventArgs args)
		{
			if (e.Data.GetDataPresent(WpfFrameworkElement.CustomCursor_DataKey))
				e.Data.SetDataEx(WpfFrameworkElement.CustomCursor_DataKey, false);
			dragEnterEffects = null;
			Callback.OnDragLeave(Widget, args);
			if (sw.DropTargetHelper.IsSupported(e.Data))
			{
				sw.WpfDataObjectExtensions.SetDropDescription(e.Data, sw.DropImageType.Invalid, null, null);
				sw.DragSourceHelper.SetDropDescriptionIsDefault(e.Data, true);
			}
		}

		protected virtual void HandleDragOver(sw.DragEventArgs e, DragEventArgs args)
		{
			var lastCursor = MouseHandler.s_CursorSetCount;
			if (dragEnterEffects != null)
				args.Effects = dragEnterEffects.Value;
			Callback.OnDragOver(Widget, args);
			e.Effects = args.Effects.ToWpf();

			e.Handled = true;
			if (lastCursor != MouseHandler.s_CursorSetCount)
				e.Data.SetDataEx(WpfFrameworkElement.CustomCursor_DataKey, true);
		}

		protected virtual DragEventArgs GetDragEventArgs(sw.DragEventArgs data, object controlObject)
		{
			var dragData = (data.Data as sw.DataObject).ToEto();

			Control source = WpfFrameworkElement.DragSourceControl;

			var location = data.GetPosition(Control).ToEto();
			var modifiers = Keys.None;
			if (data.KeyStates.HasFlag(sw.DragDropKeyStates.AltKey))
				modifiers |= Keys.Alt;
			if (data.KeyStates.HasFlag(sw.DragDropKeyStates.ControlKey))
				modifiers |= Keys.Control;
			if (data.KeyStates.HasFlag(sw.DragDropKeyStates.ShiftKey))
				modifiers |= Keys.Shift;
			var buttons = MouseButtons.None;
			if (data.KeyStates.HasFlag(sw.DragDropKeyStates.LeftMouseButton))
				buttons |= MouseButtons.Primary;
			if (data.KeyStates.HasFlag(sw.DragDropKeyStates.RightMouseButton))
				buttons |= MouseButtons.Alternate;
			if (data.KeyStates.HasFlag(sw.DragDropKeyStates.MiddleMouseButton))
				buttons |= MouseButtons.Middle;
			return new WpfDragEventArgs(source, dragData, data.AllowedEffects.ToEto(), location, modifiers, buttons, controlObject);
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
			if (args.KeyData != Keys.None)
			{
				Callback.OnKeyDown(Widget, args);
				e.Handled = args.Handled;
			}
		}

		void HandleKeyUp(object sender, swi.KeyEventArgs e)
		{
			var args = e.ToEto(KeyEventType.KeyUp);
			if (args.KeyData != Keys.None)
			{
				Callback.OnKeyUp(Widget, args);
				e.Handled = args.Handled;
			}
		}

		void HandleMouseMove(object sender, swi.MouseEventArgs e)
		{
			var args = e.ToEto(ContainerControl);
			Callback.OnMouseMove(Widget, args);
			e.Handled = args.Handled;
		}

		protected virtual void HandleMouseUp(object sender, swi.MouseButtonEventArgs e)
		{
			var args = e.ToEto(ContainerControl, swi.MouseButtonState.Released);
			if (isMouseCaptured)
			{
				isMouseCaptured = false;
				if (Control.IsMouseCaptured)
					Control.ReleaseMouseCapture();
			}

			Callback.OnMouseUp(Widget, args);
			e.Handled = args.Handled;
			
			// If the mouse was captured intrinsically we need to release capture otherwise it hangs the app
			// since the caller is overriding default behaviour.
			if (e.Handled && Control.IsMouseCaptured)
				Control.ReleaseMouseCapture();
		}

		void HandleMouseDoubleClick(object sender, swi.MouseButtonEventArgs e)
		{
			var args = e.ToEto(ContainerControl);
			Callback.OnMouseDoubleClick(Widget, args);
			e.Handled = args.Handled;
		}
		
		protected virtual void HandleLostMouseCapture(object sender, swi.MouseEventArgs e)
		{
			if (isMouseCaptured)
			{
				// lost mouse capture without a MouseUp event firing
				// this can happen when something happens during the mouse dragging, such as showing a dialog.
				isMouseCaptured = false;

				var args = e.ToEto(ContainerControl, swi.MouseButtonState.Released);
				Callback.OnMouseUp(Widget, args);
			}
		}

		protected virtual void HandleMouseDown(object sender, swi.MouseButtonEventArgs e)
		{
            isMouseCaptured = false;
            var args = e.ToEto(ContainerControl);
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
				Control.CaptureMouse();
				isMouseCaptured = true;
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
			if (Widget.IsAttached)
			{
				SetDefaultScale();
			}
		}
		
		protected virtual void SetDefaultScale() => SetScale(true, true);

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
			if (NeedsPixelSizeNotifications && Win32.PerMonitorDpiSupported)
			{
				var parent = Widget.ParentWindow;
				if (parent != null)
					parent.LogicalPixelSizeChanged -= Parent_PixelSizeChanged;
			}
		}

		public virtual void SetParent(Container oldParent, Container newParent)
		{
			if (newParent == null && Widget.VisualParent != null)
			{
				// don't use GetWpfContainer() extension as we don't want to traverse themed controls
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

		System.Windows.Forms.Screen SwfScreen => Win32.GetScreenFromWindow(Widget.ParentWindow?.NativeHandle ?? IntPtr.Zero);

		public PointF PointFromScreen(PointF point)
		{
			if (!ContainerControl.IsLoaded)
				return point;

			// ensure we're connected to a presentation source
			var presentationSource = sw.PresentationSource.FromVisual(ContainerControl) as HwndSource;
			if (presentationSource == null)
				return point;

			point = point.LogicalToScreen(Widget.ParentWindow?.Screen);
			
			if (Win32.IsSystemDpiAware)
			{
				var logicalPixelSize = Win32.GetLogicalPixelSize(SwfScreen);
				var systemDpi = Win32.SystemDpi;

				// WPF does not take into account the location of the element in the form..
				var rootVisual = ContainerControl.GetVisualParents().OfType<sw.UIElement>().Last();
				var location = ContainerControl.TranslatePoint(new sw.Point(0, 0), rootVisual).ToEto();
				point -= (location * logicalPixelSize) - (location * systemDpi);

				point = Win32.ExecuteInDpiAwarenessContext(() => ContainerControl.PointFromScreen(point.ToWpf())).ToEto();

				point = point * systemDpi / logicalPixelSize;
			}
			else
			{
				point = Win32.ExecuteInDpiAwarenessContext(() => ContainerControl.PointFromScreen(point.ToWpf())).ToEto();
			}
			return point;

		}

		public PointF PointToScreen(PointF point)
		{
			if (!ContainerControl.IsLoaded)
				return point;
			
			// ensure we're connected to a presentation source
			var presentationSource = sw.PresentationSource.FromVisual(ContainerControl) as HwndSource;
			if (presentationSource == null)
				return point;

			PointF pt;
			if (Win32.IsSystemDpiAware)
			{
				var logicalPixelSize = Win32.GetLogicalPixelSize(SwfScreen);
				var systemDpi = Win32.SystemDpi;
				point = point / systemDpi * logicalPixelSize;

				pt = Win32.ExecuteInDpiAwarenessContext(() => ContainerControl.PointToScreen(point.ToWpf())).ToEto();
				
				// WPF does not take into account the location of the element in the form..
				var rootVisual = ContainerControl.GetVisualParents().OfType<sw.UIElement>().Last();
				var location = ContainerControl.TranslatePoint(new sw.Point(0, 0), rootVisual).ToEto();
				pt += (location * logicalPixelSize) - (location * systemDpi);
			}
			else
			{
				pt = Win32.ExecuteInDpiAwarenessContext(() => ContainerControl.PointToScreen(point.ToWpf())).ToEto();
			}

			return Point.Truncate(pt).ScreenToLogical(SwfScreen);
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

		public void DoDragDrop(DataObject data, DragEffects allowedAction, Image image, PointF offset)
		{
			WpfFrameworkElementHelper.ShouldCaptureMouse = false;
			var dataObject = data.ToWpf();

			WpfFrameworkElement.DragSourceControl = Widget;

			sw.DragSourceHelper.RegisterDefaultDragSource(Control, dataObject);
			sw.DragSourceHelper.AllowDropDescription(true);

			sw.WpfDataObjectExtensions.SetDropDescription(dataObject, sw.DropImageType.Invalid, null, null);
			if (image != null)
			{
				sw.WpfDataObjectExtensions.SetDragImage(dataObject, image.ToWpf(), offset.ToWpf());
			}
			else
			{
				// no image, but use new drag manager anyway because it's awesome
				image = new Bitmap(1, 1, PixelFormat.Format32bppRgba);
				sw.WpfDataObjectExtensions.SetDragImage(dataObject, image.ToWpf(), PointF.Empty.ToWpf());
			}

			var effects = sw.DragDrop.DoDragDrop(Control, dataObject, allowedAction.ToWpf());

			WpfFrameworkElement.DragSourceControl = null;
			sw.DragSourceHelper.UnregisterDefaultDragSource(Control);
			
			var args = new DragEventArgs(Widget, data, allowedAction, PointFromScreen(Mouse.Position), Keyboard.Modifiers, Mouse.Buttons);
			args.Effects = effects.ToEto();
			Callback.OnDragEnd(Widget, args);
		}


		public virtual Window GetNativeParentWindow()
		{
			if (!Widget.Loaded)
				return null;
			// hosted in a WPF window
			var window = Control.GetParent<sw.Window>().ToEtoWindow();
			if (window != null)
				return window;

			// possibly hosted in win32/winforms/mfc using an ElementHost, find it and get the parent hwnd
			var last = Control.GetParents().Last() as swm.Visual;
			if (last == null)
				return null;

			// get the hwndsource, if it's null it hasn't been shown/realized yet.
			var presentationSource = sw.PresentationSource.FromVisual(last) as HwndSource;
			if (presentationSource == null)
				return null;

			IntPtr parentHandle = presentationSource.Handle;
			if (parentHandle == IntPtr.Zero)
				return null;

			// get the root window (without traversing owners)
			IntPtr handle = Win32.GetAncestor(parentHandle, Win32.GA.GA_ROOT);
			if (handle == IntPtr.Zero)
				return null;

			// if it's a windows forms control, use that
			var winform = swf.Control.FromHandle(handle) as swf.Form;
			if (winform != null)
				return WinFormsHelpers.ToEto(winform);

			// otherwise, we're hosted it something native like win32 or mfc.
			// TODO: check if handle is a window?
			return WinFormsHelpers.ToEtoWindow(handle);
		}

		protected void AttachPropertyChanged(sw.DependencyProperty property, EventHandler<sw.DependencyPropertyChangedEventArgs> handler, sw.DependencyObject control = null, object key = null)
		{
			control = control ?? Control;
			Widget.Properties.Set(key ?? property, PropertyChangeNotifier.Register(property, handler, control));
		}

		public void Print()
		{
			var dlg = new swc.PrintDialog();
			if (dlg.ShowDialog() == true)
			{
				dlg.PrintVisual(ContainerControl, string.Empty);
			}
			WpfFrameworkElementHelper.ShouldCaptureMouse = false;
		}

		internal bool PerformOnLoad(Action action)
		{
			if (Control.IsLoaded)
				return false;
			var actionList = Widget.Properties.Get<List<Action>>(WpfFrameworkElement.LoadActionList_Key);
			if (actionList == null)
			{
				actionList = new List<Action>();
				Control.Loaded += (sender, e) =>
				{
					for (int i = 0; i < actionList.Count; i++)
					{
						actionList[i]();
					}
					actionList.Clear();
				};
				Widget.Properties.Set(WpfFrameworkElement.LoadActionList_Key, actionList);
			}
			actionList.Add(action);
			return true;
		}

		public void UpdateLayout()
		{
			// allow WPF controls to actually get their Loaded event fired.
			ContainerControl.Dispatcher.Invoke(new Action(() => { }), sw.Threading.DispatcherPriority.ApplicationIdle, null);

			// update the layout
			ContainerControl.UpdateLayout();
		}
	}
}
