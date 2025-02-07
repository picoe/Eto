namespace Eto.Mac.Drawing
{
	public class SystemColorsHandler : SystemColors.IHandler
	{
		public Color ControlText => NSColor.ControlText.ToEtoWithAppearance();

		public Color HighlightText => NSColor.AlternateSelectedControlText.ToEtoWithAppearance();

		public Color Control => NSColor.Control.ToEtoWithAppearance();

		public Color Highlight => NSColor.SelectedContentBackground.ToEtoWithAppearance();

		public Color WindowBackground => NSColor.WindowBackground.ToEtoWithAppearance();

		public Color DisabledText => NSColor.DisabledControlText.ToEtoWithAppearance();

		public Color ControlBackground => NSColor.ControlBackground.ToEtoWithAppearance();

		public Color SelectionText => NSColor.SelectedText.ToEtoWithAppearance();

		public Color Selection => NSColor.SelectedTextBackground.ToEtoWithAppearance();

		public Color LinkText => NSColor.Link.ToEtoWithAppearance();
	}
}

