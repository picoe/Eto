using System;
using System.Xml;
using System.Collections.Generic;

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
		
		public static T? GetEnumAttribute<T>(this XmlElement element, string name, bool ignoreCase = true)
			where T: struct
		{
			string val = element.GetAttribute(name);
			T result;
			if (!string.IsNullOrEmpty(val) && Enum.TryParse<T>(val, ignoreCase, out result)) return result;
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

		public static float? GetFloatAttribute(this XmlElement element, string name)
		{
			//return XmlExtensions.GetAttribute<float>(element, name, float.TryParse);
			string attr = element.GetAttribute(name);
			float result;
			if (!string.IsNullOrWhiteSpace(attr) && float.TryParse(attr, out result)) return result;
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
		
		
		public static void WriteChildListXml<T>(this XmlElement element, IEnumerable<T> list, string childElement, string listElement = null)
			where T: IXmlReadable
		{
			XmlElement listNode = (!string.IsNullOrEmpty (listElement)) ? element.OwnerDocument.CreateElement (listElement) : element;
			
			foreach (T child in list) {
				var childNode = element.OwnerDocument.CreateElement(childElement);
				child.WriteXml(childNode);
				listNode.AppendChild (childNode);
			}

			if (listNode != element && !listNode.IsEmpty) {
				element.AppendChild (listNode);
			}
		}
		
		public static void ReadChildListXml<T>(this XmlElement element, IList<T> list, CreateFromXml<T> create, string childElement, string listElement = null)
			where T: IXmlReadable
		{
			XmlNodeList childNodes = null;
			if (listElement != null) {
				var listNode = element.SelectSingleNode (listElement);
				if (listNode != null)
					childNodes = listNode.SelectNodes (childElement);
			}
			else 
				childNodes = element.SelectNodes (childElement);
			
			if (childNodes != null) {
				list.Clear ();
				foreach (XmlElement childNode in childNodes) {
					var item = create(childNode);
					item.ReadXml(childNode);
					list.Add (item);
				}
			}
		}
		
		public static void ReadChildListXml<T>(this XmlElement element, IList<T> list, string childElement, string listElement = null)
			where T: IXmlReadable, new()
		{
			ReadChildListXml<T>(element, list, delegate { return new T(); }, childElement, listElement);
		}
	}
}

