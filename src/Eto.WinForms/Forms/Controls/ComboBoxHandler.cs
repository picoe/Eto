using System;
using System.Collections.Generic;
using System.Linq;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms.Forms.Controls
{
	public class ComboBoxHandler : DropDownHandler<EtoComboBox, ComboBox, ComboBox.ICallback>, ComboBox.IHandler
	{
		bool readOnly;
		int suppressTextChanged;

		public ComboBoxHandler()
		{
			Control.DropDownStyle = swf.ComboBoxStyle.DropDown;
			Control.MinSize = new sd.Size(100, 0);
			Control.TextChanged += ControlOnTextChanged;
		}

		void ControlOnTextChanged(object sender, EventArgs e)
		{
			if (suppressTextChanged > 0)
				return;
			var selected = SelectedIndex;
			var text = Text;
			var item = Control.Items.Cast<object>().FirstOrDefault(r => Widget.ItemTextBinding.GetValue(r) == text);
			var newIndex = item != null ? Control.Items.IndexOf(item) : -1;
			if (selected != newIndex)
			{
				suppressTextChanged++;
				var selectionStart = Control.SelectionStart;
				var selectionLength = Control.SelectionLength;
				SelectedIndex = newIndex;
				Text = text;
				Control.SelectionStart = selectionStart;
				Control.SelectionLength = selectionLength;
				suppressTextChanged--;
			}
			Callback.OnTextChanged(Widget, EventArgs.Empty);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ComboBox.TextChangedEvent:
					// handled intrinically
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public override string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public bool ReadOnly
		{
			get { return readOnly; }
			set
			{
				if (readOnly != value)
				{
					if (readOnly)
						Control.KeyPress -= ControlOnKeyPress;
					readOnly = value;
					if (readOnly)
						Control.KeyPress += ControlOnKeyPress;
				}
			}
		}

		public bool AutoComplete
		{
			get { return Control.AutoCompleteMode != swf.AutoCompleteMode.None; }
			set
			{
				Control.AutoCompleteMode = value ? swf.AutoCompleteMode.Append : swf.AutoCompleteMode.None;
				Control.AutoCompleteSource = swf.AutoCompleteSource.ListItems;
			}
		}

		static void ControlOnKeyPress(object sender, swf.KeyPressEventArgs e)
		{
			e.Handled = true;
		}
	}
}
