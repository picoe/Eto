namespace Eto.Android
	{
	public class SystemColorsHandler : SystemColors.IHandler
		{
		// TODO: Not really implemented - just try to approximate the default WinForms look for now.

		public Color DisabledText => Colors.Gray;

		public Color ControlText => Colors.Black;

		public Color HighlightText => Colors.White;

		public Color Control => Colors.LightSlateGray;

		public Color ControlBackground => Colors.LightSlateGray;

		public Color Highlight => Colors.Blue;

		public Color WindowBackground => Colors.White;

		public Color SelectionText => HighlightText;

		public Color Selection => Highlight;

		public Color LinkText => Colors.Blue;
		}
	}
