using System.Runtime.CompilerServices;

namespace Eto.GirCore.Forms.Controls
{
	public class DropDownHandler : GirControl<Gtk.DropDown, DropDown, DropDown.ICallback>, DropDown.IHandler
	{
		static readonly object ShowBorderKey = new object();

		readonly Gtk.StringList model;
		CollectionHandler? collection;
		IIndirectBinding<string>? itemTextBinding;
		IIndirectBinding<string>? itemKeyBinding;
		Gtk.CssProvider? cssProvider;
		readonly Dictionary<string, string> styleCache = new Dictionary<string, string>();
		bool suppressSelectionChanged;
		Color textColor = Colors.Black;

		public DropDownHandler()
		{
			model = Gtk.StringList.New(Array.Empty<string>());
			Control = Gtk.DropDown.NewFromStrings(Array.Empty<string>());
			Control.Model = model;
			Control.EnableSearch = true;
			Control.ShowArrow = true;
			GObject.Object.NotifySignal.Connect(Control, HandleSelectedNotify, detail: "selected");
		}

		void HandleSelectedNotify(GObject.Object sender, GObject.Object.NotifySignalArgs args)
		{
			if (!suppressSelectionChanged)
				Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
		}

		void AddStyle(string style, [CallerMemberName] string? caller = null)
		{
			if (caller == null)
				return;
			if (cssProvider == null)
			{
				cssProvider = Gtk.CssProvider.New();
				Control.GetStyleContext().AddProvider(cssProvider, 600);
			}
			styleCache[caller] = style;
			cssProvider.LoadFromString(string.Join("\n", styleCache.Values));
		}

		string GetItemText(object? item)
		{
			if (item == null)
				return string.Empty;
			return Widget.ItemTextBinding?.GetValue(item) ?? string.Empty;
		}

		void RefreshData(int? preferredSelectedIndex = null)
		{
			var selectedIndex = preferredSelectedIndex ?? SelectedIndex;
			var items = collection?.Collection?.Select(GetItemText).ToArray() ?? Array.Empty<string>();
			suppressSelectionChanged = true;
			model.Splice(0, model.NItems, items);
			suppressSelectionChanged = false;

			if (selectedIndex >= 0 && selectedIndex < items.Length)
				SelectedIndex = selectedIndex;
			else if (items.Length == 0)
				SelectedIndex = -1;
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public required DropDownHandler Handler { get; init; }

			protected override void OnRegisterCollection(EventArgs e)
			{
				base.OnRegisterCollection(e);
				Handler.RefreshData();
			}

			protected override void OnUnregisterCollection(EventArgs e)
			{
				base.OnUnregisterCollection(e);
				Handler.RefreshData(-1);
			}

			public override void AddItem(object item) => Handler.RefreshData();

			public override void InsertItem(int index, object item) => Handler.RefreshData();

			public override void RemoveItem(int index) => Handler.RefreshData();

			public override void RemoveAllItems() => Handler.RefreshData(-1);
		}

		public int SelectedIndex
		{
			get
			{
				var selected = Control.Selected;
				return selected == uint.MaxValue ? -1 : (int)selected;
			}
			set
			{
				suppressSelectionChanged = true;
				Control.Selected = value < 0 ? uint.MaxValue : (uint)value;
				suppressSelectionChanged = false;
			}
		}

		public IEnumerable<object>? DataStore
		{
			get => collection?.Collection;
			set
			{
				var selected = Widget.SelectedValue;
				var oldSelectedIndex = SelectedIndex;

				collection?.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);

				if (selected != null)
				{
					var newIndex = collection.IndexOf(selected);
					SelectedIndex = newIndex;
					if (newIndex != oldSelectedIndex)
						Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public Color TextColor
		{
			get => textColor;
			set
			{
				textColor = value;
				AddStyle($"dropdown, button, label {{ color: {value.ToHex()}; }}");
			}
		}

		public IIndirectBinding<string>? ItemTextBinding
		{
			get => itemTextBinding;
			set
			{
				itemTextBinding = value;
				if (Widget.Loaded)
					RefreshData();
			}
		}

		public IIndirectBinding<string>? ItemKeyBinding
		{
			get => itemKeyBinding;
			set => itemKeyBinding = value;
		}

		public bool ShowBorder
		{
			get => Widget.Properties.Get(ShowBorderKey, true);
			set
			{
				if (!Widget.Properties.TrySet(ShowBorderKey, value, true))
					return;

				var styleContext = Control.GetStyleContext();
				if (value)
					styleContext.RemoveClass("flat");
				else
					styleContext.AddClass("flat");
			}
		}
	}
}
