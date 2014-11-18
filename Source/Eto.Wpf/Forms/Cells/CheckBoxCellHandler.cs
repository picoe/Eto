using System;
using Eto.Forms;
using swc = System.Windows.Controls;
using swd = System.Windows.Data;
using sw = System.Windows;

namespace Eto.Wpf.Forms.Cells
{
	public class CheckBoxCellHandler : CellHandler<swc.DataGridCheckBoxColumn, CheckBoxCell, CheckBoxCell.ICallback>, CheckBoxCell.IHandler
	{
		bool? GetValue(object dataItem)
		{
			if (Widget.Binding != null)
			{
				return Widget.Binding.GetValue(dataItem);
			}
			return null;
		}

		void SetValue(object dataItem, bool? value)
		{
			if (Widget.Binding != null)
			{
				Widget.Binding.SetValue(dataItem, value);
			}
		}

		class Column : swc.DataGridCheckBoxColumn
		{
			public CheckBoxCellHandler Handler { get; set; }

			protected override sw.FrameworkElement GenerateElement(swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateElement(cell, dataItem);
				element.DataContextChanged += (sender, e) =>
				{
					var control = sender as swc.CheckBox;
					control.IsChecked = Handler.GetValue(control.DataContext);
					Handler.FormatCell(control, cell, dataItem);
				};
				return Handler.SetupCell(element);
			}

			protected override sw.FrameworkElement GenerateEditingElement(swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateEditingElement(cell, dataItem);
				element.Name = "control";
				element.DataContextChanged += (sender, e) =>
				{
					var control = sender as swc.CheckBox;
					control.IsChecked = Handler.GetValue(control.DataContext);
					Handler.FormatCell(control, cell, dataItem);
				};
				return Handler.SetupCell(element);
			}

			protected override bool CommitCellEdit(sw.FrameworkElement editingElement)
			{
				var control = editingElement as swc.CheckBox ?? editingElement.FindChild<swc.CheckBox>("control");
				Handler.SetValue(control.DataContext, control.IsChecked);
				return true;
			}

		}

		public CheckBoxCellHandler()
		{
			Control = new Column { Handler = this };
		}
	}
}