using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Eto.Forms
{
	/// <summary>
	/// Interface to allow you to share common code with <see cref="Clipboard"/> and <see cref="DataObject"/>.
	/// </summary>
	public interface IDataObject
	{
		/// <summary>
		/// Gets the type id's for each type of data in the data object.
		/// </summary>
		/// <value>The content types in the data object.</value>
		string[] Types { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Eto.Forms.IDataObject"/> contains a value for <see cref="Text"/>.
		/// </summary>
		/// <value><c>true</c> if the data object contains text; otherwise, <c>false</c>.</value>
		bool ContainsText { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Eto.Forms.IDataObject"/> contains a value for <see cref="Html"/>.
		/// </summary>
		/// <value><c>true</c> if the data object contains html; otherwise, <c>false</c>.</value>
		bool ContainsHtml { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Eto.Forms.IDataObject"/> contains a value for <see cref="Image"/>.
		/// </summary>
		/// <value><c>true</c> if the data object contains an image; otherwise, <c>false</c>.</value>
		bool ContainsImage { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Eto.Forms.IDataObject"/> contains a value for <see cref="Uris"/>.
		/// </summary>
		/// <remarks>
		/// This can be a mix of both URL and File objects.  You can use <see cref="Uri.IsFile"/> to test for that.
		/// On some platforms, (e.g. windows), only a single URL can be retrieved or set.
		/// </remarks>
		/// <value><c>true</c> if the data object contains uris; otherwise, <c>false</c>.</value>
		bool ContainsUris { get; }

		/// <summary>
		/// Sets a string into the data object with the specified type identifier.
		/// </summary>
		/// <remarks>
		/// This is useful when setting alternate string values into the data object that are not plain text.
		/// If you are storing plain text, use the <see cref="Text"/> property instead.
		/// </remarks>
		/// <seealso cref="GetString"/>
		/// <param name="value">Value to set in the data object.</param>
		/// <param name="type">Type identifier that was used to store the data.</param>
		void SetString(string value, string type);

		/// <summary>
		/// sets a data array into the data object with the specified type identifier.
		/// </summary>
		/// <param name="value">Data to store in the data object.</param>
		/// <param name="type">Type identifier to store the data.</param>
		void SetData(byte[] value, string type);

		/// <summary>
		/// Gets a string from the data object with the specified type identifier.
		/// </summary>
		/// <returns>The string.</returns>
		/// <seealso cref="SetString"/>
		/// <param name="type">Type identifier that was used to store the data.</param>
		string GetString(string type);

		/// <summary>
		/// Gets a value indicating that data with the specified type is contained in the data object.
		/// </summary>
		/// <returns><c>true</c> if the data object contains the specified type, <c>false</c> otherwise.</returns>
		/// <param name="type">Type to test.</param>
		bool Contains(string type);

		/// <summary>
		/// Gets a data array from the data object with the specified type identifier.
		/// </summary>
		/// <returns>The data array, or null if not found in the data object.</returns>
		/// <seealso cref="SetData"/>
		/// <param name="type">Type identifier that was used to store the data.</param>
		byte[] GetData(string type);


		/// <summary>
		/// Gets or sets the plain text in the data object.
		/// </summary>
		/// <value>The plain text in the data object, or null if no plain text string in the data object.</value>
		string Text { get; set; }

		/// <summary>
		/// Gets or sets html text in the data object.
		/// </summary>
		/// <value>The html value in the data object, or null if no html in the data object.</value>
		string Html { get; set; }

		/// <summary>
		/// Gets or sets an image in the data object.
		/// </summary>
		/// <value>The image in the data object, or null if no image is in the data object.</value>
		Image Image { get; set; }

		/// <summary>
		/// Gets or sets the Uri's of the files in the data object.
		/// </summary>
		/// <remarks>
		/// This can be a mix of both URL and File objects.  You can use <see cref="Uri.IsFile"/> to test for that.
		/// On some platforms, (e.g. windows), only a single URL can be retrieved or set.
		/// </remarks>
		/// <value>The uris of the files, or null if no files are in the data object.</value>
		Uri[] Uris { get; set; }

		/// <summary>
		/// Clears the data object completely of all values
		/// </summary>
		void Clear();

		/// <summary>
		/// Sets the <paramref name="value"/> into the data object with the specified <paramref name="type"/> using serialization or type converter
		/// </summary>
		/// <remarks>
		/// The object specified must be serializable or have a type converter to convert to a string.
		/// </remarks>
		/// <param name="value">Serializable value to set as a value in the data object</param>
		/// <param name="type">Type identifier to set the value for</param>
		void SetObject(object value, string type);

		/// <summary>
		/// Gets an object from the data object with the specified type
		/// </summary>
		/// <remarks>
		/// This is useful when you know the type of object, and it is serializable or has a type converter to convert from string.
		/// If it cannot be converted it will return the default value.
		/// </remarks>
		/// <typeparam name="T">Type of the object to get</typeparam>
		/// <param name="type">Type identifier to get from the data object</param>
		/// <returns>An instance of the object to recieve, or the default value.</returns>
		T GetObject<T>(string type);
	}

	/// <summary>
	/// Drag/Drop action data.
	/// </summary>
	[Handler(typeof(IHandler))]
	public class DataObject : Widget, IDataObject
	{
		new IHandler Handler => (IHandler)base.Handler;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.DataObject"/> class.
		/// </summary>
		public DataObject()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.DataObject"/> class.
		/// </summary>
		/// <param name="handler">Handler implementation.</param>
		public DataObject(IHandler handler) : base(handler)
		{
		}

		/// <summary>
		/// Gets the types of data in the data object to be used with <see cref="GetString"/> and <see cref="GetData"/>
		/// </summary>
		/// <value>The types.</value>
		public string[] Types => Handler.Types;


		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Eto.Forms.DataObject"/> contains a value for <see cref="Text"/>.
		/// </summary>
		/// <value><c>true</c> if the data object contains text; otherwise, <c>false</c>.</value>
		public bool ContainsText => Handler.ContainsText;

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Eto.Forms.DataObject"/> contains a value for <see cref="Html"/>.
		/// </summary>
		/// <value><c>true</c> if the data object contains html; otherwise, <c>false</c>.</value>
		public bool ContainsHtml => Handler.ContainsHtml;

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Eto.Forms.DataObject"/> contains a value for <see cref="Image"/>.
		/// </summary>
		/// <value><c>true</c> if the data object contains an image; otherwise, <c>false</c>.</value>
		public bool ContainsImage => Handler.ContainsImage;

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Eto.Forms.DataObject"/> contains a value for <see cref="Uris"/>.
		/// </summary>
		/// <remarks>
		/// This can be a mix of both URL and File objects.  You can use <see cref="Uri.IsFile"/> to test for that.
		/// On some platforms, (e.g. windows), only a single URL can be retrieved or set.
		/// </remarks>
		/// <value><c>true</c> if the data object contains uris; otherwise, <c>false</c>.</value>
		public bool ContainsUris => Handler.ContainsUris;

		/// <summary>
		/// Sets a string into the data object with the specified custom type.
		/// </summary>
		/// <param name="value">Value to store.</param>
		/// <param name="type">Type of the data.</param>
		public void SetString(string value, string type) => Handler.SetString(value, type);

		/// <summary>
		/// Sets a data stream into the clipboard with the specified type identifier.
		/// </summary>
		/// <param name="stream">Stream to store in the clipboard.</param>
		/// <param name="type">Type identifier when retrieving the stream.</param>
		public void SetDataStream(Stream stream, string type)
		{
			if (stream.CanSeek)
			{
				var buffer = new byte[stream.Length];
				if (stream.Position != 0)
					stream.Seek(0, SeekOrigin.Begin);
				stream.Read(buffer, 0, buffer.Length);
				SetData(buffer, type);
			}
			else
			{
				using (var ms = new MemoryStream())
				{
					stream.CopyTo(ms);
					SetData(ms.ToArray(), type);
				}
			}
		}

		/// <summary>
		/// Sets the <paramref name="value"/> into the data object with the specified <paramref name="type"/> using serialization or type converter
		/// </summary>
		/// <remarks>
		/// The object specified must be serializable or have a type converter to convert to a string.
		/// </remarks>
		/// <param name="value">Serializable value to set as a value in the data object</param>
		/// <param name="type">Type identifier to set the value for</param>
		public void SetObject(object value, string type)
		{
			if (Handler.TrySetObject(value, type))
				return;

			if (value == null)
				return;

			var baseType = value.GetType();
			baseType = Nullable.GetUnderlyingType(baseType) ?? baseType;

			if (baseType.GetTypeInfo().IsSerializable)
			{
				using (var ms = new MemoryStream())
				{
					var binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(ms, value);
					SetDataStream(ms, type);
					return;
				}
			}
			var converter = System.ComponentModel.TypeDescriptor.GetConverter(baseType);
			if (converter != null && converter.CanConvertTo(typeof(string)))
			{
				SetString(converter.ConvertToString(value), type);
				return;
			}
			throw new InvalidOperationException("T must be serializable or convertable to string");
		}

		/// <summary>
		/// Gets an object from the data object with the specified type
		/// </summary>
		/// <remarks>
		/// This is useful when you know the type of object, and it is serializable or has a type converter to convert from string.
		/// If it cannot be converted it will return the default value.
		/// </remarks>
		/// <typeparam name="T">Type of the object to get</typeparam>
		/// <param name="type">Type identifier to get from the data object</param>
		/// <returns>An instance of the object to recieve, or the default value.</returns>
		public T GetObject<T>(string type)
		{
			if (Handler.TryGetObject(type, out var obj) && obj is T handlerValue)
				return handlerValue;

			var baseType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

			try
			{
				if (baseType.GetTypeInfo().IsSerializable && GetObject(type) is T value)
				{
					return value;
				}

				var converter = System.ComponentModel.TypeDescriptor.GetConverter(baseType);
				if (converter?.CanConvertFrom(typeof(string)) == true)
				{
					return (T)converter.ConvertFromString(GetString(type));
				}
			}
			catch (Exception ex)
			{
				// log error in debug
				Debug.WriteLine(ex);
			}
			return default;
		}

		/// <summary>
		/// Gets a serialized value with the specified <paramref name="type"/> identifier.
		/// </summary>
		/// <param name="type">type identifier to get the value for.</param>
		/// <returns>Value of the object if deserializable, otherwise null.</returns>
		public object GetObject(string type)
		{
			if (Handler.TryGetObject(type, out var value))
				return value;

			var stream = GetDataStream(type);
			if (stream == null)
				return null;
			try
			{
				var binaryFormatter = new BinaryFormatter();
				return binaryFormatter.Deserialize(stream);
			}
			catch (Exception ex)
			{
				// log error in debug
				Debug.WriteLine(ex);
				return null;
			}
		}

		/// <summary>
		/// Sets a data array for the specified type identifier.
		/// </summary>
		/// <param name="value">byte array to set.</param>
		/// <param name="type">Type identifier.</param>
		public void SetData(byte[] value, string type) => Handler.SetData(value, type);

		/// <summary>
		/// Gets a value indicating that data with the specified type is contained in the data object.
		/// </summary>
		/// <returns><c>true</c> if the data object contains the specified type, <c>false</c> otherwise.</returns>
		/// <param name="type">Type to test.</param>
		public bool Contains(string type) => Handler.Contains(type);

		/// <summary>
		/// Gets a string from the data object with the specified type identifier.
		/// </summary>
		/// <returns>The string value, or null if it does not exist in the data object.</returns>
		/// <param name="type">Type identifier for the string.</param>
		public string GetString(string type) => Handler.GetString(type);

		/// <summary>
		/// Gets the data stream with the specified type identifier.
		/// </summary>
		/// <returns>The data stream if found, or null otherwise.</returns>
		/// <seealso cref="SetDataStream"/>
		/// <param name="type">Type identifier that was used to store the data.</param>
		public Stream GetDataStream(string type)
		{
			var buffer = GetData(type);
			return buffer == null ? null : new MemoryStream(buffer, false);
		}

		/// <summary>
		/// Gets a byte array from the data object with the specified type identifier.
		/// </summary>
		/// <returns>The byte data.</returns>
		/// <param name="type">Type identifier to get the data for.</param>
		public byte[] GetData(string type) => Handler.GetData(type);

		/// <summary>
		/// Gets or sets the plain text in the data object.
		/// </summary>
		/// <value>The plain text in the data object, or null if the data object contains no text.</value>
		public string Text
		{
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}

		/// <summary>
		/// Gets or sets html text in the data object.
		/// </summary>
		/// <value>The html value in the data object, or null if the data object contains no html.</value>
		public string Html
		{
			get { return Handler.Html; }
			set { Handler.Html = value; }
		}

		/// <summary>
		/// Gets or sets an image in the data object.
		/// </summary>
		/// <value>The image in the data object, or null if the data object contains no image.</value>
		public Image Image
		{
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}

		/// <summary>
		/// Gets or sets the Uri's of the files in the data object
		/// </summary>
		/// <value>The uris of the files, or null if no files are in the data object.</value>
		public Uri[] Uris
		{
			get { return Handler.Uris; }
			set { Handler.Uris = value; }
		}

		/// <summary>
		/// Clears the data object
		/// </summary>
		public void Clear() => Handler.Clear();

		/// <summary>
		/// Handler interface for platform implementations of <see cref="DataObject"/>
		/// </summary>
		public new interface IHandler : Widget.IHandler, IDataObject
		{
			/// <summary>
			/// Attempts to set the specified object to the clipboard in a native-supplied way
			/// </summary>
			/// <remarks>
			/// This is used so native handlers can set certain objects in a particular way.
			/// For example, on macOS, setting a Color object for <see cref="DataFormats.Color"/> will use native API to set the value.
			/// </remarks>
			/// <param name="value">Value to set</param>
			/// <param name="type">Data format type</param>
			/// <returns>true if the native handler set the value, or false to fallback to serialization or conversion to string</returns>
			bool TrySetObject(object value, string type);

			/// <summary>
			/// Attempts to get the specified value from the clipboard in a native-supplied way
			/// </summary>
			/// <param name="type">Data format type to get the value</param>
			/// <param name="value">Value returned</param>
			/// <returns>True if the value was returned, false otherwise</returns>
			bool TryGetObject(string type, out object value);
		}
	}
}
