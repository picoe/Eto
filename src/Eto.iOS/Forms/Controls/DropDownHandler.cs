using System;
using System.Reflection;
using SD = System.Drawing;
using Eto.Forms;
using UIKit;
using Eto.Drawing;
using Foundation;
using System.Collections.Generic;
using System.Linq;

namespace Eto.iOS.Forms.Controls
{
	public class DropDownHandler : DropDownHandler<DropDown, DropDown.ICallback, UIPickerView>
	{
	}

	public class DropDownHandler<TWidget, TCallback, TPicker> : BasePickerHandler<TWidget, TCallback, TPicker>, DropDown.IHandler
		where TWidget: DropDown
		where TCallback: DropDown.ICallback
		where TPicker: UIPickerView
	{
		CollectionHandler collection;
		int selectedIndex = -1;

		class DataSource : UIPickerViewDataSource
		{
			WeakReference handler;

			public DropDownHandler<TWidget, TCallback, TPicker> Handler { get { return (DropDownHandler<TWidget, TCallback, TPicker>)handler.Target; } set { handler = new WeakReference(value); } }

			public override nint GetComponentCount(UIPickerView pickerView)
			{
				return 1;
			}

			public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
			{
				var data = Handler.collection;
				return data != null ? data.Count : 0;
			}
		}

		class Delegate : UIPickerViewDelegate
		{
			WeakReference handler;

			public DropDownHandler<TWidget, TCallback, TPicker> Handler { get { return (DropDownHandler<TWidget, TCallback, TPicker>)handler.Target; } set { handler = new WeakReference(value); } }

			public override string GetTitle(UIPickerView pickerView, nint row, nint component)
			{
				var data = Handler.collection;
				return data != null ? Handler.Widget.ItemTextBinding.GetValue(data.ElementAt((int)row)) : string.Empty;
			}
		}

		public override TPicker CreatePicker()
		{
			var picker = new UIPickerView();
			picker.ShowSelectionIndicator = true;
			picker.DataSource = new DataSource { Handler = this };
			picker.Delegate = new Delegate { Handler = this };
			return (TPicker)picker;
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public DropDownHandler<TWidget, TCallback, TPicker> Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
			}

			public override void AddItem(object item)
			{
			}

			public override void InsertItem(int index, object item)
			{
			}

			public override void RemoveItem(int index)
			{
				if (Handler.SelectedIndex == index)
				{
					Handler.SelectedIndex = -1;
				}
			}

			public override void RemoveAllItems()
			{
				Handler.SelectedIndex = -1;
			}
		}

		public int SelectedIndex
		{
			get	{ return selectedIndex; }
			set
			{
				if (value != selectedIndex)
				{
					selectedIndex = value;
					UpdateText();
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
				}
			}
		}

		protected override string GetTextValue()
		{
			if (collection != null && selectedIndex >= 0 && selectedIndex < collection.Count)
			{
				var item = collection.ElementAt(selectedIndex);
				return Widget.ItemTextBinding.GetValue(item);
			}
			return null;
		}


		public IEnumerable<object> DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				var index = selectedIndex;
				selectedIndex = -1;
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
				SelectedIndex = index;
			}
		}

		protected override void UpdateValue(TPicker picker)
		{
			SelectedIndex = (int)picker.SelectedRowInComponent(0);
		}

		protected override void UpdatePicker(TPicker picker)
		{
			picker.ReloadAllComponents();
			picker.Select(Math.Max(0, SelectedIndex), 0, false);
		}

		public Color TextColor
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}
}
