using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class GroupBoxHandler : WpfControl<swc.GroupBox, GroupBox>, IGroupBox
	{
		public GroupBoxHandler ()
		{
			Control = new swc.GroupBox ();
		}
		
		public Eto.Drawing.Size ClientSize
		{
			get
			{
				return this.Size;
			}
			set
			{
				// TODO
				this.Size = value;
			}
		}

		public object ContainerObject
		{
			get { return Control; }
		}

		public void SetLayout (Layout layout)
		{
			Control.Content = (System.Windows.UIElement)layout.ControlObject;
		}

		public Eto.Drawing.Size? MinimumSize
		{
			get
			{
				if (Control.MinHeight == 0 && Control.MinWidth == 0)
					return null;
				return new Eto.Drawing.Size ((int)Control.MinHeight, (int)Control.MinWidth);
			}
			set
			{
				if (value != null) {
					Control.MinHeight = value.Value.Height;
					Control.MinWidth = value.Value.Width;
				}
				else
					Control.MinWidth = Control.MinHeight = 0;
			}
		}

		public string Text
		{
			get { return Control.Header as string; }
			set { Control.Header = value; }
		}
	}
}
