namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(MaskedTextBox))]
	public class MaskedTextBoxSection : DynamicLayout
	{
		static bool rememberValue;
		static double lastValue;

		public MaskedTextBoxSection()
		{
			DefaultSpacing = new Size(5, 5);
			Padding = new Padding(10);

			var enabledCheckBox = new CheckBox { Text = "Enabled", Checked = true };
			enabledCheckBox.CheckedChanged += (sender, e) => Set(m => m.Enabled = enabledCheckBox.Checked == true);

			var readOnlyCheckBox = new CheckBox { Text = "ReadOnly", Checked = false };
			readOnlyCheckBox.CheckedChanged += (sender, e) => Set(m => m.ReadOnly = readOnlyCheckBox.Checked == true);


			var tb = new NumericMaskedTextBox<double> { Value = rememberValue ? lastValue : 123.456 };
			tb.ValueChanged += (sender, e) => lastValue = tb.Value;
			LogValueChanged(tb);

			var l = new Label();
			l.TextBinding.Bind(Binding.Property(tb, c => c.Value).Convert(r => "Value: " + Convert.ToString(r)));

			var cultureSelector = new CultureDropDown();
			cultureSelector.SelectedValueBinding.Bind(tb, s => s.Culture);

			var rememberCheckBox = new CheckBox { Text = "Remember Value", Checked = rememberValue };
			rememberCheckBox.CheckedChanged += (sender, e) => rememberValue = rememberCheckBox.Checked == true;

			AddAutoSized(enabledCheckBox);
			AddAutoSized(readOnlyCheckBox);

			BeginGroup("FixedMaskedTextProvider", padding: 10);
			AddAutoSized(new MaskedTextBox(new FixedMaskedTextProvider("(999) 000-0000")) { ShowPromptMode = ShowPromptMode.OnFocus, PlaceholderText = "(123) 456-7890" });
			AddAutoSized(LogValueChanged(new MaskedTextBox<DateTime?>(new FixedMaskedTextProvider<DateTime?>("&&/90/0000") { ConvertToValue = s => DateTime.TryParse(s, out var dt) ? dt : (DateTime?)null })));
			AddAutoSized(new MaskedTextBox(new FixedMaskedTextProvider(">L0L 0L0")));
			AddAutoSized(new MaskedTextBox { InsertMode = InsertKeyMode.Toggle });
			EndGroup();

			BeginGroup("NumericMaskedTextBox<double>", padding: 10);
			AddSeparateRow(tb, l, rememberCheckBox, null);
			AddSeparateRow("Culture:", cultureSelector, null);
			BeginHorizontal();
			EndHorizontal();
			EndGroup();

			AddSpace();
			
		}

		void Set(Action<MaskedTextBox> action)
		{
			foreach (var child in Children.OfType<MaskedTextBox>())
			{
				action(child);
			}
		}
		
		MaskedTextBox<T> LogValueChanged<T>(MaskedTextBox<T> maskedTextBox)
		{
			maskedTextBox.ValueChanged += (sender, e) => Log.Write(sender, $"Value changed: {maskedTextBox.Value}");
			maskedTextBox.TextChanged += (sender, e) => Log.Write(sender, $"Text changed: {maskedTextBox.Text}");
			return maskedTextBox;
		}
	}
}

