using System;
using Eto.Forms;
using MonoMac.AppKit;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Platform.Mac.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class GroupBoxHandler : MacContainer<NSBox, GroupBox>, IGroupBox
	{
		Font font;

		public class EtoBox : NSBox, IMacControl
		{
			public object Handler { get; set; }
		}
		
		public GroupBoxHandler ()
		{
			Control = new EtoBox { Handler = this };
			Control.Title = string.Empty;
			Control.ContentView = new NSView ();
			Enabled = true;
		}

		public override object ContainerObject {
			get {
				return Control.ContentView;
			}
		}
		
		public override bool Enabled { get; set; }
		
		public override Eto.Drawing.Size ClientSize {
			get {
				var view = Control.ContentView as NSView;
				return Generator.ConvertF (view.Frame.Size);
			}
			set {
				Control.SetFrameFromContentFrame (new System.Drawing.RectangleF (0, 0, value.Width, value.Height));
			}
		}

		public Font Font
		{
			get {
				if (font == null)
					font = new Font (Widget.Generator, new FontHandler (Control.TitleFont));
				return font;
			}
			set {
				font = value;
				if (font != null)
					Control.TitleFont = ((FontHandler)font.Handler).Control;
				else
					Control.TitleFont = null;
				LayoutIfNeeded ();
			}
		}

		public virtual string Text {
			get { return Control.Title; }
			set { Control.Title = value; }
		}
		
		public override Eto.Drawing.Size GetPreferredSize ()
		{
			return base.GetPreferredSize () + new Size (14, (int)(Control.TitleFont.LineHeight () * 1.4));
		}
		
		public override void SetContentSize (SD.SizeF contentSize)
		{
			Control.SetFrameFromContentFrame (new System.Drawing.RectangleF (0, 0, contentSize.Width, contentSize.Height));
		}
	}
}
