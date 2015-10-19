using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections;
using Eto.Drawing;
using Portable.Xaml;

namespace Eto.Designer.Completion
{

	class TypeCompletion : Completion
	{
		public Assembly Assembly { get; set; }

		public string Namespace { get; set; }

		public override IEnumerable<CompletionItem> GetClasses(IEnumerable<string> path)
		{
			var prefix = Prefix ?? "";

			var types = Assembly.ExportedTypes.Where(r => !r.IsGenericType && !r.IsAbstract && r.Namespace == Namespace).ToList();

			Func<Type, bool> filter = null;
			var contentType = GetContentType(path.LastOrDefault(), types);
			if (contentType != null)
			{
				var converter = TypeDescriptor.GetConverter(contentType);
				if (converter != null)
				{
					filter = t => contentType.IsAssignableFrom(t) || converter.CanConvertFrom(t);
				}
				else
				{
					filter = contentType.IsAssignableFrom;
				}

				types.RemoveAll(r => !filter(r));

				// todo: move this out somehow
				foreach (var xt in XamlLanguage.AllTypes)
				{
					if (filter(xt.UnderlyingType))
						yield return new CompletionItem
						{
							Name = "x:" + xt.Name,
							Type = CompletionType.Class
						};
				}

				if (!contentType.IsAbstract && !types.Contains(contentType))
					types.Add(contentType);
			}

			foreach (var result in types)
			{
				yield return new CompletionItem
				{ 
					Name = prefix + result.Name, 
					Description = XmlComments.GetSummary(result),
					Type = CompletionType.Class
				}; 
			}
		}

		Type GetContentType(string last, List<Type> types)
		{
			if (string.IsNullOrEmpty(last))
				return null;
			last = Namespace + "." + last;
			var type = types.FirstOrDefault(r => r.FullName == last);
			if (type != null)
			{
				var contentProperty = type.GetCustomAttribute<ContentPropertyAttribute>();
				if (contentProperty != null)
				{
					var prop = type.GetProperty(contentProperty.Name);
					var propType = prop.PropertyType;
					if (typeof(IList).IsAssignableFrom(propType))
					{
						var list = propType.GetInterfaces().FirstOrDefault(r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(IList<>));
						if (list != null)
						{
							return list.GenericTypeArguments[0];
						}
					}

					return prop.PropertyType;
				}
			}
			return null;
		}

		public override IEnumerable<CompletionItem> GetProperties(string objectName, IEnumerable<string> path)
		{
			var fullName = Namespace + "." + objectName;
			var type = Assembly.GetType(fullName, false);
			if (type != null)
			{
				foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
				{
					if (prop.SetMethod == null || !prop.SetMethod.IsPublic)
						continue;

					if (prop.GetCustomAttribute<ObsoleteAttribute>() != null)
						continue;
					yield return new CompletionItem
					{ 
						Name = prop.Name,
						Description = XmlComments.GetSummary(prop),
						Type = CompletionType.Property 
					};
				}
				foreach (var evt in type.GetEvents(BindingFlags.Public | BindingFlags.Instance))
				{
					yield return new CompletionItem
					{ 
						Name = evt.Name,
						Description = XmlComments.GetSummary(evt),
						Type = CompletionType.Event 
					};
				}
			}
		}

		public override IEnumerable<CompletionItem> GetPropertyValues(string objectName, string propertyName, IEnumerable<string> path)
		{
			var fullName = Namespace + "." + objectName;
			var type = Assembly.GetType(fullName, false);
			if (type != null)
			{
				var prop = type.GetRuntimeProperty(propertyName);
				if (prop != null)
				{
					if (prop.PropertyType == typeof(bool))
					{
						yield return new CompletionItem { Type = CompletionType.Literal, Name = "True" };
						yield return new CompletionItem { Type = CompletionType.Literal, Name = "False" };
					}
					else if (prop.PropertyType == typeof(Color))
					{
						foreach (var col in typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public).Where(r => r.PropertyType == typeof(Color)))
						{
							yield return new CompletionItem { Type = CompletionType.Literal, Name = col.Name };
						}
						yield return new CompletionItem { Type = CompletionType.Literal, Name = "#FFFFFF" };
						yield return new CompletionItem { Type = CompletionType.Literal, Name = "#FFFFFFFF" };
					}
					else if (prop.PropertyType.IsEnum)
					{
						foreach (var name in Enum.GetNames(prop.PropertyType))
						{
							yield return new CompletionItem
							{ 
								Type = CompletionType.Literal,
								Name = name,
								Description = XmlComments.GetEnum(prop, name)
							};
						}
					}
				}
			}
		}
	}
}
