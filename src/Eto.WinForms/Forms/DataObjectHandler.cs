using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.WinForms.Drawing;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using sdi = System.Drawing.Imaging;
using sc = System.ComponentModel;
using System.Collections.Specialized;
using System.IO;

namespace Eto.WinForms.Forms
{
	public class DataObjectHandler : DataObjectHandler<DataObject, DataObject.ICallback>, DataObject.IHandler
	{
		public DataObjectHandler()
		{
			Control = new swf.DataObject(new DragDropLib.DataObject());
			IsExtended = true;
		}

		public DataObjectHandler(swf.IDataObject data)
		{
			IsExtended = data is DragDropLib.DataObject;
			Control = data as swf.DataObject ?? new swf.DataObject(data);
		}

		public override string[] Types => Control.GetFormats();

		protected override swf.IDataObject InnerDataObject => Control;

		protected override bool InnerContainsFileDropList => Control.ContainsFileDropList();

		protected override StringCollection InnerGetFileDropList() => Control.GetFileDropList();

		public override bool Contains(string type) => Control.GetDataPresent(type);

		public override void Clear()
		{
			Control = new swf.DataObject(new DragDropLib.DataObject());
			IsExtended = true;
		}

		public override bool ContainsText => Control.ContainsText();

		public override bool ContainsHtml => Control.ContainsText(swf.TextDataFormat.Html);

		protected override bool InnerContainsImage => Control.ContainsImage();

		protected override sd.Image InnerGetImage() => Control.GetImage();

		protected override object InnerGetData(string type) => Control.GetData(type);
	}

	public abstract class DataObjectHandler<TWidget, TCallback> : WidgetHandler<swf.DataObject, TWidget, TCallback>, IDataObject
		where TWidget: Widget, IDataObject
	{
		public const string UniformResourceLocatorW_Format = "UniformResourceLocatorW";
		public const string UniformResourceLocator_Format = "UniformResourceLocator";

		protected bool IsExtended { get; set; }

		protected virtual void Update()
		{
		}

		public virtual void SetData(byte[] value, string type)
		{
			Control.SetData(type, value);
			Update();
		}

		public virtual void SetString(string value, string type)
		{
			Control.SetData(type, value);
			Update();
		}

		public virtual string Html
		{
			set
			{
				Control.SetText(value ?? string.Empty, System.Windows.Forms.TextDataFormat.Html);
				Update();
			}
			get
			{
				return Control.ContainsText(swf.TextDataFormat.Html) ? Control.GetText(swf.TextDataFormat.Html) : null;
			}
		}

		public virtual string Text
		{
			set
			{
				if (IsExtended)
					swf.SwfDataObjectExtensions.SetDataEx(Control, swf.DataFormats.Text, value ?? string.Empty);
				else 
					Control.SetText(value ?? string.Empty);
				Update();
			}
			get
			{
				return Control.ContainsText() ? Control.GetText() : null;
			}
		}

		protected abstract bool InnerContainsImage { get; }
		protected abstract sd.Image InnerGetImage();


		public Image Image
		{
			get
			{
				if (InnerContainsImage && InnerGetImage() is sd.Bitmap bmp)
					return bmp.ToEto();
				if (Contains(swf.DataFormats.Dib) && InnerGetData(swf.DataFormats.Dib) is Stream stream)
					return Win32.FromDIB(stream);
				return null;
			}
			set
			{
				var dib = (value as Bitmap).ToDIB();
				if (dib != null)
				{
					// write a DIB here, so we can preserve transparency of the image
					Control.SetData(swf.DataFormats.Dib, dib);
				}
				else
					Control.SetImage(value.ToSD());

				Update();
			}
		}

		protected abstract swf.IDataObject InnerDataObject { get; }

		protected abstract bool InnerContainsFileDropList { get; }
		protected abstract StringCollection InnerGetFileDropList();


		protected abstract object InnerGetData(string type);

		public byte[] GetData(string type)
		{
			if (Contains(type))
			{
				return GetAsData(InnerGetData(type));
			}
			return null;
		}

		protected byte[] GetAsData(object data)
		{
			if (data is byte[] bytes)
				return bytes;
			if (data is string str)
				return Encoding.UTF8.GetBytes(str);
			if (data is MemoryStream ms)
				return ms.ToArray();
			if (data is Stream stream)
			{
				ms = new MemoryStream();
				stream.CopyTo(ms);
				ms.Position = 0;
				return ms.ToArray();
			}
			if (data != null)
			{
				var converter = sc.TypeDescriptor.GetConverter(data.GetType());
				if (converter != null && converter.CanConvertTo(typeof(byte[])))
				{
					return converter.ConvertTo(data, typeof(byte[])) as byte[];
				}
#pragma warning disable 618
				var etoConverter = TypeDescriptor.GetConverter(data.GetType());
				if (etoConverter != null && etoConverter.CanConvertTo(typeof(byte[])))
				{
					return etoConverter.ConvertTo(data, typeof(byte[])) as byte[];
				}
#pragma warning restore 618
			}
			if (data is IConvertible)
				return Convert.ChangeType(data, typeof(byte[])) as byte[];
			return null;
		}

		public string GetString(string type) => GetString(type, Encoding.UTF8);

		protected virtual string GetString(string type, Encoding encoding)
		{
			if (string.IsNullOrEmpty(type))
				return Text;
			if (!Contains(type))
				return null;
			return GetAsString(InnerGetData(type), encoding);
		}

		protected string GetAsString(object data, Encoding encoding)
		{
			if (data is string str)
				return str;
			if (data is MemoryStream ms)
				return encoding.GetString(ms.ToArray()).TrimEnd('\0'); // can contain a zero at the end, thanks windows.
			if (data != null)
			{
				var converter = sc.TypeDescriptor.GetConverter(data.GetType());
				if (converter != null && converter.CanConvertTo(typeof(string)))
				{
					return converter.ConvertTo(data, typeof(string)) as string;
				}
#pragma warning disable 618
				var etoConverter = TypeDescriptor.GetConverter(data.GetType());
				if (etoConverter != null && etoConverter.CanConvertTo(typeof(string)))
				{
					return etoConverter.ConvertTo(data, typeof(string)) as string;
				}
#pragma warning restore 618
			}
			if (data is IConvertible)
				return Convert.ChangeType(data, typeof(string)) as string;
			return null;
		}

		public abstract string[] Types { get; }

		public virtual Uri[] Uris
		{
			get
			{
				var list = InnerContainsFileDropList
					? InnerGetFileDropList().OfType<string>().Select(s => new Uri(s))
					: null;

				string urlString = null;
				if (Contains(UniformResourceLocatorW_Format))
					urlString = GetString(UniformResourceLocatorW_Format, Encoding.Unicode);
				else if (Contains(UniformResourceLocator_Format))
					urlString = GetString(UniformResourceLocator_Format);

				if (!string.IsNullOrEmpty(urlString) && Uri.TryCreate(urlString, UriKind.RelativeOrAbsolute, out var uri))
				{
					var uris = new[] { uri };
					list = list?.Concat(uris) ?? uris;
				}
				return list?.ToArray();
			}
			set
			{
				if (value != null)
				{
					var coll = value as IList<Uri> ?? value.ToList();

					// file uris
					var files = new StringCollection();
					files.AddRange(coll.Where(r => r.IsFile).Select(r => r.AbsolutePath).ToArray());
					if (files.Count > 0)
						Control.SetFileDropList(files);

					// web uris (windows only supports one)
					var url = coll.Where(r => !r.IsFile).FirstOrDefault();
					if (url != null)
					{
						SetString(url.ToString(), UniformResourceLocator_Format);
						SetString(url.ToString(), UniformResourceLocatorW_Format);
					}
				}
				else
				{
					Control.SetData(swf.DataFormats.FileDrop, null);
					Control.SetData(UniformResourceLocatorW_Format, null);
					Control.SetData(UniformResourceLocator_Format, null);
				}
				Update();
			}
		}

		public abstract bool ContainsText { get; }

		public abstract bool ContainsHtml { get; }

		public bool ContainsImage => InnerContainsImage || Contains(swf.DataFormats.Dib);

		public bool ContainsUris => InnerContainsFileDropList 
			|| Contains(UniformResourceLocatorW_Format)
			|| Contains(UniformResourceLocator_Format);

		public abstract void Clear();

		public abstract bool Contains(string type);

		public void SetObject(object value, string type) => Widget.SetObject(value, type);

		public T GetObject<T>(string type) => Widget.GetObject<T>(type);

		public bool TrySetObject(object value, string type) => false;

		public bool TryGetObject(string type, out object value)
		{
			value = null;
			return false;
		}
	}
}
