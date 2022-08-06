using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
using a = Android;

namespace Eto.Android.Forms.Controls
{
	internal interface IListBoxHandler
	{
		ListBox Widget { get; }

		void UpdateAdapter();
	}

	public class ListBoxHandler : ListBoxHandler<aw.Spinner, ListBox, ListBox.ICallback>, ListBox.IHandler
	{
	}

	public class ListBoxHandler<TControl, TWidget, TCallback> : AndroidControl<TControl, TWidget, TCallback>, ListBox.IHandler, IListBoxHandler
		where TControl : aw.Spinner
		where TWidget : ListBox
		where TCallback : ListBox.ICallback
	{
		public override av.View ContainerControl { get { return Control; } }

		protected List<EtoListBoxItem> adapterSource;
		aw.ArrayAdapter<EtoListBoxItem> adapter;
		CollectionHandler collection;
		aw.Spinner spinnerControl;

		public ListBoxHandler()
		{
			Control = (TControl)new aw.Spinner(Platform.AppContextThemed);

			adapterSource = new List<EtoListBoxItem>();
			adapter = new aw.ArrayAdapter<EtoListBoxItem>(Control.Context, a.Resource.Layout.SimpleSpinnerItem, a.Resource.Id.Text1);
						
			Control.Adapter = adapter;

			int lastSelected = -1;
			Control.ItemSelected += (s, e) =>
			{
				if (e.Position!= lastSelected)
				{
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
					lastSelected = SelectedIndex;
				}
			};

			Control.NothingSelected+= (s, e) =>
			{
				if (lastSelected != -1)
				{
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
					lastSelected = -1;
				}
			};
		}
		
		private void Control_ItemSelected(Object sender, aw.AdapterView.ItemSelectedEventArgs e)
		{
		}

		public IEnumerable<object> DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler(adapterSource) { Handler = this };
				collection.Register(value);
			}
		}

		void IListBoxHandler.UpdateAdapter()
		{
			adapter.Clear();
			adapter.AddAll(adapterSource);
		}

		ListBox IListBoxHandler.Widget => Widget;

		public int SelectedIndex
		{
			get => Control.SelectedItemPosition;
			set => Control.SetSelection(value);
		}

		public Eto.Drawing.Font Font
		{
			get
			{
				return null;
				throw new NotImplementedException();
			}
			set
			{
				return;
				throw new NotImplementedException();
			}
		}

		public Eto.Drawing.Color TextColor
		{
			get;
			set;
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			private readonly List<EtoListBoxItem> adapterSource;

			public CollectionHandler(List<EtoListBoxItem> adapterSource)
			{
				this.adapterSource = adapterSource;
			}

			public IListBoxHandler Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
				adapterSource.AddRange(items.Select(r => new EtoListBoxItem(Handler.Widget, r)).ToArray());
				Handler.UpdateAdapter();
			}

			public override void AddItem(object item)
			{
				adapterSource.Add(new EtoListBoxItem(Handler.Widget, item));
				Handler.UpdateAdapter();
			}

			public override void InsertItem(int index, object item)
			{
				adapterSource.Insert(index, new EtoListBoxItem(Handler.Widget, item));
				Handler.UpdateAdapter();
			}

			public override void RemoveItem(int index)
			{
				adapterSource.RemoveAt(index);
				Handler.UpdateAdapter();
			}

			public override void RemoveAllItems()
			{
				adapterSource.Clear();
				Handler.UpdateAdapter();
			}
		}

		public IIndirectBinding<String> ItemTextBinding
		{
			get;
			set;
		}

		public IIndirectBinding<String> ItemKeyBinding
		{
			get;
			set;
		}

		public ContextMenu ContextMenu
		{
			get;
			set;
		}
	}

	public class EtoListBoxItem
	{
		ListBox ListBox { get; set; }
		public object Value { get; set; }
		public Image Image => ListBox.ItemImageBinding?.GetValue(Value);

		public override string ToString() => ListBox.ItemTextBinding?.GetValue(Value) ?? string.Empty;

		public EtoListBoxItem(ListBox listBox, object value)
		{
			ListBox = listBox;
			Value = value;
		}
	}
}
