using System;
using System.Reflection;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class CheckBoxHandler : WindowsControl<SWF.CheckBox, CheckBox>, ICheckBox
	{

		public CheckBoxHandler()
		{
			Control = new SWF.CheckBox();
			Control.CheckedChanged += delegate {
				Widget.OnCheckedChanged(EventArgs.Empty);
			};
		}

		#region ICheckBox Members

		public bool Checked
		{
			get { return Control.Checked; }
			set { Control.Checked = value; }
		}

		#endregion
	}
}
