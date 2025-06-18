using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eto.Test.Sections.Behaviors;

[Section("Behaviors", "Mnemonic")]
public class MnemonicSection : Panel
{
	public MnemonicSection()
	{
		var layout = new DynamicLayout { Padding = Padding.Empty, Spacing = new Size(5, 5) };

		layout.AddSpace();
		layout.BeginCentered();

		layout.AddSeparateRow(CreateUseMnemonicCheckBox());
		layout.AddSeparateRow(CreateAlwaysShowMnemonicCheckBox());

		layout.EndCentered();

		if (!Platform.SupportedFeatures.HasFlag(PlatformFeatures.Mnemonics))
		{
			layout.AddRow(new Panel { Height = 20 });
			layout.AddRow(new Label { Text = "Mnemonics are not supported on this platform,\nso the shortcuts won't work.", TextAlignment = TextAlignment.Center, TextColor = Colors.Orange });
			layout.AddRow(new Panel { Height = 20 });
		}

		layout.AddSpace();
		layout.BeginCentered();

		layout.AddRow(new Label { Text = "Press Alt+I to press the button below." });
		layout.AddRow(new Button(OnButtonClicked) { Text = "&I" });

		layout.AddRow(new Label { Text = "Press Alt+J to check the box below." });
		layout.AddRow(new CheckBox { Text = "&J" });

		layout.AddRow(new Label { Text = "Press Alt+K to select the radio button below." });
		layout.AddRow(new RadioButton { Text = "&K" });

		layout.AddRow(new Label { Text = "Press Alt+&L to select the text box below." });
		layout.AddRow(new TextBox { });

		layout.EndCentered();
		layout.AddSpace();

		Content = layout;
	}

	private Control CreateAlwaysShowMnemonicCheckBox()
	{
		var control = new CheckBox { Text = "AlwaysShowMnemonic" };
		control.Checked = control.AlwaysShowMnemonic;
		control.CheckedChanged += (sender, e) =>
		{
			foreach (var child in Children.OfType<IMnemonicControl>())
			{
				child.AlwaysShowMnemonic = control.Checked == true;
			}
		};
		return control;

	}

	private void OnButtonClicked(object sender, EventArgs e)
	{
		Log.Write(sender, "Button was clicked");
	}

	Control CreateUseMnemonicCheckBox()
	{
		var control = new CheckBox { Text = "UseMnemonic" };
		control.Checked = control.UseMnemonic;
		control.CheckedChanged += (sender, e) =>
		{
			foreach (var child in Children.OfType<IMnemonicControl>())
			{
				child.UseMnemonic = control.Checked == true;
			}
		};
		return control;
	}
}
