using System;
using Eto.Forms;
using System.Diagnostics;
using Eto.Drawing;


namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(MaskedTextStepper))]
	public class MaskedTextStepperSection : StackLayout
	{
		public MaskedTextStepperSection()
		{
			Spacing = 5;
			Padding = new Padding(10);
			var tb = new NumericMaskedTextStepper<decimal> { Value = 123.456M };
			var l = new Label();
			l.TextBinding.Bind(Binding.Property(tb, c => c.Value).Convert(r => "Value: " + Convert.ToString(r)));
			Items.Add(new StackLayout { Orientation = Orientation.Horizontal, Spacing = 5, Items = { tb, l } });
			Items.Add(new NumericMaskedTextStepper<double> { Value = 0.000000123 });
			Items.Add(new MaskedTextStepper(new FixedMaskedTextProvider("(999) 000-0000")) { ShowPromptOnFocus = true, PlaceholderText = "(123) 456-7890" });
			Items.Add(new MaskedTextStepper<DateTime>(new FixedMaskedTextProvider<DateTime>("&&/90/0000") { ConvertToValue = DateTime.Parse }));
			Items.Add(new MaskedTextStepper(new FixedMaskedTextProvider(">L0L 0L0")));
			Items.Add(new MaskedTextStepper { InsertMode = InsertKeyMode.Toggle });
		}
	}
}