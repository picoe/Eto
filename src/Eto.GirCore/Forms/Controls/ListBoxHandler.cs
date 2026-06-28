using System.Runtime.CompilerServices;

namespace Eto.GirCore.Forms.Controls
{
	public class ListBoxHandler : GirControl<Gtk.ListBox, ListBox, ListBox.ICallback>, ListBox.IHandler
	{
		static readonly object BorderKey = new object();

		readonly Gtk.ScrolledWindow scroll;
		CollectionHandler? collection;
		IIndirectBinding<string>? itemTextBinding;
		IIndirectBinding<string>? itemKeyBinding;
		IIndirectBinding<Image>? itemImageBinding;
		Gtk.CssProvider? cssProvider;
		readonly Dictionary<string, string> styleCache = new Dictionary<string, string>();
		bool suppressSelectionChanged;
		Color textColor = Colors.Black;

		public override Gtk.Widget ContainerControl => scroll;

		public ListBoxHandler()
		{
			Control = Gtk.ListBox.New();
			Control.SelectionMode = Gtk.SelectionMode.Single;
			Control.OnSelectedRowsChanged += (sender, e) => HandleSelectionChanged();
			Control.OnRowActivated += (sender, e) => Callback.OnActivated(Widget, EventArgs.Empty);

			scroll = Gtk.ScrolledWindow.New();
			scroll.SetChild(Control);
			scroll.HasFrame = true;

			// Size = new Size(80, 80);
		}

		void HandleSelectionChanged()
		{
			if (!suppressSelectionChanged)
				Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
		}

		string GetItemText(object? item)
		{
			if (item == null)
				return string.Empty;
			return Widget.ItemTextBinding?.GetValue(item) ?? string.Empty;
		}

		Gtk.ListBoxRow CreateRow(object item)
		{
			var row = Gtk.ListBoxRow.New();
			var label = Gtk.Label.New(GetItemText(item));
			label.Xalign = 0;
			row.SetChild(label);
			return row;
		}

		void RefreshData()
		{
			var selectedIndex = SelectedIndex;
			suppressSelectionChanged = true;
			Control.RemoveAll();
			if (collection?.Collection != null)
			{
				foreach (var item in collection.Collection)
					Control.Append(CreateRow(item));
			}
			suppressSelectionChanged = false;

			if (selectedIndex >= 0 && selectedIndex < (collection?.Count ?? 0))
				SelectedIndex = selectedIndex;
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

		public int SelectedIndex
		{
			get => Control.GetSelectedRow()?.GetIndex() ?? -1;
			set
			{
				suppressSelectionChanged = true;
				if (value < 0)
					Control.UnselectAll();
				else
					Control.SelectRow(Control.GetRowAtIndex(value));
				suppressSelectionChanged = false;
			}
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public required ListBoxHandler Handler { get; init; }

			protected override void OnRegisterCollection(EventArgs e)
			{
				base.OnRegisterCollection(e);
				Handler.RefreshData();
			}

			protected override void OnUnregisterCollection(EventArgs e)
			{
				base.OnUnregisterCollection(e);
				Handler.Control.RemoveAll();
			}

			public override void AddItem(object item) => Handler.Control.Append(Handler.CreateRow(item));

			public override void InsertItem(int index, object item) => Handler.Control.Insert(Handler.CreateRow(item), index);

			public override void RemoveItem(int index)
			{
				var row = Handler.Control.GetRowAtIndex(index);
				if (row != null)
					Handler.Control.Remove(row);
			}

			public override void RemoveAllItems() => Handler.Control.RemoveAll();
		}

		public IEnumerable<object>? DataStore
		{
			get => collection?.Collection;
			set
			{
				collection?.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
			}
		}

		public Color TextColor
		{
			get => textColor;
			set
			{
				textColor = value;
				AddStyle($"label {{ color: {value.ToHex()}; }}");
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

		public IIndirectBinding<Image>? ItemImageBinding
		{
			get => itemImageBinding;
			set
			{
				itemImageBinding = value;
				if (Widget.Loaded)
					RefreshData();
			}
		}

		public BorderType Border
		{
			get => Widget.Properties.Get(BorderKey, BorderType.Bezel);
			set
			{
				if (Widget.Properties.TrySet(BorderKey, value, BorderType.Bezel))
					scroll.HasFrame = value != BorderType.None;
			}
		}
	}
}
