using System;
using Eto.Forms;
using System.Diagnostics;


namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(MaskedTextBox))]
	public class MaskedTextBoxSection : StackLayout
	{
		public MaskedTextBoxSection()
		{
			var tb = new NumericMaskedTextBox<decimal> { Value = 123.456M };
			var l = new Label();
			l.TextBinding.Bind(Binding.Property(tb, c => c.Value).Convert(r => "Value: " + Convert.ToString(r)));
			Items.Add(new StackLayout { Orientation = Orientation.Horizontal, Items = { tb, l } });
			Items.Add(new MaskedTextBox(new FixedMaskedTextProvider("(999) 000-0000")) { ShowPromptOnFocus = true, PlaceholderText = "(123) 456-7890" });
			Items.Add(new MaskedTextBox<DateTime>(new FixedMaskedTextProvider<DateTime>("&&/90/0000") { ConvertToValue = DateTime.Parse }));
			Items.Add(new MaskedTextBox(new FixedMaskedTextProvider(">L0L 0L0")));
			Items.Add(new MaskedTextBox{ InsertMode = InsertKeyMode.Toggle });
		}
	}
}

