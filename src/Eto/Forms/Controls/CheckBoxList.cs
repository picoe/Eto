using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using System.Collections;

namespace Eto.Forms;

/// <summary>
/// A control that shows a list of check boxes.
/// </summary>
/// <remarks>
/// The list of items can be added manually by using <see cref="CheckBoxList.Items"/>. 
/// Use <see cref="CheckBoxList.DataStore"/> to have a dynamic list of items controlled by a custom collection.
/// </remarks>
/// <example>
/// This creates a list of check boxes and adds items manually:
/// <code>
/// var myCheckBoxList = new CheckBoxList();
/// myCheckBoxList.Items.Add(new ListItem { Text = "Item 1" });
/// myCheckBoxList.Items.Add(new ListItem { Text = "Item 2" });
/// </code>
/// This creates a list of check boxes and uses an external collection to control the items:
/// <code>
/// var myCustomList = new List&lt;string&gt;() { "Item 1", "Item 2" };
/// var myCheckboxList = new CheckBoxList() { DataStore = myCustomList };
/// </code>
/// </example>
[ContentProperty("Items")]
public class CheckBoxList : Panel
{
	Orientation orientation;
	ItemDataStore dataStore;
	readonly List<CheckBox> buttons = new List<CheckBox>();
	Size spacing = new Size(0, 0);
	bool settingChecked;

	/// <summary>
	/// Gets or sets the binding to get the text for each check box.
	/// </summary>
	/// <remarks>
	/// By default, this will bind to a "Text" property, or <see cref="IListItem.Text"/> when implemented.
	/// </remarks>
	/// <value>The text binding.</value>
	public IIndirectBinding<string> ItemTextBinding { get; set; }

	/// <summary>
	/// Gets or sets the binding to get the tooltip for each check box.
	/// </summary>
	/// <value>The item tool tip binding.</value>
	public IIndirectBinding<string> ItemToolTipBinding { get; set; }

	/// <summary>
	/// Gets or sets the binding to get the key for each check box.
	/// </summary>
	/// <remarks>
	/// By default, this will bind to a "Key" property, or <see cref="IListItem.Key"/> when implemented.
	/// </remarks>
	/// <value>The key binding.</value>
	public IIndirectBinding<string> ItemKeyBinding { get; set; }


	static readonly object SelectedValuesChangedKey = new object();
		
	/// <summary>
	/// Occurs when <see cref="SelectedValues"/> changes.
	/// </summary>
	public event EventHandler<EventArgs> SelectedValuesChanged
	{
		add { Properties.AddEvent(SelectedValuesChangedKey, value); }
		remove { Properties.RemoveEvent(SelectedValuesChangedKey, value); }
	}

	/// <summary>
	/// Raises the <see cref="SelectedValuesChanged"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnSelectedValuesChanged(EventArgs e)
	{
		Properties.TriggerEvent(SelectedValuesChangedKey, this, e);
	}

	static readonly object SelectedKeysChangedKey = new object();

	/// <summary>
	/// Occurs when <see cref="SelectedKeys"/> changes.
	/// </summary>
	public event EventHandler<EventArgs> SelectedKeysChanged
	{
		add { Properties.AddEvent(SelectedKeysChangedKey, value); }
		remove { Properties.RemoveEvent(SelectedKeysChangedKey, value); }
	}

	/// <summary>
	/// Raises the <see cref="SelectedKeysChanged"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnSelectedKeysChanged(EventArgs e) => Properties.TriggerEvent(SelectedKeysChangedKey, this, e);

	/// <summary>
	/// Gets or sets the selected key of the currently selected item using <see cref="ItemKeyBinding"/>.
	/// </summary>
	/// <value>The selected key.</value>
	public IEnumerable<string> SelectedKeys
	{
		get => SelectedValues?.Select(r => ItemKeyBinding.GetValue(r));
		set
		{
			var keys = value.ToList();
			settingChecked = true;
			var changed = false;
			foreach (var button in buttons)
			{
				var key = ItemKeyBinding.GetValue(button.Tag);
				var isChecked = keys.Contains(key);
				if (button.Checked != isChecked)
				{
					changed = true;
					button.Checked = isChecked;
				}
			}
			settingChecked = false;
			if (changed)
				TriggerSelectionChanged();
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="Eto.Forms.CheckBoxList"/> is enabled.
	/// </summary>
	/// <remarks>
	/// When the control is disabled, the user will not be able to change the selected check box.
	/// However, you can still programatically change the selection.
	/// </remarks>
	/// <value><see langword="true"/> if enabled; otherwise, <see langword="false"/>.</value>
	public override bool Enabled
	{
		get { return base.Enabled; }
		set
		{
			base.Enabled = value;
			foreach (var button in buttons)
			{
				button.Enabled = value;
			}
		}
	}

	/// <summary>
	/// Gets or sets the selected values, which are <see cref="ListItem"/>s or objects in your custom data store.
	/// </summary>
	/// <value>The selected values.</value>
	public IEnumerable<object> SelectedValues
	{
		get => buttons.Where(r => r.Checked == true).Select(r => r.Tag);
		set
		{
			var items = value?.ToList();
			settingChecked = true;
			var changed = false;
			foreach (var button in buttons)
			{
				var item = button.Tag;
				var isChecked = items != null && items.Contains(item);
				if (button.Checked != isChecked)
				{
					changed = true;
					button.Checked = isChecked;
				}
			}
			settingChecked = false;
			if (changed)
				TriggerSelectionChanged();
		}
	}

	static readonly object TextColor_Key = new object();

	/// <summary>
	/// Gets or sets the color of the check box text.
	/// </summary>
	/// <value>The color of the check box text.</value>
	public Color TextColor
	{
		get { return Properties.Get(TextColor_Key, SystemColors.ControlText); }
		set
		{
			if (value == TextColor) return;
				
			Properties.Set(TextColor_Key, value);
			foreach (var button in buttons)
			{
				button.TextColor = value;
			}
		}
	}

	class ItemDataStore : EnumerableChangedHandler<object>
	{
		public CheckBoxList Handler { get; set; }

		public override void AddItem(object item)
		{
			var button = Handler.CreateCheckBox(item);
			Handler.buttons.Add(button);
			Handler.LayoutCheckBoxes();
		}

		public override void InsertItem(int index, object item)
		{
			var button = Handler.CreateCheckBox(item);
			Handler.buttons.Insert(index, button);
			Handler.LayoutCheckBoxes();
		}

		public override void RemoveItem(int index)
		{
			var button = Handler.buttons[index];
			Handler.buttons.RemoveAt(index);
			Handler.UnregisterCheckBox(button);
			Handler.LayoutCheckBoxes();
			if (button.Checked == true)
				Handler.TriggerSelectionChanged();
		}

		public override void RemoveAllItems()
		{
			Handler.Clear();
		}
	}

	/// <summary>
	/// Gets or sets the orientation of the check boxes.
	/// </summary>
	/// <value>The check box orientation.</value>
	public Orientation Orientation
	{
		get { return orientation; }
		set
		{
			if (orientation == value) return;
				
			orientation = value;
			LayoutCheckBoxes();
		}
	}

	/// <summary>
	/// Gets or sets the spacing between each check box.
	/// </summary>
	/// <value>The spacing between check boxes.</value>
	public Size Spacing
	{
		get { return spacing; }
		set
		{
			if (spacing == value) return;
				
			spacing = value;
			LayoutCheckBoxes();
		}
	}

	/// <summary>
	/// Gets the item collection, when adding items programatically.
	/// </summary>
	/// <remarks>
	/// This is used when you want to add items manually. Use <see cref="DataStore"/>
	/// when you have an existing collection you want to bind to directly.
	/// </remarks>
	/// <value>The item collection.</value>
	public ListItemCollection Items
	{
		get
		{
			var items = (ListItemCollection)DataStore;
			if (items == null)
			{
				items = CreateDefaultItems();
				DataStore = items;
			}
			return items;
		}
	}

	/// <summary>
	/// Gets or sets the data store of the items shown in the list.
	/// </summary>
	/// <remarks>
	/// When using a custom object collection, you can use the <see cref="ItemTextBinding"/> and <see cref="ItemKeyBinding"/> 
	/// to specify how to get the text/key values for each item.
	/// </remarks>
	/// <value>The data store.</value>
	public IEnumerable<object> DataStore
	{
		get { return dataStore == null ? null : dataStore.Collection; }
		set
		{
			buttons.Clear();
			dataStore = new ItemDataStore { Handler = this };
			dataStore.Register(value);
			LayoutCheckBoxes();
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.CheckBoxList"/> class.
	/// </summary>
	public CheckBoxList()
	{
		ItemTextBinding = new ListItemTextBinding();
		ItemKeyBinding = new ListItemKeyBinding();
	}

	/// <summary>
	/// Raises the load event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (DataStore == null)
			DataStore = CreateDefaultItems();
		else
		{
			LayoutCheckBoxes();
		}
	}

	void EnsureCheckBoxes()
	{
		if (DataStore == null)
			DataStore = CreateDefaultItems();
	}

	void LayoutCheckBoxes(bool force = false)
	{
		if (!Loaded && !force)
			return;
		SuspendLayout();
		var layout = new TableLayout { Spacing = spacing, Padding = Padding.Empty };
		if (orientation == Orientation.Horizontal)
		{
			var row = new TableRow();
			foreach (var checkbox in buttons)
			{
				row.Cells.Add(checkbox);
			}
			row.Cells.Add(null);
			layout.Rows.Add(row);
		}
		else
		{
			foreach (var checkbox in buttons)
			{
				layout.Rows.Add(checkbox);
			}
			layout.Rows.Add(null);
		}
		Content = layout;
		ResumeLayout();
	}

	void Clear()
	{
		foreach (var c in buttons)
			UnregisterCheckBox(c);
		buttons.Clear();
		LayoutCheckBoxes();
	}

	void TriggerSelectionChanged()
	{
		OnSelectedValuesChanged(EventArgs.Empty);
		OnSelectedKeysChanged(EventArgs.Empty);
	}

	CheckBox CreateCheckBox(object item)
	{
		var checkbox = new CheckBox();
		if (Properties.ContainsKey(TextColor_Key))
			checkbox.TextColor = TextColor;
		checkbox.CheckedChanged += HandleCheckedChanged;
		checkbox.Text = ItemTextBinding.GetValue(item);
		if (ItemToolTipBinding != null)
			checkbox.ToolTip = ItemToolTipBinding.GetValue(item);
		checkbox.Tag = item;
		checkbox.Enabled = base.Enabled;
		return checkbox;
	}

	internal override void InternalEnsureLayout()
	{
		if (Content == null)
			LayoutCheckBoxes(true);
		base.InternalEnsureLayout();
	}

	void UnregisterCheckBox(CheckBox checkbox)
	{
		checkbox.CheckedChanged -= HandleCheckedChanged;
	}

	void HandleCheckedChanged(object sender, EventArgs e)
	{
		var checkbox = (CheckBox)sender;
		if (!settingChecked)
		{
			TriggerSelectionChanged();
		}
	}

	/// <summary>
	/// Creates the default items.
	/// </summary>
	/// <returns>The default items.</returns>
	protected virtual ListItemCollection CreateDefaultItems() => new ListItemCollection();

	/// <summary>
	/// Gets a binding to the <see cref="SelectedValues"/> property.
	/// </summary>
	/// <value>The selected value binding.</value>
	public BindableBinding<CheckBoxList, IEnumerable<object>> SelectedValuesBinding
	{
		get
		{
			return new BindableBinding<CheckBoxList, IEnumerable<object>>(
				this,
				c => c.SelectedValues,
				(c, v) => c.SelectedValues = v,
				(c, h) => c.SelectedValuesChanged += h,
				(c, h) => c.SelectedValuesChanged -= h
			);
		}
	}

	/// <summary>
	/// Gets a binding to the <see cref="SelectedKeys"/> property.
	/// </summary>
	/// <value>The selected index binding.</value>
	public BindableBinding<CheckBoxList, IEnumerable<string>> SelectedKeysBinding
	{
		get
		{
			return new BindableBinding<CheckBoxList, IEnumerable<string>>(
				this,
				c => c.SelectedKeys,
				(c, v) => c.SelectedKeys = v,
				(c, h) => c.SelectedKeysChanged += h,
				(c, h) => c.SelectedKeysChanged -= h
			);
		}
	}
}