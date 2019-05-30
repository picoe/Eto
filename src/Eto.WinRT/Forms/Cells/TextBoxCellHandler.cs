#if TODO_XAML
using System;
using Eto.Forms;
using swc = Windows.UI.Xaml.Controls;
using swd = Windows.UI.Xaml.Data;
using sw = Windows.UI.Xaml;
using swm = Windows.UI.Xaml.Media;

namespace Eto.WinRT.Forms.Controls
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
					var control = sender as swc.TextBlock;
					control.Text = Handler.GetValue (control.DataContext);
					Handler.FormatCell (control, cell, dataItem);
				};
				return Handler.SetupCell (element);
			}

			protected override sw.FrameworkElement GenerateEditingElement (swc.DataGridCell cell, object dataItem)
			{
				var element = (swc.TextBox)base.GenerateEditingElement (cell, dataItem);
				element.Name = "control";
				element.DataContextChanged += (sender, e) => {
					var control = sender as swc.TextBox;
					control.Text = Handler.GetValue (control.DataContext);
					Handler.FormatCell (control, cell, dataItem);
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
#endif