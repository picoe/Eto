using System.Windows;
using System.Windows.Controls;
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

			public Column()
			{
				// DataGridCheckBoxColumn defaults ElementStyle/EditingElementStyle to a hard-coded
				// Style with no BasedOn, and assigns it as the generated CheckBox's local Style. A
				// local Style suppresses the implicit CheckBox style, so cells keep the stock WPF
				// chrome even when the application's theme restyles CheckBox. Clear both and apply
				// what they set (see DataGridCheckBoxColumn.DefaultElementStyle) as plain property
				// values instead, which leaves the theme's style free to apply.
				ElementStyle = null;
				EditingElementStyle = null;
			}

			/// <summary>
			/// Sets what the cleared ElementStyle/EditingElementStyle used to, except that Eto centers
			/// the check box in the row rather than aligning it to the top.
			/// </summary>
			static void ApplyElementDefaults(swc.CheckBox element, bool isEditing)
			{
				element.HorizontalAlignment = sw.HorizontalAlignment.Center;
				element.VerticalAlignment = sw.VerticalAlignment.Center;
				if (!isEditing)
				{
					element.IsHitTestVisible = false;
					element.Focusable = false;
				}
			}

			protected override sw.FrameworkElement GenerateElement(swc.DataGridCell cell, object dataItem)
			{
				var element = (swc.CheckBox)base.GenerateElement(cell, dataItem);
				ApplyElementDefaults(element, isEditing: false);
				InitializeElement(element, cell, dataItem);
				return Handler.SetupCell(element, cell);
			}

			void InitializeElement(swc.CheckBox element, swc.DataGridCell cell, object dataItem)
			{
				if (!IsControlInitialized(element))
				{
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
						Handler.ContainerHandler.CellEdited(Handler, control);
					};
					element.Unchecked += (sender, e) =>
					{
						if (!enableEvents)
							return;
						var control = (swc.CheckBox)sender;
						Handler.SetValue(control.DataContext, control.IsChecked);
						Handler.ContainerHandler.CellEdited(Handler, control);
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
				ApplyElementDefaults(element, isEditing: true);
				InitializeElement(element, cell, dataItem);
				return Handler.SetupCell(element, cell);
			}
			protected override bool CommitCellEdit(sw.FrameworkElement editingElement)
			{
				// does not get called when we set the checkbox value in OnMouseUp manually
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