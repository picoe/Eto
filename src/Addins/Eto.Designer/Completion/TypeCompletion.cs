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
using Eto.Forms;

namespace Eto.Designer.Completion
{
	class TypeCompletion : Completion
	{
		public Assembly Assembly { get; set; }

		public string Namespace { get; set; }

		List<Type> exportedTypes;

		List<Type> GetExportedTypes()
		{
			if (exportedTypes == null)
				exportedTypes = Assembly.ExportedTypes.Where(r => !r.IsGenericType && !r.IsAbstract && r.Namespace == Namespace).ToList();
			return exportedTypes;
		}

		public override Func<Type, bool> GetFilter(IEnumerable<string> path)
		{
			string propertyName;
			var nodeType = GetNodeType(path.LastOrDefault(), out propertyName);
			var contentType = GetContentType(nodeType, propertyName);
			if (contentType != null)
			{
				var converter = System.ComponentModel.TypeDescriptor.GetConverter(contentType);
				if (converter != null)
				{
					return t => contentType.IsAssignableFrom(t) || converter.CanConvertFrom(t);
				}
				else
				{
					return contentType.IsAssignableFrom;
				}
			}
			return null;
		}

		public override IEnumerable<CompletionItem> GetClasses(IEnumerable<string> path, Func<Type, bool> filter)
		{
			var prefixWithColon = PrefixWithColon;

			string contentProperty;
			var nodeType = GetNodeType(path.LastOrDefault(), out contentProperty);
			var contentType = GetContentType(nodeType, contentProperty);

			var types = GetExportedTypes();
			if (filter != null)
			{
				foreach (var result in types.Where(filter))
				{
					// special case, don't allow windows as a child control
					if (contentType != null
					    && typeof(Window).IsAssignableFrom(result))
						continue;
						

					yield return new CompletionItem
					{ 
						Name = prefixWithColon + result.Name, 
						Description = XmlComments.GetSummary(result),
						Type = CompletionType.Class
					}; 
				}
			}

			if (contentType != null 
				&& !contentType.IsAbstract 
				&& !types.Contains(contentType)
				&& contentType.Assembly == Assembly)
				yield return new CompletionItem
				{ 
					Name = prefixWithColon + contentType.Name, 
					Description = XmlComments.GetSummary(contentType),
					Type = CompletionType.Class
				}; 
			if (nodeType != null)
			{
				yield return new CompletionItem
				{
					Name = prefixWithColon + nodeType.Name + ".",
					Description = "Add a property tag",
					Type = CompletionType.Property
				};
			}

		}

		Type GetNodeType(string last, out string propertyName)
		{
			propertyName = null;
			var prefix = PrefixWithColon;
			if (!string.IsNullOrEmpty(prefix))
			{
				if (!last.StartsWith(prefix))
					return null;
				last = last.Substring(prefix.Length);
			}
			else if (last.Contains(':'))
				return null;

			if (string.IsNullOrEmpty(last))
				return null;
			var dotIndex = last.IndexOf('.');
			if (dotIndex > 0)
			{
				propertyName = last.Substring(dotIndex + 1);
				last = last.Substring(0, dotIndex);
			}
			last = Namespace + "." + last;
			var types = GetExportedTypes();
			return types.FirstOrDefault(r => r.FullName == last);
		}

		Type GetContentType(Type type, string propertyName)
		{
			if (type != null)
			{
				if (string.IsNullOrEmpty(propertyName))
				{
					var contentProperty = type.GetCustomAttribute<ContentPropertyAttribute>();
					if (contentProperty != null)
					{
						propertyName = contentProperty.Name;
					}
				}
				if (!string.IsNullOrEmpty(propertyName))
				{
					var prop = type.GetProperty(propertyName);
					if (prop != null)
					{
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
			}
			return null;
		}

		public override bool? HasContent(string objectName, IEnumerable<string> path)
		{
			var prefix = PrefixWithColon;
			if (prefix != null)
			{
				if (!objectName.StartsWith(prefix))
					return false;
				objectName = objectName.Substring(prefix.Length);
			}

			var fullName = Namespace + "." + objectName;
			var type = Assembly.GetType(fullName, false);
			if (type != null)
			{
				return type.GetTypeInfo().GetCustomAttribute<Eto.ContentPropertyAttribute>() != null;
			}
			return base.HasContent(objectName, path);
		}

		public override IEnumerable<CompletionItem> GetProperties(string objectName, IEnumerable<string> path)
		{
			var prefix = PrefixWithColon;
			if (prefix != null)
			{
				if (!objectName.StartsWith(prefix))
					yield break;
				objectName = objectName.Substring(prefix.Length);
			}
				
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
			var prefix = PrefixWithColon;
			if (prefix != null)
			{
				if (!objectName.StartsWith(prefix))
					yield break;
				objectName = objectName.Substring(prefix.Length);
			}

			var fullName = Namespace + "." + objectName;
			var type = Assembly.GetType(fullName, false);
			if (type != null)
			{
				var prop = type.GetRuntimeProperty(propertyName);
				if (prop != null)
				{
					var propertyType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
					if (propertyType == typeof(bool))
					{
						yield return new CompletionItem { Type = CompletionType.Literal, Name = "True" };
						yield return new CompletionItem { Type = CompletionType.Literal, Name = "False" };
					}
					else if (propertyType == typeof(Color))
					{
						foreach (var col in typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public).Where(r => r.PropertyType == typeof(Color)))
						{
							yield return new CompletionItem { Type = CompletionType.Literal, Name = col.Name };
						}
						yield return new CompletionItem { Type = CompletionType.Literal, Name = "#FFFFFF" };
						yield return new CompletionItem { Type = CompletionType.Literal, Name = "#FFFFFFFF" };
					}
					else if (propertyType.IsEnum)
					{
						foreach (var name in Enum.GetNames(propertyType))
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
