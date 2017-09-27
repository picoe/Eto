using System;
using Eto.Forms;
using Eto.Drawing;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
using swi = Windows.UI.Xaml.Input;
using swc = Windows.UI.Xaml.Controls;
using swm = Windows.UI.Xaml.Media;
using wuc = Windows.UI.Core;
using System.Linq;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace Eto.WinRT.Forms
{
	/// <summary>
	/// Xaml Framework element handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IWpfFrameworkElement
	{
		wf.Size GetPreferredSize(wf.Size constraint);
		sw.FrameworkElement ContainerControl { get; }
		void SetScale(bool xscale, bool yscale);
		wf.Size ParentMinimumSize { get; set; }
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

		public static wf.Size GetPreferredSize(this Control control, wf.Size available)
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

	public abstract class WpfFrameworkElement<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Control.IHandler, IWpfFrameworkElement
		where TControl : FrameworkElement
		where TWidget : Control
		where TCallback : Control.ICallback
	{
		wf.Size preferredSize = new wf.Size(double.NaN, double.NaN);
		Size? newSize;
		Cursor cursor;
		wf.Size parentMinimumSize;
		//bool isMouseOver;
		//bool isMouseCaptured;
		public bool XScale { get; private set; }
		public bool YScale { get; private set; }

		protected wf.Size PreferredSize { get { return preferredSize; } set { preferredSize = value; } }

		protected virtual wf.Size DefaultSize { get { return new wf.Size(double.NaN, double.NaN); } }

		public abstract Color BackgroundColor { get; set; }

		public virtual bool UseMousePreview { get { return false; } }

		public virtual bool UseKeyPreview { get { return false; } }

		public wf.Size ParentMinimumSize
		{
			get { return parentMinimumSize; }
			set
			{
				parentMinimumSize = value;
				SetSize();
			}
		}

		public virtual sw.FrameworkElement ContainerControl { get { return Control; } }

		bool ControlIsLoaded
		{
			get
			{
#if TODO_XAML
				return Control.IsLoaded;
#else
				return true;
#endif
			}
		}

		void InvalidateControlVisual()
		{
#if TODO_XAML
			Control.InvalidateVisual();
#else
			Control.InvalidateArrange();
			Control.InvalidateMeasure();
#endif
		}

		public virtual Size Size
		{
			get
			{
				if (newSize != null)
					return newSize.Value;
				if (!ControlIsLoaded)
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
			ContainerControl.Width = XScale && ControlIsLoaded ? double.NaN : Math.Max(preferredSize.Width, parentMinimumSize.Width);
			ContainerControl.Height = YScale && ControlIsLoaded ? double.NaN : Math.Max(preferredSize.Height, parentMinimumSize.Height);
			var defaultSize = DefaultSize;
			ContainerControl.MinWidth = XScale && ControlIsLoaded ? 0 : Math.Max(0, double.IsNaN(preferredSize.Width) ? defaultSize.Width : preferredSize.Width);
			ContainerControl.MinHeight = YScale && ControlIsLoaded ? 0 : Math.Max(0, double.IsNaN(preferredSize.Height) ? defaultSize.Height : preferredSize.Height);
		}

		public virtual wf.Size GetPreferredSize(wf.Size constraint)
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

		public virtual bool Enabled
		{
#if TODO_XAML
			get { return Control.IsEnabled; }
			set { Control.IsEnabled = value; }
#else
			get; set;
#endif
		}

		public virtual Cursor Cursor
		{
			get { return cursor; }
			set
			{
				cursor = value;
#if TODO_XAML
				Control.Cursor = cursor != null ? ((CursorHandler)cursor.Handler).Control : null;
#else
				throw new NotImplementedException();
#endif
			}
		}

		public string ToolTip
		{
#if TODO_XAML
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
#else
			get; set;
#endif
		}

		public virtual void Invalidate(bool invalidateChildren)
		{
			InvalidateControlVisual();
		}

		public virtual void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			InvalidateControlVisual();
		}

		public void SuspendLayout()
		{

		}

		public void ResumeLayout()
		{
		}

		public virtual void Focus()
		{
#if TODO_XAML
			if (ControlIsLoaded)
				Control.Focus();
			else
				Control.Loaded += HandleFocus;
#else
			//throw new NotImplementedException();
#endif
		}

		void HandleFocus(object sender, sw.RoutedEventArgs e)
		{
#if TODO_XAML
			Control.Focus();
			Control.Loaded -= HandleFocus;
#else
			throw new NotImplementedException();
#endif
		}

		protected virtual void EnsureLoaded()
		{
			Control.EnsureLoaded();
		}

		public virtual bool HasFocus
		{
#if TODO_XAML
			get { return Control.IsFocused; }
#else
			get { return Control.HasFocus(null); }
#endif
		}

		public bool Visible
		{
			get { return Control.Visibility == Visibility.Visible; }
			set
			{
				Control.Visibility = (value) ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public override void AttachEvent(string id)
		{
			var wpfcontrol = Control as swc.Control;
			switch (id)
			{
#if TODO_XAML
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
#endif
				case Eto.Forms.Control.SizeChangedEvent:
					Size? oldSize = null;
					ContainerControl.SizeChanged += (sender, e) =>
					{
						newSize = e.NewSize.ToEtoSize(); // so we can report this back in Control.Size
						if (newSize != oldSize)
						{
							Callback.OnSizeChanged(Widget, EventArgs.Empty);
							oldSize = newSize;
						}
						newSize = null;
					};
					break;
				case Eto.Forms.Control.KeyDownEvent:
#if TODO_XAML
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
#endif
					break;
				case Eto.Forms.Control.KeyUpEvent:
					Control.KeyUp += (sender, e) =>
					{
#if TODO_XAML
						var args = e.ToEto(KeyEventType.KeyUp);
						Widget.OnKeyUp(args);
						e.Handled = args.Handled;
#endif
					};
					break;
				case Eto.Forms.Control.ShownEvent:
#if TODO_XAML
					ContainerControl.IsVisibleChanged += (sender, e) =>
					{
						if ((bool)e.NewValue)
						{
							Widget.OnShown(EventArgs.Empty);
						}
					};
#endif
					break;
				case Eto.Forms.Control.GotFocusEvent:
					Control.GotFocus += (sender, e) => Callback.OnGotFocus(Widget, EventArgs.Empty);
					break;
				case Eto.Forms.Control.LostFocusEvent:
					Control.LostFocus += (sender, e) => Callback.OnLostFocus(Widget, EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

#if TODO_XAML
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

		void HandleKeyDown(object sender, wuc.KeyEventArgs e)
		{
			var args = e.ToEto(KeyEventType.KeyDown);
			Widget.OnKeyDown(args);
			e.Handled = args.Handled;
		}
#endif
		
#if TODO_XAML
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
#endif

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

		public IEnumerable<string> SupportedPlatformCommands
		{
			get { return Enumerable.Empty<string>(); }
		}

		public void MapPlatformCommand(string systemAction, Command command)
		{
		}

		public PointF PointFromScreen(PointF point)
		{
#if TODO_XAML
			return ControlIsLoaded ? Control.PointFromScreen(point.ToWpf()).ToEto() : point;
#else
			throw new NotImplementedException();
#endif
		}

		public PointF PointToScreen(PointF point)
		{
#if TODO_XAML
			return ControlIsLoaded ? Control.PointToScreen(point.ToWpf()).ToEto() : point;
#else
			throw new NotImplementedException();
#endif
		}

		public Point Location
		{
			get
			{
				if (Widget.VisualParent == null)
					return Point.Empty;
#if TODO_XAML
				return Control.TranslatePoint(new wf.Point(0, 0), Widget.VisualParent.GetContainerControl()).ToEtoPoint();
#else
				return Point.Empty;
				throw new NotImplementedException();
#endif
			}
		}
	}
}
