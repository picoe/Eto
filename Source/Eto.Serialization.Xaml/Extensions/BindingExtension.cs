using Eto.Forms;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
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
				var widget = provideValue.TargetObject as BindableWidget;
				var propertyInfo = provideValue.TargetProperty as PropertyInfo;
				if (propertyInfo != null)
				{
					widget.BindDataContext<object>(propertyInfo.Name, Path, DualBindingMode.TwoWay);
					return propertyInfo.GetValue(provideValue.TargetObject, null);
				}
			}
			return null;
		}
	}
}