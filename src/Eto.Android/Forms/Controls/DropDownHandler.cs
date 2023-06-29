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
	internal interface IDropDownHandler
		{
		DropDown Widget { get; }

		void UpdateAdapter();
		}

	public class DropDownHandler : DropDownHandler<aw.Spinner, DropDown, DropDown.ICallback>, DropDown.IHandler
		{
		}

	public class DropDownHandler<TControl, TWidget, TCallback> : AndroidControl<TControl, TWidget, TCallback>, DropDown.IHandler, IDropDownHandler
		where TControl : aw.Spinner
		where TWidget : DropDown
		where TCallback : DropDown.ICallback
		{
		public override av.View ContainerControl { get { return Control; } }

		protected List<EtoDropDownItem> adapterSource;
		aw.ArrayAdapter<EtoDropDownItem> adapter;
		CollectionHandler collection;
		aw.Spinner spinnerControl;

		public DropDownHandler()
		{
			Control = (TControl)new aw.Spinner(Platform.AppContextThemed);

			adapterSource = new List<EtoDropDownItem>();
			adapter = new aw.ArrayAdapter<EtoDropDownItem>(Control.Context, a.Resource.Layout.SimpleSpinnerItem, a.Resource.Id.Text1);
						
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

		void IDropDownHandler.UpdateAdapter()
		{
			adapter.Clear();
			adapter.AddAll(adapterSource);
		}

		DropDown IDropDownHandler.Widget => Widget;

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
			private readonly List<EtoDropDownItem> adapterSource;

			public CollectionHandler(List<EtoDropDownItem> adapterSource)
			{
				this.adapterSource = adapterSource;
			}

			public IDropDownHandler Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
				adapterSource.AddRange(items.Select(r => new EtoDropDownItem(Handler.Widget, r)).ToArray());
				Handler.UpdateAdapter();
			}

			public override void AddItem(object item)
			{
				adapterSource.Add(new EtoDropDownItem(Handler.Widget, item));
				Handler.UpdateAdapter();
			}

			public override void InsertItem(int index, object item)
			{
				adapterSource.Insert(index, new EtoDropDownItem(Handler.Widget, item));
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
	}

	public class EtoDropDownItem
	{
		DropDown DropDown { get; set; }
		public object Value { get; set; }
		public Image Image => DropDown.ItemImageBinding?.GetValue(Value);

		public override string ToString() => DropDown.ItemTextBinding?.GetValue(Value) ?? string.Empty;

		public EtoDropDownItem(DropDown dropDown, object value)
		{
			DropDown = dropDown;
			Value = value;
		}
	}
}
