using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(Label))]
	public class LabelSection : Panel
	{
		public LabelSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.Add(NormalLabel());
			layout.Add(FontLabel());
			layout.Add(NoWrapLabel());
			layout.AddSeparateRow(null, UnderlineLabel(), HotkeyLabel(), HotkeyUnderlineLabel(), null);
			layout.AddSeparateRow(null, ColorLabel(), BackgroundColorLabel(), null);
			layout.Add(CenterLabel());
			layout.Add(RightLabel());
			layout.Add(MiddleLabel(), yscale: true);
			layout.Add(BottomLabel(), yscale: true);

			Content = layout;
		}

		Control NormalLabel()
		{
			return new Label
			{
				Text = "Normal Label"
			};
		}

		Control FontLabel()
		{
			return new Label
			{
				Text = "Font Label",
				Font = Fonts.Sans(14, FontStyle.Bold),
			};
		}

		Control CenterLabel()
		{
			return new Label
			{
				Text = "Center Align",
				TextAlignment = TextAlignment.Center
			};
		}

		Control RightLabel()
		{
			return new Label
			{
				Text = "Right Align",
				TextAlignment = TextAlignment.Right
			};
		}

		Control MiddleLabel()
		{
			return new Label
			{
				Text = "Middle Center Align",
				TextAlignment = TextAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				BackgroundColor = Colors.AliceBlue
			};
		}

		Control BottomLabel()
		{
			return new Label
			{
				Text = "Bottom Center Align",
				TextAlignment = TextAlignment.Center,
				VerticalAlignment = VerticalAlignment.Bottom,
				BackgroundColor = Colors.AliceBlue
			};
		}

		Control ColorLabel()
		{
			return new Label
			{
				Text = "Custom Color",
				TextColor = Color.FromArgb(unchecked((int)0xFF00a000))
			};
		}

		Control BackgroundColorLabel()
		{
			return new Label
			{
				Text = "Custom Background Color",
				BackgroundColor = Colors.Green,
			};
		}

		Control UnderlineLabel()
		{
			return new Label
			{
				Text = "Underlined Text",
				Font = SystemFonts.Label(decoration: FontDecoration.Underline)
			};
		}

		Control HotkeyLabel()
		{
			return new Label
			{
				Text = "Some &Hotkey"
			};
		}

		Control HotkeyUnderlineLabel()
		{
			return new Label
			{
				Text = "Some Underlined &Hotkey",
				Font = SystemFonts.Label(decoration: FontDecoration.Underline)
			};
		}

		Control NoWrapLabel()
		{
			return new Label
			{
				Text = "No wrapping on this long label that should not wrap. No wrapping on this long label that should not wrap",
				Wrap = WrapMode.None
			};
		}
	}
}

