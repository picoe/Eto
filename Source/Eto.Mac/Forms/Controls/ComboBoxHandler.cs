using System;
using SD = System.Drawing;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	public class ComboBoxHandler : MacControl<NSComboBox, ComboBox, ComboBox.ICallback>, ComboBox.IHandler
	{
		CollectionHandler collection;
		bool editable;

		public class EtoComboBox : NSComboBox, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}

		public void Create(bool isEditable)
		{
			editable = isEditable;
			Control = new EtoComboBox { Handler = this ,Cell=new NSComboBoxCell()};
			Control.Editable = editable;
			// Add Observer to monitor SelectionDidChangeNotification, but no response. May be a bug?
			this.AddObserver(NSComboBox.SelectionDidChangeNotification, SelectionDidChanged, Control);
		}

		static void SelectionDidChanged(ObserverActionEventArgs e)
		{
			var handler = e.Handler as ComboBoxHandler;
			handler.Callback.OnSelectedIndexChanged(handler.Widget, EventArgs.Empty);
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ComboBoxHandler Handler { get; set; }

			public override int IndexOf(object item)
			{
				var binding = Handler.Widget.TextBinding;
				return (int)Handler.Control.IndexOf(NSObject.FromObject(binding.GetValue(item)));
			}

			public override void AddRange(IEnumerable<object> items)
			{
				var oldIndex = Handler.Control.SelectedIndex;
				var binding = Handler.Widget.TextBinding;
				foreach (var item in items.Select(r => binding.GetValue(r)))
				{
					Handler.Control.Add(NSObject.FromObject(item));
				}
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.LayoutIfNeeded();
			}

			public override void AddItem(object item)
			{
				var oldIndex = Handler.Control.SelectedIndex;
				var binding = Handler.Widget.TextBinding;
				Handler.Control.Add(NSObject.FromObject(binding.GetValue(item)));
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.LayoutIfNeeded();
			}

			public override void InsertItem(int index, object item)
			{
				var oldIndex = Handler.Control.SelectedIndex;
				var binding = Handler.Widget.TextBinding;
				Handler.Control.Insert(NSObject.FromObject(binding.GetValue(item)), index);
				if (oldIndex == -1)
					Handler.Control.SelectItem(-1);
				Handler.LayoutIfNeeded();
			}

			public override void RemoveItem(int index)
			{
				var selected = Handler.SelectedIndex;
				Handler.Control.RemoveAt(index);
				Handler.LayoutIfNeeded();
				if (Handler.Widget.Loaded && selected == index)
				{
					Handler.Control.SelectItem(-1);
					Handler.Callback.OnSelectedIndexChanged(Handler.Widget, EventArgs.Empty);
				}
			}

			public override void RemoveAllItems()
			{
				var change = Handler.SelectedIndex != -1;
				Handler.Control.RemoveAll();
				Handler.LayoutIfNeeded();
				if (Handler.Widget.Loaded && change)
				{
					Handler.Control.SelectItem(-1);
					Handler.Callback.OnSelectedIndexChanged(Handler.Widget, EventArgs.Empty);
				}
			}
		}

		public IEnumerable<object> DataStore
		{
			get { return collection == null ? null : collection.Collection; }
			set
			{
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
			}
		}

		public int SelectedIndex
		{
			get	{ return (int)Control.SelectedIndex; }
			set
			{
				if (value != SelectedIndex)
				{
					Control.SelectItem(value);
					if (Widget.Loaded)
						Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public string Text
		{
			get
			{
				return editable ? Control.StringValue: "";
			}
			set
			{
				if (editable && value != null)
				{
					Control.StringValue = value;
				}
			}
		}
	}
}