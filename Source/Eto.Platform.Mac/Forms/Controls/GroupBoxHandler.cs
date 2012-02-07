using System;
using Eto.Forms;
using MonoMac.AppKit;
using SD = System.Drawing;
using Eto.Drawing;

namespace Eto.Platform.Mac
{
	public class GroupBoxHandler : MacContainer<NSBox, GroupBox>, IGroupBox
	{
		public GroupBoxHandler ()
		{
			Control = new NSBox ();
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

		public virtual string Text {
			get { return Control.Title; }
			set { Control.Title = value; }
		}
		
		public override Eto.Drawing.Size GetPreferredSize ()
		{
			return base.GetPreferredSize () + new Size (14, 22);
		}
		
		public override void SetContentSize (SD.SizeF contentSize)
		{
			Control.SetFrameFromContentFrame (new System.Drawing.RectangleF (0, 0, contentSize.Width, contentSize.Height));
		}
	}
}
