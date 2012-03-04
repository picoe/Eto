using System;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class TextBoxCellHandler : CellHandler<swf.DataGridViewTextBoxCell, TextBoxCell>, ITextBoxCell
	{
		public TextBoxCellHandler ()
		{
			Control = new swf.DataGridViewTextBoxCell();
		}

		public override void SetCellValue (object dataItem, object value)
		{
			if (Widget.Binding != null) {
				Widget.Binding.SetValue (dataItem, value);
			}
		}

		public override object GetCellValue (object dataItem)
		{
			if (Widget.Binding != null) {
				return Widget.Binding.GetValue (dataItem);
			}
			return null;
		}

	}
}

