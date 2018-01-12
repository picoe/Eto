using Eto.Forms;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Globalization;

#if PORTABLE
using Portable.Xaml;
using Portable.Xaml.Markup;
#else
using System.Xaml;
using System.Windows.Markup;
#endif

namespace Eto.Serialization.Xaml.Extensions
{

	[MarkupExtensionReturnType(typeof(object))]
	public class BindingExtension : MarkupExtension
	{
		/// <summary>
		/// Gets or sets the path of the property to bind to
		/// </summary>
		/// <value>The path to the bound property</value>
		[ConstructorArgument("path")]
		public string Path { get; set; }

		/// <summary>
		/// Gets or sets the converter for the value of the binding.
		/// </summary>
		/// <value>The converter.</value>
		public IValueConverter Converter { get; set; }

		/// <summary>
		/// Gets or sets the mode of the binding for direct bindings.
		/// </summary>
		/// <value>The binding mode.</value>
		public DualBindingMode Mode { get; set; } = DualBindingMode.TwoWay;

		/// <summary>
		/// Gets or sets the culture to pass to the converter
		/// </summary>
		/// <value>The converter culture.</value>
		public CultureInfo ConverterCulture { get; set; }

		/// <summary>
		/// Gets or sets the parameter to pass to the <see cref="Converter"/> during conversion.
		/// </summary>
		/// <value>The converter parameter.</value>
		public object ConverterParameter { get; set; }

		public BindingExtension()
		{
		}

		public BindingExtension(string path)
		{
			Path = path;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var provideValue = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (provideValue == null)
				return null;
			var propertyInfo = provideValue.TargetProperty as PropertyInfo;
			if (propertyInfo == null)
				return null;

			// Indirect binding 
			var pti = propertyInfo.PropertyType.GetTypeInfo();
			if (pti.IsGenericType && pti.GetGenericTypeDefinition() == typeof(IIndirectBinding<>))
			{
				if (Converter != null)
				{
					var instance = Binding.Property<object>(Path);
					var convert = instance.GetType().GetRuntimeMethod("Convert", new[] { typeof(IValueConverter), typeof(object), typeof(CultureInfo) });
					var convertMethod = convert.MakeGenericMethod(pti.GenericTypeArguments);
					return convertMethod.Invoke(instance, new object[] { Converter, ConverterParameter, ConverterCulture });
				}

				var propertyBindingType = typeof(PropertyBinding<>).MakeGenericType(pti.GenericTypeArguments);
				return Activator.CreateInstance(propertyBindingType, Path, false);
			}

			// Direct binding
			var widget = provideValue.TargetObject as BindableWidget;
			var propertyType = propertyInfo.PropertyType;
			if (widget != null && !typeof(IBinding).GetTypeInfo().IsAssignableFrom(propertyType.GetTypeInfo()))
			{
				var sourceBinding = Binding.Property<object>(Path);
				if (Converter != null)
				{
					sourceBinding = sourceBinding.Convert(Converter, propertyType, ConverterParameter, ConverterCulture);
				}

				var binding = widget.BindDataContext(Binding.Property<object>(propertyInfo.Name), sourceBinding, Mode);

				binding.Mode = Mode;

				return propertyInfo.GetValue(provideValue.TargetObject, null);
			}

			if (provideValue.TargetObject == null)
				throw new InvalidOperationException("Target object cannot be null");

			throw new InvalidOperationException(string.Format("Type '{0}' is not bindable", provideValue.TargetObject.GetType()));
		}
	}
}