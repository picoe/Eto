using System;
using System.ComponentModel;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using System.Threading;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
using Eto.Mac.Forms.Controls;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms
{
	public class EtoWindow : NSWindow, IMacControl
	{
		CGRect oldFrame;
		bool zoom;

		public WeakReference WeakHandler { get; set; }

		public IMacWindow Handler { get { return (IMacWindow)WeakHandler.Target; } set { WeakHandler = new WeakReference(value); } }

		public EtoWindow(CGRect rect, NSWindowStyle style, NSBackingStore store, bool flag)
			: base(rect, style, store, flag)
		{
		}

		public bool? CanFocus { get; set; } = true;

		public override bool CanBecomeKeyWindow => CanFocus ?? base.CanBecomeKeyWindow;

		public bool DisableCenterParent { get; set; }

		public NSWindow OwnerWindow { get; set; }

		public override void Center()
		{
			if (DisableCenterParent)
				return;
			// implement centering to parent if there is a parent window for this one..
			var window = OwnerWindow ?? ParentWindow;
			if (window != null)
			{
				var parentFrame = window.Frame;
				var frame = Frame;
				var location = new CGPoint((parentFrame.Width - frame.Width) / 2 + parentFrame.X, (parentFrame.Height - frame.Height) / 2 + parentFrame.Y);
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
				base.Zoom(sender ?? this); // null when double clicking the title bar, but xammac/monomac doesn't allow it
				zoom = true;
			}
			Handler.Callback.OnWindowStateChanged(Handler.Widget, EventArgs.Empty);
		}

		public bool DisableSetOrigin { get; set; }

		public override void SetFrameOrigin(CGPoint aPoint)
		{
			if (!DisableSetOrigin)
				base.SetFrameOrigin(aPoint);
		}

		public override void RecalculateKeyViewLoop()
		{
			base.RecalculateKeyViewLoop();

			NSView last = null;
			Handler?.RecalculateKeyViewLoop(ref last);
		}
	}

	public class EtoPanel : NSPanel, IMacControl
	{
		CGRect oldFrame;
		bool zoom;

		public WeakReference WeakHandler { get; set; }

		public IMacWindow Handler { get { return (IMacWindow)WeakHandler.Target; } set { WeakHandler = new WeakReference(value); } }

		public EtoPanel(CGRect rect, NSWindowStyle style, NSBackingStore store, bool flag)
			: base(rect, style, store, flag)
		{
		}

		public bool? CanFocus { get; set; }

		public override bool CanBecomeKeyWindow => CanFocus ?? base.CanBecomeKeyWindow;

		public bool DisableCenterParent { get; set; }

		public NSWindow OwnerWindow { get; set; }

		public override void Center()
		{
			if (DisableCenterParent)
				return;
			// implement centering to parent if there is a parent window for this one..
			var window = OwnerWindow ?? ParentWindow;
			if (window != null)
			{
				var parentFrame = window.Frame;
				var frame = Frame;
				var location = new CGPoint((parentFrame.Width - frame.Width) / 2 + parentFrame.X, (parentFrame.Height - frame.Height) / 2 + parentFrame.Y);
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
				base.Zoom(sender ?? this); // null when double clicking the title bar, but xammac/monomac doesn't allow it
				zoom = true;
			}
			Handler.Callback.OnWindowStateChanged(Handler.Widget, EventArgs.Empty);
		}

		public bool DisableSetOrigin { get; set; }

		public override void SetFrameOrigin(CGPoint aPoint)
		{
			if (!DisableSetOrigin)
				base.SetFrameOrigin(aPoint);
		}

		public override void RecalculateKeyViewLoop()
		{
			base.RecalculateKeyViewLoop();

			NSView last = null;
			Handler?.RecalculateKeyViewLoop(ref last);
		}
	}

	class EtoContentView : MacPanelView
	{
	}

	public interface IMacWindow : IMacControlHandler
	{
		Rectangle? RestoreBounds { get; set; }

		Window Widget { get; }

		NSMenu MenuBar { get; }

		NSObject FieldEditorClient { get; set; }

		MacFieldEditor FieldEditor { get; }

		bool CloseWindow(Action<CancelEventArgs> closing = null);

		NSWindow Control { get; }

		Window.ICallback Callback { get; }
	}

	static class MacWindow
	{
		internal static readonly object InitialLocation_Key = new object();
		internal static readonly object PreferredClientSize_Key = new object();
		internal static readonly Selector selSetStyleMask = new Selector("setStyleMask:");
		internal static IntPtr selMainMenu = Selector.GetHandle("mainMenu");
		internal static IntPtr selSetMainMenu = Selector.GetHandle("setMainMenu:");
		internal static readonly object SetAsChildWindow_Key = new object();
		internal static readonly IntPtr selWindows_Handle = Selector.GetHandle("windows");
		internal static readonly IntPtr selCount_Handle = Selector.GetHandle("count");
		internal static readonly IntPtr selObjectAtIndex_Handle = Selector.GetHandle("objectAtIndex:");
		internal static readonly IntPtr selMakeKeyWindow_Handle = Selector.GetHandle("makeKeyWindow");
		internal static readonly IntPtr selIsVisible_Handle = Selector.GetHandle("isVisible");
	}

	public abstract class MacWindow<TControl, TWidget, TCallback> : MacPanel<TControl, TWidget, TCallback>, Window.IHandler, IMacWindow
		where TControl : NSWindow
		where TWidget : Window
		where TCallback : Window.ICallback
	{
		MacFieldEditor fieldEditor;
		MenuBar menuBar;
		Icon icon;
		Eto.Forms.ToolBar toolBar;
		Rectangle? restoreBounds;
		bool setInitialSize;
		WindowState? initialState;
		bool maximizable = true;
		Point? oldLocation;


		Point? InitialLocation
		{
			get => Widget.Properties.Get<Point?>(MacWindow.InitialLocation_Key);
			set => Widget.Properties.Set(MacWindow.InitialLocation_Key, value);
		}

		Window.ICallback IMacWindow.Callback { get { return Callback; } }

		public override NSView ContainerControl { get { return Control.ContentView; } }

		public override object EventObject { get { return Control; } }

		public NSObject FieldEditorClient { get; set; }

		public MacFieldEditor FieldEditor => fieldEditor;

		/// <summary>
		/// Allow moving the window by dragging the background, null to only enable it in certain cases (e.g. when borderless)
		/// </summary>
		public bool MovableByWindowBackground
		{
			get => Control.MovableByWindowBackground;
			set => Control.MovableByWindowBackground = value;
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			SizeF naturalSize = new SizeF(200, 200);
			var preferredClientSize = PreferredClientSize;
			if (Content != null && Content.Visible)
			{
				var contentControl = Content.GetMacControl();
				if (contentControl != null)
				{
					if (preferredClientSize?.Width > 0)
						availableSize.Width = preferredClientSize.Value.Width;
					if (preferredClientSize?.Height > 0)
						availableSize.Height = preferredClientSize.Value.Height;
					naturalSize = contentControl.GetPreferredSize(availableSize - Padding.Size) + Padding.Size;
				}
			}
			if (preferredClientSize != null)
			{
				if (preferredClientSize.Value.Width >= 0)
					naturalSize.Width = preferredClientSize.Value.Width;
				if (preferredClientSize.Value.Height >= 0)
					naturalSize.Height = preferredClientSize.Value.Height;
			}

			return naturalSize;
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
							return new CGSize((float)Math.Max(frameSize.Width, value.Width), (float)Math.Max(frameSize.Height, value.Height));
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

		IntPtr oldMenu;

		static void HandleDidBecomeKey(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as MacWindow<TControl, TWidget, TCallback>;
			handler?.SetMenu();
		}

		static void HandleDidResignKey(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as MacWindow<TControl, TWidget, TCallback>;
			handler?.RestoreMenu();
		}

		static bool HandleShouldZoom(NSWindow window, CGRect newFrame)
		{
			var handler = GetHandler(window) as MacWindow<TControl, TWidget, TCallback>;
			if (handler == null)
				return true;
			if (!handler.Maximizable)
				return false;
			if (!window.IsZoomed && window.Screen != null)
			{
				handler.RestoreBounds = handler.Widget.Bounds;
			}
			return true;

		}

		static void HandleWillMiniaturize(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as MacWindow<TControl, TWidget, TCallback>;
			if (handler == null)
				return;
			handler.RestoreBounds = handler.Widget.Bounds;
		}

		static void HandleWillClose(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as MacWindow<TControl, TWidget, TCallback>;
			if (handler == null || !handler.Widget.Loaded) // could already be closed
				return;
			if (ApplicationHandler.Instance.ShouldCloseForm(handler.Widget, true))
				handler.Callback.OnClosed(handler.Widget, EventArgs.Empty);
		}

		static bool HandleWindowShouldClose(NSObject sender)
		{
			var handler = GetHandler(sender) as MacWindow<TControl, TWidget, TCallback>;
			if (handler == null)
				return true;
			var args = new CancelEventArgs();
			if (ApplicationHandler.Instance.ShouldCloseForm(handler.Widget, false))
				handler.Callback.OnClosing(handler.Widget, args);
			return !args.Cancel;
		}

		static void HandleWindowStateChanged(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as MacWindow<TControl, TWidget, TCallback>;
			if (handler == null)
				return;
			handler.Callback.OnWindowStateChanged(handler.Widget, EventArgs.Empty);
		}

		static void HandleGotFocus(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as MacWindow<TControl, TWidget, TCallback>;
			if (handler == null)
				return;
			handler.Callback.OnGotFocus(handler.Widget, EventArgs.Empty);
		}

		static void HandleLostFocus(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as MacWindow<TControl, TWidget, TCallback>;
			if (handler == null)
				return;
			handler.Callback.OnLostFocus(handler.Widget, EventArgs.Empty);
		}


		public override void AttachEvent(string id)
		{
			// when attaching to a native control, we can't handle any events as it'll override its delegate.
			if (!(Control is IMacControl))
				return;
			switch (id)
			{
				case Window.ClosedEvent:
					Control.WillClose += HandleWillClose;
					break;
				case Window.ClosingEvent:
					Control.WindowShouldClose = HandleWindowShouldClose;
					break;
				case Window.WindowStateChangedEvent:
					Control.DidMiniaturize += HandleWindowStateChanged;
					Control.DidDeminiaturize += HandleWindowStateChanged;
					break;
				case Eto.Forms.Control.ShownEvent:
					// handled when shown
					break;
				case Eto.Forms.Control.GotFocusEvent:
					Control.DidBecomeKey += HandleGotFocus;
					break;
				case Eto.Forms.Control.LostFocusEvent:
					Control.DidResignKey += HandleLostFocus;
					break;
				case Eto.Forms.Control.SizeChangedEvent:
					{
						Size? oldSize = null;
						AddObserver(NSWindow.DidResizeNotification, e =>
						{
							var handler = (MacWindow<TControl, TWidget, TCallback>)e.Handler;
							var newSize = handler.Size;
							if (oldSize != newSize)
							{
								handler.Callback.OnSizeChanged(handler.Widget, EventArgs.Empty);
								oldSize = newSize;
							}
						});
					}
					break;
				case Window.LocationChangedEvent:
					{
						AddObserver(NSWindow.DidMoveNotification, e =>
						{
							var handler = e.Handler as MacWindow<TControl, TWidget, TCallback>;
							if (handler != null)
							{
								var old = handler.oldLocation;
								handler.oldLocation = null;
								var newLocation = handler.Location;
								if (old != newLocation)
								{
									handler.oldLocation = newLocation;
									handler.Callback.OnLocationChanged(handler.Widget, EventArgs.Empty);
								}
							}
						});
						// WillMove is only called when the user moves the window via the mouse
						Control.WillMove += HandleWillMove;
					}
					break;
				case Window.LogicalPixelSizeChangedEvent:
					AddObserver(NSWindow.DidChangeBackingPropertiesNotification, e =>
					{
						var handler = e.Handler as MacWindow<TControl, TWidget, TCallback>;
						if (handler != null)
						{
							var args = new NSWindowBackingPropertiesEventArgs(e.Notification);
							if (args.OldScaleFactor != handler.Control.BackingScaleFactor)
								handler.Callback.OnLogicalPixelSizeChanged(handler.Widget, EventArgs.Empty);
						}
					});
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
				Control.ContentView.AddCursorRect(new CGRect(CGPoint.Empty, Control.Frame.Size), Cursor.ControlObject as NSCursor);
			}
			else
				Control.ContentView.DiscardCursorRects();
		}

		/// <summary>
		/// Tracks movement of the window until the mouse up button is found
		/// </summary>
		static void HandleWillMove(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as MacWindow<TControl, TWidget, TCallback>;
			if (handler == null)
				return;
			handler.oldLocation = null;
			// find offset of mouse cursor to location of window
			var moveOffset = Size.Round((SizeF)(Mouse.Position - handler.Location));

			ThreadPool.QueueUserWorkItem(a =>
			{
				bool tracking = true;
				while (tracking)
				{
					NSApplication.SharedApplication.InvokeOnMainThread(() =>
					{
						var newLocation = Point.Round(Mouse.Position - moveOffset);
						if (handler.oldLocation != newLocation)
						{
							handler.Callback.OnLocationChanged(handler.Widget, EventArgs.Empty);
							handler.oldLocation = newLocation;
						}
						// check for mouse up event
						tracking = NSApplication.SharedApplication.NextEventEx(NSEventMask.LeftMouseUp, null, NSRunLoop.NSRunLoopEventTracking, false) == null;
					});
				}
				handler.oldLocation = null;
				NSApplication.SharedApplication.InvokeOnMainThread(() => handler.Callback.OnLocationChanged(handler.Widget, EventArgs.Empty));
			});
		}

		protected virtual void ConfigureWindow()
		{
			var myWindow = Control as EtoWindow;
			if (myWindow != null)
				myWindow.Handler = this;
			Control.ContentView = new EtoContentView { WeakHandler = new WeakReference(this) };
			//Control.ContentMinSize = new System.Drawing.SizeF(0, 0);
			Control.ContentView.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;

			if (!MacVersion.IsAtLeast(10, 12))
			{
				// need at least one constraint to enable auto-layout, which calls NSView.Layout automatically.
				Control.ContentView.AddConstraint(NSLayoutConstraint.Create(Control.ContentView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, Control.ContentView, NSLayoutAttribute.Leading, 1, 0));
			}

			Control.ReleasedWhenClosed = false;
			Control.HasShadow = true;
			Control.ShowsResizeIndicator = true;
			Control.AutorecalculatesKeyViewLoop = true;
			Control.WillReturnFieldEditor = HandleWillReturnFieldEditor;
			Control.DidBecomeKey += HandleDidBecomeKey;
			Control.DidResignKey += HandleDidResignKey;
			Control.ShouldZoom = HandleShouldZoom;
			Control.WillMiniaturize += HandleWillMiniaturize;
#if MONOMAC
			// AppKit still calls some delegate methods on the window after closing a form (e.g. WillReturnFieldEditor),
			// causing exceptions trying to recreate the delegate if it has been garbage collected.
			// This is because MonoMac doesn't use ref counting to determine when objects can be GC'd like Xamarin.Mac.
			// We avoid this problem by clearing out the delegate after the window is closed.
			// In Eto, we don't expect any events to be called after that point anyway.
			Widget.Closed += (sender, e) => Application.Instance.AsyncInvoke(() => Control.Delegate = null);
#endif
		}

		static NSObject HandleWillReturnFieldEditor(NSWindow sender, NSObject client)
		{
			var handler = GetHandler(sender) as MacWindow<TControl, TWidget, TCallback>;
			if (handler == null)
				return null;
			handler.FieldEditorClient = client;
			return handler.fieldEditor ?? (handler.fieldEditor = new MacFieldEditor());
		}

		public override NSView ContentControl => Control.ContentView;

		public virtual string Title { get => Control.Title; set => Control.Title = value ?? ""; }

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
				if (Control.RespondsToSelector(MacWindow.selSetStyleMask))
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
				if (Control.RespondsToSelector(MacWindow.selSetStyleMask))
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
					SetButtonStates();
				}
			}
		}

		public bool ShowInTaskbar
		{
			get;
			set;
		}

		protected virtual NSWindowLevel TopmostWindowLevel => NSWindowLevel.PopUpMenu;

		public bool Topmost
		{
			get => Control.Level >= NSWindowLevel.Floating;
			set
			{
				if (Topmost != value)
				{
					Control.Level = value ? TopmostWindowLevel : NSWindowLevel.Normal;
				}
			}
		}

		public override Size Size
		{
			get
			{
				if (!Widget.Loaded)
					return UserPreferredSize;
				return Control.Frame.Size.ToEtoSize();
			}
			set
			{
				var oldFrame = Control.Frame;
				var newFrame = oldFrame;
				if (value.Width >= 0)
					newFrame.Width = value.Width;
				if (value.Height > 0)
				{
					newFrame.Height = value.Height;
					newFrame.Y = (nfloat)Math.Max(0, oldFrame.Y - (value.Height - oldFrame.Height));
				}
				Control.SetFrame(newFrame, true);
				UserPreferredSize = value;
				SetAutoSize();
			}
		}


		protected override void SetAutoSize()
		{
			base.SetAutoSize();
			if (PreferredClientSize != null)
				AutoSize &= PreferredClientSize.Value.Width == -1 || PreferredClientSize.Value.Height == -1;
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
					SetMenu();
			}
		}

		void SetMenu()
		{
			if (MenuBar != null)
			{
				// if not zero, it's already saved
				if (oldMenu == IntPtr.Zero)
				{
					oldMenu = Messaging.IntPtr_objc_msgSend(NSApplication.SharedApplication.Handle, MacWindow.selMainMenu);
					if (oldMenu != IntPtr.Zero)
					{
						// remember old native menu so we can restore it later
						MacExtensions.Retain(oldMenu);
					}
				}

				NSApplication.SharedApplication.MainMenu = MenuBar;
				RemoveSuperfluousCloseAll();
			}
			else
			{
				// restore the menu since we no longer have a menu specific to this window.
				RestoreMenu();
			}

		}

		void RestoreMenu()
		{
			if (oldMenu != IntPtr.Zero)
			{
				// restore old native menu
				Messaging.void_objc_msgSend_IntPtr(NSApplication.SharedApplication.Handle, MacWindow.selSetMainMenu, oldMenu);
				MacExtensions.Release(oldMenu);
				oldMenu = IntPtr.Zero;
				RemoveSuperfluousCloseAll();
			}
		}

		/// <summary>
		/// Removes the Close All menu item for document-based apps
		/// </summary>
		/// <remarks>
		/// macOS automatically re-adds this back for document based apps, but not for SaveAs/Duplicate
		/// Appears to be a bug (feature) of macOS.
		/// </remarks>
		void RemoveSuperfluousCloseAll()
		{
			var menu = NSApplication.SharedApplication.MainMenu;
			if (menu == null)
				return;
			for (int j = 0; j < menu.Count; j++)
			{
				var item = menu.ItemAt(j);
				if (!item.HasSubmenu)
					continue;
				var submenu = item.Submenu;
				for (int i = 0; i < submenu.Count; i++)
				{
					var submenuItem = submenu.ItemAt(i);
					if (submenuItem.Title == "<<Close All - unlocalized>>" && submenuItem.Action?.Name == "closeAll:")
					{
						submenu.RemoveItemAt(i);
						return;
					}
				}
			}
		}

		public bool CloseWindow(Action<CancelEventArgs> closing = null)
		{
			if (!Widget.Loaded)
				return true;

			var args = new CancelEventArgs();
			Callback.OnClosing(Widget, args);
			if (!args.Cancel && closing != null)
				closing(args);
			if (!args.Cancel)
			{
				Callback.OnClosed(Widget, EventArgs.Empty);
			}
			return !args.Cancel;
		}

		public virtual void Close()
		{
			var args = new CancelEventArgs();
			Callback.OnClosing(Widget, args);
			if (!args.Cancel)
				Control.Close();
		}

		public Eto.Forms.ToolBar ToolBar
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

		public Size? PreferredClientSize
		{
			get { return Widget.Properties.Get<Size?>(MacWindow.PreferredClientSize_Key); }
			set { Widget.Properties[MacWindow.PreferredClientSize_Key] = value; }
		}

		public override Size ClientSize
		{
			get { return Control.ContentView.Frame.Size.ToEtoSize(); }
			set
			{
				var oldFrame = Control.Frame;
				var oldSize = Control.ContentView.Frame;
				Control.SetFrameOrigin(new CGPoint(oldFrame.X, (nfloat)Math.Max(0, oldFrame.Y - (value.Height - oldSize.Height))));
				Control.SetContentSize(value.ToNS());
				if (!Widget.Loaded)
				{
					PreferredClientSize = value;
					if (value.Width != -1 && value.Height != -1)
						UserPreferredSize = new Size(-1, -1);
					else if (value.Width != -1)
						UserPreferredSize = new Size(-1, UserPreferredSize.Height);
					else if (value.Height != -1)
						UserPreferredSize = new Size(UserPreferredSize.Width, -1);
				}
				SetAutoSize();
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
				if (Visible != value)
				{
					Control.IsVisible = value;
					if (Widget.Loaded && value)
						FireOnShown();
				}
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
				if (Widget.Loaded)
					SetLocation(value);
				else
					InitialLocation = value;
				var etoWindow = Control as EtoWindow;
				if (etoWindow != null)
					etoWindow.DisableCenterParent = true;
			}
		}

		void SetLocation(Point value)
		{
			// location is relative to the main screen, translate to bottom left, inversed
			var mainFrame = NSScreen.Screens[0].Frame;
			var frame = Control.Frame;
			var point = new CGPoint((nfloat)value.X, (nfloat)(mainFrame.Height - value.Y));
			if (Control.Screen == null)
			{
				// ensure that the control lands on a screen
				point.X = (nfloat)Math.Min(Math.Max(mainFrame.X, point.X), mainFrame.Right - frame.Width);
				point.Y = (nfloat)Math.Min(Math.Max(mainFrame.Y, point.Y), mainFrame.Bottom - frame.Height);
			}
			Control.SetFrameTopLeftPoint(point);
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

		public Rectangle RestoreBounds
		{
			get { return WindowState == WindowState.Normal ? Widget.Bounds : restoreBounds ?? Widget.Bounds; }
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
			if (AutoSize)
			{
				AutoSize = false;
				var availableSize = SizeF.PositiveInfinity;
				var borderSize = GetBorderSize();
				if (UserPreferredSize.Width != -1)
					availableSize.Width = UserPreferredSize.Width - borderSize.Width;
				if (UserPreferredSize.Height != -1)
					availableSize.Height = UserPreferredSize.Height - borderSize.Height;
				var size = GetPreferredSize(availableSize);
				SetContentSize(size.ToNS());
				setInitialSize = true;
			}
			PositionWindow();
			base.OnLoad(e);
		}

		Size GetBorderSize()
		{
			return Control.FrameRectFor(CGRect.Empty).Size.ToEtoSize();
		}

		protected override void FireOnShown()
		{
			Control.ContentView.LayoutSubtreeIfNeeded();
			base.FireOnShown();
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			if (initialState != null)
			{
				WindowState = initialState.Value;
				initialState = null;
				Callback.OnSizeChanged(Widget, EventArgs.Empty);
			}
			else if (setInitialSize)
				Callback.OnSizeChanged(Widget, EventArgs.Empty);
		}

		protected virtual void PositionWindow()
		{
			if (InitialLocation != null)
			{
				SetLocation(InitialLocation.Value);
				InitialLocation = null;
			}
			else
				Control.Center();
		}

		#region IMacContainer implementation

		void SetContentSize(CGSize contentSize)
		{
			if (MinimumSize != Size.Empty)
			{
				contentSize.Width = (nfloat)Math.Max(contentSize.Width, MinimumSize.Width);
				contentSize.Height = (nfloat)Math.Max(contentSize.Height, MinimumSize.Height);
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

		Window IMacWindow.Widget => Widget;

		NSWindow IMacWindow.Control => Control;

		#endregion

		public Screen Screen
		{
			get
			{
				var screen = Control.Screen;
				return screen != null ? new Screen(new ScreenHandler(screen)) : null;
			}
		}

		public override PointF PointFromScreen(PointF point)
		{
			var sdpoint = point.ToNS();
			sdpoint = Control.ConvertBaseToScreen(sdpoint);
			sdpoint.Y = Control.Screen.Frame.Height - sdpoint.Y;
			return sdpoint.ToEto();
		}

		public override PointF PointToScreen(PointF point)
		{
			var sdpoint = point.ToNS();
			sdpoint = Control.ConvertBaseToScreen(sdpoint);
			sdpoint.Y = Control.Screen.Frame.Height - sdpoint.Y;
			return sdpoint.ToEto();
		}

		public WindowStyle WindowStyle
		{
			get { return Control.StyleMask.ToEtoWindowStyle(); }
			set
			{
				if (Control.RespondsToSelector(MacWindow.selSetStyleMask))
				{
					var newStyleMask = value.ToNS(Control.StyleMask);
					var title = Title;
					//Control.StyleMask = 0; // only way to reset titled??
					Control.StyleMask = newStyleMask;
					Title = title; // gets cleared here.. ugh.

					// don't use animation when there's no border.
					if (value == WindowStyle.None && Control.AnimationBehavior == NSWindowAnimationBehavior.Default)
						Control.AnimationBehavior = NSWindowAnimationBehavior.None;
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
			// resign key window.  This does not set another window as key, so we do it below to match other platforms.
			Control.ResignKeyWindow();

			// using NSApplication.SharedApplication.Windows is not recommended apparently as it can prevent windows from properly closing/disposing.

			// find the visible window ordered before this one, and make it key
			var arrPtr = Messaging.IntPtr_objc_msgSend(NSApplication.SharedApplication.Handle, MacWindow.selWindows_Handle);
			if (arrPtr != IntPtr.Zero)
			{
				var thisWindowHandle = Control.Handle;
				var lastWindowHandle = IntPtr.Zero;
				var count = Messaging.nuint_objc_msgSend(arrPtr, MacWindow.selCount_Handle);
				for (nuint i = 0; i < count; i++)
				{
					var windowHandle = Messaging.IntPtr_objc_msgSend_nuint(arrPtr, MacWindow.selObjectAtIndex_Handle, i);
					if (windowHandle == thisWindowHandle)
					{
						// found this window, set the last visible as key
						if (lastWindowHandle != IntPtr.Zero)
							Messaging.void_objc_msgSend(lastWindowHandle, MacWindow.selMakeKeyWindow_Handle);
						break;
					}
					// only set as last if it is visible
					if (Messaging.bool_objc_msgSend(windowHandle, MacWindow.selIsVisible_Handle))
						lastWindowHandle = windowHandle;
				}
			}

			/* the above does this but in a less dangerous/leaky way.
			var window = NSApplication.SharedApplication.Windows.FirstOrDefault(r => r.IsVisible && r != Control);
			if (window != null)
				window.MakeKeyWindow();
			/**/

			// order back after finding the window to set as key
			Control.OrderBack(Control);
		}

		protected virtual bool DefaultSetAsChildWindow => false;

		/// <summary>
		/// Gets or sets a value indicating that this window should be set as a child of its owner.
		/// When it is a child, it will move with the owner.
		/// This is useful when you want a modal dialog to move with the window, or to disable this
		/// feature for forms with the owner set.
		/// </summary>
		public bool SetAsChildWindow
		{
			get => Widget.Properties.Get<bool>(MacWindow.SetAsChildWindow_Key, DefaultSetAsChildWindow);
			set => Widget.Properties.Set(MacWindow.SetAsChildWindow_Key, value, DefaultSetAsChildWindow);
		}

		protected void EnsureOwner() => SetOwner(Widget.Owner);

		public virtual void SetOwner(Window owner)
		{
			if (SetAsChildWindow && Widget.Loaded)
			{
				if (owner != null)
				{
					var macWindow = owner.Handler as IMacWindow;
					if (macWindow != null)
						macWindow.Control.AddChildWindow(Control, NSWindowOrderingMode.Above);
				}
				else
				{
					var parentWindow = Control.ParentWindow;
					if (parentWindow != null)
						parentWindow.RemoveChildWindow(Control);
				}
			}
		}

		public float LogicalPixelSize => Screen?.LogicalPixelSize ?? 1f;
	}
}
