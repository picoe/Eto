#if XML
using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace Eto
{
	/// <summary>
	/// Interface to declare an object that can serialize to/from xml
	/// </summary>
	/// <remarks>
	/// This interface is useful when you are reading/writing objects to xml
	/// manually by using <see cref="XmlExtensions"/>.
	/// 
	/// There are methods to load/save a collection of child objects and singular
	/// child objects when you implement this interface which makes reading/writing to xml
	/// super easy.
	/// </remarks>
	public interface IXmlReadable
	{
		/// <summary>
		/// Reads/deserializes the xml element into the object
		/// </summary>
		/// <param name="element">Element that represents the object</param>
		void ReadXml(XmlElement element);

		/// <summary>
		/// Writes/serializes the object into the xml element
		/// </summary>
		/// <param name="element">Element that will represent the object</param>
		void WriteXml(XmlElement element);
	}
	
	/// <summary>
	/// Delegate to create the specified object from an XmlElement
	/// </summary>
	/// <remarks>
	/// This is used for certain <see cref="XmlExtensions"/> to create child objects when reading xml elements.
	/// 
	/// The implementors of this delegate typically do not need to read the object from XML, just create the
	/// instance of the object based on certain criteria (e.g. a type or ID)
	/// </remarks>
	/// <typeparam name="T">Type of object to create based on the element</typeparam>
	/// <param name="element">Element to create the object from</param>
	/// <returns>A new instance of the specified type for the element</returns>
	public delegate T CreateFromXml<T>(XmlElement element);
	
	/// <summary>
	/// Delegate to translate an attribute value to the specified type
	/// </summary>
	/// <typeparam name="T">Type to translate to</typeparam>
	/// <param name="attribute">Attribute value to translate from</param>
	/// <param name="result">Resulting value from the attribute type</param>
	/// <returns>True if the translation was sucessful, false otherwise</returns>
	public delegate bool XmlToValue<T>(string attribute, out T result);
	
	/// <summary>
	/// Extensions for reading/writing xml values
	/// </summary>
	public static class XmlExtensions
	{
		/// <summary>
		/// Gets a string attribute value from the specified element
		/// </summary>
		/// <remarks>
		/// This differs from the regular <see cref="XmlElement.GetAttribute(string)"/> in that if the string is empty
		/// it will return null.
		/// </remarks>
		/// <param name="element">Element to read the attribute</param>
		/// <param name="name">Name of the attribute to read</param>
		/// <returns>A string value of the attribute, or null if it is empty or null</returns>
		public static string GetStringAttribute(this XmlElement element, string name)
		{
			string attr = element.GetAttribute(name);
			if (string.IsNullOrEmpty(attr)) return null;
			else return attr;
		}
		
		/// <summary>
		/// Gets a boolean attribute value from the specified element.  The value should be 'true', 'false' or empty.
		/// </summary>
		/// <remarks>
		/// This uses <see cref="Boolean.TryParse"/> to parse the value.
		/// </remarks>
		/// <param name="element">Element to read the attribute from</param>
		/// <param name="name">Name of the attribute</param>
		/// <returns>True or False if the value is a valid boolean value, null otherwise</returns>
		public static bool? GetBoolAttribute(this XmlElement element, string name)
		{
			string attr = element.GetAttribute(name);
			bool result;
			if (!string.IsNullOrWhiteSpace(attr) && bool.TryParse(attr, out result)) return result;
			return null;
		}

		/// <summary>
		/// Gets a enumeration attribute value from the specified element.
		/// </summary>
		/// <remarks>
		/// This uses <see cref="Enum.TryParse{T}(string, out T)"/> to parse the value
		/// </remarks>
		/// <typeparam name="T">Type of enumeration to read</typeparam>
		/// <param name="element">Element to read the attribute value from</param>
		/// <param name="name">Name of the attribute value</param>
		/// <param name="ignoreCase">True to ignore case when parsing, false to be case sensitive</param>
		/// <returns>Value of the parsed enumeration, or null if it cannot be parsed</returns>
		public static T? GetEnumAttribute<T>(this XmlElement element, string name, bool ignoreCase = true)
			where T: struct
		{
			string val = element.GetAttribute(name);
			T result;
			if (!string.IsNullOrEmpty(val) && Enum.TryParse<T>(val, ignoreCase, out result)) return result;
			return null;
		}

		/// <summary>
		/// Gets an integer attribute value from the specified element
		/// </summary>
		/// <remarks>
		/// This uses <see cref="Int32.TryParse(string, out Int32)"/> to parse the value.
		/// </remarks>
		/// <param name="element">Element to read the attribute from</param>
		/// <param name="name">Name of the attribute</param>
		/// <returns>Integer value of the attribute, or null if it is invalid or missing</returns>
		public static int? GetIntAttribute (this XmlElement element, string name)
		{
			string attr = element.GetAttribute(name);
			int result;
			if (!string.IsNullOrWhiteSpace(attr) && int.TryParse(attr, out result)) return result;
			return null;
		}

		/// <summary>
		/// Gets a float attribute value from the specified element
		/// </summary>
		/// <remarks>
		/// This uses <see cref="float.TryParse(string, out float)"/> to parse the value.
		/// </remarks>
		/// <param name="element">Element to read the attribute from</param>
		/// <param name="name">Name of the attribute</param>
		/// <returns>Float value of the attribute, or null if it is invalid or missing</returns>
		public static float? GetFloatAttribute (this XmlElement element, string name)
		{
			string attr = element.GetAttribute(name);
			float result;
			if (!string.IsNullOrWhiteSpace(attr) && float.TryParse(attr, out result)) return result;
			return null;
		}

		/// <summary>
		/// Gets a translated value of an attribute of the specified element
		/// </summary>
		/// <param name="element">Element to read the attribute from</param>
		/// <param name="name">Name of the attribute</param>
		/// <param name="translate">Delegate used to translate the string value to the desired type</param>
		/// <returns>Value returned by the translate delegate, or null if the translate delegate returned false</returns>
		public static T? GetAttribute<T> (this XmlElement element, string name, XmlToValue<T> translate)
			where T: struct
		{
			string attr = element.GetAttribute(name);
			T result;
			if (!string.IsNullOrEmpty(attr) && translate(attr, out result)) return result;
			return null;
		}
		
		/// <summary>
		/// Sets an attribute of the specified element to a value
		/// </summary>
		/// <remarks>
		/// This uses <see cref="Convert.ToString(object)"/> to translate the value to a string attribute value.
		/// </remarks>
		/// <typeparam name="T">Type of the value to set (usually inferred so you don't have to set it)</typeparam>
		/// <param name="element">Element to set the attribute value</param>
		/// <param name="name">Name of the attribute to set</param>
		/// <param name="value">Value to set</param>
		public static void SetAttribute<T>(this XmlElement element, string name, T value)
		{
			string attrValue = Convert.ToString(value, CultureInfo.InvariantCulture);
			if (!string.IsNullOrEmpty(attrValue))
				element.SetAttribute(name, attrValue);
		}
		
		/// <summary>
		/// Adds a new element as a child to the specified element, if the child value is not null
		/// </summary>
		/// <remarks>
		/// This allows you to easily write child objects to xml, when they implement the <see cref="IXmlReadable"/> interface.
		/// </remarks>
		/// <seealso cref="ReadChildXml{T}(XmlElement, string)"/>
		/// <example>
		/// <code><![CDATA[
		/// public class MyChild : IXmlReadable {
		///		// implement IXmlReadable
		/// }
		/// 
		/// public class MyParent : IXmlReadable {
		/// 
		///		public MyChild Child { get; set; }
		///		
		///		public void WriteXml (XmlElement element) {
		///			element.WriteChildXml("mychild", this.Child);
		///		}
		///		
		///		public void ReadXml (XmlElement element) {
		///			this.Child = element.ReadChildXml<MyChild>("mychild");
		///		}
		/// }
		/// ]]></code>
		/// </example>
		/// <typeparam name="T">Type of child to write (must be <see cref="IXmlReadable"/>)</typeparam>
		/// <param name="element">Element to add the child to</param>
		/// <param name="childElementName">Name of the child element to create</param>
		/// <param name="child">Child value to translate to a new child element</param>
		public static void WriteChildXml<T>(this XmlElement element, string childElementName, T child)
			where T: IXmlReadable
		{
			if (child == null) return;
			var childElement = element.OwnerDocument.CreateElement(childElementName);
			child.WriteXml(childElement);
			element.AppendChild(childElement);
		}

		/// <summary>
		/// Reads a single child xml element from the specified element as a given type
		/// </summary>
		/// <remarks>
		/// This is useful for reading a (singular) child element of an xml element into an object that implements
		/// <see cref="IXmlReadable"/>.
		/// 
		/// If you want to use a class that requires certain parameters for construction or to create a different derived type
		/// based on certain attributes of the child xml element, use <see cref="ReadChildXml{T}(XmlElement, string, CreateFromXml{T})"/> instead.
		/// </remarks>
		/// <seealso cref="WriteChildXml{T}(XmlElement, string, T)"/>
		/// <seealso cref="ReadChildXml{T}(XmlElement, string, CreateFromXml{T})"/>
		/// <example>
		/// <code><![CDATA[
		/// public class MyChild : IXmlReadable {
		///		// implement IXmlReadable
		/// }
		/// 
		/// public class MyParent : IXmlReadable {
		/// 
		///		public MyChild Child { get; set; }
		///		
		///		public void WriteXml (XmlElement element) {
		///			element.WriteChildXml("mychild", this.Child);
		///		}
		///		
		///		public void ReadXml (XmlElement element) {
		///			this.Child = element.ReadChildXml<MyChild>("mychild");
		///		}
		/// }
		/// ]]></code>
		/// </example>
		/// <typeparam name="T">Type of child to read (must be <see cref="IXmlReadable"/>)</typeparam>
		/// <param name="element">Element to read the child node from</param>
		/// <param name="childElementName">Name of the child element to read</param>
		/// <returns>A new instance of the specified type if the child element exists with properties read from xml, otherwise null</returns>
		public static T ReadChildXml<T>(this XmlElement element, string childElementName)
			where T: IXmlReadable, new()
		{
			return ReadChildXml<T>(element, childElementName, delegate { return new T();});
		}

		/// <summary>
		/// Reads a single child xml element from the specified element as a given type, constructing the child programatically
		/// </summary>
		/// <remarks>
		/// This is useful for reading a (singular) child element of an xml element into an object that implements
		/// <see cref="IXmlReadable"/>.  This also gives you a way to create the instance used for the child object
		/// programatically.
		/// 
		/// If your child class does not require special construction logic and has a default constructor, you can use <see cref="ReadChildXml{T}(XmlElement, string)"/> instead.
		/// </remarks>
		/// <seealso cref="WriteChildXml{T}(XmlElement, string, T)"/>
		/// <seealso cref="ReadChildXml{T}(XmlElement, string)"/>
		/// <example>
		/// <code><![CDATA[
		/// public class MyChild : IXmlReadable {
		///		public static MyChild CreateFromXml (XmlElement element) {
		///			switch (element.GetAttribute("type")) {
		///				case "typea": return new MyChildTypeA();
		///				case "typeb": return new MyChildTypeB();
		///				default: return null;
		///			}
		///		}
		///		
		///		// implement IXmlReadable (usually as virtual methods for derived classes to override)
		/// }
		/// 
		/// public class MyChildTypeA : MyChild {
		/// }
		/// 
		/// public class MyChildTypeB : MyChild {
		/// }
		/// 
		/// public class MyParent : IXmlReadable {
		/// 
		///		public MyChild Child { get; set; }
		///		
		///		public void WriteXml (XmlElement element) {
		///			element.WriteChildXml("mychild", this.Child);
		///		}
		///		
		///		public void ReadXml (XmlElement element) {
		///			this.Child = element.ReadChildXml<MyChild>("mychild", MyChild.CreateFromXml);
		///		}
		/// }
		/// ]]></code>
		/// </example>
		/// <typeparam name="T">Type of child to read (must be <see cref="IXmlReadable"/>)</typeparam>
		/// <param name="element">Element to read the child node from</param>
		/// <param name="childElementName">Name of the child element to read</param>
		/// <param name="create">Delegate to create the child object instance if needed</param>
		/// <returns>A new instance of the specified type if the child element exists with properties read from xml, otherwise null</returns>
		public static T ReadChildXml<T> (this XmlElement element, string childElementName, CreateFromXml<T> create)
			where T: IXmlReadable
		{
			var childElement = element.SelectSingleNode(childElementName) as XmlElement;
			if (childElement == null) return default(T);
			var child = create(childElement);
			if (child != null) child.ReadXml(childElement);
			return child;
		}

		/// <summary>
		/// Reads a single child xml element from the specified element as a given type, constructing the child programatically
		/// </summary>
		/// <remarks>
		/// This is useful for reading a (singular) child element of an xml element into an object that implements
		/// <see cref="IXmlReadable"/>.  This also gives you a way to create the instance used for the child object
		/// programatically.
		/// 
		/// If your child class does not require special construction logic and has a default constructor, you can use <see cref="ReadChildXml{T}(XmlElement, string)"/> instead.
		/// </remarks>
		/// <seealso cref="WriteChildXml{T}(XmlElement, string, T)"/>
		/// <seealso cref="ReadChildXml{T}(XmlElement, string)"/>
		/// <example>
		/// <code><![CDATA[
		/// public class MyChild : IXmlReadable {
		///		// implement IXmlReadable
		/// }
		/// 
		/// public class MyParent : IXmlReadable {
		/// 
		///		public MyChild Child { get; set; }
		///		
		///		public void WriteXml (XmlElement element) {
		///			element.WriteChildXml("mychild", this.Child);
		///		}
		///		
		///		public void ReadXml (XmlElement element) {
		///			this.Child = new MyChild();
		///			element.ReadChildXml<MyChild>("mychild", this.Child);
		///		}
		/// }
		/// ]]></code>
		/// </example>
		/// <typeparam name="T">Type of child to read (must be <see cref="IXmlReadable"/>)</typeparam>
		/// <param name="element">Element to read the child node from</param>
		/// <param name="childElementName">Name of the child element to read</param>
		/// <param name="child">Instance of the child object to read the XML into</param>
		/// <returns>A new instance of the specified type if the child element exists with properties read from xml, otherwise null</returns>
		public static void ReadChildXml<T> (this XmlElement element, string childElementName, T child)
			where T: IXmlReadable
		{
			var childElement = element.SelectSingleNode(childElementName) as XmlElement;
			
			if (childElement != null) child.ReadXml(childElement);
		}
		
		/// <summary>
		/// Writes a list of <see cref="IXmlReadable"/> objects as child elements of the specified element, with an optional child list element
		/// </summary>
		/// <remarks>
		/// This extension is useful for writing lists of objects to xml. With this, you can write the list as direct
		/// children of the specified element, or to insert an additional list element using the <paramref name="listElement"/> parameter.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// public class MyChild : IXmlReadable {
		///		// implement IXmlReadable
		/// }
		/// 
		/// public class MyParent : IXmlReadable {
		/// 
		///		public List<MyChild> Children { get; set; }
		///		
		///		public void WriteXml (XmlElement element) {
		///			element.WriteChildListXml<MyChild>(this.Children, "mychild", "children");
		///		}
		///		
		///		public void ReadXml (XmlElement element) {
		///			this.Children = new List<MyChild>();
		///			element.ReadChildListXml<MyChild>(this.Children, "mychild", "children");
		///		}
		/// }
		/// ]]></code>
		/// </example>
		/// <typeparam name="T">Type of items in the list of elements to write</typeparam>
		/// <param name="element">Element to write the child elements to</param>
		/// <param name="list">List of objects to serialize to xml</param>
		/// <param name="childElement">Name of each child element to create</param>
		/// <param name="listElement">Name of the list element to contain the child elements, or null to add the child elements directly to the specified <paramref name="element"/></param>
		public static void WriteChildListXml<T>(this XmlElement element, IEnumerable<T> list, string childElement, string listElement = null)
			where T: IXmlReadable
		{
			XmlElement listNode = (!string.IsNullOrEmpty (listElement)) ? element.OwnerDocument.CreateElement (listElement) : element;
			
			foreach (T child in list) {
				if (child != null) {
					var childNode = element.OwnerDocument.CreateElement(childElement);
					child.WriteXml(childNode);
					listNode.AppendChild (childNode);
				}
			}

			if (listNode != element && !listNode.IsEmpty) {
				element.AppendChild (listNode);
			}
		}

		/// <summary>
		/// Reads child elements into a list, constructing the child objects programatically
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// public class MyChild : IXmlReadable {
		///		public static MyChild CreateFromXml (XmlElement element) {
		///			switch (element.GetAttribute("type")) {
		///				case "typea": return new MyChildTypeA();
		///				case "typeb": return new MyChildTypeB();
		///				default: return null;
		///			}
		///		}
		///		
		///		// implement IXmlReadable (usually as virtual methods for derived classes to override)
		/// }
		/// 
		/// public class MyChildTypeA : MyChild {
		/// }
		/// 
		/// public class MyChildTypeB : MyChild {
		/// }
		/// 
		/// public class MyParent : IXmlReadable {
		/// 
		///		public List<MyChild> Children { get; set; }
		///		
		///		public void WriteXml (XmlElement element) {
		///			element.WriteChildListXml<MyChild>(this.Children, "mychild", "children");
		///		}
		///		
		///		public void ReadXml (XmlElement element) {
		///			this.Children = new List<MyChild>();
		///			element.ReadChildListXml<MyChild>(this.Children, MyChild.CreateFromXml, "mychild", "children");
		///		}
		/// }
		/// ]]></code>
		/// </example>
		/// <typeparam name="T">Type of each child object</typeparam>
		/// <param name="element">Element to read the child elements from</param>
		/// <param name="list">List to add the child elements to</param>
		/// <param name="create">Delegate to create the child object to add to the list</param>
		/// <param name="childElement">Name of the child elements to read</param>
		/// <param name="listElement">If specified, the list element where the child elements are to be read from, or null to read the child elements directly from the <paramref name="element"/></param>
		public static void ReadChildListXml<T> (this XmlElement element, IList<T> list, CreateFromXml<T> create, string childElement, string listElement = null)
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
					if (item != null) {
						item.ReadXml(childNode);
						list.Add (item);
					}
				}
			}
		}

		/// <summary>
		/// Reads child elements into a list
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// public class MyChild : IXmlReadable {
		///		// implement IXmlReadable
		/// }
		/// 
		/// public class MyParent : IXmlReadable {
		/// 
		///		public List<MyChild> Children { get; set; }
		///		
		///		public void WriteXml (XmlElement element) {
		///			element.WriteChildListXml<MyChild>(this.Children, "mychild", "children");
		///		}
		///		
		///		public void ReadXml (XmlElement element) {
		///			this.Children = new List<MyChild>();
		///			element.ReadChildListXml<MyChild>(this.Children, "mychild", "children");
		///		}
		/// }
		/// ]]></code>
		/// </example>
		/// <typeparam name="T">Type of each child object</typeparam>
		/// <param name="element">Element to read the child elements from</param>
		/// <param name="list">List to add the child elements to</param>
		/// <param name="childElement">Name of the child elements to read</param>
		/// <param name="listElement">If specified, the list element where the child elements are to be read from, or null to read the child elements directly from the <paramref name="element"/></param>
		public static void ReadChildListXml<T> (this XmlElement element, IList<T> list, string childElement, string listElement = null)
			where T: IXmlReadable, new()
		{
			ReadChildListXml<T>(element, list, delegate { return new T(); }, childElement, listElement);
		}

		/// <summary>
		/// Saves the specified <see cref="IXmlReadable"/> object to an xml file
		/// </summary>
		/// <param name="obj">Object to serialize to xml</param>
		/// <param name="fileName">File to save as</param>
		/// <param name="documentElementName">Document element name</param>
		public static void SaveXml (this IXmlReadable obj, string fileName, string documentElementName = "object")
		{
			using (var stream = new MemoryStream ()) {
				SaveXml(obj, stream, documentElementName);
				stream.Position = 0;
				using (var fileStream = File.Create (fileName)) {
					stream.WriteTo (fileStream);
				}
			}
		}

		/// <summary>
		/// Saves the specified <see cref="IXmlReadable"/> object to an xml stream
		/// </summary>
		/// <param name="obj">Object to serialize to xml</param>
		/// <param name="stream">Stream to save as</param>
		/// <param name="documentElementName">Document element name</param>
		public static void SaveXml (this IXmlReadable obj, Stream stream, string documentElementName = "object")
		{
			var doc = new XmlDocument ();
			var topNode = doc.CreateElement (documentElementName);
			obj.WriteXml (topNode);
			doc.AppendChild (topNode);
			doc.Save (stream);
		}

		/// <summary>
		/// Loads the specified <see cref="IXmlReadable"/> object from an xml file
		/// </summary>
		/// <param name="obj">Object to serialize from xml</param>
		/// <param name="fileName">File to load from</param>
		public static void LoadXml (this IXmlReadable obj, string fileName)
		{
			using (var fileStream = File.OpenRead (fileName)) {
				LoadXml (obj, fileStream);
			}
		}

		/// <summary>
		/// Loads the specified <see cref="IXmlReadable"/> object from an xml stream
		/// </summary>
		/// <param name="obj">Object to serialize from xml</param>
		/// <param name="stream">Stream to load from</param>
		public static void LoadXml (this IXmlReadable obj, Stream stream)
		{
			var doc = new XmlDocument();
			doc.Load (stream);
			obj.ReadXml (doc.DocumentElement);
		}
	}
}
#endif