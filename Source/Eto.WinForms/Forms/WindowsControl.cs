using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using Eto.WinForms.Drawing;
using System.Collections.Generic;
using System.Linq;
using Eto.WinForms.Forms.Menu;

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
				var userSize = Handler.UserDesiredSize;
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
			var size = UserDesiredSize;
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

		static readonly object DesiredSizeKey = new object();

		public Size UserDesiredSize
		{
			get { return Widget.Properties.Get<Size?>(DesiredSizeKey) ?? new Size(-1, -1); }
			set { Widget.Properties.Set(DesiredSizeKey, value); }
		}

		static readonly object DesiredClientSizeKey = new object();

		public Size UserDesiredClientSize
		{
			get { return Widget.Properties.Get<Size?>(DesiredClientSizeKey) ?? new Size(-1, -1); }
			set { Widget.Properties.Set(DesiredClientSizeKey, value); }
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

		public virtual bool ShouldCaptureMouse
		{
			get { return false; }
		}

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
				default:
					base.AttachEvent(id);
					break;
			}
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
					return UserDesiredSize;
				return ContainerControl.Size.ToEto();
			}
			set
			{
                UserDesiredSize = value;
				SetAutoSize();
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

		protected virtual void SetAutoSize()
		{
			ContainerControl.AutoSize = 
				(UserDesiredSize.Width == -1 || UserDesiredSize.Height == -1)
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
				SetAutoSize();
				Control.ClientSize = value.ToSD();
			}
		}

		public bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		static readonly object CursorKey = new object();

		public Cursor Cursor
		{
			get { return Widget.Properties.Get<Cursor>(CursorKey); }
			set
			{
				Widget.Properties[CursorKey] = value;
				Control.Cursor = value != null ? value.ControlObject as swf.Cursor : null;
			}
		}

		static readonly object ToolTipKey = new object();

		public string ToolTip
		{
			get { return Widget.Properties.Get<string>(ToolTipKey); }
			set
			{
				Widget.Properties[ToolTipKey] = value;
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

		static readonly object InternalVisibleKey = new object();

		bool IWindowsControl.InternalVisible
		{
			get { return Widget.Properties.Get<bool?>(InternalVisibleKey) ?? true; }
		}

		public virtual bool Visible
		{
			get { return ContainerControl.IsHandleCreated ? ContainerControl.Visible : Widget.Properties.Get<bool?>(InternalVisibleKey) ?? true; }
			set
			{
				if (Visible != value)
				{
					Widget.Properties[InternalVisibleKey] = value;
					ContainerControl.Visible = value;
					SetMinimumSize(updateParent: true);
				}
			}
		}

		public virtual void SetParent(Container parent)
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
		}

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

		static readonly object FontKey = new object();

		public Font Font
		{
			get
			{
				return Widget.Properties.Create<Font>(FontKey, () => new Font(new FontHandler(Control.Font)));
			}
			set
			{
				Widget.Properties[FontKey] = value;
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
	}
}
