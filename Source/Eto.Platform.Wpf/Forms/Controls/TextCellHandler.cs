using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using swd = System.Windows.Data;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TextCellHandler : CellHandler<swc.DataGridTextColumn, TextCell>, ITextCell
	{
		public TextCellHandler ()
		{
			Control = new swc.DataGridTextColumn ();
		}

		public override void Bind (int column)
		{
			Control.Binding = new swd.Binding {
				Mode = swd.BindingMode.TwoWay,
				Path = new sw.PropertyPath (string.Format (".[{0}]", column))
			};
		}

	}
}
