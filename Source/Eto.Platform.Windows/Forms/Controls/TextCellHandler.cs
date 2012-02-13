using System;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class TextCellHandler : CellHandler<swf.DataGridViewTextBoxCell, TextCell>, ITextCell
	{
		public TextCellHandler ()
		{
			Control = new swf.DataGridViewTextBoxCell();
		}
	}
}

