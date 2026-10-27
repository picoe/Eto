using System.Collections.Concurrent;

namespace Eto.Wpf.Drawing
{
	public class SystemColorsHandler : SystemColors.IHandler
	{
		/// <summary>
		/// Colors last resolved on the application's thread, for the threads that must not resolve them
		/// themselves. See <see cref="GetResourceColor"/>.
		/// </summary>
		static readonly ConcurrentDictionary<object, Color> _resourceColors = new ConcurrentDictionary<object, Color>();

		/// <summary>
		/// Re-reads every system color on the application's thread, so its brushes are realized there and
		/// the cache matches the merged resources. Call whenever the theme or the OS colors change.
		/// </summary>
		internal static void RefreshResourceColors()
		{
			var app = sw.Application.Current;

			// SystemEvents raises on a thread of its own, and only the application's may read the brushes.
			if (app != null && !app.CheckAccess())
			{
				app.Dispatcher.BeginInvoke(new Action(RefreshResourceColors), swt.DispatcherPriority.Background);
				return;
			}

			_resourceColors.Clear();
			if (app == null)
				return;

			// Driven off the interface so a color added to SystemColors cannot be left out here.
			var handler = new SystemColorsHandler();
			foreach (var property in typeof(SystemColors.IHandler).GetProperties())
				property.GetValue(handler);
		}

		/// <summary>
		/// Resolves <paramref name="key"/> to a color, or null when it cannot be read from this thread.
		/// </summary>
		/// <remarks>
		/// The theme brushes take their color from a DynamicResource so they cannot be frozen, which
		/// leaves them owned by the thread that first realizes them - reading one from anywhere else
		/// throws (RH-98688). So nothing is resolved off the application's thread; its last read is used.
		/// </remarks>
		static Color? GetResourceColor(object key)
		{
			var app = sw.Application.Current;
			if (app == null)
				return null;

			if (!app.CheckAccess())
				return _resourceColors.TryGetValue(key, out var cached) ? cached : (Color?)null;

			Color? color = null;
			var resource = app.TryFindResource(key);
			if (resource is swm.Color resourceColor)
				color = resourceColor.ToEto();
			// false only when another thread owns the brush, so it can never be read here - skip it
			else if (resource is swm.SolidColorBrush brush && brush.CheckAccess())
				color = brush.Color.ToEto();

			if (color != null)
				_resourceColors[key] = color.Value;
			else
				_resourceColors.TryRemove(key, out _);
			return color;
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
