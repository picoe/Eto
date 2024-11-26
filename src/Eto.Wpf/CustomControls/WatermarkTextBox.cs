namespace Eto.Wpf.CustomControls
{
	public class WatermarkTextBox : swc.TextBox
	{
		public static readonly sw.DependencyProperty WatermarkProperty =
			sw.DependencyProperty.Register(nameof(Watermark), typeof(object), typeof(WatermarkTextBox), new sw.PropertyMetadata(null));

		public static readonly sw.DependencyProperty WatermarkTemplateProperty =
			sw.DependencyProperty.Register(nameof(WatermarkTemplate), typeof(sw.DataTemplate), typeof(WatermarkTextBox), new sw.PropertyMetadata(null));

		public static readonly sw.DependencyProperty KeepWatermarkOnGotFocusProperty =
			sw.DependencyProperty.Register(nameof(KeepWatermarkOnGotFocus), typeof(bool), typeof(WatermarkTextBox), new sw.PropertyMetadata(false));

		public object Watermark
		{
			get => GetValue(WatermarkProperty);
			set => SetValue(WatermarkProperty, value);
		}

		public sw.DataTemplate WatermarkTemplate
		{
			get => (sw.DataTemplate)GetValue(WatermarkTemplateProperty);
			set => SetValue(WatermarkTemplateProperty, value);
		}

		public bool KeepWatermarkOnGotFocus
		{
			get => (bool)GetValue(KeepWatermarkOnGotFocusProperty);
			set => SetValue(KeepWatermarkOnGotFocusProperty, value);
		}
	}
}
