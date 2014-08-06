using System;
using Eto.Forms;
using MonoMac.AppKit;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Mac.Drawing;
using MonoMac.Foundation;
#if Mac64
using CGFloat = System.Double;
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
#else
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using NSPoint = System.Drawing.PointF;
using CGFloat = System.Single;
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class GroupBoxHandler : MacPanel<NSBox, GroupBox, GroupBox.ICallback>, GroupBox.IHandler
	{
		Font font;

		public class EtoBox : NSBox, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}
		
		public GroupBoxHandler ()
		{
			Control = new EtoBox { Handler = this };
			Control.Title = string.Empty;
			Control.ContentView = new NSView ();
			Enabled = true;
		}

		public override NSView ContainerControl
		{
			get { return Control; }
		}

		public override NSView ContentControl
		{
			get { return (NSView)Control.ContentView; }
		}
		
		public override bool Enabled { get; set; }
		
		public override Size ClientSize {
			get {
				var view = Control.ContentView as NSView;
				return view.Frame.Size.ToEtoSize ();
			}
			set {
				Control.SetFrameFromContentFrame (new NSRect(0, 0, value.Width, value.Height));
			}
		}

		public Font Font
		{
			get {
				return font ?? (font = new Font(new FontHandler(Control.TitleFont)));
			}
			set {
				font = value;
				Control.TitleFont = font == null ? null : ((FontHandler)font.Handler).Control;
				LayoutIfNeeded ();
			}
		}

		public virtual string Text {
			get { return Control.Title; }
			set { Control.Title = value; }
		}
		
		public override SizeF GetPreferredSize (SizeF availableSize)
		{
			return base.GetPreferredSize (availableSize) + new SizeF (14, Control.TitleFont.LineHeight () + 9);
		}
		
		public override void SetContentSize (NSSize contentSize)
		{
			Control.SetFrameFromContentFrame (new NSRect(0, 0, contentSize.Width, contentSize.Height));
		}
	}
}
