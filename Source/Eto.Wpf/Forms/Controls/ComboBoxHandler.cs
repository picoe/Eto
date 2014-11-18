using System;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;

namespace Eto.Wpf.Forms.Controls
{
	public class ComboBoxHandler : DropDownHandler<EtoComboBox, ComboBox, ComboBox.ICallback>, ComboBox.IHandler
	{
		bool textChanging;
		string lastText;

		public ComboBoxHandler()
		{
			Control.RetainSize = true;
			Control.IsEditable = true;
			Control.IsTextSearchEnabled = false;
			Control.AddHandler(swc.Primitives.TextBoxBase.TextChangedEvent, new swc.TextChangedEventHandler((sender, e) => HandleTextChanged()));
		}

		void HandleTextChanged()
		{
			if (textChanging) return;
			try
			{
				textChanging = true;

				var text = Text;
				if (text != lastText)
				{
					Callback.OnTextChanged(Widget, EventArgs.Empty);
					lastText = text;
				}

				if (!AutoComplete)
				{
					// with autocomplete off, items aren't selected based on typed text but should be
					var item = DataStore.FirstOrDefault(o => Widget.TextBinding.GetValue(o) == text);
					if (item != null)
					{
						Control.SelectedItem = item;
						return;
					}
				}
				else if (Control.SelectedItem != null)
				{
					// with autocomplete on, selected item is set even though text doesn't actually match
					var val = Widget.TextBinding.GetValue(Control.SelectedItem);
					if (text == val) return;
				}

				// unselect the current item manually
				var textBox = Control.TextBox;
				if (textBox != null)
				{
					// keeping selection if there's a textbox
					var selectionStart = textBox.SelectionStart;
					var selectionLength = textBox.SelectionLength;
					Control.SelectedIndex = -1;
					Control.Text = text;
					textBox.SelectionStart = selectionStart;
					textBox.SelectionLength = selectionLength;
				}
				else
				{
					Control.SelectedIndex = -1;
					Control.Text = text;
				}
			}
			finally
			{
				textChanging = false;
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ComboBox.TextChangedEvent:
					// handled automatically
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				var size = base.DefaultSize;
				return new Size(100, size.Height);
			}
		}

		public string Text
		{
			get { return Control.Text; }
			set
			{
				if (value != Text)
				{
					Control.Text = value;
					HandleTextChanged();
				}
			}
		}

		public bool ReadOnly
		{
			get { return Control.IsReadOnly; }
			set { Control.IsReadOnly = value; }
		}

		public bool AutoComplete
		{
			get { return Control.IsTextSearchEnabled; }
			set { Control.IsTextSearchEnabled = value; }
		}
	}
}
