using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms
{
	public class LinkButtonHandler : WindowsControl<swf.LinkLabel, LinkButton, LinkButton.ICallback>, LinkButton.IHandler
	{
		public LinkButtonHandler()
		{
			Control = new swf.LinkLabel
			{
				AutoSize = true
			};
		}

		public Color TextColor
		{
			get { return Control.LinkColor.ToEto(); }
			set { Control.LinkColor = value.ToSD(); }
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case LinkButton.ClickEvent:
					Control.Click += (sender, e) => Callback.OnClick(Widget, EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}


		public Color DisabledTextColor
		{
			get { return Control.DisabledLinkColor.ToEto(); }
			set { Control.DisabledLinkColor = value.ToSD(); }
		}
	}
}
