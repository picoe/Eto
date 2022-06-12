using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Eto.Drawing;

namespace Eto.Forms.ThemedControls
{
	/// <summary>
	/// Handler for the <see cref="CollectionEditor"/> control.
	/// </summary>
	public class ThemedCollectionEditorHandler : ThemedControlHandler<ThemedCollectionEditor, CollectionEditor, CollectionEditor.ICallback>, CollectionEditor.IHandler
	{
		/// <summary>
		/// Creates the control for this handler.
		/// </summary>
		/// <returns>A new instance of the control for the handler</returns>
		protected override ThemedCollectionEditor CreateControl() => new ThemedCollectionEditor();

		/// <summary>
		/// Data store of the items to edit
		/// </summary>
		public IEnumerable<object> DataStore
		{
			get => Control.DataStore;
			set => Control.DataStore = value;
		}

		/// <summary>
		/// Gets or sets the type of element to create when adding new elements to the data store
		/// </summary>
		public Type ElementType
		{
			get => Control.ElementType;
			set => Control.ElementType = value;
		}
	}

	interface IValueTypeWrapper
	{
		object Key { get; }
	}

	interface IValueTypeWrapperHost
	{
		object GetValue(object key);
		void SetValue(object key, object value);
	}

	static class ValueTypeWrapper
	{
		class Wrapper<T> : IValueTypeWrapper
		{
			object _key;
			IValueTypeWrapperHost _host;
			public Wrapper(IValueTypeWrapperHost host, object key)
			{
				_host = host;
				_key = key;
			}

			object IValueTypeWrapper.Key => _key;

			public T Value
			{
				get
				{
					var val = _host.GetValue(_key);
					if (val == null)
						return default(T);
					return (T)val;
				}
				set => _host.SetValue(_key, value);
			}
		}

		public static bool NeedsWrap(Type type)
		{
			if (type == null)
				return false;
			if (type == typeof(string))
				return true;
			var typeInfo = type.GetTypeInfo();
			if (typeInfo.IsValueType)
				return true;
			if (typeInfo.IsArray)
				return true;
			return false;
		}


		public static IValueTypeWrapper Create(IValueTypeWrapperHost host, Type type, object key)
		{
			var wrapperType = typeof(Wrapper<>).MakeGenericType(type);
			return Activator.CreateInstance(wrapperType, host, key) as IValueTypeWrapper;
		}

		public static object Wrap(IValueTypeWrapperHost host, object key, object value)
		{
			if (value == null)
				return null;
			var type = value.GetType();
			if (!NeedsWrap(type))
				return value;
			return Create(host, type, key);
		}
	}

	/// <summary>
	/// Implementation of the CollectionEditor using a GridView and PropertyGrid.
	/// </summary>
	public class ThemedCollectionEditor : Panel, IValueTypeWrapperHost
	{
		GridView _list;
		object _selectedObject;
		InternalCollection<object> _dataStore;
		PropertyGrid _propertyGrid;
		Panel _extraContent;

		class InternalCollection<T> : ObservableCollection<T>
		{
			public bool EnableNotifications { get; set; } = true;

			public InternalCollection(IEnumerable<T> collection) : base(collection)
			{
			}

			protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
			{
				if (EnableNotifications)
					base.OnCollectionChanged(e);
			}

		}

		/// <summary>
		/// Data store of the items to edit
		/// </summary>
		public IEnumerable<object> DataStore
		{
			get => _dataStore;
			set
			{
				_dataStore = new InternalCollection<object>(value ?? Enumerable.Empty<object>());
				_list.DataStore = _dataStore;
				if (_dataStore.Count > 0)
					_list.SelectedRow = 0;
			}
		}

		object Wrap(object value, int index)
		{
			if (ValueTypeWrapper.NeedsWrap(ElementType))
			{
				return ValueTypeWrapper.Create(this, ElementType, index);
			}
			return value;
		}

		object SelectedObject
		{
			get => _selectedObject;
			set
			{
				_selectedObject = Wrap(value, _list.SelectedRow);
				_propertyGrid.SelectedObject = _selectedObject;
			}
		}

		/// <summary>
		/// Gets or sets the type of element to create when adding new elements to the data store
		/// </summary>
		public Type ElementType { get; set; }

		/// <summary>
		/// Gets or sets a control for extra content in this control.  This appears below the property grid in the same line as the add/remove buttons.
		/// </summary>
		public Control ExtraContent
		{
			get => _extraContent.Content;
			set => _extraContent.Content = value;
		}

		/// <summary>
		/// Initializes a new instance of the ThemedCollectionEditor
		/// </summary>
		public ThemedCollectionEditor()
		{
			_list = new GridView
			{
				Width = 200,
				ShowHeader = false,
				AllowDrop = true,
				Columns =
				{
					new GridColumn
					{
						AutoSize = true,
						DataCell = new TextBoxCell { Binding = Binding.Delegate((object o) => Convert.ToString(o)) }
					}
				}
			};
			_list.MouseMove += list_MouseMove;
			_list.DragOver += list_DragOver;
			_list.DragDrop += list_DragDrop;


			var addButton = new ButtonSegmentedItem { Text = Application.Instance.Localize(this, "+") };
			addButton.Click += AddButton_Click;

			var removeButton = new ButtonSegmentedItem { Text = Application.Instance.Localize(this, "-") };
			removeButton.Click += RemoveButton_Click;

			var segments = new SegmentedButton { Items = { addButton, removeButton } };


			var listButtons = new Panel();
			listButtons.Padding = new Padding(0, 4, 0, 0);
			listButtons.Content = TableLayout.AutoSized(segments);

			_propertyGrid = new PropertyGrid { ShowDescription = false };

			_list.SelectedItemBinding.Bind(this, t => t.SelectedObject);

			_extraContent = new Panel();

			Content = new TableLayout(
				TableRow.Scaled(new Splitter
				{
					Orientation = Orientation.Horizontal,
					FixedPanel = SplitterFixedPanel.None,
					Panel1MinimumSize = 100,
					Panel2MinimumSize = 100,
					RelativePosition = 0.4,
					Panel1 = new TableLayout
					{
						Rows =
						{
							Application.Instance.Localize(this, "Members"),
							new TableRow(_list) { ScaleHeight = true }
						}
					},
					Panel2 = new TableLayout
					{
						Rows =
						{
							Application.Instance.Localize(this, "Properties"),
							new TableRow(_propertyGrid) { ScaleHeight = true },
						}
					}
				}),
				new TableLayout(new TableRow(listButtons, null, _extraContent))
			);
		}

		static string s_DragDataName = typeof(ThemedCollectionEditor).FullName;

		private void list_DragOver(object sender, DragEventArgs e)
		{
			var info = _list.GetDragInfo(e);
			if (info == null || !e.Data.Contains(s_DragDataName))
				return;

			info.RestrictToInsert();
			if (e.Data.Contains(s_DragDataName))
				e.Effects = DragEffects.Move;
		}

		private void list_DragDrop(object sender, DragEventArgs e)
		{
			var info = _list.GetDragInfo(e);
			if (info == null)
				return;

			var index = info.InsertIndex;
			if (index >= 0)
			{
				//_dataStore.EnableNotifications = false;
				var sourceRows = e.Data.GetObject(s_DragDataName) as int[];
				var data = new List<object>();
				foreach (var row in sourceRows.OrderByDescending(r => r))
				{
					var item = _dataStore[row];
					data.Add(item);
					_dataStore.RemoveAt(row);
					if (row < index)
						index--;
				}
				var selectIndex = index;
				foreach (var item in data)
				{
					_dataStore.Insert(index++, item);
				}
				_list.SelectedRow = selectIndex;
				//_dataStore.EnableNotifications = true;
				//_dataStore.Reset();
			}

		}

		private void list_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Buttons == MouseButtons.Primary)
			{
				var cell = _list.GetCellAt(e.Location);
				if (cell.Item == null)
					return;

				var data = new DataObject();
				data.SetObject(_list.SelectedRows.ToArray(), s_DragDataName);
				data.Text = _list.SelectedItem?.ToString() ?? "";
				_list.DoDragDrop(data, DragEffects.Move);
				e.Handled = true;
			}
		}

		private void RemoveButton_Click(object sender, EventArgs e)
		{
			var rows = _list.SelectedRows.ToList();
			for (int i = rows.Count - 1; i >= 0; i--)
			{
				_dataStore.RemoveAt(rows[i]);
			}
			if (rows.Count > 0)
			{
				_list.SelectRow(Math.Min(_dataStore.Count - 1, rows[0]));
			}
		}

		object CreateNewInstance()
		{
			var type = ElementType;
			var typeInfo = type.GetTypeInfo();
			if (typeInfo.IsValueType || type.GetConstructor(new Type[0]) != null)
			{
				return Activator.CreateInstance(type);
			}
			// special case string so we can add them to the list
			if (type == typeof(string))
				return string.Empty;
			return null;
		}

		private void AddButton_Click(object sender, EventArgs e)
		{
			var item = CreateNewInstance();
			if (item == null)
			{
				var msg = Application.Instance.Localize(this, "Could not create an instance of {0}. No default constructor found.");
				MessageBox.Show(this, string.Format(msg, ElementType.Name), MessageBoxType.Warning);
				return;
			}

			_dataStore.Add(item);
			_list.SelectRow(_dataStore.Count - 1);
		}

		object IValueTypeWrapperHost.GetValue(object key)
		{
			var index = (int)key;
			return index >= 0 ? _dataStore[index] : null;
		}

		void IValueTypeWrapperHost.SetValue(object key, object value)
		{
			var index = (int)key;
			_dataStore.EnableNotifications = false;
			_dataStore[index] = value;
			_list.ReloadData(index);
			_dataStore.EnableNotifications = true;
		}
	}
}
