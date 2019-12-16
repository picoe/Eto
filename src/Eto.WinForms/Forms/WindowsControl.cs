using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using Eto.WinForms.Drawing;
using System.Collections.Generic;
using System.Linq;
using Eto.WinForms.Forms.Menu;
using System.Reflection;
using System.Diagnostics;

namespace Eto.WinForms.Forms
{
	public interface IWindowsControl: Control.IHandler
	{
		bool InternalVisible { get; }

		swf.DockStyle DockStyle { get; }

		swf.Control ContainerControl { get; }

		Size ParentMinimumSize { get; set; }

		Size GetPreferredSize(Size availableSize, bool useCache = false);

		bool SetMinimumSize(bool updateParent = false, bool useCache = false);

		void SetScale(bool xscale, bool yscale);

		bool ShouldCaptureMouse { get; }

		bool XScale { get; }

		bool YScale { get; }

		bool BackgroundColorSet { get; }

		Control.ICallback Callback { get; }

		void BeforeAddControl(bool top = true);

		bool ShouldBubbleEvent(swf.Message msg);

		bool UseShellDropManager { get; set; }
	}

	public static class WindowsControlExtensions
	{
		public static IWindowsControl GetWindowsHandler(this Control control)
		{
			if (control == null)
				return null;

			var handler = control.Handler as IWindowsControl;
			if (handler != null)
				return handler;

			var controlObject = control.ControlObject as Control;
			return controlObject != null ? controlObject.GetWindowsHandler() : null;

		}

		public static Size GetPreferredSize(this Control control, Size? availableSize = null)
		{
			var handler = control.GetWindowsHandler();
			return handler != null ? handler.GetPreferredSize(availableSize ?? Size.Empty) : Size.Empty;
		}

		public static swf.Control GetContainerControl(this Control control)
		{
			if (control == null)
				return null;

			var handler = control.Handler as IWindowsControl;
			if (handler != null)
				return handler.ContainerControl;

			var controlObject = control.ControlObject as Control;
			if (controlObject != null)
				return controlObject.GetContainerControl();

			return control.ControlObject as swf.Control;
		}

		public static void SetScale(this Control control, bool xscale, bool yscale)
		{
			var handler = control.GetWindowsHandler();

			if (handler != null)
				handler.SetScale(xscale, yscale);
		}
	}

	static class WindowsControl
	{
		public static readonly object DesiredSizeKey = new object();
		public static readonly object DesiredClientSizeKey = new object();
		public static readonly object CursorKey = new object();
		public static readonly object ToolTipKey = new object();
		public static readonly object InternalVisibleKey = new object();
		public static readonly object FontKey = new object();
		public static readonly object Enabled_Key = new object();
		public static readonly object UseShellDropManager_Key = new object();

		internal static Control DragSourceControl { get; set; }
	}

	public abstract class WindowsControl<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Control.IHandler, IWindowsControl
		where TControl : swf.Control
		where TWidget : Control
		where TCallback : Control.ICallback
	{

		// used in DrawableHandler
		public class PanelBase<THandler> : swf.Panel
			where THandler: WindowsControl<TControl, TWidget, TCallback>
		{
			public THandler Handler { get; set; }

			public PanelBase( THandler handler = null )
			{
				Handler = handler;
				Size = sd.Size.Empty;
				MinimumSize = sd.Size.Empty;
				AutoSize = true;
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink;
			}

			public override sd.Size GetPreferredSize(sd.Size proposedSize)
			{
				var userSize = Handler.UserPreferredSize;
				var size = userSize.Width >= 0 && userSize.Height >= 0 ? sd.Size.Empty
					: base.GetPreferredSize(proposedSize);
				if (userSize.Width >= 0)
					size.Width = Math.Max(userSize.Width, MinimumSize.Width);
				if (userSize.Height >= 0)
					size.Height = Math.Max(userSize.Height, MinimumSize.Height);
				return size;
			}
			// Need to override IsInputKey to capture 
			// the arrow keys.
			protected override bool IsInputKey(swf.Keys keyData)
			{
				switch (keyData & swf.Keys.KeyCode)
				{
					case swf.Keys.Up:
					case swf.Keys.Down:
					case swf.Keys.Left:
					case swf.Keys.Right:
					case swf.Keys.Back:
						return true;
					default:
						return base.IsInputKey(keyData);
				}
			}
		}

		// used in Panel+PixelLayout and similar code is in TableLayout
		public class EtoPanel<THandler> : PanelBase<THandler>
			where THandler : WindowsControl<TControl, TWidget, TCallback>
		{
			public EtoPanel( THandler handler = null )
				: base( handler )
			{ }

			// optimization especially for content on drawable
			protected override void OnBackColorChanged( EventArgs e )
			{
				SetStyle
					( swf.ControlStyles.AllPaintingInWmPaint
					| swf.ControlStyles.DoubleBuffer
					, BackColor.A != 255 );
				base.OnBackColorChanged( e );
			}
			protected override void OnParentBackColorChanged( EventArgs e )
			{
				SetStyle
					( swf.ControlStyles.AllPaintingInWmPaint
					| swf.ControlStyles.DoubleBuffer
					, BackColor.A != 255 );
				base.OnParentBackColorChanged( e );
			}
		}

		Size parentMinimumSize;

		public override IntPtr NativeHandle { get { return Control.Handle; } }

		Control.ICallback IWindowsControl.Callback { get { return Callback; } }

		public bool XScale { get; set; }

		public bool YScale { get; set; }

		public virtual Size? GetDefaultSize(Size availableSize) { return null; }// Control.GetPreferredSize(availableSize.ToSD()).ToEto(); }

		protected void ClearCachedDefaultSize()
		{
			cachedDefaultSize = null;
		}

		Size? cachedDefaultSize;
		public virtual Size GetPreferredSize(Size availableSize, bool useCache = false)
		{
			var size = UserPreferredSize;
			if (size.Width == -1 || size.Height == -1)
			{
				Size? defSize;
				if (useCache)
					defSize = cachedDefaultSize ?? GetDefaultSize(availableSize);
				else
					defSize = GetDefaultSize(availableSize);
				if (defSize != null)
				{
					if (size.Width == -1) size.Width = defSize.Value.Width;
					if (size.Height == -1) size.Height = defSize.Value.Height;
				}
			}
			return Size.Max(parentMinimumSize, size);
		}

		public Size UserPreferredSize
		{
			get { return Widget.Properties.Get<Size?>(WindowsControl.DesiredSizeKey) ?? new Size(-1, -1); }
			set
			{
				if (Widget.Properties.TrySet(WindowsControl.DesiredSizeKey, value))
					SetAutoSize();
			}
		}

		public Size UserDesiredClientSize
		{
			get { return Widget.Properties.Get<Size?>(WindowsControl.DesiredClientSizeKey) ?? new Size(-1, -1); }
			set
			{
				if (Widget.Properties.TrySet(WindowsControl.DesiredClientSizeKey, value))
					SetAutoSize();
			}
		}

		public virtual Size ParentMinimumSize
		{
			get { return parentMinimumSize; }
			set
			{
				if (parentMinimumSize != value)
				{
					parentMinimumSize = value;
					SetMinimumSize(useCache: true);
				}
			}
		}

		public virtual bool ShouldCaptureMouse => false;

		public virtual swf.Control ContainerControl
		{
			get { return Control; }
		}

		protected override void Initialize()
		{
			base.Initialize();
			XScale = true;
			YScale = true;
			Control.Margin = swf.Padding.Empty;
			Control.Tag = this;
		}

		public virtual swf.DockStyle DockStyle
		{
			get { return swf.DockStyle.Fill; }
		}

		public virtual bool SetMinimumSize(bool updateParent = false, bool useCache = false)
		{
			if (!Widget.Loaded)
				return false;
			return SetMinimumSizeInternal(updateParent, useCache);
		}

		bool SetMinimumSizeInternal(bool updateParent, bool useCached = false)
		{
			Size size = Size.Empty;
			if (!XScale || !YScale)
			{
				var preferredSize = GetPreferredSize(Size.Empty, useCached);
				if (!XScale) size.Width = preferredSize.Width;
				if (!YScale) size.Height = preferredSize.Height;
			}
			var ret = SetMinimumSize(size);
			if (updateParent && Widget.Loaded)
			{
				var parent = Widget.VisualParent.GetWindowsHandler();
				if (parent != null)
					parent.SetMinimumSize(updateParent);
			}
			return ret;
		}

		protected virtual bool SetMinimumSize(Size size)
		{
			var sdsize = size.ToSD();
			if (ContainerControl.MinimumSize != sdsize)
			{
				ContainerControl.MinimumSize = sdsize;
				//System.Diagnostics.Debug.Print(string.Format("Min Size: {0}, Type:{1}", sdsize, Widget));
				return true;
			}
			return false;
		}

		public virtual void SetScale(bool xscale, bool yscale)
		{
			XScale = xscale;
			YScale = yscale;
			SetMinimumSize(false, useCache: true);
		}

		public void SetScale()
		{
			SetScale(XScale, YScale);
		}

		ContextMenu contextMenu;
		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set
			{
				contextMenu = value;
				Control.ContextMenuStrip = contextMenu != null ? ((ContextMenuHandler)contextMenu.Handler).Control : null;
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.KeyDownEvent:
					if (!ApplicationHandler.BubbleKeyEvents)
					{
						Control.KeyDown += Control_KeyDown;
						Control.KeyPress += Control_KeyPress;
					}
					break;
				case Eto.Forms.Control.KeyUpEvent:
					if (!ApplicationHandler.BubbleKeyEvents)
					{
						Control.KeyUp += Control_KeyUp;
					}
					break;
				case TextControl.TextChangedEvent:
					Control.TextChanged += Control_TextChanged;
					break;
				case Eto.Forms.Control.TextInputEvent:
					HandleEvent(Eto.Forms.Control.KeyDownEvent);
					break;
				case Eto.Forms.Control.SizeChangedEvent:
					Control.SizeChanged += Control_SizeChanged;
					break;
				case Eto.Forms.Control.MouseDoubleClickEvent:
					if (!ApplicationHandler.BubbleMouseEvents)
					{
						Control.MouseDoubleClick += HandleDoubleClick;
					}
					break;
				case Eto.Forms.Control.MouseEnterEvent:
					Control.MouseEnter += HandleControlMouseEnter;
					break;
				case Eto.Forms.Control.MouseLeaveEvent:
					Control.MouseLeave += HandleControlMouseLeave;
					break;
				case Eto.Forms.Control.MouseDownEvent:
					if (!ApplicationHandler.BubbleMouseEvents)
					{
						Control.MouseDown += HandleMouseDown;
						if (ShouldCaptureMouse)
							HandleEvent(Eto.Forms.Control.MouseUpEvent);
					}
					break;
				case Eto.Forms.Control.MouseUpEvent:
					if (!ApplicationHandler.BubbleMouseEvents)
					{
						Control.MouseUp += HandleMouseUp;
						if (ShouldCaptureMouse)
							HandleEvent(Eto.Forms.Control.MouseDownEvent);
					}
					break;
				case Eto.Forms.Control.MouseMoveEvent:
					if (!ApplicationHandler.BubbleMouseEvents)
					{
						Control.MouseMove += HandleMouseMove;
					}
					break;
				case Eto.Forms.Control.MouseWheelEvent:
					if (!ApplicationHandler.BubbleMouseEvents)
					{
						Control.MouseWheel += HandleMouseWheel;
					}
					break;
				case Eto.Forms.Control.GotFocusEvent:
					Control.GotFocus += (sender, e) => Callback.OnGotFocus(Widget, EventArgs.Empty);
					break;
				case Eto.Forms.Control.LostFocusEvent:
					Control.LostFocus += (sender, e) => Callback.OnLostFocus(Widget, EventArgs.Empty);
					break;
				case Eto.Forms.Control.ShownEvent:
					bool? last = null;
					Control.VisibleChanged += (sender, e) =>
					{
						var visible = Control.Visible;
						if (last == visible || !Widget.Loaded)
							return;
						last = visible;
						if (visible)
							Callback.OnShown(Widget, EventArgs.Empty);
					};
					break;
				case Eto.Forms.Control.DragDropEvent:
					Control.DragDrop += (sender, e) =>
					{
						var args = GetDragEventArgs(e);
						Callback.OnDragLeave(Widget, args);
						Callback.OnDragDrop(Widget, args);
						e.Effect = args.Effects.ToSwf();
					};
					break;
				case Eto.Forms.Control.DragOverEvent:
					Control.DragOver += (sender, e) =>
					{
						var args = GetDragEventArgs(e);
						Callback.OnDragOver(Widget, args);
						e.Effect = args.Effects.ToSwf();
					};
					break;
				case Eto.Forms.Control.DragEnterEvent:
					Control.DragEnter += (sender, e) =>
					{
						var args = GetDragEventArgs(e);
						Callback.OnDragEnter(Widget, args);
						e.Effect = args.Effects.ToSwf();
					};
					break;
				case Eto.Forms.Control.DragLeaveEvent:
					Control.DragLeave += (sender, e) =>
					{
						// how do we get the args?
						Callback.OnDragLeave(Widget, new DragEventArgs(null, new DataObject(), DragEffects.None, PointF.Empty, Keys.None, MouseButtons.None));
					};
					break;
				case Eto.Forms.Control.EnabledChangedEvent:
					Control.EnabledChanged += Control_EnabledChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void Control_EnabledChanged(object sender, EventArgs e)
		{
			Callback.OnEnabledChanged(Widget, EventArgs.Empty);
		}


		DragEventArgs GetDragEventArgs(swf.DragEventArgs data)
		{
			var dragData = data.Data.ToEto();
			var source = WindowsControl.DragSourceControl;
			var modifiers = data.GetEtoModifiers();
			var buttons = data.GetEtoButtons();
			var location = PointFromScreen(new PointF(data.X, data.Y));
			return new SwfDragEventArgs(source, dragData, data.AllowedEffect.ToEto(), location, modifiers, buttons);
		}

		void HandleMouseWheel(object sender, swf.MouseEventArgs e)
		{
			Callback.OnMouseWheel(Widget, e.ToEto(Control));
		}

		void HandleControlMouseLeave(object sender, EventArgs e)
		{
			Callback.OnMouseLeave(Widget, new MouseEventArgs(MouseButtons.None, swf.Control.ModifierKeys.ToEto(), swf.Control.MousePosition.ToEto()));
		}

		void HandleControlMouseEnter(object sender, EventArgs e)
		{
			Callback.OnMouseEnter(Widget, new MouseEventArgs(MouseButtons.None, swf.Control.ModifierKeys.ToEto(), swf.Control.MousePosition.ToEto()));
		}

		void HandleDoubleClick(object sender, swf.MouseEventArgs e)
		{
			var ee = e.ToEto(Control);
			Callback.OnMouseDoubleClick(Widget, ee);
			if (!ee.Handled)
				Callback.OnMouseDown(Widget, ee);
		}

		void HandleMouseUp(Object sender, swf.MouseEventArgs e)
		{
			if (ShouldCaptureMouse)
				Control.Capture = false;
			Callback.OnMouseUp(Widget, e.ToEto(Control));
		}

		void HandleMouseMove(Object sender, swf.MouseEventArgs e)
		{
			Callback.OnMouseMove(Widget, e.ToEto(Control));
		}

		void HandleMouseDown(object sender, swf.MouseEventArgs e)
		{
			Callback.OnMouseDown(Widget, e.ToEto(Control));
			if (ShouldCaptureMouse)
				Control.Capture = true;
		}

		public virtual string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; SetMinimumSize(); }
		}

		public virtual Size Size
		{
			get {
				if (!Widget.Loaded)
					return UserPreferredSize;
				return ContainerControl.Size.ToEto();
			}
			set
			{
				if (UserPreferredSize == value)
					return;
				UserPreferredSize = value;
				if (Widget.Loaded)
					SetScale();
				var minset = SetMinimumSize();
				ContainerControl.Size = value.ToSD();
				if (minset && ContainerControl.IsHandleCreated)
				{
					var parent = Widget.VisualParent.GetWindowsHandler();
					if (parent != null)
						parent.SetMinimumSize();
				}
			}
		}

		public virtual int Width
		{
			get => Size.Width;
			set => Size = new Size(value, UserPreferredSize.Height);
		}

		public virtual int Height
		{
			get => Size.Height;
			set => Size = new Size(UserPreferredSize.Width, value);
		}

		protected virtual void SetAutoSize()
		{
			ContainerControl.AutoSize = 
				(UserPreferredSize.Width == -1 || UserPreferredSize.Height == -1)
				&& (UserDesiredClientSize.Width == -1 || UserDesiredClientSize.Height == -1);
		}

		public virtual Size ClientSize
		{
			get
			{
				if (!Widget.Loaded)
					return UserDesiredClientSize;
				return Control.ClientSize.ToEto();
			}
			set
			{
				UserDesiredClientSize = value;
				Control.ClientSize = value.ToSD();
			}
		}

		public bool Enabled
		{
			get { return Control.Enabled; }
			set
			{
				if (value != Widget.Properties.Get<bool?>(WindowsControl.Enabled_Key))
				{
					Widget.Properties.Set<bool?>(WindowsControl.Enabled_Key, value);
					Control.Enabled = value;
				}
			}
		}

		public Cursor Cursor
		{
			get { return Widget.Properties.Get<Cursor>(WindowsControl.CursorKey); }
			set
			{
				Widget.Properties[WindowsControl.CursorKey] = value;
				Control.Cursor = value != null ? value.ControlObject as swf.Cursor : null;
			}
		}

		public string ToolTip
		{
			get { return Widget.Properties.Get<string>(WindowsControl.ToolTipKey); }
			set
			{
				Widget.Properties[WindowsControl.ToolTipKey] = value;
				SetToolTip();
			}
		}

		public virtual void Invalidate(bool invalidateChildren)
		{
			Control.Invalidate(invalidateChildren);
		}

		public virtual void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			Control.Invalidate(rect.ToSD(), invalidateChildren);
		}

		public virtual Color BackgroundColor
		{
			get { return Control.BackColor.ToEto(); }
			set { backgroundColorSet = true; Control.BackColor = value.ToSD(); }
		}
		bool backgroundColorSet;
		public bool BackgroundColorSet {
			get { return backgroundColorSet;  }
			set
			{
				if (!( backgroundColorSet = value ))
					Control.BackColor = sd.Color.Empty;
			}
		}

		public virtual void SuspendLayout()
		{
			Control.SuspendLayout();
		}

		public virtual void ResumeLayout()
		{
			Control.ResumeLayout();
		}

		public void Focus()
		{
			if (Widget.Loaded && Control.IsHandleCreated)
				Control.Focus();
			else
				Widget.LoadComplete += Widget_LoadComplete;
		}

		void Widget_LoadComplete(object sender, EventArgs e)
		{
			Widget.LoadComplete -= Widget_LoadComplete;
			Control.Focus();
		}

		public bool HasFocus
		{
			get { return Control.Focused; }
		}

		bool IWindowsControl.InternalVisible
		{
			get { return Widget.Properties.Get<bool?>(WindowsControl.InternalVisibleKey) ?? true; }
		}

		static MethodInfo getStateMethod = typeof(swf.Control).GetMethod("GetState", BindingFlags.Instance | BindingFlags.NonPublic);

		bool WouldBeVisible
		{
			get
			{
				// use Control.GetState() to tell if the control should be visible
				// see https://stackoverflow.com/a/5980637/981187
				var ctl = ContainerControl;
				if (getStateMethod == null) return ctl.Visible;
				return (bool)(getStateMethod.Invoke(ctl, new object[] { 2 }));
			}
		}

		public virtual bool Visible
		{
			get { return ContainerControl.IsHandleCreated ? WouldBeVisible : Widget.Properties.Get<bool?>(WindowsControl.InternalVisibleKey) ?? true; }
			set
			{
				if (Visible != value)
				{
					Widget.Properties[WindowsControl.InternalVisibleKey] = value;
					ContainerControl.Visible = value;
					SetMinimumSize(updateParent: true);
				}
			}
		}

		public virtual void SetParent(Container oldParent, Container newParent)
		{
		}

		void Control_SizeChanged(object sender, EventArgs e)
		{
			Callback.OnSizeChanged(Widget, e);
		}


		public virtual void OnPreLoad(EventArgs e)
		{
		}

		public virtual void OnLoad(EventArgs e)
		{
			SetMinimumSizeInternal(false);

			if (Widget.VisualParent?.Loaded != false && !(Widget is Window))
			{
				// adding dynamically or loading without a parent (e.g. embedding into a native app)
				Application.Instance.AsyncInvoke(FireOnShown);
			}
		}

		static void FireOnShown(Control control)
		{
			if (!control.Visible)
				return;
			var handler = control.Handler as IWindowsControl;
			handler?.Callback.OnShown(control, EventArgs.Empty);

			foreach (var ctl in control.VisualControls)
			{
				if (ctl.Visible)
					FireOnShown(ctl);
			}
		}

		protected void FireOnShown() => FireOnShown(Widget);

		public virtual void OnLoadComplete(EventArgs e)
		{
			SetMinimumSizeInternal(false);
			SetToolTip();
		}

		public virtual void OnUnLoad(EventArgs e)
		{
		}

		public virtual void BeforeAddControl(bool top = true)
		{
		}

		void SetToolTip()
		{
			if (Widget.ParentWindow != null)
			{
				var parent = Widget.ParentWindow.Handler as IWindowHandler;
				if (parent != null)
					parent.ToolTips.SetToolTip(Control, ToolTip);
			}
		}

		Keys key;
		bool handled;
		char keyChar;
		bool charPressed;
		public Keys? LastKeyDown { get; set; }

		void Control_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			charPressed = false;
			handled = true;
			key = e.KeyData.ToEto();

			if (key != Keys.None && LastKeyDown != key)
			{
				var kpea = new KeyEventArgs(key, KeyEventType.KeyDown);
				Callback.OnKeyDown(Widget, kpea);

				handled = e.SuppressKeyPress = e.Handled = kpea.Handled;
			}
			else
				handled = false;

			if (!handled && charPressed)
			{
				// this is when something in the event causes messages to be processed for some reason (e.g. show dialog box)
				// we want the char event to come after the dialog is closed, and handled is set to true!
				var kpea = new KeyEventArgs(key, KeyEventType.KeyDown, keyChar);
				Callback.OnKeyDown(Widget, kpea);
				e.SuppressKeyPress = e.Handled = kpea.Handled;
			}

			LastKeyDown = null;
		}

		void Control_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			charPressed = true;
			keyChar = e.KeyChar;
			if (!handled)
			{
				if (!char.IsControl(e.KeyChar))
				{
					var tia = new TextInputEventArgs(keyChar.ToString());
					Callback.OnTextInput(Widget, tia);
					e.Handled = tia.Cancel;
				}

				if (!e.Handled)
				{
					var kpea = new KeyEventArgs(key, KeyEventType.KeyDown, keyChar);
					Callback.OnKeyDown(Widget, kpea);
					e.Handled = kpea.Handled;
				}
			}
			else
				e.Handled = true;
		}


		void Control_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			key = e.KeyData.ToEto();

			var kpea = new KeyEventArgs(key, KeyEventType.KeyUp);
			Callback.OnKeyUp(Widget, kpea);
			e.Handled = kpea.Handled;
		}

		void Control_TextChanged(object sender, EventArgs e)
		{
			var widget = Widget as TextControl;
			if (widget != null)
			{
				var callback = (TextControl.ICallback)((ICallbackSource)widget).Callback;
				callback.OnTextChanged(widget, e);
			}
		}

		public Font Font
		{
			get
			{
				return Widget.Properties.Create<Font>(WindowsControl.FontKey, () => new Font(new FontHandler(Control.Font)));
			}
			set
			{
				Widget.Properties[WindowsControl.FontKey] = value;
				Control.Font = value.ToSD();
			}
		}

		public IEnumerable<string> SupportedPlatformCommands
		{
			get { return Enumerable.Empty<string>(); }
		}

		public virtual void MapPlatformCommand(string systemAction, Command command)
		{
		}

		public virtual PointF PointFromScreen(PointF point)
		{
			return !Control.IsDisposed ? Control.PointToClient(point.ToSDPoint()).ToEto() : PointF.Empty; // safety check added because this is hit in certain situations.
		}

		public virtual PointF PointToScreen(PointF point)
		{
			return !Control.IsDisposed ? Control.PointToScreen(point.ToSDPoint()).ToEto() : PointF.Empty; // safety check added because this is hit in certain situations.
		}

		public Point Location
		{
			get { return Control.Location.ToEto(); }
		}


		public virtual bool ShouldBubbleEvent(swf.Message msg)
		{
			return true;
		}

		public virtual Color TextColor
		{
			get { return Control.ForeColor.ToEto(); }
			set { Control.ForeColor = value.ToSD(); }
		}

		public virtual int TabIndex
		{
			get { return Control.TabIndex == 0 ? int.MaxValue : Control.TabIndex - 1; }
			set { Control.TabIndex = value == int.MaxValue ? 0 : value + 1; }
		}

		public virtual IEnumerable<Control> VisualControls => Enumerable.Empty<Control>();

		/// <summary>
		/// Gets or sets a value indicating that the shell drop manager should be used.
		/// </summary>
		public bool UseShellDropManager
		{
			get => Widget.Properties.Get(WindowsControl.UseShellDropManager_Key, true);
			set
			{
				if (Widget.Properties.TrySet(WindowsControl.UseShellDropManager_Key, value, true))
				{
					if (value && Control.AllowDrop)
					{
						DropBehavior = new SwfShellDropBehavior(Control);
					}
					else
					{
						DropBehavior?.Detach();
						DropBehavior = null;
					}
				}
			}
		}

		public bool AllowDrop
		{
			get => Control.AllowDrop;
			set
			{
				Control.AllowDrop = value;
				if (value && UseShellDropManager)
					DropBehavior = new SwfShellDropBehavior(Control);
				else
				{
					DropBehavior?.Detach();
					DropBehavior = null;
				}
			}
		}

		SwfShellDropBehavior DropBehavior
		{
			get => Widget.Properties.Get<SwfShellDropBehavior>(typeof(SwfShellDropBehavior));
			set => Widget.Properties.Set(typeof(SwfShellDropBehavior), value);
		}

		public void DoDragDrop(DataObject data, DragEffects allowedEffects, Image image, PointF cursorOffset)
		{
			var dataObject = data.ToSwf();
			WindowsControl.DragSourceControl = Widget;
			if (UseShellDropManager)
			{
				swf.DragSourceHelper.AllowDropDescription(true);

				swf.SwfDataObjectExtensions.SetDropDescription(dataObject, swf.DropImageType.Invalid, null, null);
				if (image == null)
					image = new Bitmap(1, 1, PixelFormat.Format32bppRgba);

				swf.SwfDataObjectExtensions.SetDragImage(dataObject, image.ToSD(), cursorOffset.ToSDPoint());
				swf.DragSourceHelper.RegisterDefaultDragSource(Control, dataObject);
				Control.DoDragDrop(dataObject, allowedEffects.ToSwf());
				swf.DragSourceHelper.UnregisterDefaultDragSource(Control);
			}
			else
			{
				if (image != null)
					Debug.WriteLine("DoDragDrop cannot show drag image when UseShellDropManager is false");

				Control.DoDragDrop(dataObject, allowedEffects.ToSwf());
			}
			WindowsControl.DragSourceControl = null;
		}

		public Window GetNativeParentWindow() => ContainerControl.FindForm().ToEtoWindow();
	}
}
