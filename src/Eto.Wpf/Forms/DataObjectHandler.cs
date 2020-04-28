using Eto.Forms;
using System;
using sc = System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sw = System.Windows;
using Eto.Drawing;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Media.Imaging;
using static System.Windows.WpfDataObjectExtensions;

namespace Eto.Wpf.Forms
{
	public class DataFormatsHandler : DataFormats.IHandler
	{
		public virtual string Text => sw.DataFormats.UnicodeText;
		public virtual string Html => sw.DataFormats.Html;
		public virtual string Color => "Color";
	}

	public class DataObjectHandler : DataObjectHandler<DataObject, DataObject.ICallback>
	{
		public override string[] Types => Control.GetFormats();

		public override bool ContainsText => Control.ContainsText();

		public override bool ContainsHtml => Control.ContainsText(sw.TextDataFormat.Html);

		protected override bool InnerContainsImage => Control.ContainsImage();

		protected override bool InnerContainsFileDropList => Control.ContainsFileDropList();

		public DataObjectHandler()
		{
			Control = new sw.DataObject(new DragDropLib.DataObject());
			IsExtended = true;
		}
		public DataObjectHandler(sw.IDataObject data)
			: base(data)
		{
		}

		public DataObjectHandler(sw.DataObject data)
			: base(data)
		{
		}

		protected override object InnerGetData(string type) => Control.GetData(type);

		public override void Clear()
		{
			if (IsExtended)
				Control = new sw.DataObject(new DragDropLib.DataObject());
			else
				Control = new sw.DataObject();
			Update();
		}

		protected override BitmapSource InnerGetImage() => Control.GetImage();

		protected override StringCollection InnerGetFileDropList() => Control.GetFileDropList();

		public override bool Contains(string type) => Control.GetDataPresent(type);
	}

	public abstract class DataObjectHandler<TWidget, TCallback> : WidgetHandler<sw.DataObject, TWidget, TCallback>, DataObject.IHandler
		where TWidget: Widget, IDataObject
	{
		protected bool IsExtended { get; set; }
		public const string UniformResourceLocatorW_Format = "UniformResourceLocatorW";
		public const string UniformResourceLocator_Format = "UniformResourceLocator";

		public DataObjectHandler()
		{
		}

		public DataObjectHandler(sw.IDataObject data)
		{
			IsExtended = data is DragDropLib.DataObject;
			Control = new sw.DataObject(data);
		}

		public DataObjectHandler(sw.DataObject data)
		{
			Control = data;
		}

		protected virtual void Update()
		{
		}

		public abstract string[] Types { get; }

		public abstract bool ContainsText { get; }

		public virtual string Text
		{
			get { return ContainsText ? Control.GetText() : null; }
			set
			{
				if (IsExtended)
					Control.SetDataEx(sw.DataFormats.UnicodeText, value);
				else
					Control.SetText(value);
				Update();
			}
		}

		public abstract bool ContainsHtml { get; }

		public virtual string Html
		{
			get { return ContainsHtml ? Control.GetText(sw.TextDataFormat.Html) : null; }
			set
			{
				if (IsExtended)
					Control.SetDataEx(sw.DataFormats.Html, value);
				else
					Control.SetText(value, sw.TextDataFormat.Html);
				Update();
			}
		}

		protected abstract bool InnerContainsImage { get; }
		protected abstract BitmapSource InnerGetImage();

		public bool ContainsImage => InnerContainsImage || Contains(sw.DataFormats.Dib);

		public Image Image
		{
			get
			{
				if (InnerContainsImage)
				{
					// clipboard returns true but the image returned is null sometimes.. hrmph.
					var img = InnerGetImage().ToEto();
					if (img != null)
						return img;
				}
				if (Contains(sw.DataFormats.Dib) && InnerGetData(sw.DataFormats.Dib) is Stream stream)
					return Win32.FromDIB(stream);
				return null;
			}
			set
			{
				var dib = (value as Bitmap).ToDIB();
				if (dib != null)
				{
					// write a DIB here, so we can preserve transparency of the image
					if (IsExtended)
						Control.SetDataEx(sw.DataFormats.Dib, dib);
					else
						Control.SetData(sw.DataFormats.Dib, dib);
				}
				else if (IsExtended)
					Control.SetDataEx(sw.DataFormats.Bitmap, value.ToWpf());
				else
					Control.SetImage(value.ToWpf());

				Update();
			}
		}

		protected abstract bool InnerContainsFileDropList { get; }

		protected abstract StringCollection InnerGetFileDropList();

		public bool ContainsUris => InnerContainsFileDropList
			|| Contains(UniformResourceLocatorW_Format)
			|| Contains(UniformResourceLocator_Format);

		public Uri[] Uris
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
					{
						if (IsExtended)
							Control.SetDataEx(sw.DataFormats.FileDrop, files.OfType<string>().ToArray());
						else
							Control.SetFileDropList(files);
					}

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
					if (IsExtended)
					{
						Control.SetDataEx(sw.DataFormats.FileDrop, null);
						Control.SetDataEx(UniformResourceLocatorW_Format, null);
						Control.SetDataEx(UniformResourceLocator_Format, null);
					}
					else
					{
						Control.SetData(sw.DataFormats.FileDrop, null);
						Control.SetData(UniformResourceLocatorW_Format, null);
						Control.SetData(UniformResourceLocator_Format, null);
					}
				}
				Update();
			}
		}

		public abstract void Clear();

		public abstract bool Contains(string type);

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

		protected string GetString(string type, Encoding encoding)
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

		public void SetData(byte[] value, string type)
		{
			if (IsExtended)
				Control.SetDataEx(type, value);
			else
				Control.SetData(type, value);
			Update();
		}

		public void SetString(string value, string type)
		{
			if (string.IsNullOrEmpty(type))
				Text = value;
			else if (IsExtended)
				Control.SetDataEx(type, value);
			else
				Control.SetData(type, value);

			Update();
		}

		public void SetDragImage(Bitmap bitmap, PointF offset)
		{
			sw.WpfDataObjectExtensions.SetDragImage(Control, bitmap.ToWpf(), offset.ToWpf());
		}

		public bool TrySetObject(object value, string type)
		{
			return false;
		}

		public bool TryGetObject(string type, out object value)
		{
			value = null;
			return false;
		}

		public void SetObject(object value, string type) => Widget.SetObject(value, type);

		public T GetObject<T>(string type) => Widget.GetObject<T>(type);
	}
}
