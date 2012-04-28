using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using swd = System.Windows.Data;
using sw = System.Windows;
using swm = System.Windows.Media;

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
				return Handler.SetupCell (element);
			}

			protected override sw.FrameworkElement GenerateEditingElement (swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateEditingElement (cell, dataItem) as swc.TextBox;
				element.Name = "control";
				element.DataContextChanged += (sender, e) => {
					var text = sender as swc.TextBox;
					text.Text = Handler.GetValue (text.DataContext);
				};
				return Handler.SetupCell(element);
			}

			protected override object PrepareCellForEdit (sw.FrameworkElement editingElement, sw.RoutedEventArgs editingEventArgs)
			{
				var control = editingElement as swc.TextBox ?? editingElement.FindChild<swc.TextBox> ("control");
				return base.PrepareCellForEdit (control, editingEventArgs);
			}

			protected override bool CommitCellEdit (sw.FrameworkElement editingElement)
			{
				var control = editingElement as swc.TextBox ?? editingElement.FindChild<swc.TextBox> ("control");
				Handler.SetValue (control.DataContext, control.Text);
				return true;
			}

		}

		public TextBoxCellHandler ()
		{
			Control = new Column { Handler = this };
		}
	}
}
