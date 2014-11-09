using System;
using Eto.Forms;
using swc = System.Windows.Controls;
using swd = System.Windows.Data;
using sw = System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Eto.Wpf.Forms.Cells
{
	public class ComboBoxCellHandler : CellHandler<swc.DataGridComboBoxColumn, ComboBoxCell, ComboBoxCell.ICallback>, ComboBoxCell.IHandler
	{
		CollectionHandler collection;

		string GetValue(object dataItem)
		{
			if (Widget.Binding != null)
			{
				var val = Widget.Binding.GetValue(dataItem);
				if (val != null)
					return Convert.ToString(val);
			}
			return null;
		}

		void SetValue(object dataItem, object value)
		{
			if (Widget.Binding != null)
			{
				Widget.Binding.SetValue(dataItem, Convert.ToString(value));
			}
		}

		class Column : swc.DataGridComboBoxColumn
		{
			public ComboBoxCellHandler Handler { get; set; }

			protected override sw.FrameworkElement GenerateElement(swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateElement(cell, dataItem);
				element.DataContextChanged += (sender, e) =>
				{
					var control = sender as swc.ComboBox;
					control.SelectedValue = Handler.GetValue(control.DataContext);
					Handler.FormatCell(control, cell, dataItem);
				};
				Handler.FormatCell(element, cell, dataItem);
				return Handler.SetupCell(element);
			}

			protected override sw.FrameworkElement GenerateEditingElement(swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateEditingElement(cell, dataItem);
				element.Name = "control";
				element.DataContextChanged += (sender, e) =>
				{
					var control = sender as swc.ComboBox;
					control.SelectedValue = Handler.GetValue(control.DataContext);
					Handler.FormatCell(control, cell, dataItem);
				};
				Handler.FormatCell(element, cell, dataItem);
				return Handler.SetupCell(element);
			}

			protected override bool CommitCellEdit(sw.FrameworkElement editingElement)
			{
				var control = editingElement as swc.ComboBox ?? editingElement.FindChild<swc.ComboBox>("control");
				Handler.SetValue(control.DataContext, control.SelectedValue);
				return true;
			}
		}

		public ComboBoxCellHandler()
		{
			Control = new Column
			{
				Handler = this,
				SelectedValuePath = "Key",
				DisplayMemberPath = "Text",
				//SelectedValueBinding = new swd.Binding { Converter = new WpfTextBindingBlock(() => Widget.ComboKeyBinding), Mode = swd.BindingMode.OneWay }
				//TextBinding = new swd.Binding { Converter = new WpfTextBindingBlock(() => Widget.ComboTextBinding), Mode = swd.BindingMode.OneWay }
			};
		}

		struct Item
		{
			ComboBoxCellHandler handler;
			object value;
			public string Text { get { return handler.Widget.ComboTextBinding.GetValue(value); } }
			public string Key { get { return handler.Widget.ComboKeyBinding.GetValue(value); } }
			public Item(ComboBoxCellHandler handler, object value)
			{
				this.handler = handler;
				this.value = value;
			}
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ComboBoxCellHandler Handler { get; set; }

			public ObservableCollection<Item> Items { get; set; }

			public CollectionHandler()
			{
				Items = new ObservableCollection<Item>();
			}

			public override void AddItem(object item)
			{
				Items.Add(new Item(Handler, item));	
			}

			public override void InsertItem(int index, object item)
			{
				Items.Insert(index, new Item(Handler, item));
			}

			public override void RemoveItem(int index)
			{
				Items.RemoveAt(index);
			}

			public override void RemoveAllItems()
			{
				Items.Clear();
			}
		}

		public IEnumerable<object> DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
				Control.ItemsSource = collection.Items;
			}
		}
	}
}