using System;
using Eto;
using Eto.Drawing;
using System.IO;

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
		new IHandler Handler { get { return (IHandler)base.Handler; } }

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
		/// Handler interface for the <see cref="Clipboard"/>.
		/// </summary>
		public new interface IHandler : Widget.IHandler, IDataObject
		{
		}
	}
}

