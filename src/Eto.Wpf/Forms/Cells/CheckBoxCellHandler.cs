using System;
using Eto.Forms;
using swc = System.Windows.Controls;
using swd = System.Windows.Data;
using sw = System.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

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
			bool enableEvents;

			protected override sw.FrameworkElement GenerateElement(swc.DataGridCell cell, object dataItem)
			{
				var element = (swc.CheckBox)base.GenerateElement(cell, dataItem);
				InitializeElement(element, cell, dataItem);
				return Handler.SetupCell(element);
			}

			void InitializeElement(swc.CheckBox element, swc.DataGridCell cell, object dataItem)
			{
				if (!IsControlInitialized(element))
				{
					element.VerticalAlignment = sw.VerticalAlignment.Center;
					element.DataContextChanged += (sender, e) =>
					{
						var control = sender as swc.CheckBox;
						enableEvents = false;
						control.IsChecked = Handler.GetValue(control.DataContext);
						enableEvents = true;
						Handler.FormatCell(control, cell, control.DataContext);
					};
					element.Checked += (sender, e) =>
					{
						if (!enableEvents)
							return;
						var control = (swc.CheckBox)sender;
						Handler.SetValue(control.DataContext, control.IsChecked);
					};
					element.Unchecked += (sender, e) =>
					{
						if (!enableEvents)
							return;
						var control = (swc.CheckBox)sender;
						Handler.SetValue(control.DataContext, control.IsChecked);
					};
					SetControlInitialized(element, true);
				}
				else
				{
					element.IsChecked = Handler.GetValue(dataItem);
					Handler.FormatCell(element, cell, dataItem);
				}
			}

			protected override sw.FrameworkElement GenerateEditingElement(swc.DataGridCell cell, object dataItem)
			{
				var element = (swc.CheckBox)base.GenerateEditingElement(cell, dataItem);
				InitializeElement(element, cell, dataItem);
				return Handler.SetupCell(element);
			}
			protected override bool CommitCellEdit(sw.FrameworkElement editingElement)
			{
				Handler.ContainerHandler.CellEdited(Handler, editingElement);
				return base.CommitCellEdit(editingElement);
			}
		}

		public CheckBoxCellHandler()
		{
			Control = new Column { Handler = this };
		}

		public override void OnMouseUp(GridCellMouseEventArgs args, DependencyObject hitTestResult, DataGridCell cell)
		{
			// check/uncheck right away, otherwise it takes three clicks to change the value.. 
			if (!cell.IsReadOnly)
			{
				var checkBox = hitTestResult.GetVisualParents().TakeWhile(r => !(r is swc.DataGridCell)).OfType<swc.CheckBox>().FirstOrDefault();
				if (checkBox != null)
				{
					var value = checkBox.IsChecked ?? false;
					checkBox.IsChecked = !value;

					args.Handled = true;
				}
			}
			base.OnMouseUp(args, hitTestResult, cell);
		}
	}
}