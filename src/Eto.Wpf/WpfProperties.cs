namespace Eto.Wpf
{
	public static class WpfProperties
	{
		public static string GetEtoStyle(sw.DependencyObject obj) => (string)obj.GetValue(EtoStyleProperty);
		public static void SetEtoStyle(sw.DependencyObject obj, string value) => obj.SetValue(EtoStyleProperty, value);

		public static readonly sw.DependencyProperty EtoStyleProperty =
				sw.DependencyProperty.RegisterAttached("EtoStyle", typeof(string), typeof(WpfProperties), new sw.UIPropertyMetadata(string.Empty));



		public static string GetEtoModifier(sw.DependencyObject obj) => (string)obj.GetValue(EtoModifierProperty);
		public static void SetEtoModifier(sw.DependencyObject obj, string value) => obj.SetValue(EtoModifierProperty, value);

		public static readonly sw.DependencyProperty EtoModifierProperty =
				sw.DependencyProperty.RegisterAttached("EtoModifier", typeof(string), typeof(WpfProperties), new sw.UIPropertyMetadata(string.Empty));

	}
}
