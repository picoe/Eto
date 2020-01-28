using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using Portable.Xaml.Markup;
using Portable.Xaml;

namespace Eto.Serialization.Xaml
{
	class DesignerUserControl : Panel
	{
		readonly Label label;
		public string Text
		{
			get { return label.Text; }
			set { label.Text = value; }
		}

		public new string ToolTip
		{
			get { return label.ToolTip; }
			set { label.ToolTip = value; }
		}

		public object GenericProperty { get; set; }

		public DesignerUserControl()
		{
			BackgroundColor = Colors.White;
			Padding = new Padding(20);
			Content = label = new Label
			{
				VerticalAlignment = VerticalAlignment.Center,
				TextAlignment = TextAlignment.Center,
				Font = SystemFonts.Default(8),
				Text = "[User Control]"
			};
		}
	}

	class DesignerMarkupExtension : MarkupExtension
	{
		public string Text { get; set; }
		public string ToolTip { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var xscp = serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;
			if (xscp == null)
				return null;

			var sc = xscp.SchemaContext;

			var provideValue = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

			var propertyInfo = provideValue?.TargetProperty as PropertyInfo;
			if (propertyInfo != null)
			{
				if (typeof(Control).IsAssignableFrom(propertyInfo.PropertyType))
					return new DesignerUserControl { Text = Text, ToolTip = ToolTip };
				return propertyInfo.GetValue(provideValue.TargetObject, null);
			}

			var targetObject = provideValue.TargetObject;

			if (targetObject != null)
			{
				var coltype = sc.GetXamlType(targetObject.GetType());
				if (coltype != null)
				{
					var ct = sc.GetXamlType(typeof(Control));
					if (ct.CanConvertTo(coltype.ItemType) || coltype.ItemType.CanConvertFrom(ct))
					{
						return new DesignerUserControl { Text = Text, ToolTip = ToolTip };
					}
				}
			}

			return null;
		}
	}
}