using System;
using System.Linq;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms.Forms.Controls
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

		public override Color TextColor
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

		static readonly Win32.WM[] intrinsicEvents = { Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}
	}
}
