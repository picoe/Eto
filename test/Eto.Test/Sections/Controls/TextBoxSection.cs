namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(TextBox))]
	public class TextBoxSection : TextBoxSection<TextBox>
	{
	}
	
	[Section("Controls", typeof(SearchBox))]
	public class SearchBoxSection : TextBoxSection<SearchBox>
	{
	}
	
	public class TextBoxSection<T> : Scrollable
		where T: TextBox, new()
	{
		bool cancelTextChanging;
		bool cancelTextInput;

		public TextBoxSection()
		{
			var textBox = new T();
			LogEvents(textBox);

			var cancelTextChangingCheckBox = new CheckBox { Text = "Cancel TextChanging" };
			cancelTextChangingCheckBox.CheckedBinding.Bind(() => cancelTextChanging, v => cancelTextChanging = v ?? false);

			var cancelTextInputCheckBox = new CheckBox { Text = "Cancel TextInput" };
			cancelTextInputCheckBox.CheckedBinding.Bind(() => cancelTextInput, v => cancelTextInput = v ?? false);

			var placeholderText = new TextBox();
			placeholderText.TextBinding.Bind(textBox, c => c.PlaceholderText);

			var setTextButton = new Button { Text = "Set Text" };
			setTextButton.Click += (sender, e) => textBox.Text = "Some Text";

			var selectAllButton = new Button { Text = "SelectAll" };
			selectAllButton.Click += (sender, e) => textBox.SelectAll();

			var enabledCheckBox = new CheckBox { Text = "Enabled" };
			enabledCheckBox.CheckedBinding.Bind(textBox, c => c.Enabled);

			var readOnlyCheckBox = new CheckBox { Text = "ReadOnly" };
			readOnlyCheckBox.CheckedBinding.Bind(textBox, c => c.ReadOnly);

			var alignmentDropDown = new EnumDropDown<TextAlignment>();
			alignmentDropDown.SelectedValueBinding.Bind(textBox, c => c.TextAlignment);

			var showBorderCheckBox = new CheckBox { Text = "ShowBorder" };
			showBorderCheckBox.CheckedBinding.Bind(textBox, c => c.ShowBorder);

			var maxLengthStepper = new NumericStepper { MinValue = 0 };
			maxLengthStepper.ValueBinding.Bind(textBox, c => c.MaxLength);

			var selectionMode = new EnumDropDown<AutoSelectMode>();
			selectionMode.SelectedValueBinding.Bind(textBox, c => c.AutoSelectMode);
			
			var alwaysShowSelection = new CheckBox { Text = "AlwaysShowSelection" };
			alwaysShowSelection.CheckedBinding.Bind(textBox, c => c.AlwaysShowSelection);

			var layout = new DynamicLayout { Padding = 10, DefaultSpacing = new Size(5, 5) };
			layout.AddSeparateRow(null, enabledCheckBox, readOnlyCheckBox, showBorderCheckBox, null);
			layout.AddSeparateRow(null, "TextAlignment", alignmentDropDown, "SelectionMode", selectionMode, alwaysShowSelection, null);
			layout.AddSeparateRow(null, "MaxLength", maxLengthStepper, "PlaceholderText", placeholderText, null);
			layout.AddSeparateRow(null, cancelTextChangingCheckBox, cancelTextInputCheckBox, null);
			layout.AddSeparateRow(null, setTextButton, selectAllButton, null);
			layout.Add(null);
			layout.AddCentered(textBox);
			layout.AddCentered(DifferentSize());
			layout.Add(null);

			Content = layout;
		}

		Control DifferentSize()
		{
			var control = new T { Text = "Different Size (300x50)", Size = new Size(300, 50) };
			LogEvents(control);
			return control;
		}

		void LogEvents(TextBox control)
		{
			control.MouseDown += (sender, e) => Log.Write(control, $"MouseDown: {e.Location}, Index: {control.GetCharacterIndex(e.Location)}");
			control.TextChanging += (sender, e) =>
			{
				if (cancelTextChanging)
					e.Cancel = true;
				Log.Write(control, $"TextChanging, Source: {e.Source}, Range: {e.Range}, Text: {e.Text}, Cancel: {e.Cancel}");
			};
			control.TextChanged += (sender, e) => Log.Write(control, $"TextChanged, Text: {control.Text}, Selection: {control.Selection}");
			control.TextInput += (sender, e) =>
			{
				if (cancelTextInput)
					e.Cancel = true;
				Log.Write(control, "TextInput: {0}, Cancel: {1}", e.Text, e.Cancel);
			};
			control.KeyDown += (sender, e) =>
			{
				if (e.KeyData == (Keys.Slash | Keys.Shift))
				{
					e.Handled = true;
					Log.Write(control, $"Selection: {control.Selection}");
				}
			};
		}
	}
}

