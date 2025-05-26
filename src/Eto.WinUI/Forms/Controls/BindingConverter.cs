namespace Eto.WinUI.Forms.Controls;

public class BindingConverter<T> : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (parameter is IIndirectBinding<T> binding)
		{
			return binding.GetValue(value);
		}
		return value;
	}
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}

public class StringBindingConverter : BindingConverter<string>
{
}

