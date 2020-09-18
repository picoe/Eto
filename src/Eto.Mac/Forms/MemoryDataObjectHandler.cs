using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
using System.Linq;
using System.Collections.Generic;

#if XAMMAC2
using AppKit;
using Foundation;
using ObjCRuntime;
using MobileCoreServices;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.MobileCoreServices;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms
{
	public class MemoryDataObjectHandler : WidgetHandler<Dictionary<string, MemoryDataObjectHandler.BaseItem>, DataObject, DataObject.ICallback>, DataObject.IHandler, IDataObject, IDataObjectHandler
	{
		const string UrlKey = "ba330802-0ac2-4ee0-a22f-0e67316ac339";

		public abstract class BaseItem
		{
			public abstract void Apply(NSPasteboard pasteboard, string type);
			public abstract void Apply(NSPasteboardItem item, string type);
			public virtual IEnumerable<NSObject> GetItems() => null;
		}

		public class StringItem : BaseItem
		{
			public string Value { get; set; }
			public override void Apply(NSPasteboard pasteboard, string type) => pasteboard.SetStringForType(Value, type);
			public override void Apply(NSPasteboardItem item, string type) => item.SetStringForType(Value, type);
		}

		public class DataItem : BaseItem
		{
			public NSData Value { get; set; }
			public override void Apply(NSPasteboard pasteboard, string type) => pasteboard.SetDataForType(Value, type);
			public override void Apply(NSPasteboardItem item, string type) => item.SetDataForType(Value, type);
		}

		public class PropertyListItem : BaseItem
		{
			public NSObject Value { get; set; }
			public override void Apply(NSPasteboard pasteboard, string type) => pasteboard.SetPropertyListForType(Value, type);
			public override void Apply(NSPasteboardItem item, string type) => item.SetPropertyListForType(Value, type);
		}

		public class UrlItem : BaseItem
		{
			Uri[] _values;
			NSUrl[] _nsvalues;
			public Uri[] Values
			{
				get => _values;
				set
				{
					_values = value;
					_nsvalues = null;
				}
			}
			NSUrl[] GetNSValues()
			{
				if (_nsvalues != null)
					return _nsvalues;
				_nsvalues = _values.Where(r => r.IsAbsoluteUri).Select(r => (NSUrl)r).ToArray();
				return _nsvalues;
			}

			public override void Apply(NSPasteboard pasteboard, string type) => pasteboard.WriteObjects(GetNSValues());
			public override void Apply(NSPasteboardItem item, string type) { }
			public override IEnumerable<NSObject> GetItems() => GetNSValues();
		}

		public class ColorItem : BaseItem
		{
			public NSColor Value { get; set; }
			public override void Apply(NSPasteboard pasteboard, string type) => pasteboard.WriteObjects(new[] { Value });
			public override void Apply(NSPasteboardItem item, string type) { }
		}


		public MemoryDataObjectHandler()
		{
			Control = new Dictionary<string, BaseItem>();
		}

		public void SetData(byte[] value, string type)
		{
			Control[type] = value == null ? null : new DataItem { Value = NSData.FromArray(value) };
		}

		public string Html
		{
			get => GetDataItem<StringItem>(NSPasteboard.NSPasteboardTypeHTML)?.Value;
			set => Control[NSPasteboard.NSPasteboardTypeHTML] = string.IsNullOrEmpty(value) ? null : new StringItem { Value = value };
		}

		public void SetString(string value, string type) => Control[type] = string.IsNullOrEmpty(value) ? null : new StringItem { Value = value };

		public string Text
		{
			get => GetDataItem<StringItem>(NSPasteboard.NSPasteboardTypeString)?.Value;
			set => Control[NSPasteboard.NSPasteboardTypeString] = value == null ? null : new StringItem { Value = value };
		}

		public Image Image
		{
			set
			{
				var handler = value.Handler as IImageHandler;
				if (handler != null)
				{
					var data = handler.GetImage().AsTiff();
					Control[NSPasteboard.NSPasteboardTypeTIFF] = new DataItem { Value = data };
				}
				else Control[NSPasteboard.NSPasteboardTypeTIFF] = null;
			}
			get
			{
				var item = GetDataItem<DataItem>(NSPasteboard.NSPasteboardTypeTIFF);
				var data = item?.Value?.AsStream();
				return data != null ? new Bitmap(data) : null;
			}
		}

		T GetDataItem<T>(string type)
			where T : BaseItem
		{
			if (Control.TryGetValue(type, out var val))
			{
				return val as T;
			}
			return null;
		}

		public byte[] GetData(string type) => GetDataItem<DataItem>(type)?.Value?.ToArray();

		public string GetString(string type) => GetDataItem<StringItem>(type)?.Value;


		public string[] Types => Control.Keys.ToArray();

		public Uri[] Uris
		{
			get => GetDataItem<UrlItem>(UrlKey)?.Values;
			set => Control[UrlKey] = value?.Length > 0 ? new UrlItem { Values = value } : null;
		}

		public bool ContainsText => GetDataItem<StringItem>(NSPasteboard.NSPasteboardTypeString) != null;

		public bool ContainsHtml => GetDataItem<StringItem>(NSPasteboard.NSPasteboardTypeHTML) != null;

		public bool ContainsImage => GetDataItem<DataItem>(NSPasteboard.NSPasteboardTypeTIFF) != null;

		public bool ContainsUris => GetDataItem<UrlItem>(UrlKey) != null;

		public void Clear()
		{
			Control.Clear();
		}

		public void Apply(NSPasteboard pasteboard)
		{
			foreach (var item in Control)
			{
				item.Value?.Apply(pasteboard, item.Key);
			}
		}

		public IEnumerable<NSObject> GetPasteboardItems()
		{
			NSPasteboardItem pasteboardItem = null;
			foreach (var item in Control)
			{
				var items = item.Value.GetItems();
				if (items != null)
				{
					foreach (var i in items)
					{
						yield return i;
					}
				}
				else
				{
					if (pasteboardItem == null)
						pasteboardItem = new NSPasteboardItem();
					item.Value?.Apply(pasteboardItem, item.Key);
				}
			}
			if (pasteboardItem != null)
				yield return pasteboardItem;
		}

		public bool ContainsString(string type) => GetDataItem<StringItem>(type) != null;

		public bool Contains(string type) => GetDataItem<DataItem>(type) != null;

		public bool TrySetObject(object value, string type)
		{
			if (type == NSPasteboard.NSPasteboardTypeColor && value is Color color)
			{
				Control[type] = new ColorItem { Value = color.ToNSUI() };
				return true;
			}
			return false;
		}

		public bool TryGetObject(string type, out object value)
		{
			var colorItem = GetDataItem<ColorItem>(type);
			if (colorItem != null)
			{
				value = colorItem.Value.ToEto();
				return true;
			}
			value = null;
			return false;
		}

		public void SetObject(object value, string type) => Widget.SetObject(value, type);

		public T GetObject<T>(string type) => Widget.GetObject<T>(type);
	}
}
