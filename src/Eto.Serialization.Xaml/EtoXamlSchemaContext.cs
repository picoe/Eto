#if PORTABLE
using Portable.Xaml;
using Portable.Xaml.Markup;
#else
using System.Xaml;
using System.Windows.Markup;
#endif

namespace Eto.Serialization.Xaml
{
	class EtoXamlSchemaContext : XamlSchemaContext
	{
		public const string EtoFormsNamespace = "http://schema.picoe.ca/eto.forms";
		public const string DesignNamespace = "http://schema.picoe.ca/eto.forms/design";
		readonly Dictionary<Type, XamlType> typeCache = new Dictionary<Type, XamlType>();

		public bool DesignMode { get; set; }

		static readonly Assembly EtoAssembly = typeof(Platform).GetTypeInfo().Assembly;

		protected override XamlType GetXamlType(string xamlNamespace, string name, params XamlType[] typeArguments)
		{
			XamlType type = null;
			try
			{
				type = base.GetXamlType(xamlNamespace, name, typeArguments);
			}
			catch
			{
				if (!DesignMode || type != null)
					throw;
				// in designer mode, fail gracefully
				type = FindLoadedType(xamlNamespace, name, typeArguments)
					?? new EtoDesignerType(typeof(DesignerMarkupExtension), this) { TypeName = name, Namespace = xamlNamespace };
			}
			if (type == null && DesignMode)
				type = FindLoadedType(xamlNamespace, name, typeArguments);
			return type;
		}

		/// <summary>
		/// Finds a type from a clr-namespace without an assembly, which the designer can't otherwise
		/// resolve as there is no class being loaded to take the local assembly from.
		/// </summary>
		XamlType FindLoadedType(string xamlNamespace, string name, XamlType[] typeArguments)
		{
			const string clrNamespace = "clr-namespace:";
			if (typeArguments?.Length > 0
				|| xamlNamespace == null
				|| !xamlNamespace.StartsWith(clrNamespace, StringComparison.Ordinal)
				|| xamlNamespace.IndexOf(';') >= 0)
				return null;

			var fullName = xamlNamespace.Substring(clrNamespace.Length).Trim() + "." + name;
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly.IsDynamic)
					continue;
				try
				{
					var type = assembly.GetType(fullName, false);
					if (type != null)
						return GetXamlType(type);
				}
				catch
				{
					// the project's types can fail to load when one of their dependencies is missing
				}
			}
			return null;
		}

		public override XamlType GetXamlType(Type type)
		{
			XamlType xamlType;
			if (typeCache.TryGetValue(type, out xamlType))
				return xamlType;

			var info = type.GetTypeInfo();

			if (
				info.IsSubclassOf(typeof(Widget))
				|| info.Assembly == EtoAssembly // struct
				|| (
					// nullable struct
				    info.IsGenericType
				    && info.GetGenericTypeDefinition() == typeof(Nullable<>)
					&& Nullable.GetUnderlyingType(type).GetTypeInfo().Assembly == EtoAssembly
				))
			{
				xamlType = new EtoXamlType(type, this);
				typeCache.Add(type, xamlType);
				return xamlType;
			}
			return base.GetXamlType(type);
		}
		bool isInResourceMember;
		PropertyInfo resourceMember;

		internal bool IsResourceMember(PropertyInfo member)
		{
			if (member == null)
				return false;
			if (resourceMember == null)
			{
				if (isInResourceMember)
					return false;
				isInResourceMember = true;
				try
				{
					resourceMember = typeof(Widget).GetRuntimeProperty("Properties");
				}
				finally
				{
					isInResourceMember = false;
				}
			}

			return member.DeclaringType == resourceMember.DeclaringType
				   && member.Name == resourceMember.Name;
		}

		class PropertiesXamlMember : XamlMember
		{
			public PropertiesXamlMember(PropertyInfo propertyInfo, XamlSchemaContext context)
				: base(propertyInfo, context)
			{
			}

			protected override bool LookupIsAmbient() => true;
		}

		protected override XamlMember GetProperty(PropertyInfo propertyInfo)
		{
			if (IsResourceMember(propertyInfo))
			{
				return new PropertiesXamlMember(propertyInfo, this);
			}

			return base.GetProperty(propertyInfo);
		}

		protected override XamlMember GetEvent(EventInfo eventInfo)
		{
			if (DesignMode)
			{
				// in design mode, ignore wiring up events
				return new EmptyXamlMember(eventInfo, this);
			}

			return base.GetEvent(eventInfo);
		}
	}
}