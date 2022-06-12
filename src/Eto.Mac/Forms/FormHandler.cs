using System;
using Eto.Forms;



namespace Eto.Mac.Forms
{
	public class FormHandler : FormHandler<NSWindow>
	{
		public FormHandler()
		{
		}

		public FormHandler(NSWindow window)
			: base(window)
		{
		}
		public FormHandler(NSWindowController controller)
			: base(controller)
		{
		}

		protected override NSWindow CreateControl()
		{
			return new EtoWindow(new CGRect(0, 0, 200, 200), 
				NSWindowStyle.Resizable | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Titled, 
				NSBackingStore.Buffered, false);
		}
	}

	public class FormHandler<TWindow> : MacWindow<TWindow, Form, Form.ICallback>, Form.IHandler
		where TWindow: NSWindow
	{
		#pragma warning disable 414
		// keep reference to controller so it doesn't get disposed
		NSWindowController controller;
		#pragma warning restore 414
		protected override bool DisposeControl { get { return false; } }

		public FormHandler()
		{
		}

		public FormHandler(NSWindow window)
		{
			Control = (TWindow)window;
		}

		public FormHandler(NSWindowController controller)
		{
			this.controller = controller;
			Control = (TWindow)controller.Window;
		}

		protected override void Initialize()
		{
			ConfigureWindow();

			base.Initialize();
		}

		protected override bool DefaultSetAsChildWindow => true;

		public virtual void Show()
		{
			var visible = Control.IsVisible;
			if (ShowActivated)
			{
				if (WindowState == WindowState.Minimized)
					Control.MakeKeyWindow();
				else
					Control.MakeKeyAndOrderFront(ApplicationHandler.Instance.AppDelegate);
			}
			else
			{
				Control.OrderFront(ApplicationHandler.Instance.AppDelegate);
			}
			
			// setting the owner shows the window, so we have to do this here.
			EnsureOwner();

			if (!visible)
			{
				FireOnShown();
			}
		}

		public override bool Visible
		{
			get => base.Visible;
			set
			{
				if (value && !ShowActivated)
				{
					if (value != Visible)
					{
						Control.OrderFront(ApplicationHandler.Instance.AppDelegate);
						FireOnShown();
					}
				}
				else
					base.Visible = value;
			}
		}

		public bool ShowActivated { get; set; } = true;

		public bool CanFocus
		{
			get { return (Control as EtoWindow)?.CanFocus ?? Control.CanBecomeKeyWindow; }
			set
			{
				var etoWindow = Control as EtoWindow;
				if (etoWindow != null)
					etoWindow.CanFocus = value;
			}
		}
	}
}
