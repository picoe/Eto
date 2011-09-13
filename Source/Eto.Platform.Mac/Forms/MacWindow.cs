using System;
using System.ComponentModel;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac
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

		public override void SetFrame (System.Drawing.RectangleF frameRect, bool display)
		{
			if (Handler.MinimumSize != null) {
				frameRect.Width = Math.Max(frameRect.Width, Handler.MinimumSize.Value.Width);
				frameRect.Height = Math.Max(frameRect.Height, Handler.MinimumSize.Value.Height);
			}
			base.SetFrame (frameRect, display);
		}
	}
	
	public interface IMacWindow
	{
		Rectangle? RestoreBounds { get; set; }
		Window Widget { get; }
		NSMenu MenuBar { get; }
		NSObject FieldEditorObject { get; set; }
		Size? MinimumSize { get; }
	}
	
	class MacWindowDelegate : NSWindowDelegate
	{
		public IMacWindow Handler { get; set; }
		
		public override NSObject WillReturnFieldEditor (NSWindow sender, NSObject client)
		{
			Handler.FieldEditorObject = client;
			return null;
		}
		
		public override bool WindowShouldClose (NSObject sender)
		{
			var args = new CancelEventArgs ();
			Handler.Widget.OnClosing (args);
			return !args.Cancel;
		}
		
		public override void WillClose (NSNotification notification)
		{
			Handler.Widget.OnClosed (EventArgs.Empty);
		}
		
		public override void WillMiniaturize (NSNotification notification)
		{
			Handler.RestoreBounds = Handler.Widget.Bounds;
			Handler.Widget.OnMinimized (EventArgs.Empty);
		}

		public override void DidBecomeKey (NSNotification notification)
		{
			if (Handler.MenuBar != null) 
				NSApplication.SharedApplication.SetMainMenu (Handler.MenuBar);
			Handler.Widget.OnShown (EventArgs.Empty);
		}
		
		public override bool ShouldZoom (NSWindow window, System.Drawing.RectangleF newFrame)
		{
			if (!window.IsZoomed) {
				Handler.RestoreBounds = Handler.Widget.Bounds;
				Handler.Widget.OnMaximized (EventArgs.Empty);
			}
			return true;
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
		
		public NSObject FieldEditorObject { get; set; }
		
		public bool AutoSize { get; private set; }
		
		public Size? MinimumSize {
			get; set;
		}
		
		public NSMenu MenuBar
		{
			get { return menuBar != null ? menuBar.ControlObject as NSMenu : null; }
		}
		

		public MacWindow ()
		{
			AutoSize = true;
		}
		
		protected void ConfigureWindow ()
		{
			Control.Handler = this;
			Control.ContentView = new NSView();
			//Control.ContentMinSize = new System.Drawing.SizeF(0, 0);
			Control.ContentView.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
			Control.ReleasedWhenClosed = false;
			Control.HasShadow = true;
			Control.ShowsResizeIndicator = true;
			Control.Delegate = new MacWindowDelegate{ Handler = this };
		}
		
		public virtual object ContainerObject {
			get { return Control.ContentView; }
		}
		
		public virtual string Text { get { return Control.Title; } set { Control.Title = value; } }

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
		
		protected virtual void OnSizeChanged (EventArgs e)
		{
			if (MinimumSize != null) {
				var size = this.Size;
				size.Width = Math.Max (size.Width, MinimumSize.Value.Width);
				size.Height = Math.Max (size.Height, MinimumSize.Value.Height);
				if (size != this.Size)
					this.Size = size;
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
				control.SetFrameSize (view.Frame.Size);
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
				if (Control.IsMiniaturized)
					return WindowState.Minimized;
				else if (Control.IsZoomed)
					return WindowState.Maximized;
				else
					return WindowState.Normal;
			}
			set {
				switch (value) {
				case WindowState.Maximized: 
					if (!Control.IsZoomed)
						Control.Zoom (Control);
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
		
		public virtual void OnLoad (EventArgs e)
		{
			if (this.AutoSize && Widget.Layout != null) {
				var layout = Widget.Layout.Handler as IMacLayout;
				if (layout != null)
					layout.SizeToFit ();
			}
		}
		
		public virtual void OnLoadComplete (EventArgs e)
		{
		}

		#region ISynchronizeInvoke implementation
		
		public IAsyncResult BeginInvoke (Delegate method, object[] args)
		{
			var helper = new InvokeHelper{ Delegate = method, Args = args };
			Control.BeginInvokeOnMainThread (helper.Action);
			return null;
		}

		public object EndInvoke (IAsyncResult result)
		{
			return null;
		}

		public object Invoke (Delegate method, object[] args)
		{
			var helper = new InvokeHelper{ Delegate = method, Args = args };
			Control.InvokeOnMainThread (helper.Action);
			return null;
		}

		public bool InvokeRequired {
			get { 
				return !NSThread.Current.IsMainThread;
			}
		}
		
		#endregion

		#region IMacContainer implementation
		public void SetContentSize (SD.SizeF contentSize)
		{
			if (MinimumSize != null) {
				contentSize.Width = Math.Max (contentSize.Width, MinimumSize.Value.Width);
				contentSize.Height = Math.Max (contentSize.Height, MinimumSize.Value.Height);
			}
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
	}
}
