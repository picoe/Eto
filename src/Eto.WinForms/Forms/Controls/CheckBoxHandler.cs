using System;
using System.Linq;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class CheckBoxHandler : WindowsControl<swf.CheckBox, CheckBox, CheckBox.ICallback>, CheckBox.IHandler
	{
		public CheckBoxHandler()
		{
			Control = new swf.CheckBox();
			Control.AutoSize = true;
			Control.CheckStateChanged += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
		}

		public bool? Checked
		{
			get
			{
				switch (Control.CheckState)
				{
					case swf.CheckState.Checked:
						return true;
					case swf.CheckState.Unchecked:
						return false;
					default:
						return null;
				}
			}
			set
			{
				if (value == null)
					Control.CheckState = swf.CheckState.Indeterminate;
				else if (value.Value)
					Control.CheckState = swf.CheckState.Checked;
				else
					Control.CheckState = swf.CheckState.Unchecked;
			}
		}

		public bool ThreeState
		{
			get { return Control.ThreeState; }
			set { Control.ThreeState = value; }
		}

		static readonly Win32.WM[] intrinsicEvents = { Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}
	}
}
