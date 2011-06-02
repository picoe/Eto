using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class GroupBoxHandler : MacContainer<NSBox, GroupBox>, IGroupBox
	{
		public GroupBoxHandler()
		{
			Control = new NSBox();
		}

		public override object ContainerObject
		{
			get
			{
				return Control.ContentView;
			}
		}
		
		public override Eto.Drawing.Size ClientSize {
			get {
				var view = Control.ContentView as NSView;
				return Generator.ConvertF(view.Frame.Size);
			}
			set {
				Control.SetFrameFromContentFrame(new System.Drawing.RectangleF(0, 0, value.Width, value.Height));
			}
		}

		public virtual string Text
		{
			get { return Control.Title; }
			set { Control.Title = value; }
		}
	}
}
