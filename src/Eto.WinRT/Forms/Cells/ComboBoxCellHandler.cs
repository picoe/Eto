#if TODO_XAML
using System;
using Eto.Forms;
using swc = Windows.UI.Xaml.Controls;
using swd = Windows.UI.Xaml.Data;
using sw = Windows.UI.Xaml;
using System.Collections;

namespace Eto.WinRT.Forms.Controls
{
	public class ComboBoxCellHandler : CellHandler<swc.DataGridComboBoxColumn, ComboBoxCell>, IComboBoxCell
	{
		IListStore store;

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
				Widget.Binding.SetValue (dataItem, Convert.ToString (value));
			}
		}

		class Column : swc.DataGridComboBoxColumn
		{
			public ComboBoxCellHandler Handler { get; set; }


			protected override sw.FrameworkElement GenerateElement (swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateElement (cell, dataItem);
				element.DataContextChanged += (sender, e) => {
					var control = sender as swc.ComboBox;
					control.SelectedValue = Handler.GetValue (control.DataContext);
					Handler.FormatCell (control, cell, dataItem);
				};
				Handler.FormatCell (element, cell, dataItem);
				return Handler.SetupCell (element);
			}

			protected override sw.FrameworkElement GenerateEditingElement (swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateEditingElement (cell, dataItem);
				element.Name = "control";
				element.DataContextChanged += (sender, e) => {
					var control = sender as swc.ComboBox;
					control.SelectedValue = Handler.GetValue (control.DataContext);
					Handler.FormatCell (control, cell, dataItem);
				};
				Handler.FormatCell (element, cell, dataItem);
				return Handler.SetupCell (element);
			}

			protected override bool CommitCellEdit (sw.FrameworkElement editingElement)
			{
				var control = editingElement as swc.ComboBox ?? editingElement.FindChild<swc.ComboBox> ("control");
				Handler.SetValue (control.DataContext, control.SelectedValue);
				return true;
			}

		}

		public ComboBoxCellHandler ()
		{
			Control = new Column { 
				Handler = this,
				DisplayMemberPath = "Text",
				SelectedValuePath = "Key"
			};
		}

		public IListStore DataStore
		{
			get { return store; }
			set
			{
				store = value;
				Control.ItemsSource = store as IEnumerable ?? store.AsEnumerable();
			}
		}
	}
}
#endif