using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xaml;
using System.Windows.Markup;
using System.Linq;

namespace Eto.Xaml
{
	
	public class EtoXamlType : XamlType
	{
		public EtoXamlType(Type underlyingType, XamlSchemaContext schemaContext)
			: base(underlyingType, schemaContext)
		{
		}

		T GetCustomAttribute<T>(bool inherit = true)
		{
			return UnderlyingType.GetCustomAttributes(typeof(T), true).OfType<T>().FirstOrDefault();
		}

		protected override XamlMember LookupContentProperty()
		{
			var contentAttribute = GetCustomAttribute<ContentPropertyAttribute>();
			if (contentAttribute == null || contentAttribute.Name == null)
				return base.LookupContentProperty();
			return GetMember(contentAttribute.Name);
		}

		protected override XamlMember LookupAliasedProperty(XamlDirective directive)
		{
			if (directive == XamlLanguage.Name)
			{
				// mono doesn't support the name attribute yet (throws null exception)
				if (!EtoEnvironment.Platform.IsMono)
				{
					var nameAttribute = GetCustomAttribute<RuntimeNamePropertyAttribute>();
					if (nameAttribute != null && nameAttribute.Name != null)
						return GetMember(nameAttribute.Name);
				}
			}
			return base.LookupAliasedProperty(directive);
		}
	}
}