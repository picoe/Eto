using System;
using System.Xml;

namespace Eto
{
	
	public interface IXmlReadable
	{
		void ReadXml(XmlElement element);
		void WriteXml(XmlElement element);
		
	}
	
	public delegate T CreateFromXml<T>(XmlElement element);
	
	public delegate bool XmlToValue<T>(string attribute, out T result);
	
	public static class XmlExtensions
	{
		public static bool? GetBoolAttribute(this XmlElement element, string name)
		{
			//return XmlExtensions.GetAttribute<bool>(element, name, bool.TryParse);
			string attr = element.GetAttribute(name);
			bool result;
			if (!string.IsNullOrWhiteSpace(attr) && bool.TryParse(attr, out result)) return result;
			return null;
		}

		public static int? GetIntAttribute(this XmlElement element, string name)
		{
			//return XmlExtensions.GetAttribute<int>(element, name, int.TryParse);
			string attr = element.GetAttribute(name);
			int result;
			if (!string.IsNullOrWhiteSpace(attr) && int.TryParse(attr, out result)) return result;
			return null;
		}
		
		public static T? GetAttribute<T>(this XmlElement element, string name, XmlToValue<T> translate)
			where T: struct
		{
			string attr = element.GetAttribute(name);
			T result;
			if (!string.IsNullOrEmpty(attr) && translate(attr, out result)) return result;
			return null;
		}
		
		public static void SetAttribute<T>(this XmlElement element, string name, T value)
		{
			string attrValue = Convert.ToString(value);
			if (!string.IsNullOrEmpty(attrValue))
				element.SetAttribute(name, attrValue);
		}
		
		public static void WriteChildXml<T>(this XmlElement element, string childElementName, T child)
			where T: IXmlReadable
		{
			if (child == null) return;
			var childElement = element.OwnerDocument.CreateElement(childElementName);
			child.WriteXml(childElement);
			element.AppendChild(childElement);
		}

		public static T ReadChildXml<T>(this XmlElement element, string childElementName)
			where T: IXmlReadable, new()
		{
			return ReadChildXml<T>(element, childElementName, delegate { return new T();});
		}
		
		public static T ReadChildXml<T>(this XmlElement element, string childElementName, CreateFromXml<T> create)
			where T: IXmlReadable
		{
			var childElement = element.SelectSingleNode(childElementName) as XmlElement;
			if (childElement == null) return default(T);
			var child = create(childElement);
			if (child != null) child.ReadXml(childElement);
			return child;
		}
		
		public static void ReadChildXml<T>(this XmlElement element, string childElementName, T child)
			where T: IXmlReadable
		{
			var childElement = element.SelectSingleNode(childElementName) as XmlElement;
			
			if (childElement != null) child.ReadXml(childElement);
		}
	}
}

