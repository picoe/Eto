using System;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class CheckBoxCellHandler : CellHandler<swf.DataGridViewCheckBoxCell, CheckBoxCell>, ICheckBoxCell
	{
		public CheckBoxCellHandler ()
		{
			Control = new swf.DataGridViewCheckBoxCell ();
		}

		public override object GetCellValue (object itemValue)
		{
			if (itemValue == null) return false;
			return base.GetCellValue (itemValue);
		}
	}
}

