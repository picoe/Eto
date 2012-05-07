﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using swd = System.Windows.Data;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class CheckBoxCellHandler : CellHandler<swc.DataGridCheckBoxColumn, CheckBoxCell>, ICheckBoxCell
	{
		bool? GetValue (object dataItem)
		{
			if (Widget.Binding != null) {
				var val = Widget.Binding.GetValue (dataItem);
				if (val != null)
					return Convert.ToBoolean (val);
			}
			return null;
		}

		void SetValue (object dataItem, object value)
		{
			if (Widget.Binding != null) {
				Widget.Binding.SetValue (dataItem, value);
			}
		}

		class Column : swc.DataGridCheckBoxColumn
		{
			public CheckBoxCellHandler Handler { get; set; }

			protected override sw.FrameworkElement GenerateElement (swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateElement (cell, dataItem);
				element.DataContextChanged += (sender, e) => {
					var control = sender as swc.CheckBox;
					control.IsChecked = Handler.GetValue (control.DataContext);
				};
				return Handler.SetupCell(element);
			}

			protected override sw.FrameworkElement GenerateEditingElement (swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateEditingElement (cell, dataItem);
				element.Name = "control";
				element.DataContextChanged += (sender, e) => {
					var control = sender as swc.CheckBox;
					control.IsChecked = Handler.GetValue (control.DataContext);
				};
				return Handler.SetupCell(element);
			}

			protected override object PrepareCellForEdit (sw.FrameworkElement editingElement, sw.RoutedEventArgs editingEventArgs)
			{
				var control = editingElement as swc.CheckBox ?? editingElement.FindChild<swc.CheckBox> ("control");
				return base.PrepareCellForEdit (editingElement, editingEventArgs);
			}

			protected override bool CommitCellEdit (sw.FrameworkElement editingElement)
			{
				var control = editingElement as swc.CheckBox ?? editingElement.FindChild<swc.CheckBox> ("control");
				Handler.SetValue (control.DataContext, control.IsChecked);
				return true;
			}

		}

		public CheckBoxCellHandler ()
		{
			Control = new Column { Handler = this };
		}
	}
}