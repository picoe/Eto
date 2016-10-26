using Eto.Drawing;
using Eto.Forms;
using Eto.Wpf.Drawing;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using sw = System.Windows;
using swm = System.Windows.Media;
using swmi = System.Windows.Media.Imaging;

namespace Eto.Wpf.Forms
{
	public class ClipboardHandler : WidgetHandler<object, Clipboard>, Clipboard.IHandler
	{
		public string[] Types
		{
			get { return sw.Clipboard.GetDataObject().GetFormats(); }
		}

		public void SetString(string value, string type)
		{
			if (string.IsNullOrEmpty(type))
				sw.Clipboard.SetText(value);
			else
				sw.Clipboard.SetData(type, value);
		}

		public void SetData(byte[] value, string type)
		{
			sw.Clipboard.SetData(type, value);
		}

		public string GetString(string type)
		{
			if (string.IsNullOrEmpty(type))
				return Text;
			return sw.Clipboard.ContainsData(type) ? Convert.ToString(sw.Clipboard.GetData(type)) : null;
		}

		public byte[] GetData(string type)
		{
			if (sw.Clipboard.ContainsData(type))
			{
				var data = sw.Clipboard.GetData(type);
				var bytes = data as byte[];
				if (bytes != null)
					return bytes;
				if (data != null)
				{
					var converter = TypeDescriptor.GetConverter(data.GetType());
					if (converter != null && converter.CanConvertTo(typeof(byte[])))
					{
						return converter.ConvertTo(data, typeof(byte[])) as byte[];
					}
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

		public string Text
		{
			get { return sw.Clipboard.ContainsText() ? sw.Clipboard.GetText() : null; }
			set { sw.Clipboard.SetText(value); }
		}

		public string Html
		{
			get { return sw.Clipboard.ContainsText(sw.TextDataFormat.Html) ? sw.Clipboard.GetText(sw.TextDataFormat.Html) : null; }
			set { sw.Clipboard.SetText(value, sw.TextDataFormat.Html); }
		}

		public Image Image
		{
			get { return sw.Clipboard.ContainsImage() ? new Bitmap(new BitmapHandler(sw.Clipboard.GetImage())) : null; }
			set
			{
				var dib = (value as Bitmap).ToDIB();
				if (dib != null)
				{
					// write a DIB here, so we can preserve transparency of the image
					sw.Clipboard.SetData(sw.DataFormats.Dib, dib);
					return;
				}

				sw.Clipboard.SetImage(value.ToWpf());
			}
		}

		public void Clear()
		{
			sw.Clipboard.Clear();
		}
	}
}
