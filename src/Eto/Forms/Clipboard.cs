using System;
using Eto;
using Eto.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Reflection;

namespace Eto.Forms
{
	/// <summary>
	/// Object to handle the system clipboard. Use <see cref="Clipboard.Instance"/> to avoid creating multiple copies of this object.
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(Clipboard.IHandler))]
	public class Clipboard : Widget, IDataObject
	{
		new IHandler Handler => (IHandler)base.Handler;

		static readonly object Clipboard_Key = new object();

		/// <summary>
		/// Gets the shared clipboard instance
		/// </summary>
		/// <value>The clipboard instance.</value>
		public static Clipboard Instance => Platform.Instance?.GetSharedProperty(Clipboard_Key, () => new Clipboard());

		/// <summary>
		/// Gets the type id's for each type of data in the clipboard.
		/// </summary>
		/// <value>The content types in the clipboard.</value>
		public string[] Types => Handler.Types;

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Eto.Forms.Clipboard"/> contains a value for <see cref="Text"/>.
		/// </summary>
		/// <value><c>true</c> if the data object contains text; otherwise, <c>false</c>.</value>
		public bool ContainsText => Handler.ContainsText;

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Eto.Forms.Clipboard"/> contains a value for <see cref="Html"/>.
		/// </summary>
		/// <value><c>true</c> if the data object contains html; otherwise, <c>false</c>.</value>
		public bool ContainsHtml => Handler.ContainsHtml;

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Eto.Forms.Clipboard"/> contains a value for <see cref="Image"/>.
		/// </summary>
		/// <value><c>true</c> if the data object contains an image; otherwise, <c>false</c>.</value>
		public bool ContainsImage => Handler.ContainsImage;

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Eto.Forms.Clipboard"/> contains a value for <see cref="Uris"/>.
		/// </summary>
		/// <remarks>
		/// This can be a mix of both URL and File objects.  You can use <see cref="Uri.IsFile"/> to test for that.
		/// On some platforms, (e.g. windows), only a single URL can be retrieved or set.
		/// </remarks>
		/// <value><c>true</c> if the data object contains uris; otherwise, <c>false</c>.</value>
		public bool ContainsUris => Handler.ContainsUris;

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
				var ms = new MemoryStream();
				stream.CopyTo(ms);
				SetData(ms.ToArray(), type);
			}
		}

		/// <summary>
		/// sets a data array into the clipboard with the specified type identifier.
		/// </summary>
		/// <param name="value">Data to store in the clipboard.</param>
		/// <param name="type">Type identifier to store the data.</param>
		public void SetData(byte[] value, string type) => Handler.SetData(value, type);

		/// <summary>
		/// Gets a value indicating that data with the specified type is contained in the clipboard.
		/// </summary>
		/// <returns><c>true</c> if the clipboard contains the specified type, <c>false</c> otherwise.</returns>
		/// <param name="type">Type to test.</param>
		public bool Contains(string type) => Handler.Contains(type);

		/// <summary>
		/// Gets a data array from the clipboard with the specified type identifier.
		/// </summary>
		/// <returns>The data array, or null if not found in the clipboard.</returns>
		/// <seealso cref="SetData"/>
		/// <param name="type">Type identifier that was used to store the data.</param>
		public byte[] GetData(string type) => Handler.GetData(type);

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
		/// Sets a string into the clipboard with the specified type identifier.
		/// </summary>
		/// <remarks>
		/// This is useful when setting alternate string values into the clipboard that are not plain text.
		/// If you are storing plain text, use the <see cref="Text"/> property instead.
		/// </remarks>
		/// <seealso cref="GetString"/>
		/// <param name="value">Value to set in the clipboard.</param>
		/// <param name="type">Type identifier that was used to store the data.</param>
		public void SetString(string value, string type) => Handler.SetString(value, type);

		/// <summary>
		/// Gets a string from the clipboard with the specified type identifier.
		/// </summary>
		/// <returns>The string.</returns>
		/// <seealso cref="SetString"/>
		/// <param name="type">Type identifier that was used to store the data.</param>
		public string GetString(string type) => Handler.GetString(type);

		/// <summary>
		/// Gets or sets the plain text in the clipboard.
		/// </summary>
		/// <value>The plain text in the clipboard, or null if no plain text string in the clipboard.</value>
		public string Text
		{
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}

		/// <summary>
		/// Gets or sets html text in the clipboard.
		/// </summary>
		/// <value>The html value in the clipboard, or null if no html in the clipboard.</value>
		public string Html
		{
			get { return Handler.Html; }
			set { Handler.Html = value; }
		}

		/// <summary>
		/// Gets or sets an image in the clipboard.
		/// </summary>
		/// <value>The image in the clipboard, or null if no image is in the clipboard.</value>
		public Image Image
		{
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}

		/// <summary>
		/// Gets or sets the Uri's of the files in the clipboard.
		/// </summary>
		/// <value>The uris of the files, or null if no files are in the clipboard.</value>
		public Uri[] Uris
		{
			get { return Handler.Uris; }
			set { Handler.Uris = value; }
		}

		/// <summary>
		/// Clears the clipboard completely of all values
		/// </summary>
		public void Clear() => Handler.Clear();

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
		/// Handler interface for the <see cref="Clipboard"/>.
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

