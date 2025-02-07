namespace Eto.Mac.Forms
{
	public class TrayIndicatorHandler : WidgetHandler<NSStatusItem, TrayIndicator, TrayIndicator.ICallback>, TrayIndicator.IHandler
	{
		string title;
		Image image;
		ContextMenu menu;

		public string Title
		{
			get { return title; }
			set
			{
				title = value;
				if (Control != null)
					Control.Button.ToolTip = value ?? string.Empty;
			}
		}

		class TrayAction : NSObject
		{
			public TrayIndicatorHandler Handler { get; set; }

			[Export("activate")]
			public void Activate() => Handler?.Callback.OnActivated(Handler.Widget, EventArgs.Empty);
		}

		public bool Visible
		{
			get { return Control != null; }
			set 
			{
				if (value)
				{
					if (Control == null)
					{
						Control = NSStatusBar.SystemStatusBar.CreateStatusItem(NSStatusItemLength.Variable);
						Control.Menu = menu.ToNS();

						Control.Button.Image = image.ToNS((int)Math.Ceiling(NSStatusBar.SystemStatusBar.Thickness));
						Control.Button.Activated += Button_Activated;

					}
				}
				else if (Control != null)
				{
					NSStatusBar.SystemStatusBar.RemoveStatusItem(Control);
					Control = null;
				}
			}
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				if (Control != null)
				{
					Control.Button.Image = value.ToNS((int)Math.Ceiling(NSStatusBar.SystemStatusBar.Thickness));
				}
			}
		}

		public ContextMenu Menu
		{
			get { return menu; }
			set
			{
				menu = value;
				if (Control != null)
					Control.Menu = menu.ToNS();
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TrayIndicator.ActivatedEvent:
					// always handled
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void Button_Activated(object sender, EventArgs e) => Callback.OnActivated(Widget, EventArgs.Empty);

		protected override void Dispose(bool disposing)
		{
			if (Control != null)
			{
				NSStatusBar.SystemStatusBar.RemoveStatusItem(Control);
				Control = null;
			}
			base.Dispose(disposing);
		}
	}
}
