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
	public class TextBoxCellHandler : CellHandler<swc.DataGridTextColumn, TextBoxCell>, ITextBoxCell
	{
		string GetValue (object dataItem)
		{
			if (Widget.Binding != null) {
				var val = Widget.Binding.GetValue (dataItem);
				if (val != null)
					return Convert.ToString (val);
			}
			return null;
		}

		void SetValue (object dataItem, object value)
		{
			if (Widget.Binding != null) {
				Widget.Binding.SetValue (dataItem, value);
			}
		}

		class Column : swc.DataGridTextColumn
		{
			public TextBoxCellHandler Handler { get; set; }

			protected override sw.FrameworkElement GenerateElement (swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateElement (cell, dataItem);
				element.DataContextChanged += (sender, e) => {
					var text = sender as swc.TextBlock;
					text.Text = Handler.GetValue (text.DataContext);
				};
				return element;
			}

			protected override sw.FrameworkElement GenerateEditingElement (swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateEditingElement (cell, dataItem) as swc.TextBox;
				element.DataContextChanged += (sender, e) => {
					var text = sender as swc.TextBox;
					text.Text = Handler.GetValue (text.DataContext);
				};
				return element;
			}

			protected override bool CommitCellEdit (sw.FrameworkElement editingElement)
			{
				var text = editingElement as swc.TextBox;
				Handler.SetValue (text.DataContext, text.Text);
				return true;
			}

		}

		public TextBoxCellHandler ()
		{
			Control = new Column { Handler = this };
		}
	}
}
