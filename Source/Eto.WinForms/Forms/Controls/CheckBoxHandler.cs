using System;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;

namespace Eto.WinForms
{
	public class CheckBoxHandler : WindowsControl<SWF.CheckBox, CheckBox, CheckBox.ICallback>, ICheckBox
	{
		public CheckBoxHandler()
		{
			Control = new SWF.CheckBox();
			Control.AutoSize = true;
			Control.CheckStateChanged += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
		}

		public bool? Checked
		{
			get
			{
				switch (Control.CheckState)
				{
					case SWF.CheckState.Checked:
						return true;
					case SWF.CheckState.Unchecked:
						return false;
					default:
						return null;
				}
			}
			set
			{
				if (value == null)
					Control.CheckState = SWF.CheckState.Indeterminate;
				else if (value.Value)
					Control.CheckState = SWF.CheckState.Checked;
				else
					Control.CheckState = SWF.CheckState.Unchecked;
			}
		}

		public bool ThreeState
		{
			get { return Control.ThreeState; }
			set { Control.ThreeState = value; }
		}
	}
}
