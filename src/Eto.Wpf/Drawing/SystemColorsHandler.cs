namespace Eto.Wpf.Drawing
{
	public class SystemColorsHandler : SystemColors.IHandler
	{
		static Color? GetResourceColor(sw.ResourceKey key)
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

		// The entry area of controls such as a TextBox. Distinct from WindowBackground, which is the
		// window chrome (grey in None). Themes that need a specific entry colour supply "ControlBackground"
		// (palette -> Surface); the Fluent theme exposes "TextControlBackground" app-wide; otherwise it
		// maps to the WPF Window brush (white in None). The generic TextControlBackground is template-scoped
		// so it is not resolved here for the None/palette themes.
		public Color ControlBackground => GetResourceColor("ControlBackground") ?? GetResourceColor("TextControlBackground") ?? GetResourceColor(sw.SystemColors.WindowBrushKey) ?? sw.SystemColors.WindowColor.ToEto();

		public Color Control => GetResourceColor("ControlFillColorDefaultBrush") ?? GetResourceColor(sw.SystemColors.ControlBrushKey) ?? sw.SystemColors.ControlColor.ToEto();

		public Color ControlText => GetResourceColor("TextFillColorPrimaryBrush") ?? GetResourceColor(sw.SystemColors.ControlTextBrushKey) ?? sw.SystemColors.ControlTextColor.ToEto();

		public Color HighlightText => GetResourceColor("TextOnAccentFillColorPrimaryBrush") ?? GetResourceColor(sw.SystemColors.HighlightTextBrushKey) ?? sw.SystemColors.HighlightTextColor.ToEto();

		public Color Highlight => GetResourceColor("AccentFillColorDefaultBrush") ?? GetResourceColor(sw.SystemColors.HighlightBrushKey) ?? sw.SystemColors.HighlightColor.ToEto();

		public Color WindowBackground => GetResourceColor("WindowBackground") ?? GetResourceColor(sw.SystemColors.WindowBrushKey) ?? sw.SystemColors.WindowColor.ToEto();

		public Color DisabledText => GetResourceColor("TextFillColorDisabledBrush") ?? GetResourceColor(sw.SystemColors.GrayTextBrushKey) ?? sw.SystemColors.GrayTextColor.ToEto();

		public Color SelectionText => GetResourceColor("TextOnAccentFillColorSelectedTextBrush") ?? GetResourceColor(sw.SystemColors.HighlightTextBrushKey) ?? sw.SystemColors.HighlightTextColor.ToEto();

		public Color Selection => GetResourceColor("AccentFillColorSelectedTextBackgroundBrush") ?? GetResourceColor(sw.SystemColors.HighlightBrushKey) ?? sw.SystemColors.HighlightColor.ToEto();

		public Color LinkText => GetResourceColor("HyperlinkForeground") ?? GetResourceColor(sw.SystemColors.HighlightBrushKey) ?? sw.SystemColors.HighlightColor.ToEto();
	}
}
