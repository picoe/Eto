using System;
using Eto.Forms;
using Eto.Drawing;
using System.IO;
using Eto.Mac.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
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
	public interface IDataObjectHandler
	{
		void Apply(NSPasteboard pasteboard);
		IEnumerable<NSObject> GetPasteboardItems();
	}

	public class DataObjectHandler : DataObjectHandler<DataObject, DataObject.ICallback>, DataObject.IHandler
	{
		public DataObjectHandler()
		{
		}

		public DataObjectHandler(NSPasteboard item)
		{
			Control = item;
		}

	}

	public abstract class DataObjectHandler<TWidget, TCallback> : WidgetHandler<NSPasteboard, TWidget, TCallback>, IDataObject, IDataObjectHandler
		where TWidget : Widget
		where TCallback : Widget.ICallback
	{
		nint _changeCount;
		Dictionary<string, BaseItem> _dataItems;

		Dictionary<string, BaseItem> DataItems => _dataItems ?? (_dataItems = new Dictionary<string, BaseItem>());

		const string UrlKey = "ba330802-0ac2-4ee0-a22f-0e67316ac339";

		abstract class BaseItem
		{
			public abstract void Apply(NSPasteboard pasteboard, string type);
			public abstract void Apply(NSPasteboardItem item, string type);
			public virtual IEnumerable<NSObject> GetItems() => null;
		}

		class StringItem : BaseItem
		{
			public string Value { get; set; }
			public override void Apply(NSPasteboard pasteboard, string type)
			{
				//pasteboard.DeclareTypes(new[] { type }, null);
				pasteboard.SetStringForType(Value, type);
			}
			public override void Apply(NSPasteboardItem item, string type) => item.SetStringForType(Value, type);
		}

		class DataItem : BaseItem
		{
			public NSData Value { get; set; }
			public override void Apply(NSPasteboard pasteboard, string type) => pasteboard.SetDataForType(Value, type);
			public override void Apply(NSPasteboardItem item, string type) => item.SetDataForType(Value, type);
		}

		class PropertyListItem : BaseItem
		{
			public NSObject Value { get; set; }
			public override void Apply(NSPasteboard pasteboard, string type) => pasteboard.SetPropertyListForType(Value, type);
			public override void Apply(NSPasteboardItem item, string type) => item.SetPropertyListForType(Value, type);
		}

		class UrlItem : BaseItem
		{
			public NSUrl[] Values { get; set; }
			public override void Apply(NSPasteboard pasteboard, string type) => pasteboard.WriteObjects(Values);
			public override void Apply(NSPasteboardItem item, string type) { }
			public override IEnumerable<NSObject> GetItems() => Values;
		}

		void ClearIfNeeded()
		{
			if (Control.ChangeCount != _changeCount)
				_changeCount = Control.ClearContents();
		}

		public void SetData(byte[] value, string type)
		{
			if (Control == null)
			{
				DataItems[type] = new DataItem { Value = NSData.FromArray(value) };
				return;
			}
			ClearIfNeeded();
			Control.SetDataForType(NSData.FromArray(value), type);
		}

		public string Html
		{
			set
			{
				if (Control == null)
				{
					DataItems[NSPasteboard.NSPasteboardTypeHTML] = new StringItem { Value = value };
					return;
				}
				ClearIfNeeded();
				Control.SetStringForType(value, NSPasteboard.NSPasteboardTypeHTML);
			}
			get
			{
				if (Control == null)
					return GetDataItem<StringItem>(NSPasteboard.NSPasteboardTypeHTML)?.Value;
				return Control.GetStringForType(NSPasteboard.NSPasteboardTypeHTML);
			}
		}

		public void SetString(string value, string type)
		{
			if (Control == null)
			{
				DataItems[type] = new StringItem { Value = value };
				return;
			}
			ClearIfNeeded();
			Control.SetStringForType(value, type);
		}

		public string Text
		{
			set
			{
				if (Control == null)
				{
					DataItems[NSPasteboard.NSPasteboardTypeString] = new StringItem { Value = value };
					return;
				}
				ClearIfNeeded();
				Control.SetStringForType(value, NSPasteboard.NSPasteboardTypeString);
			}
			get
			{
				if (Control == null)
					return GetDataItem<StringItem>(NSPasteboard.NSPasteboardTypeString)?.Value;
				return Control.GetStringForType(NSPasteboard.NSPasteboardTypeString);
			}
		}

		public Image Image
		{
			set
			{
				var handler = value.Handler as IImageHandler;
				if (handler != null)
				{
					var data = handler.GetImage().AsTiff();
					if (Control == null)
					{
						DataItems[NSPasteboard.NSPasteboardTypeTIFF] = new DataItem { Value = data };
						return;
					}
					ClearIfNeeded();
					Control.SetDataForType(data, NSPasteboard.NSPasteboardTypeTIFF);
				}
			}
			get
			{
				if (Control == null)
				{
					var data = GetDataItem<DataItem>(NSPasteboard.NSPasteboardTypeTIFF)?.Value?.AsStream();
					return data != null ? new Bitmap(data) : null;
				}
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

		T GetDataItem<T>(string type)
			where T: BaseItem
		{
			if (DataItems.TryGetValue(type, out var val))
			{
				return val as T;
			}
			return null;
		}

		public byte[] GetData(string type)
		{
			if (Control == null)
				return GetDataItem<DataItem>(type)?.Value?.ToArray();

			var availableType = Control.GetAvailableTypeFromArray(new string[] { type });

			if (availableType != null)
			{
				var data = Control.GetDataForType(availableType);
				if (data == null)
					return null;
				var bytes = new byte[data.Length];
				Marshal.Copy(data.Bytes, bytes, 0, (int)data.Length);
				return bytes;
			}
			return null;
		}

		public string GetString(string type)
		{
			if (Control == null)
				return GetDataItem<StringItem>(type)?.Value;

			return Control.GetStringForType(type);
		}


		public string[] Types
		{
			get
			{
				if (Control == null)
					return _dataItems?.Keys.ToArray() ?? new string[0];
				return Control.Types;
			}
		}

		static Class[] UriTypes = new Class[] { new Class(typeof(NSUrl)) };

		public Uri[] Uris
		{
			get
			{
				// 10.6+ file list:
				if (Control.CanReadObjectForClasses(UriTypes, null))
				{
					var objs = Control.ReadObjectsForClasses(UriTypes, null);
					if (objs != null)
						return objs.OfType<NSUrl>().Select(r => (Uri)r).ToArray();
				}

				var availableType = Control.GetAvailableTypeFromArray(new string[] { NSPasteboard.NSFilenamesType });
				if (availableType != null)
				{
					// old-school way of passing files
					NSArray files;
					if (Control == null)
						files = GetDataItem<PropertyListItem>(availableType)?.Value as NSArray;
					else
						files = Control.GetPropertyListForType(availableType) as NSArray;

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
				var files = value?.Select(r => (NSUrl)r).ToArray();
				if (Control == null)
				{
					DataItems[UrlKey] = new UrlItem { Values = files };
					return;
				}
				Control.WriteObjects(files);
			}
		}

		public void Clear()
		{
			if (Control != null)
				_changeCount = Control.ClearContents();
			_dataItems?.Clear();
		}

		public void Apply(NSPasteboard pasteboard)
		{
			if (_dataItems == null)
				return;
			foreach (var item in _dataItems)
			{
				item.Value.Apply(pasteboard, item.Key);
			}
		}

		public IEnumerable<NSObject> GetPasteboardItems()
		{
			if (_dataItems == null)
				yield break;
			NSPasteboardItem pasteboardItem = null;
			foreach (var item in _dataItems)
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
					item.Value.Apply(pasteboardItem, item.Key);
				}
			}
			if (pasteboardItem != null)
				yield return pasteboardItem;
		}
	}
}
