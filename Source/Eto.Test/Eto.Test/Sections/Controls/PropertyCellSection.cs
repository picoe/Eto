using System;
using Eto.Forms;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Eto.Drawing;
using System.Diagnostics;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", "PropertyCell")]
	public class PropertyCellSection : Panel
	{
		public class MyModel : INotifyPropertyChanged
		{
			public string Name { get; set; }

			public Type Type { get; set; }

			object val;

			public object Value
			{
				get { return val; }
				set
				{
					if (value != val)
					{
						val = value;
						Log.Write(this, "Changed value of {0} to {1}", Name, val);
						OnPropertyChanged("Value");
					}
				}
			}

			protected virtual void OnPropertyChanged(string propertyName = null)
			{
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}

			#region INotifyPropertyChanged implementation

			public event PropertyChangedEventHandler PropertyChanged;

			#endregion
		}

		public PropertyCellSection()
		{
			var infoColumn = new GridColumn
			{
				HeaderText = "Name",
				Editable = false,
				DataCell = new TextBoxCell { Binding = Binding.Property((MyModel m) => m.Name) }
			};
			var valueProperty = Binding.Property((MyModel m) => m.Value);

			var propertyCell = new PropertyCell
			{ 
				TypeBinding = Binding.Property((MyModel m) => (object)m.Type),
				Types =
				{
					new PropertyCellTypeBoolean { ItemBinding = valueProperty.OfType<bool?>() },
					new PropertyCellTypeString { ItemBinding = valueProperty.OfType<string>() },
					new PropertyCellTypeColor { ItemBinding = valueProperty.OfType<Color>() },
					new PropertyCellTypeDateTime { ItemBinding = valueProperty.OfType<DateTime?>() },
					new PropertyCellTypeEnum<Orientation> { ItemBinding = valueProperty.OfType<Orientation>() }
				}
			};

			var filtered = new FilterCollection<MyModel>(new []
			{
				new MyModel { Name = "Bool Property (checked)", Type = typeof(bool?), Value = true },
				new MyModel { Name = "Bool Property", Type = typeof(bool), Value = false },
				new MyModel { Name = "String Property", Type = typeof(string), Value = "hello" },
				new MyModel { Name = "Color Property", Type = typeof(Color), Value = Colors.Blue },
				new MyModel { Name = "DateTime Property", Type = typeof(DateTime), Value = DateTime.Today },
				new MyModel { Name = "Enum Property", Type = typeof(Orientation), Value = Orientation.Vertical }
			});

			var valueColumn = new GridColumn
			{
				AutoSize = true,
				Editable = true,
				HeaderText = "Value",
				DataCell = propertyCell,
			};

			var grid = new GridView
			{
				Columns = { infoColumn, valueColumn }
			};


			grid.DataStore = filtered;


			var searchBox = new SearchBox();
			searchBox.TextChanged += (sender, e) => filtered.Filter = m => m.Name.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

			Content = new StackLayout
			{
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Spacing = 5,
				Padding = 10,
				Items =
				{
					searchBox,
					new StackLayoutItem(grid, expand: true)
				}
			};
		}
	}
}

