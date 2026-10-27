namespace Eto.Wpf.Forms.Controls
{
	/// <summary>
	/// Attached properties for styling text selection in a way that compiles on all target frameworks.
	/// </summary>
	/// <remarks>
	/// <see cref="swc.Primitives.TextBoxBase"/>.SelectionTextBrush was added in .NET Framework 4.7.2, so styles
	/// (e.g. the palette theme) cannot set it directly when Eto.Wpf is compiled for net462. This attached
	/// property forwards to SelectionTextBrush — directly on frameworks that have it, via reflection on
	/// net462 (where it still works at runtime on 4.7.2+, and is silently ignored on older runtimes).
	/// Note the brush only takes effect with non-adorner text selection rendering
	/// (see <see cref="ResourceThemeHandler.UseNonAdornerTextSelection"/>).
	/// </remarks>
	public static class EtoTextSelection
	{
		public static readonly sw.DependencyProperty TextBrushProperty = sw.DependencyProperty.RegisterAttached(
			"TextBrush", typeof(swm.Brush), typeof(EtoTextSelection), new sw.PropertyMetadata(null, TextBrush_Changed));

		public static swm.Brush GetTextBrush(sw.DependencyObject obj) => (swm.Brush)obj.GetValue(TextBrushProperty);

		public static void SetTextBrush(sw.DependencyObject obj, swm.Brush value) => obj.SetValue(TextBrushProperty, value);

#if NET462
		static readonly PropertyInfo s_selectionTextBrushProperty = typeof(swc.Primitives.TextBoxBase).GetProperty("SelectionTextBrush");

		static void TextBrush_Changed(sw.DependencyObject d, sw.DependencyPropertyChangedEventArgs e)
		{
			if (d is swc.Primitives.TextBoxBase textBox)
				s_selectionTextBrushProperty?.SetValue(textBox, e.NewValue);
		}
#else
		static void TextBrush_Changed(sw.DependencyObject d, sw.DependencyPropertyChangedEventArgs e)
		{
			if (d is swc.Primitives.TextBoxBase textBox)
				textBox.SelectionTextBrush = (swm.Brush)e.NewValue;
		}
#endif
	}
}
