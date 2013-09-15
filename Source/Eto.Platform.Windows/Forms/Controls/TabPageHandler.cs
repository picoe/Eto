using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class TabPageHandler : WindowsDockContainer<swf.TabPage, TabPage>, ITabPage
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
				Control.UseVisualStyleBackColor = false;
				base.BackgroundColor = value;
			}
		}
		
		public Eto.Drawing.Image Image {
			get { return image; }
			set {
				if (tabcontrol != null && !string.IsNullOrEmpty (this.Control.ImageKey)) {
					this.tabcontrol.ImageList.Images.RemoveByKey (this.Control.ImageKey);
					this.Control.ImageIndex = -1;
				}
				image = value;
				if (image != null) {
					this.Control.ImageKey = Guid.NewGuid ().ToString ();
					SetImage ();
				}
				else {
					this.Control.ImageKey = null;
				}
			}
		}
		
		void SetImage ()
		{
			if (tabcontrol != null && image != null && this.Control.ImageIndex == -1) {
				this.tabcontrol.ImageList.AddImage (image, this.Control.ImageKey);
				// must set image index since it doesn't work with key for tabs.. ugh
				this.Control.ImageIndex = this.tabcontrol.ImageList.Images.IndexOfKey (this.Control.ImageKey);
			}
		}

		public override void SetParent(Container parent)
		{
			base.SetParent(parent);
			tabcontrol = parent != null ? parent.ControlObject as swf.TabControl : null;
			SetImage ();
		}
	}
}
