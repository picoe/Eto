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
	class FlippedView : NSView {
		
		public override bool IsFlipped {
			get {
				return true;
			}
		}
	}
	public class MyWindow : NSWindow
	{
		
		public MyWindow(SD.Rectangle rect, NSWindowStyle style, NSBackingStore store, bool flag)
			: base(rect, style, store, flag)
		{
		}
		
		public override void Center ()
		{
			// implement centering to parent if there is a parent window for this one..
			if (this.ParentWindow != null)
			{
				var parentFrame = this.ParentWindow.Frame;
				var frame = this.Frame;
				SD.PointF location = new SD.PointF((parentFrame.Width - frame.Width) / 2 + parentFrame.X, (parentFrame.Height - frame.Height) / 2 + parentFrame.Y);
				this.SetFrameOrigin (location);
			}
			else base.Center ();
		}
		
	}
	
	public abstract class MacWindow<T, W> : MacObject<T, W>, IWindow, IMacContainer
		where T: NSWindow
		where W: Eto.Forms.Window
	{
		MenuBar menuBar;
		Icon icon;
		ToolBar toolBar;
		NSWindowController controller;
		
		public bool AutoSize { get; private set; }
		

		public MacWindow(NSWindowStyle style)
		{
			var rect = new SD.Rectangle(0, 0, 200, 200);
			AutoSize = true;
			controller = new NSWindowController(new MyWindow(rect, style, NSBackingStore.Buffered, false));
			Control = (T)controller.Window;
			Control.ContentView = new FlippedView();
			//Control.ContentMinSize = new System.Drawing.SizeF(0, 0);
			Control.ContentView.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
			Control.HasShadow = true;
			Control.ShowsResizeIndicator = true; //(style & NSWindowStyle.Resizable) != 0;
		}
		
		public NSWindowController Controller { get { return controller; } }
		
		public virtual object ContainerObject
		{
			get { return Control.ContentView; }
		}
		
		
		public virtual string Text { get { return Control.Title; } set { Control.Title = value; } }

		public bool Resizable
		{
			get { return (Control.StyleMask & NSWindowStyle.Resizable) != 0; }
			set
			{
				if (Control.RespondsToSelector (new Selector("setStyleMask:"))) {
					if (value) Control.StyleMask |= NSWindowStyle.Resizable;
					else Control.StyleMask &= ~NSWindowStyle.Resizable;
				}
				else {
					// 10.5, what do we do?!
				}
			}
		}
		
		public virtual Size Size {
			get {
				return Generator.ConvertF(Control.Frame.Size);
			}
			set {
				Control.SetFrame(Generator.ConvertF(Control.Frame, value), false);
				AutoSize = false;
			}
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler)
			{
			case Window.ShownEvent:
				Control.DidBecomeKey += delegate {
					Widget.OnShown (EventArgs.Empty);
				};
				break;
			case Window.MaximizedEvent:
				Control.ShouldZoom = (window, newFrame) => {
					if (!window.IsZoomed) Widget.OnMaximized(EventArgs.Empty);
					return true;
				};
				break;
			case Window.MinimizedEvent:
				Control.WillMiniaturize += delegate {
					Widget.OnMinimized(EventArgs.Empty);
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public virtual void SetLayout(Layout layout)
		{
			var maclayout = layout.Handler as IMacLayout;
			if (maclayout == null) return;
			var control = maclayout.LayoutObject as NSView;
			if (control != null)
			{
				var view = ContainerObject as NSView;
				control.SetFrameSize(view.Frame.Size);
				control.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
				view.AddSubview(control);
			}
		}
		
		public virtual void SetParentLayout (Layout layout)
		{
		}
		
		public virtual void SetParent (Control parent)
		{
		}

		public MenuBar Menu
		{
			get
			{
				return menuBar;
			}
			set
			{
				this.menuBar = value;
				NSApplication.SharedApplication.SetMainMenu((NSMenu)value.ControlObject);
				//if (menuBar != null) menuBox.Remove((Gtk.Widget)menuBar.ControlObject);
				//menuBar = value;
				//menuBox.PackStart((Gtk.Widget)value.ControlObject); //, false, false, 0);
				//((Gtk.Widget)value.ControlObject).ShowAll();
			}
		}

		public void Close()
		{
			Control.PerformClose(Control);
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

		public Icon Icon
		{
			get { return icon; }
			set
			{
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
			Control.ContentView.SetNeedsDisplayInRect(Generator.ConvertF(rect));
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
			Control.BecomeFirstResponder();
		}

		public Color BackgroundColor { get; set; }

		public string Id { get; set; }

		public Size ClientSize
		{
			get { return Generator.ConvertF(Control.ContentView.Frame.Size); }
			set { 
				Control.SetContentSize(Generator.ConvertF(value));
				AutoSize = false;
			}
		}

		public bool Enabled { get; set; }

		public bool HasFocus
		{
			get { return Control.IsKeyWindow; }
			set
			{
				if (value) Control.MakeKeyWindow();
				else Control.ResignKeyWindow();
			}
		}

		public bool Visible
		{
			get { return Control.IsVisible; }
			set
			{
				if (value && !Control.IsVisible) Control.MakeKeyAndOrderFront(NSApplication.SharedApplication);
				if (!value && Control.IsVisible) Control.PerformClose(NSApplication.SharedApplication);
				// huh?
			}
		}
		
		public void Minimize ()
		{
			Control.Miniaturize(Control);
		}
		
		public virtual void OnLoad (EventArgs e)
		{
			if (this.AutoSize && Widget.Layout != null) {
				var layout = Widget.Layout.Handler as IMacLayout;
				if (layout != null) layout.SizeToFit ();
			}
		}

		#region ISynchronizeInvoke implementation
		
		public IAsyncResult BeginInvoke (Delegate method, object[] args)
		{
			var helper = new InvokeHelper{ Delegate = method, Args = args };
			Control.BeginInvokeOnMainThread(helper.Action);
			return null;
		}

		public object EndInvoke (IAsyncResult result)
		{
			return null;
		}
		

		public object Invoke (Delegate method, object[] args)
		{
			var helper = new InvokeHelper{ Delegate = method, Args = args };
			Control.InvokeOnMainThread(helper.Action);
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
			Control.SetContentSize (contentSize);
		}
		#endregion
	}
}
