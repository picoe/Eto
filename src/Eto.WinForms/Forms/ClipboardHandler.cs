using System;
using Eto.Forms;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using sdi = System.Drawing.Imaging;
using Eto.WinForms.Drawing;
using Eto.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using sc = System.ComponentModel;
using System.Text;
using System.Collections.Specialized;
using System.Linq;

namespace Eto.WinForms.Forms
{
	public class ClipboardHandler : WidgetHandler<swf.DataObject, Clipboard>, Clipboard.IHandler
	{
		public ClipboardHandler()
		{
			Control = new swf.DataObject();
		}

		void Update()
		{
			swf.Clipboard.SetDataObject(Control);
		}

		public void SetData(byte[] value, string type)
		{
			Control.SetData(type, value);
			Update();
		}

		public void SetString(string value, string type)
		{
			Control.SetData(type, value);
			Update();
		}

		public string Html
		{
			set
			{
				Control.SetText(value ?? string.Empty, swf.TextDataFormat.Html);
				Update();
			}
			get
			{
				return swf.Clipboard.ContainsText(swf.TextDataFormat.Html) ? swf.Clipboard.GetText(swf.TextDataFormat.Html)?.TrimEnd('\0') : null;
			}
		}

		public string Text
		{
			set
			{
				Control.SetText(value ?? string.Empty);
				Update();
			}
			get
			{
				return swf.Clipboard.ContainsText() ? swf.Clipboard.GetText() : null;
			}
		}

		public Image Image
		{
			set
			{
				var dib = (value as Bitmap)?.ToDIB();
				if (dib != null)
					Control.SetData(swf.DataFormats.Dib, dib);
				else
					Control.SetImage(value.ToSD());
				Update();
			}
			get
			{
				Image result = null;

				try
				{
					var sdimage = GetImageFromClipboard() as sd.Bitmap;

					if (sdimage != null)
					{
						var handler = new BitmapHandler(sdimage);

						result = new Bitmap(handler);
					}
				}
				catch
				{
				}

				return result;
			}
		}

		/// <summary>
		/// see http://stackoverflow.com/questions/11273669/how-to-paste-a-transparent-image-from-the-clipboard-in-a-c-sharp-winforms-app
		/// </summary>
		static sd.Image GetImageFromClipboard()
		{
			if (swf.Clipboard.GetDataObject() == null)
				return null;

			if (swf.Clipboard.GetDataObject().GetDataPresent(swf.DataFormats.Dib))
			{
				var dib = ((System.IO.MemoryStream)swf.Clipboard.GetData(swf.DataFormats.Dib)).ToArray();

				var width = BitConverter.ToInt32(dib, 4);
				var height = BitConverter.ToInt32(dib, 8);
				var bpp = BitConverter.ToInt16(dib, 14);

				if (bpp == 32)
				{
					var gch = GCHandle.Alloc(dib, GCHandleType.Pinned);

					sd.Bitmap bmp = null;

					try
					{
						var ptr = new IntPtr((long)gch.AddrOfPinnedObject() + 40);

						bmp = new sd.Bitmap(width, height, width * 4, sdi.PixelFormat.Format32bppArgb, ptr);

						var result = new sd.Bitmap(bmp);

						// Images are rotated and flipped for some reason.
						// This rotates them back.
						result.RotateFlip(sd.RotateFlipType.Rotate180FlipX);

						return result;
					}
					finally
					{
						gch.Free();

						if (bmp != null)
							bmp.Dispose();
					}
				}
			}
			if (swf.Clipboard.ContainsFileDropList())
			{
				var list = swf.Clipboard.GetFileDropList();
				if (list != null && list.Count > 0)
				{
					var path = list[0];
					sd.Image bmp = null;
					try
					{
						bmp = sd.Image.FromFile(path);
						var result = new sd.Bitmap(bmp);
						return result;
					}
					catch
					{
					}
					finally
					{
						if (bmp != null)
							bmp.Dispose();
					}
				}
			}

			return swf.Clipboard.ContainsImage() ? swf.Clipboard.GetImage() : null;
		}

		public byte[] GetData(string type)
		{
			if (swf.Clipboard.ContainsData(type))
			{
				var data = swf.Clipboard.GetData(type);
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
			if (swf.Clipboard.ContainsData(type))
				return swf.Clipboard.GetData(type) as string;
			return null;
		}

		public string[] Types => swf.Clipboard.GetDataObject()?.GetFormats();

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

		public DataObject DataObject
		{
			get => swf.Clipboard.GetDataObject().ToEto();
			set
			{
				Control = value.ToSwf();
				Update();
			}
		}

		public void Clear()
		{
			swf.Clipboard.Clear();
			Control = new swf.DataObject();
		}
	}
}

