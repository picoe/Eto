using System;
using System.ComponentModel;
using System.Linq;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using Eto.Platform.Mac.Forms.Controls;
using System.Threading;

namespace Eto.Platform.Mac.Forms
{
	public class MyWindow : NSWindow, IMacControl
	{
		SD.RectangleF oldFrame;
		bool zoom;

		public WeakReference WeakHandler { get; set; }

		public IMacWindow Handler { get { return (IMacWindow)WeakHandler.Target; } set { WeakHandler = new WeakReference(value); } }

		public MyWindow(SD.Rectangle rect, NSWindowStyle style, NSBackingStore store, bool flag)
			: base(rect, style, store, flag)
		{
		}

		public override void Center()
		{
			// implement centering to parent if there is a parent window for this one..
			if (ParentWindow != null)
			{
				var parentFrame = ParentWindow.Frame;
				var frame = Frame;
				var location = new SD.PointF((parentFrame.Width - frame.Width) / 2 + parentFrame.X, (parentFrame.Height - frame.Height) / 2 + parentFrame.Y);
				SetFrameOrigin(location);
			}
			else
				base.Center();
		}

		public override void Zoom(NSObject sender)
		{
			if (zoom)
			{
				SetFrame(oldFrame, true, true);
				zoom = false;
			}
			else
			{
				oldFrame = Frame;
				base.Zoom(sender);
				zoom = true;
			}
			Handler.Widget.OnWindowStateChanged(EventArgs.Empty);
		}
	}

	public interface IMacWindow
	{
		Rectangle? RestoreBounds { get; set; }

		Window Widget { get; }

		NSMenu MenuBar { get; }

		NSObject FieldEditorObject { get; set; }

		Size MinimumSize { get; }

		bool CloseWindow();

		NSWindow Control { get; }
	}

	public class CustomFieldEditor : NSTextView
	{
		WeakReference widget;

		public Control Widget { get { return (Control)widget.Target; } set { widget = new WeakReference(value); } }

		public CustomFieldEditor()
		{
			FieldEditor = true;
		}

		public CustomFieldEditor(IntPtr handle)
			: base(handle)
		{
		}

		public override void KeyDown(NSEvent theEvent)
		{
			if (!MacEventView.KeyDown(Widget, theEvent))
			{
				base.KeyDown(theEvent);
			}
		}
	}

	public abstract class MacWindow<TControl, TWidget> : MacDockContainer<TControl, TWidget>, IWindow, IMacContainer, IMacWindow
		where TControl: MyWindow
		where TWidget: Window
	{
		CustomFieldEditor fieldEditor;
		MenuBar menuBar;
		Icon icon;
		ToolBar toolBar;
		Rectangle? restoreBounds;
		bool setInitialSize;
		WindowState? initialState;
		bool maximizable = true;
		bool topmost;
		bool setInitialPosition = true;
		Point? oldLocation;

		public override NSView ContainerControl { get { return Control.ContentView; } }

		public override object EventObject { get { return Control; } }

		static readonly Selector selSetStyleMask = new Selector("setStyleMask:");

		public NSObject FieldEditorObject { get; set; }

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var content = Widget.Content.GetMacAutoSizing();
			if (content != null)
			{
				return content.GetPreferredSize(availableSize);
			}
			return new Size(200, 200);
		}

		public override Size MinimumSize
		{
			get { return base.MinimumSize; }
			set
			{
				base.MinimumSize = value;
				if (value != Size.Empty)
				{
					Control.WillResize = (sender, frameSize) =>
					{
						if (value != Size.Empty)
						{
							return new SD.SizeF(Math.Max(frameSize.Width, value.Width), Math.Max(frameSize.Height, value.Height));
						}
						return frameSize;
					};
				}
				else
					Control.WillResize = null;
			}
		}

		public NSMenu MenuBar
		{
			get { return menuBar == null ? null : menuBar.ControlObject as NSMenu; }
		}

		protected MacWindow()
		{
			AutoSize = true;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.DidBecomeKey += delegate
			{
				if (MenuBar != null)
				{
					NSApplication.SharedApplication.MainMenu = MenuBar;
				}
			};
			Control.ShouldZoom = (window, newFrame) =>
			{
				if (!Maximizable)
					return false;
				if (!window.IsZoomed && window.Screen != null)
				{
					RestoreBounds = Widget.Bounds;
				}
				return true;
			};
			Control.WillMiniaturize += delegate
			{
				RestoreBounds = Widget.Bounds;
			};
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Window.ClosedEvent:
					Control.WillClose += delegate
					{
						Widget.OnClosed(EventArgs.Empty);
					};
					break;
				case Window.ClosingEvent:
					Control.WindowShouldClose = sender =>
					{
						var args = new CancelEventArgs();
						Widget.OnClosing(args);
						return !args.Cancel;
					};
					break;
				case Window.WindowStateChangedEvent:
					Control.DidMiniaturize += delegate
					{
						Widget.OnWindowStateChanged(EventArgs.Empty);
					};
					Control.DidDeminiaturize += delegate
					{
						Widget.OnWindowStateChanged(EventArgs.Empty);
					};
					break;
				case Eto.Forms.Control.ShownEvent:
				// handled when shown
					break;
				case Eto.Forms.Control.GotFocusEvent:
					Control.DidBecomeKey += delegate
					{
						Widget.OnGotFocus(EventArgs.Empty);
					};
					break;
				case Eto.Forms.Control.LostFocusEvent:
					Control.DidResignKey += delegate
					{
						Widget.OnLostFocus(EventArgs.Empty);
					};
					break;
				case Eto.Forms.Control.SizeChangedEvent:
					{
						Size? oldSize = null;
						AddControlObserver((NSString)"frame", e =>
						{
							var widget = (Window)e.Widget;
							var newSize = widget.Size;
							if (oldSize != newSize)
							{
								widget.OnSizeChanged(EventArgs.Empty);
								oldSize = newSize;
							}
						});
					}
					break;
				case Window.LocationChangedEvent:
					{
						AddControlObserver((NSString)"frame", e =>
						{
							var handler = e.Handler as MacWindow<TControl,TWidget>;
							if (handler != null)
							{
								var old = oldLocation;
								oldLocation = null;
								var newLocation = handler.Location;
								if (old != newLocation)
								{
									oldLocation = newLocation;
									handler.Widget.OnLocationChanged(EventArgs.Empty);
								}
							}
						});
						// WillMove is only called when the user moves the window via the mouse
						Control.WillMove += HandleWillMove;
					}
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void CreateCursorRect()
		{
			if (Cursor != null)
			{
				Control.ContentView.DiscardCursorRects();
				Control.ContentView.AddCursorRect(new SD.RectangleF(SD.PointF.Empty, Control.Frame.Size), Cursor.ControlObject as NSCursor);
			}
			else
				Control.ContentView.DiscardCursorRects();
		}

		/// <summary>
		/// Tracks movement of the window until the mouse up button is found
		/// </summary>
		static void HandleWillMove(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as MacWindow<TControl,TWidget>;
			if (handler == null)
				return;
			handler.oldLocation = null;
			// find offset of mouse cursor to location of window
			var moveOffset = Size.Round((SizeF)(Mouse.GetPosition(handler.Generator) - handler.Location));

			ThreadPool.QueueUserWorkItem(a =>
			{
				bool tracking = true;
				while (tracking)
				{
					NSApplication.SharedApplication.InvokeOnMainThread(() =>
					{
						var newLocation = Point.Round(Mouse.GetPosition(handler.Generator) - moveOffset);
						if (handler.oldLocation != newLocation)
						{
							handler.Widget.OnLocationChanged(EventArgs.Empty);
							handler.oldLocation = newLocation;
						}
						// check for mouse up event
						tracking = NSApplication.SharedApplication.NextEventEx(NSEventMask.LeftMouseUp, null, NSRunLoop.NSRunLoopEventTracking, false) == null;
					});
				}
				handler.oldLocation = null;
			});
		}

		protected void ConfigureWindow()
		{
			Control.Handler = this;
			Control.ContentView = new NSView();
			//Control.ContentMinSize = new System.Drawing.SizeF(0, 0);
			Control.ContentView.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
			Control.ReleasedWhenClosed = false;
			Control.HasShadow = true;
			Control.ShowsResizeIndicator = true;
			Control.AutorecalculatesKeyViewLoop = true;
			//Control.Delegate = new MacWindowDelegate{ Handler = this };
			Control.WillReturnFieldEditor = (sender, forObject) =>
			{
				FieldEditorObject = forObject;
				var control = forObject as IMacControl;
				if (control != null)
				{
					var handler = control.WeakHandler.Target as IMacViewHandler;
					if (handler != null && handler.IsEventHandled(Eto.Forms.Control.KeyDownEvent))
					{
						if (fieldEditor == null)
							fieldEditor = new CustomFieldEditor();
						fieldEditor.Widget = handler.Widget;
						return fieldEditor;
					}
				}
				return null;
			};
		}

		public override NSView ContentControl { get { return Control.ContentView; } }

		public virtual string Title { get { return Control.Title; } set { Control.Title = value ?? ""; } }
		// Control.Title throws an exception if value is null
		void SetButtonStates()
		{
			var button = Control.StandardWindowButton(NSWindowButton.ZoomButton);
			if (button != null)
				button.Enabled = Maximizable && Resizable;
		}

		public bool Resizable
		{
			get { return Control.StyleMask.HasFlag(NSWindowStyle.Resizable); }
			set
			{
				if (Control.RespondsToSelector(selSetStyleMask))
				{
					if (value)
						Control.StyleMask |= NSWindowStyle.Resizable;
					else
						Control.StyleMask &= ~NSWindowStyle.Resizable;
					SetButtonStates();
				}
			}
		}

		public bool Minimizable
		{
			get { return Control.StyleMask.HasFlag(NSWindowStyle.Miniaturizable); }
			set
			{
				if (Control.RespondsToSelector(selSetStyleMask))
				{
					if (value)
						Control.StyleMask |= NSWindowStyle.Miniaturizable;
					else
						Control.StyleMask &= ~NSWindowStyle.Miniaturizable;
					SetButtonStates();
				}
			}
		}

		public bool Maximizable
		{
			get { return maximizable; }
			set
			{
				if (maximizable != value)
				{
					maximizable = value;
					HandleEvent(Window.WindowStateChangedEvent);
					SetButtonStates();
				}
			}
		}

		public bool ShowInTaskbar
		{
			get;
			set;
		}

		public bool Topmost
		{
			get { return topmost; }
			set
			{
				if (topmost != value)
				{
					topmost = value;
					Control.Level = value ? NSWindowLevel.PopUpMenu : NSWindowLevel.Normal;
				}
			}
		}

		public override Size Size
		{
			get
			{
				return Control.Frame.Size.ToEtoSize();
			}
			set
			{
				var oldFrame = Control.Frame;
				var newFrame = oldFrame.SetSize(value);
				newFrame.Y = Math.Max(0, oldFrame.Y - (value.Height - oldFrame.Height));
				Control.SetFrame(newFrame, true);
				AutoSize = false;
			}
		}

		public MenuBar Menu
		{
			get
			{
				return menuBar;
			}
			set
			{
				menuBar = value;
				if (Control.IsKeyWindow)
				{
					NSApplication.SharedApplication.MainMenu = (NSMenu)value.ControlObject;
				}
			}
		}

		public bool CloseWindow()
		{
			var args = new CancelEventArgs();
			Widget.OnClosing(args);
			if (!args.Cancel)
			{
				Widget.OnClosed(EventArgs.Empty);
			}
			return !args.Cancel;
		}

		public virtual void Close()
		{
			Control.Close();
		}

		public ToolBar ToolBar
		{
			get
			{
				return toolBar;
			}
			set
			{
				toolBar = value;
				Control.Toolbar = (NSToolbar)toolBar.ControlObject;
			}
		}

		public Icon Icon
		{
			get { return icon; }
			set
			{
				icon = value;
				// don't really do anything here..  no where to put it
			}
		}

		public override void Focus()
		{
			Control.BecomeFirstResponder();
		}

		public string Id { get; set; }

		public override Size ClientSize
		{
			get { return Control.ContentView.Frame.Size.ToEtoSize(); }
			set
			{ 
				var oldFrame = Control.Frame;
				var oldSize = Control.ContentView.Frame;
				Control.SetFrameOrigin(new SD.PointF(oldFrame.X, Math.Max(0, oldFrame.Y - (value.Height - oldSize.Height))));
				Control.SetContentSize(value.ToSDSizeF());
				AutoSize = false;
			}
		}

		public override bool HasFocus
		{
			get { return Control.IsKeyWindow; }
		}

		public override bool Visible
		{
			get { return Control.IsVisible; }
			set
			{
				if (value && !Control.IsVisible)
					Control.MakeKeyAndOrderFront(NSApplication.SharedApplication);
				if (!value && Control.IsVisible)
					Control.PerformClose(NSApplication.SharedApplication);
				// huh?
			}
		}

		public override Cursor Cursor
		{
			get { return base.Cursor; }
			set
			{
				base.Cursor = value;
				CreateCursorRect();
			}
		}

		public virtual Point Location
		{
			get
			{
				if (oldLocation != null)
					return oldLocation.Value;
				// translate location relative to the top left corner of main screen
				var mainFrame = NSScreen.Screens[0].Frame;
				var frame = Control.Frame;
				return new Point((int)frame.X, (int)(mainFrame.Height - frame.Y - frame.Height));
			}
			set
			{
				// location is relative to the main screen, translate to bottom left, inversed
				var mainFrame = NSScreen.Screens[0].Frame;
				var frame = Control.Frame;
				var point = new SD.PointF(value.X, mainFrame.Height - value.Y - frame.Height);
				Control.SetFrameOrigin(point);
				if (Control.Screen == null)
				{
					// ensure that the control lands on a screen
					point.X = Math.Min(Math.Max(mainFrame.X, point.X), mainFrame.Right - frame.Width);
					point.Y = Math.Min(Math.Max(mainFrame.Y, point.Y), mainFrame.Bottom - frame.Height);

					Control.SetFrameOrigin(point);
				}
				setInitialPosition = false;
			}
		}

		public WindowState WindowState
		{
			get
			{
				if (initialState != null)
					return initialState.Value;
				if (Control.IsMiniaturized)
					return WindowState.Minimized;
				if (Control.IsZoomed)
					return WindowState.Maximized;
				return WindowState.Normal;
			}
			set
			{
				if (!Widget.Loaded)
				{
					initialState = value;
					return;
				}
				switch (value)
				{
					case WindowState.Maximized: 
						if (Control.IsMiniaturized)
							Control.Deminiaturize(Control);
						if (!Control.IsZoomed)
							Control.PerformZoom(Control);
						break;
					case WindowState.Minimized:
						if (!Control.IsMiniaturized)
							Control.Miniaturize(Control);
						break;
					case WindowState.Normal: 
						if (Control.IsZoomed)
							Control.Zoom(Control);
						if (Control.IsMiniaturized)
							Control.Deminiaturize(Control);
						break;
				}
			}
		}

		public Rectangle? RestoreBounds
		{
			get { return WindowState == WindowState.Normal ? null : restoreBounds; }
			set { restoreBounds = value; }
		}

		public double Opacity
		{
			get { return Control.IsOpaque ? 1.0 : Control.AlphaValue; }
			set
			{
				Control.IsOpaque = Math.Abs(value - 1.0) < 0.01f;
				Control.AlphaValue = (float)value; 
			}
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (AutoSize)
			{
				var size = GetPreferredSize(Size.MaxValue);
				SetContentSize(size.ToSD());
				setInitialSize = true;

				PositionWindow();
			}
			else
			{
				PositionWindow();
			}
		}
		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			if (initialState != null)
			{
				WindowState = initialState.Value;
				initialState = null;
			}
		}

		protected virtual void PositionWindow()
		{
			if (setInitialPosition)
			{
				Control.Center();
				setInitialPosition = false;
			}
		}

		#region IMacContainer implementation

		public override void SetContentSize(SD.SizeF contentSize)
		{
			if (MinimumSize != Size.Empty)
			{
				contentSize.Width = Math.Max(contentSize.Width, MinimumSize.Width);
				contentSize.Height = Math.Max(contentSize.Height, MinimumSize.Height);
			}
			
			if (Widget.Loaded)
			{
				var diffy = ClientSize.Height - (int)contentSize.Height;
				var diffx = ClientSize.Width - (int)contentSize.Width;
				var frame = Control.Frame;
				if (diffx < 0 || !setInitialSize)
				{
					frame.Width -= diffx;
				}
				if (diffy < 0 || !setInitialSize)
				{
					frame.Y += diffy;
					frame.Height -= diffy;
				}
				Control.SetFrame(frame, false, false);
			}
			else
				Control.SetContentSize(contentSize);
		}

		#endregion

		#region IMacWindow implementation

		Rectangle? IMacWindow.RestoreBounds
		{
			get
			{
				return restoreBounds;
			}
			set
			{
				restoreBounds = value;
			}
		}

		Window IMacWindow.Widget
		{
			get { return Widget; }
		}

		NSWindow IMacWindow.Control
		{
			get { return Control; }
		}

		#endregion

		public Screen Screen
		{
			get { return new Screen(Generator, new ScreenHandler(Control.Screen)); }
		}

		public override PointF PointFromScreen(PointF point)
		{
			var sdpoint = point.ToSD();
			sdpoint = Control.ConvertBaseToScreen(sdpoint);
			sdpoint.Y = Control.Screen.Frame.Height - sdpoint.Y;
			return sdpoint.ToEto();
		}

		public override PointF PointToScreen(PointF point)
		{
			var sdpoint = point.ToSD();
			sdpoint = Control.ConvertBaseToScreen(sdpoint);
			sdpoint.Y = Control.Screen.Frame.Height - sdpoint.Y;
			return sdpoint.ToEto();
		}

		public WindowStyle WindowStyle
		{
			get { return Control.StyleMask.ToEtoWindowStyle(); }
			set
			{
				if (Control.RespondsToSelector(selSetStyleMask))
				{
					Control.StyleMask = value.ToNS(Control.StyleMask);
				}
			}
		}

		public void BringToFront()
		{
			Control.OrderFront(Control);
			Control.MakeKeyWindow();
		}

		public void SendToBack()
		{
			Control.OrderBack(Control);
			var window = NSApplication.SharedApplication.Windows.FirstOrDefault(r => r != Control);
			if (window != null)
				window.MakeKeyWindow();
			Control.ResignKeyWindow();
		}
	}
}
