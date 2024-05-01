using Eto.Mac.Drawing;
namespace Eto.Mac.Forms
{
	public class DataFormatsHandler : DataFormats.IHandler
	{
		public string Text => NSPasteboard.NSPasteboardTypeString;

		public string Html => NSPasteboard.NSPasteboardTypeHTML;

		public string Color => NSPasteboard.NSPasteboardTypeColor;
	}

	public interface IDataObjectHandler
	{
		void Apply(NSPasteboard pasteboard);
		IEnumerable<NSObject> GetPasteboardItems();
	}

	public class DataObjectHandler : DataObjectHandler<DataObject, DataObject.ICallback>, DataObject.IHandler
	{
		internal static Class[] UriTypes = { new Class(typeof(NSUrl)) };
		internal static IntPtr selIsFileReferenceURLHandle = Selector.GetHandle("isFileReferenceURL");

		public DataObjectHandler(NSPasteboard item)
		{
			Control = item;
		}
	}

	public abstract class DataObjectHandler<TWidget, TCallback> : WidgetHandler<NSPasteboard, TWidget, TCallback>, IDataObject
		where TWidget : Widget, IDataObject
		where TCallback : Widget.ICallback
	{
		nint _changeCount;

		public override IntPtr NativeHandle => Control.Handle;

		void ClearIfNeeded()
		{
			if (Control.ChangeCount != _changeCount)
				_changeCount = Control.ClearContents();
		}

		public void SetData(byte[] value, string type)
		{
			ClearIfNeeded();
			Control.SetDataForType(NSData.FromArray(value), type);
		}

		public string Html
		{
			set
			{
				ClearIfNeeded();
				Control.SetStringForType(value, NSPasteboard.NSPasteboardTypeHTML);
			}
			get
			{
				return Control.GetStringForType(NSPasteboard.NSPasteboardTypeHTML);
			}
		}

		public void SetString(string value, string type)
		{
			ClearIfNeeded();
			Control.SetStringForType(value, type);
		}

		public string Text
		{
			set
			{
				ClearIfNeeded();
				Control.SetStringForType(value, NSPasteboard.NSPasteboardTypeString);
			}
			get
			{
				return Control.GetStringForType(NSPasteboard.NSPasteboardTypeString);
			}
		}

		public Image Image
		{
			set
			{
				ClearIfNeeded();
				var handler = value.Handler as IImageHandler;
				if (handler != null)
				{
					var data = handler.GetImage().AsTiff();
					Control.SetDataForType(data, NSPasteboard.NSPasteboardTypeTIFF);
				}
			}
			get
			{
				var oldFail = Class.ThrowOnInitFailure;
				Class.ThrowOnInitFailure = false;
				var image = new NSImage(Control);
				Class.ThrowOnInitFailure = oldFail;
				if (image.Handle == IntPtr.Zero)
					return null;
				
				if (image.Representations().Length > 1)
					return new Icon(new IconHandler(image));
				else
					return new Bitmap(new BitmapHandler(image));
			}
		}

		public byte[] GetData(string type)
		{
			var availableType = Control.GetAvailableTypeFromArray(new string[] { type });

			if (availableType != null)
			{
				var data = Control.GetDataForType(availableType);
				if (data == null || data.Bytes == IntPtr.Zero)
					return null;
				var bytes = new byte[data.Length];
				Marshal.Copy(data.Bytes, bytes, 0, (int)data.Length);
				return bytes;
			}
			return null;
		}

		public string GetString(string type)
		{
			return Control.GetStringForType(type);
		}


		public string[] Types => Control.Types;

		static Uri UrlToUri(NSUrl url)
		{
			// why is this not mapped??
			if (Messaging.bool_objc_msgSend(url.Handle, DataObjectHandler.selIsFileReferenceURLHandle))
				return url.FilePathUrl;
			return url;
		}

		public Uri[] Uris
		{
			get
			{
				// 10.6+ file list:
				if (Control.CanReadObjectForClasses(DataObjectHandler.UriTypes, null))
				{
					var objs = Control.ReadObjectsForClasses(DataObjectHandler.UriTypes, null);
					if (objs != null)
						return objs.OfType<NSUrl>().Select(UrlToUri).ToArray();
				}

				var availableType = Control.GetAvailableTypeFromArray(new string[] { NSPasteboard.NSFilenamesType });
				if (availableType != null)
				{
					// old-school way of passing files
					var files = Control.GetPropertyListForType(availableType) as NSArray;

					if (files != null)
					{
						var fileUris = new Uri[files.Count];
						for (int i = 0; i < fileUris.Length; i++)
						{
							var str = Runtime.GetNSObject<NSString>(files.ValueAt((uint)i));
							fileUris[i] = new Uri(str);
						}
						return fileUris;
					}
				}
				return null;
			}
			set
			{
				ClearIfNeeded();
				var files = value?.Select(r => (NSUrl)r).ToArray();
				Control.WriteObjects(files);
			}
		}

		public bool ContainsText => Control.CanReadItemWithDataConformingToTypes(new[] { NSPasteboard.NSPasteboardTypeString });

		public bool ContainsHtml => Control.CanReadItemWithDataConformingToTypes(new[] { NSPasteboard.NSPasteboardTypeHTML });

		public bool ContainsImage => Control.CanReadItemWithDataConformingToTypes(new[] { NSPasteboard.NSPasteboardTypePNG, NSPasteboard.NSPasteboardTypeTIFF, NSPasteboard.NSPasteboardTypePDF });

		public bool ContainsUris => Control.CanReadObjectForClasses(DataObjectHandler.UriTypes, null)
			|| Control.GetAvailableTypeFromArray(new string[] { NSPasteboard.NSFilenamesType }) != null;

		public void Clear()
		{
			_changeCount = Control.ClearContents();
		}

		public bool Contains(string type)
		{
			return Control.GetAvailableTypeFromArray(new[] { type }) != null;
		}

		public bool TryGetObject(string type, Type objectType, out object value)
		{
			if (objectType == null || objectType == typeof(Color))
			{
				if (type == NSPasteboard.NSPasteboardTypeColor)
				{
					value = NSColor.FromPasteboard(Control)?.ToEto();
					return true;
				}
			}
			if (objectType == null || objectType == typeof(string))
			{
				if (type == NSPasteboard.NSPasteboardTypeString
					|| type == NSPasteboard.NSPasteboardTypeTabularText
					|| type == NSPasteboard.NSPasteboardTypeUrl
					|| type == NSPasteboard.NSPasteboardTypeFileUrl
					|| type == NSPasteboard.NSPasteboardTypeRTF
					|| type == NSPasteboard.NSPasteboardTypeHTML)
				{
					value = GetString(type);
					return true;
				}
			}
			if (objectType == null || objectType == typeof(Bitmap))
			{
				if (type == NSPasteboard.NSPasteboardTypeTIFF
					|| type == NSPasteboard.NSPasteboardTypePNG)
				{
					value = new Bitmap(new MemoryStream(GetData(type)));
					return true;
				}
			}
			value = null;
			return false;
		}

		public bool TrySetObject(object value, string type)
		{
			if (value is Color color && type == NSPasteboard.NSPasteboardTypeColor)
			{
				Control.WriteObjects(new[] { color.ToNSUI() });
				return true;
			}
			return false;
		}

		public void SetObject(object value, string type) => Widget.SetObject(value, type);

		public T GetObject<T>(string type) => Widget.GetObject<T>(type);
		public object GetObject(string type, Type objectType) => Widget.GetObject(type, objectType);
		public object GetObject(string type) => Widget.GetObject(type);
	}
}
