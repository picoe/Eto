using System;
using Eto.Forms;
using System.Diagnostics;
using Eto.Drawing;
using System.Linq;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(MaskedTextStepper))]
	public class MaskedTextStepperSection : StackLayout
	{
		public MaskedTextStepperSection()
		{
			Spacing = 5;
			Padding = new Padding(10);

			var enabledCheckBox = new CheckBox { Text = "Enabled", Checked = true };
			enabledCheckBox.CheckedChanged += (sender, e) =>
			{
				foreach (var child in Children.OfType<MaskedTextStepper>())
				{
					child.Enabled = enabledCheckBox.Checked == true;
				}
			};

			var readOnlyCheckBox = new CheckBox { Text = "ReadOnly", Checked = false };
			readOnlyCheckBox.CheckedChanged += (sender, e) =>
			{
				foreach (var child in Children.OfType<MaskedTextStepper>())
				{
					child.ReadOnly = readOnlyCheckBox.Checked == true;
				}
			};

			var tb = new NumericMaskedTextStepper<decimal> { Value = 123.456M };
			var l = new Label();
			l.TextBinding.Bind(Binding.Property(tb, c => c.Value).Convert(r => "Value: " + Convert.ToString(r)));

			Items.Add(enabledCheckBox);
			Items.Add(readOnlyCheckBox);
			Items.Add(new StackLayout { Orientation = Orientation.Horizontal, Spacing = 5, Items = { tb, l } });
			Items.Add(new NumericMaskedTextStepper<double> { Value = 0.000000123 });
			Items.Add(new MaskedTextStepper(new FixedMaskedTextProvider("(999) 000-0000")) { ShowPromptOnFocus = true, PlaceholderText = "(123) 456-7890" });
			Items.Add(new MaskedTextStepper<DateTime>(new FixedMaskedTextProvider<DateTime>("&&/90/0000") { ConvertToValue = DateTime.Parse }));
			Items.Add(new MaskedTextStepper(new FixedMaskedTextProvider(">L0L 0L0")));
			Items.Add(new MaskedTextStepper { InsertMode = InsertKeyMode.Toggle });


			foreach (var child in Children.OfType<MaskedTextStepper>())
			{
				child.Step += (sender, e) => Log.Write(sender, $"Step {e.Direction}");
			}
		}
	}
}