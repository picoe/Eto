namespace Eto.GtkSharp.Forms.Controls
{
	static class GtkMnemonicHelper
	{
		// Eto's mnemonic format is "&X" (and "&&" for a literal "&"). Gtk.Label.Pattern
		// uses '_' for underlined display characters and spaces for all others.
		public static string ToPatternWithMnemonicUnderline(string text)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;

			var sb = new System.Text.StringBuilder(text.Length);
			bool foundMnemonic = false;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (c == '&' && i + 1 < text.Length && text[i + 1] == '&')
				{
					sb.Append(' ');
					i++;
				}
				else if (c == '&' && !foundMnemonic && i + 1 < text.Length)
				{
					sb.Append('_');
					i++;
					foundMnemonic = true;
				}
				else
				{
					sb.Append(' ');
				}
			}
			return sb.ToString();
		}
	}
}
