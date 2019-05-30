using System;
using Eto.Forms;
using swc = System.Windows.Controls;
using swd = System.Windows.Data;
using sw = System.Windows;
using swm = System.Windows.Media;
using System.ComponentModel;

namespace Eto.Wpf.Forms.Cells
{
	public class TextBoxCellHandler : CellHandler<TextBoxCellHandler.Column, TextBoxCell, TextBoxCell.ICallback>, TextBoxCell.IHandler
	{
		static readonly object UseMouseSelectionOnly_Key = new object();

		/// <summary>
		/// This makes the mouse only select text without being able to drag/drop the text.
		/// By default, when clicking and dragging a selection it would allow you to drag the text to other controls.
		/// </summary>
		public bool UseMouseSelectionOnly
		{
			get { return Widget.Properties.Get(UseMouseSelectionOnly_Key, false); }
			set { Widget.Properties.Set(UseMouseSelectionOnly_Key, value); }
		}

		public TextAlignment TextAlignment
		{
			get { return Control.TextAlignment.ToEto(); }
			set { Control.TextAlignment = value.ToWpfTextAlignment(); }
		}

		public VerticalAlignment VerticalAlignment
		{
			get { return Control.VerticalAlignment.ToEto(); }
			set { Control.VerticalAlignment = value.ToWpf(); }
		}

		public AutoSelectMode AutoSelectMode { get; set; } = AutoSelectMode.OnFocus;

		string GetValue(object dataItem)
		{
			if (Widget.Binding != null)
			{
				return Widget.Binding.GetValue(dataItem);
			}
			return null;
		}

		void SetValue(object dataItem, string value)
		{
			if (Widget.Binding != null)
			{
				Widget.Binding.SetValue(dataItem, value);
			}
		}

		public class Column : swc.DataGridTextColumn, INotifyPropertyChanged
		{
			public TextBoxCellHandler Handler { get; set; }

			public event PropertyChangedEventHandler PropertyChanged;

			sw.TextAlignment _textAlignment;
			public sw.TextAlignment TextAlignment
			{
				get { return _textAlignment; }
				set
				{
					_textAlignment = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextAlignment)));
				}
			}

			sw.VerticalAlignment _verticalAlignment = sw.VerticalAlignment.Center;
			public sw.VerticalAlignment VerticalAlignment
			{
				get { return _verticalAlignment; }
				set
				{
					_verticalAlignment = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VerticalAlignment)));
				}
			}

			swd.Binding CreateBinding(string property)
			{
				var binding = new swd.Binding();
				binding.Source = this;
				binding.Path = new sw.PropertyPath(property);
				binding.Mode = swd.BindingMode.OneWay;
				binding.UpdateSourceTrigger = swd.UpdateSourceTrigger.PropertyChanged;
				return binding;
			}

			protected override sw.FrameworkElement GenerateElement(swc.DataGridCell cell, object dataItem)
			{
				var element = (swc.TextBlock)base.GenerateElement(cell, dataItem);
				element.SetBinding(swc.TextBlock.TextAlignmentProperty, CreateBinding(nameof(TextAlignment)));
				element.SetBinding(swc.TextBlock.VerticalAlignmentProperty, CreateBinding(nameof(VerticalAlignment)));
				element.Text = Handler.GetValue(dataItem);
				Handler.FormatCell(element, cell, dataItem);

				element.DataContextChanged += (sender, e) =>
				{
					var control = (swc.TextBlock)sender;
					control.Text = Handler.GetValue(control.DataContext);
					Handler.FormatCell(control, cell, control.DataContext);
				};
				return Handler.SetupCell(element);
			}

			protected override sw.FrameworkElement GenerateEditingElement(swc.DataGridCell cell, object dataItem)
			{
				var element = (swc.TextBox)base.GenerateEditingElement(cell, dataItem);
				element.Name = "control";
				element.SetBinding(swc.TextBlock.TextAlignmentProperty, CreateBinding(nameof(TextAlignment)));
				element.SetBinding(swc.TextBlock.VerticalAlignmentProperty, CreateBinding(nameof(VerticalAlignment)));
				element.Text = Handler.GetValue(dataItem);
				Handler.FormatCell(element, cell, dataItem);
				if (Handler.UseMouseSelectionOnly)
				{
					element.PreviewMouseLeftButtonDown += (sender, e) =>
					{
						element.Select(0, 0);
					};
				}

				element.DataContextChanged += (sender, e) =>
				{
					var control = sender as swc.TextBox;
					control.Text = Handler.GetValue(control.DataContext);
					Handler.FormatCell(control, cell, control.DataContext);
				};
				return Handler.SetupCell(element);
			}

			protected override object PrepareCellForEdit(sw.FrameworkElement editingElement, sw.RoutedEventArgs editingEventArgs)
			{
				var control = editingElement as swc.TextBox ?? editingElement.FindChild<swc.TextBox>("control");
				var result = base.PrepareCellForEdit(control, editingEventArgs);

				// AutoSelectMode.OnFocus is the default behaviour of this control
				if (Handler.AutoSelectMode == AutoSelectMode.Always)
				{
					control.SelectAll();
				}
				else if (Handler.AutoSelectMode == AutoSelectMode.Never)
				{
					control.SelectionLength = 0;
					control.SelectionStart = control.Text.Length;
				}

				return result;
			}

			protected override bool CommitCellEdit(sw.FrameworkElement editingElement)
			{
				var control = editingElement as swc.TextBox ?? editingElement.FindChild<swc.TextBox>("control");
				if (control != null)
					Handler.SetValue(control.DataContext, control.Text);
				Handler.ContainerHandler.CellEdited(Handler, editingElement);
				return true;
			}
		}

		public TextBoxCellHandler()
		{
			Control = new Column { Handler = this };
		}
	}
}
