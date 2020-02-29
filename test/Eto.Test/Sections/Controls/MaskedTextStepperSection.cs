using System;
using Eto.Forms;
using System.Diagnostics;
using Eto.Drawing;
using System.Linq;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(MaskedTextStepper))]
	public class MaskedTextStepperSection : DynamicLayout
	{
		static bool rememberValue;
		static double lastValue;

		public MaskedTextStepperSection()
		{
			DefaultSpacing = new Size(5, 5);
			Padding = new Padding(10);

			var enabledCheckBox = new CheckBox { Text = "Enabled", Checked = true };
			enabledCheckBox.CheckedChanged += (sender, e) => Set(m => m.Enabled = enabledCheckBox.Checked == true);

			var readOnlyCheckBox = new CheckBox { Text = "ReadOnly", Checked = false };
			readOnlyCheckBox.CheckedChanged += (sender, e) => Set(m => m.ReadOnly = readOnlyCheckBox.Checked == true);


			var tb = new NumericMaskedTextStepper<double> { Value = rememberValue ? lastValue : 123.456 };
			tb.ValueChanged += (sender, e) => lastValue = tb.Value;

			var l = new Label();
			l.TextBinding.Bind(Binding.Property(tb, c => c.Value).Convert(r => "Value: " + Convert.ToString(r)));

			var cultureSelector = new CultureDropDown();
			cultureSelector.SelectedValueBinding.Bind(tb, s => s.Culture);

			var rememberCheckBox = new CheckBox { Text = "Remember Value", Checked = rememberValue };
			rememberCheckBox.CheckedChanged += (sender, e) => rememberValue = rememberCheckBox.Checked == true;

			AddAutoSized(enabledCheckBox);
			AddAutoSized(readOnlyCheckBox);

			BeginGroup("FixedMaskedTextProvider", padding: 10);
			AddAutoSized(new MaskedTextStepper(new FixedMaskedTextProvider("(999) 000-0000")) { ShowPromptOnFocus = true, PlaceholderText = "(123) 456-7890" });
			AddAutoSized(new MaskedTextStepper<DateTime>(new FixedMaskedTextProvider<DateTime>("&&/90/0000") { ConvertToValue = DateTime.Parse }));
			AddAutoSized(new MaskedTextStepper(new FixedMaskedTextProvider(">L0L 0L0")));
			AddAutoSized(new MaskedTextStepper { InsertMode = InsertKeyMode.Toggle });
			EndGroup();

			BeginGroup("NumericMaskedTextStepper<double>", padding: 10);
			AddSeparateRow(tb, l, rememberCheckBox, null);
			AddSeparateRow("Culture:", cultureSelector, null);
			BeginHorizontal();
			EndHorizontal();
			EndGroup();

			AddSpace();

			foreach (var child in Children.OfType<MaskedTextStepper>())
			{
				child.Step += (sender, e) => Log.Write(sender, $"Step {e.Direction}");
			}
		}

		void Set(Action<MaskedTextStepper> action)
		{
			foreach (var child in Children.OfType<MaskedTextStepper>())
			{
				action(child);
			}

		}

	}
}