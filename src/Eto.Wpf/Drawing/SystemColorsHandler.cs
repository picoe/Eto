namespace Eto.Wpf.Drawing
{
	public class SystemColorsHandler : SystemColors.IHandler
	{
		static Color? GetResourceColor(string key)
		{
			var app = sw.Application.Current;
			if (app == null)
				return null;
			var resource = app.TryFindResource(key);
			if (resource is swm.SolidColorBrush brush)
				return brush.Color.ToEto();
			if (resource is swm.Color color)
				return color.ToEto();
			return null;
		}

		public Color ControlBackground => GetResourceColor("WindowBackground") ?? sw.SystemColors.WindowColor.ToEto();

		public Color Control => GetResourceColor("ControlFillColorDefaultBrush") ?? sw.SystemColors.ControlColor.ToEto();

		public Color ControlText => GetResourceColor("TextFillColorPrimaryBrush") ?? sw.SystemColors.ControlTextColor.ToEto();

		public Color HighlightText => GetResourceColor("TextOnAccentFillColorPrimaryBrush") ?? sw.SystemColors.HighlightTextColor.ToEto();

		public Color Highlight => GetResourceColor("AccentFillColorDefaultBrush") ?? sw.SystemColors.HighlightColor.ToEto();

		public Color WindowBackground => GetResourceColor("WindowBackground") ?? sw.SystemColors.WindowColor.ToEto();

		public Color DisabledText => GetResourceColor("TextFillColorDisabledBrush") ?? sw.SystemColors.GrayTextColor.ToEto();

		public Color SelectionText => GetResourceColor("TextOnAccentFillColorSelectedTextBrush") ?? sw.SystemColors.HighlightTextColor.ToEto();

		public Color Selection => GetResourceColor("AccentFillColorSelectedTextBackgroundBrush") ?? sw.SystemColors.HighlightColor.ToEto();

		public Color LinkText => GetResourceColor("HyperlinkForeground") ?? sw.SystemColors.HighlightColor.ToEto();
	}
}
