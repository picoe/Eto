using System;
using Eto.Forms;
using Eto.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#elif OSX
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
					Control.ToolTip = value ?? string.Empty;
			}
		}

		class TrayAction : NSObject
		{
			public TrayIndicatorHandler Handler { get; set; }

			[Export("activate")]
			public void Activate() => Handler?.Callback.OnActivated(Handler.Widget, EventArgs.Empty);
		}
		static Selector s_ButtonSelector = new Selector("button");

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

						if (Control.RespondsToSelector(s_ButtonSelector))
						{
							Control.Button.Image = image.ToNS((int)Math.Ceiling(NSStatusBar.SystemStatusBar.Thickness));
							Control.Button.Activated += Button_Activated;
						}
						else
						{
							Control.Image = image.ToNS((int)Math.Ceiling(NSStatusBar.SystemStatusBar.Thickness));
							Control.Action = new Selector("activate");
							Control.Target = new TrayAction { Handler = this };
						}
						Control.ToolTip = title ?? string.Empty; // deprecated in 10.10.  Move to Button when we remove support for < 10.10

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
					if (Control.RespondsToSelector(s_ButtonSelector))
						Control.Button.Image = value.ToNS((int)Math.Ceiling(NSStatusBar.SystemStatusBar.Thickness));
					else 
						Control.Image = value.ToNS((int)Math.Ceiling(NSStatusBar.SystemStatusBar.Thickness));
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
