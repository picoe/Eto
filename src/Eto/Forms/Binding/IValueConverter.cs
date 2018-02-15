using System;
using System.Globalization;

namespace Eto.Forms
{
	/// <summary>
	/// Interface for providing a reusable converter for binding values
	/// </summary>
	/// <remarks>
	/// This can be used with the Convert() method for a binding.
	/// </remarks>
	/// <seealso cref="IndirectBinding{T}.Convert(IValueConverter, Type, object, CultureInfo)"/>
	/// <seealso cref="IndirectBinding{T}.Convert{TValue}(IValueConverter, object, CultureInfo)"/>
	public interface IValueConverter
	{
		/// <summary>
		/// Converts the <paramref name="value"/> to the specified <paramref name="targetType"/>
		/// </summary>
		/// <remarks>
		/// This is called when translating the value from the source to the destination, usually from the 
		/// Control to the View Model.
		/// </remarks>
		/// <param name="value">Value to convert</param>
		/// <param name="targetType">Type to convert the value to</param>
		/// <param name="parameter">Context-specific parameter passed from the binding</param>
		/// <param name="culture">Culture to convert with</param>
		/// <returns>A converted value with the type of <paramref name="targetType"/>.</returns>
		object Convert(object value, Type targetType, object parameter, CultureInfo culture);

		/// <summary>
		/// Converts the <paramref name="value"/> to the specified <paramref name="targetType"/>.
		/// </summary>
		/// <remarks>
		/// This is called when translating the value back from the destination to the source, usually from the
		/// View Model to the Control.
		/// 
		/// This should be the reverse implementation of the <see cref="Convert"/> method.
		/// </remarks>
		/// <param name="value">Value to convert</param>
		/// <param name="targetType">Type to convert the value back to</param>
		/// <param name="parameter">Context-specific parameter passed from the binding</param>
		/// <param name="culture">Culture to convert with</param>
		/// <returns>A converted value with the type of <paramref name="targetType"/>.</returns>
		object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
	}
}

