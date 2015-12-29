using Eto.Forms;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Linq;


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
		[ConstructorArgument("path")]
		public string Path { get; set; }

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
			if (provideValue != null)
			{
				var propertyInfo = provideValue.TargetProperty as PropertyInfo;
				if (propertyInfo != null)
				{
					var widget = provideValue.TargetObject as BindableWidget;
					var propertyType = propertyInfo.PropertyType;
					if (widget != null && !typeof(IBinding).GetTypeInfo().IsAssignableFrom(propertyType.GetTypeInfo()))
					{
						widget.BindDataContext<object>(propertyInfo.Name, Path, DualBindingMode.TwoWay);
						return propertyInfo.GetValue(provideValue.TargetObject, null);
					}

					if (provideValue.TargetObject == null)
						throw new InvalidOperationException("Target object cannot be null");

					throw new InvalidOperationException(string.Format("Type '{0}' is not bindable", provideValue.TargetObject.GetType()));
				}
			}
			return null;
		}
	}
}