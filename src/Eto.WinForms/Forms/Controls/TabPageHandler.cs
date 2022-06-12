using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms.Forms.Controls
{
	public class TabPageHandler : WindowsPanel<swf.TabPage, TabPage, TabPage.ICallback>, TabPage.IHandler
	{
		Image image;
		swf.TabControl tabcontrol;

		public TabPageHandler ()
		{
			this.Control = new swf.TabPage {
				UseVisualStyleBackColor = true
			};
		}
		
		public override Color BackgroundColor {
			get { return base.BackgroundColor; }
			set {
				BackgroundColorSet = true;
				Control.UseVisualStyleBackColor = false;
				base.BackgroundColor = value;
			}
		}
		
		public Image Image {
			get { return image; }
			set {
				if (tabcontrol != null && !string.IsNullOrEmpty (Control.ImageKey)) {
					tabcontrol.ImageList.Images.RemoveByKey (Control.ImageKey);
					Control.ImageIndex = -1;
				}
				image = value;
				if (image != null) {
					Control.ImageKey = Guid.NewGuid ().ToString ();
					SetImage ();
				}
				else {
					Control.ImageKey = null;
				}
			}
		}
		
		void SetImage ()
		{
			if (tabcontrol != null && image != null && Control.ImageIndex == -1) {
				tabcontrol.ImageList.AddImage (image, Control.ImageKey);
				// must set image index since it doesn't work with key for tabs.. ugh
				Control.ImageIndex = tabcontrol.ImageList.Images.IndexOfKey (Control.ImageKey);
			}
		}

		public override void SetParent(Container oldParent, Container newParent)
		{
			base.SetParent(oldParent, newParent);
			tabcontrol = newParent?.ControlObject as swf.TabControl;
			SetImage ();
		}
	}
}
