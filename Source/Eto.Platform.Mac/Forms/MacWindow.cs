using System;
using System.ComponentModel;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using Eto.Platform.Mac.Forms.Controls;

namespace Eto.Platform.Mac.Forms
{
	public class MyWindow : NSWindow
	{
		SD.RectangleF oldFrame;
		bool zoom;
		
		public IMacWindow Handler { get; set; }
		
		public MyWindow (SD.Rectangle rect, NSWindowStyle style, NSBackingStore store, bool flag)
			: base(rect, style, store, flag)
		{
		}
		
		public override void Center ()
		{
			// implement centering to parent if there is a parent window for this one..
			if (this.ParentWindow != null) {
				var parentFrame = this.ParentWindow.Frame;
				var frame = this.Frame;
				SD.PointF location = new SD.PointF ((parentFrame.Width - frame.Width) / 2 + parentFrame.X, (parentFrame.Height - frame.Height) / 2 + parentFrame.Y);
				this.SetFrameOrigin (location);
			} else
				base.Center ();
		}
		
		public override void Zoom (NSObject sender)
		{
			if (zoom) {
				this.SetFrame (oldFrame, true);
				zoom = false;
			} else {
				oldFrame = this.Frame;
				base.Zoom (sender);
				zoom = true;
			}
		}
	}
	
	public interface IMacWindow
	{
		Rectangle? RestoreBounds { get; set; }

		Window Widget { get; }

		NSMenu MenuBar { get; }

		NSObject FieldEditorObject { get; set; }

		Size? MinimumSize { get; }
		
		bool CloseWindow ();
	}

	public class CustomFieldEditor : NSTextView
	{
		public Control Widget { get; set; }

		public override void KeyDown (NSEvent theEvent)
		{
			if (!MacEventView.KeyDown (Widget, theEvent)) {
				base.KeyDown (theEvent);
			}
		}
	}
		

	public abstract class MacWindow<T, W> : MacObject<T, W>, IWindow, IMacContainer, IMacWindow
		where T: MyWindow
		where W: Eto.Forms.Window
	{
		MenuBar menuBar;
		Icon icon;
		ToolBar toolBar;
		Rectangle? restoreBounds;
		Cursor cursor;
		bool setInitialSize;
		Size? minimumSize;
		WindowState? initialState;
		
		public NSObject FieldEditorObject { get; set; }
		
		public bool AutoSize { get; private set; }
		
		public virtual Size GetPreferredSize ()
		{
			if (Widget.Layout != null) {
				var layout = Widget.Layout.InnerLayout.Handler as IMacLayout;
				if (layout != null)
					return layout.GetPreferredSize ();
			}
			return new Size (200, 200);
		}
		
		public Size? MinimumSize {
			get { return minimumSize; }
			set {
				minimumSize = value;
				if (minimumSize != null) {
					Control.WillResize = (sender, frameSize) => {
						if (minimumSize != null) {
							return new SD.SizeF(Math.Max (frameSize.Width, minimumSize.Value.Width), Math.Max (frameSize.Height, minimumSize.Value.Height));
						}
						else return frameSize;
					};
				}
				else
					Control.WillResize = null;
			}
		}
		
		public NSMenu MenuBar {
			get { return menuBar != null ? menuBar.ControlObject as NSMenu : null; }
		}

		public MacWindow ()
		{
			AutoSize = true;
			
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			Control.DidBecomeKey += delegate {
				if (MenuBar != null) 
					NSApplication.SharedApplication.SetMainMenu (MenuBar);
			};
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Window.ClosedEvent:
				Control.WillClose += delegate {
					Widget.OnClosed (EventArgs.Empty);
				};
				break;
			case Window.ClosingEvent:
				Control.WindowShouldClose = (sender) => {
					var args = new CancelEventArgs ();
					Widget.OnClosing (args);
					return !args.Cancel;
				};
				break;
			case Window.MaximizedEvent:
				Control.ShouldZoom = (window, newFrame) => {
					if (!window.IsZoomed) {
						RestoreBounds = Widget.Bounds;
						Widget.OnMaximized (EventArgs.Empty);
					}
					return true;
				};
				break;
			case Window.MinimizedEvent:
				Control.WillMiniaturize += delegate {
					this.RestoreBounds = Widget.Bounds;
					Widget.OnMinimized (EventArgs.Empty);
				};
				break;
			case Eto.Forms.Control.ShownEvent:
				Control.DidBecomeKey += delegate {
					Widget.OnShown (EventArgs.Empty);
				};
				break;
			case Eto.Forms.Control.HiddenEvent:
				// handled by delegate
				break;
			case Eto.Forms.Control.KeyDownEvent:
				// TODO
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		void CreateCursorRect ()
		{
			if (cursor != null) {
				this.Control.ContentView.DiscardCursorRects ();
				this.Control.ContentView.AddCursorRect (new SD.RectangleF (SD.PointF.Empty, this.Control.Frame.Size), cursor.ControlObject as NSCursor);
			} else
				this.Control.ContentView.DiscardCursorRects ();
		}

		protected void ConfigureWindow ()
		{
			Control.Handler = this;
			Control.ContentView = new NSView ();
			//Control.ContentMinSize = new System.Drawing.SizeF(0, 0);
			Control.ContentView.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
			Control.ReleasedWhenClosed = false;
			Control.HasShadow = true;
			Control.ShowsResizeIndicator = true;
			//Control.Delegate = new MacWindowDelegate{ Handler = this };
			Control.WillReturnFieldEditor = (sender, forObject) => {
				FieldEditorObject = forObject;
				var control = forObject as IMacControl;
				if (control != null) {
					var handler = control.Handler as IMacViewHandler;
					if (handler != null && handler.IsEventHandled(TextBox.KeyDownEvent)) {
						return new CustomFieldEditor { Widget = handler.Widget };
					}
				}
				return null;
			};
		}
		
		public virtual object ContainerObject {
			get { return Control.ContentView; }
		}
		
		public virtual string Title { get { return Control.Title; } set { Control.Title = value; } }

		public bool Resizable {
			get { return (Control.StyleMask & NSWindowStyle.Resizable) != 0; }
			set {
				if (Control.RespondsToSelector (new Selector ("setStyleMask:"))) {
					if (value)
						Control.StyleMask |= NSWindowStyle.Resizable;
					else
						Control.StyleMask &= ~NSWindowStyle.Resizable;
				} else {
					// 10.5, what do we do?!
				}
			}
		}
		
		public virtual Size Size {
			get {
				return Generator.ConvertF (Control.Frame.Size);
			}
			set {
				Control.SetFrame (Generator.ConvertF (Control.Frame, value), true);
				AutoSize = false;
			}
		}
		
		public virtual void SetLayout (Layout layout)
		{
			var maclayout = layout.Handler as IMacLayout;
			if (maclayout == null)
				return;
			var control = maclayout.LayoutObject as NSView;
			if (control != null) {
				var view = ContainerObject as NSView;
				//control.SetFrameSize (view.Frame.Size);
				control.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
				view.AddSubview (control);
			}
		}
		
		public virtual void SetParentLayout (Layout layout)
		{
		}
		
		public virtual void SetParent (Control parent)
		{
		}

		public MenuBar Menu {
			get {
				return menuBar;
			}
			set {
				this.menuBar = value;
				if (Control.IsKeyWindow)
					NSApplication.SharedApplication.SetMainMenu ((NSMenu)value.ControlObject);
			}
		}
		
		public bool CloseWindow ()
		{
			var args = new CancelEventArgs ();
			Widget.OnClosing (args);
			if (!args.Cancel) {
				Widget.OnClosed (EventArgs.Empty);
			}
			return !args.Cancel;
		}

		public void Close ()
		{
			Control.PerformClose (Control);
		}

		public ToolBar ToolBar {
			get {
				return toolBar;
			}
			set {
				toolBar = value;
				Control.Toolbar = (NSToolbar)toolBar.ControlObject;
			}
		}

		public Icon Icon {
			get { return icon; }
			set {
				icon = value;
				// don't really do anything here..  no where to put it
			}
		}

		public void Invalidate ()
		{
			Control.ContentView.NeedsDisplay = true;
		}

		void IControl.Invalidate (Rectangle rect)
		{
			Control.ContentView.SetNeedsDisplayInRect (Generator.ConvertF (rect));
		}

		public Graphics CreateGraphics ()
		{
			return null;
		}

		public void SuspendLayout ()
		{
		}

		public void ResumeLayout ()
		{
		}

		public void Focus ()
		{
			Control.BecomeFirstResponder ();
		}

		public Color BackgroundColor { get; set; }

		public string Id { get; set; }

		public Size ClientSize {
			get { return Generator.ConvertF (Control.ContentView.Frame.Size); }
			set { 
				Control.SetContentSize (Generator.ConvertF (value));
				AutoSize = false;
			}
		}

		public bool Enabled { get; set; }

		public bool HasFocus {
			get { return Control.IsKeyWindow; }
			set {
				if (value)
					Control.MakeKeyWindow ();
				else
					Control.ResignKeyWindow ();
			}
		}

		public bool Visible {
			get { return Control.IsVisible; }
			set {
				if (value && !Control.IsVisible)
					Control.MakeKeyAndOrderFront (NSApplication.SharedApplication);
				if (!value && Control.IsVisible)
					Control.PerformClose (NSApplication.SharedApplication);
				// huh?
			}
		}
		
		public Cursor Cursor {
			get { return cursor; }
			set {
				cursor = value;
				CreateCursorRect ();
			}
		}

		public string ToolTip {
			get { return this.Control.ContentView.ToolTip; }
			set { Control.ContentView.ToolTip = value; }
		}
		
		public virtual Point Location {
			get {
				var height = Control.Screen.Frame.Height;
				var frame = Control.Frame;
				return new Point ((int)frame.X, (int)(height - frame.Y - frame.Height));
			}
			set {
				var height = Control.Screen.Frame.Height;
				var frame = Control.Frame;
				Control.SetFrameOrigin (new SD.PointF (value.X, height - value.Y - frame.Height));
			}
		}
		
		public WindowState State {
			get {
				if (initialState != null)
					return initialState.Value;
				if (Control.IsMiniaturized)
					return WindowState.Minimized;
				else if (Control.IsZoomed)
					return WindowState.Maximized;
				else
					return WindowState.Normal;
			}
			set {
				if (!Widget.Loaded) {
					initialState = value;
					return;
				}
				switch (value) {
				case WindowState.Maximized: 
					if (!Control.IsZoomed) {
						Control.Zoom (Control);
					}
					break;
				case WindowState.Minimized:
					if (!Control.IsMiniaturized)
						Control.Miniaturize (Control);
					break;
				case WindowState.Normal: 
					if (Control.IsMiniaturized)
						Control.Deminiaturize (Control);
					else if (Control.IsZoomed)
						Control.Zoom (Control);
					break;
				}
			}
		}
		
		public Rectangle? RestoreBounds {
			get { return State == WindowState.Normal ? null : restoreBounds; }
			set { restoreBounds = value; }
		}

		public double Opacity {
			get { return Control.IsOpaque ? 1.0 : Control.AlphaValue; }
			set {
				Control.IsOpaque = value == 1.0;
				Control.AlphaValue = (float)value; 
			}
		}
		
		public virtual void OnPreLoad (EventArgs e)
		{
		}
		
		public virtual void OnLoad (EventArgs e)
		{
			if (AutoSize) {
				var size = this.GetPreferredSize ();
				SetContentSize (Generator.ConvertF (size));
				setInitialSize = true;

				PositionWindow ();
				if (initialState != null) {
					State = initialState.Value;
					initialState = null;
				}
			} else {
				PositionWindow();
			}
		}

		protected virtual void PositionWindow()
		{
		}

		public virtual void OnLoadComplete (EventArgs e)
		{
		}
		
		public virtual void LayoutChildren ()
		{
			if (Widget.Layout != null) {
				var childLayout = Widget.Layout.InnerLayout.Handler as IMacLayout;
				if (childLayout != null) {
					childLayout.LayoutChildren ();
				}
			}
		}
		

		#region IMacContainer implementation
		public void SetContentSize (SD.SizeF contentSize)
		{
			if (MinimumSize != null) {
				contentSize.Width = Math.Max (contentSize.Width, MinimumSize.Value.Width);
				contentSize.Height = Math.Max (contentSize.Height, MinimumSize.Value.Height);
			}
			
			if (Widget.Loaded) {
				var diffy = this.ClientSize.Height - (int)contentSize.Height;
				var diffx = this.ClientSize.Width - (int)contentSize.Width;
				var frame = Control.Frame;
				if (diffx < 0 || !setInitialSize) {
					frame.Width -= diffx;
				}
				if (diffy < 0 || !setInitialSize) {
					frame.Y += diffy;
					frame.Height -= diffy;
				}
				Control.SetFrame (frame, false, false);
			} else 
				Control.SetContentSize (contentSize);
		}
		#endregion

		#region IMacWindow implementation
		Rectangle? IMacWindow.RestoreBounds {
			get {
				return restoreBounds;
			}
			set {
				restoreBounds = value;
			}
		}

		Eto.Forms.Window IMacWindow.Widget {
			get {
				return this.Widget;
			}
		}
		#endregion
		
		public virtual void MapPlatformAction (string systemAction, BaseAction action)
		{
		}
	}
}
