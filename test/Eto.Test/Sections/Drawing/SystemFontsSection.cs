using System;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", typeof(Font), "SystemFonts")]
	public class SystemFontsSection : Scrollable
	{
		public SystemFontsSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(10, 6), Padding = new Padding(10) };

			layout.BeginCentered();
			layout.AddRow("SystemFont", "Label", "Drawable", "Details");

			foreach (SystemFont systemFont in Enum.GetValues(typeof(SystemFont)))
			{
				Font font;
				try
				{
					font = SystemFonts.Cached(systemFont);
				}
				catch
				{
					layout.AddRow(systemFont.ToString(), "(not supported)", null, null);
					continue;
				}

				var label = new Label
				{
					Text = $"The quick brown fox ({systemFont})",
					VerticalAlignment = VerticalAlignment.Center,
					Font = font
				};

				var drawable = new Drawable();
				var capturedFont = font;
				var text = $"The quick brown fox ({systemFont})";
				var textSize = font.MeasureString(text);
				textSize += new Size(8, 8);
				drawable.Size = Size.Ceiling(textSize);
				drawable.Paint += (sender, e) =>
				{
					e.Graphics.DrawText(capturedFont, SystemColors.ControlText, 4, 4, text);
				};

				var details = $"{font.FamilyName}, {font.Size}pt, {font.FontStyle}";

				layout.AddRow(systemFont.ToString(), label, drawable, details);
			}

			layout.EndCentered();
			layout.AddSpace();

			Content = layout;
		}
	}
}
