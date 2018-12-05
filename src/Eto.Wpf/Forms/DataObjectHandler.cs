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

namespace Eto.Wpf.Forms
{
	public class DataObjectHandler : WidgetHandler<sw.DataObject, DataObject, DataObject.ICallback>, DataObject.IHandler
	{
		public DataObjectHandler()
		{
			Control = new sw.DataObject();
		}

		public DataObjectHandler(sw.IDataObject data)
		{
			Control = new sw.DataObject(data);
		}

		public string[] Types => Control.GetFormats();

		public string Text
		{
			get { return Control.ContainsText() ? Control.GetText() : null; }
			set { Control.SetText(value); }
		}

		public string Html
		{
			get { return Control.ContainsText(sw.TextDataFormat.Html) ? Control.GetText(sw.TextDataFormat.Html) : null; }
			set { Control.SetText(value, sw.TextDataFormat.Html); }
		}

		public Image Image
		{
			get { return Control.ContainsImage() ? Control.GetImage().ToEto() : null; }
			set
			{
				var dib = (value as Bitmap).ToDIB();
				if (dib != null)
				{
					// write a DIB here, so we can preserve transparency of the image
					Control.SetData(sw.DataFormats.Dib, dib);
					return;
				}

				Control.SetImage(value.ToWpf());
			}
		}

		public Uri[] Uris
		{
			get
			{
				return Control.ContainsFileDropList() ? Control.GetFileDropList().OfType<string>().Select(s => new Uri(s)).ToArray() : null;
			}
			set
			{
				if (value != null)
				{
					var files = new StringCollection();
					files.AddRange(value.Select(r => r.AbsolutePath).ToArray());
					Control.SetFileDropList(files);
				}
				else
				{
					Control.SetFileDropList(null);
				}
			}
		}

		public void Clear()
		{
			Control = new sw.DataObject();
		}

		public byte[] GetData(string type)
		{
			if (Control.GetDataPresent(type))
			{
				var data = Control.GetData(type);
				var bytes = data as byte[];
				if (bytes != null)
					return bytes;
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
				if (data is string)
				{
					return Encoding.UTF8.GetBytes(data as string);
				}
				if (data is IConvertible)
				{
					return Convert.ChangeType(data, typeof(byte[])) as byte[];
				}
			}
			return null;
		}

		public string GetString(string type)
		{
			if (string.IsNullOrEmpty(type))
				return Text;
			return Control.GetDataPresent(type) ? Convert.ToString(Control.GetData(type)) : null;
		}

		public void SetData(byte[] value, string type)
		{
			Control.SetData(type, value);
		}

		public void SetString(string value, string type)
		{
			if (string.IsNullOrEmpty(type))
				Control.SetText(value);
			else
				Control.SetData(type, value);
		}
	}
}
