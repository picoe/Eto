using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using System.Collections;

namespace Eto.Forms
{
	/// <summary>
	/// Shows a list of radio buttons.
	/// </summary>
	/// <remarks>
	/// The list of items can be added manually using <see cref="RadioButtonList.Items"/>, or 
	/// use the <see cref="RadioButtonList.DataStore"/> to have a dynamic list of items controlled by a custom collection.
	/// </remarks>
	[ContentProperty("Items")]
	public class RadioButtonList : Panel
	{
		Orientation orientation;
		ItemDataStore dataStore;
		readonly List<RadioButton> buttons = new List<RadioButton>();
		RadioButton controller;
		RadioButton selectedButton;
		Size spacing = new Size(0, 0);
		bool settingChecked;

		/// <summary>
		/// Gets or sets the binding to get the text for each radio button.
		/// </summary>
		/// <remarks>
		/// By default, this will bind to a "Text" property, or <see cref="IListItem.Text"/> when implemented.
		/// </remarks>
		/// <value>The text binding.</value>
		public IIndirectBinding<string> ItemTextBinding { get; set; }

		/// <summary>
		/// Gets or sets the binding to get the key for each radio button.
		/// </summary>
		/// <remarks>
		/// By default, this will bind to a "Key" property, or <see cref="IListItem.Key"/> when implemented.
		/// </remarks>
		/// <value>The key binding.</value>
		public IIndirectBinding<string> ItemKeyBinding { get; set; }

		/// <summary>
		/// Gets or sets the binding to get the tooltip text for each radio button.
		/// </summary>
		/// <value>The item tool tip binding.</value>
		public IIndirectBinding<string> ItemToolTipBinding { get; set; }

		/// <summary>
		/// Gets or sets the binding to get the text for each radio button.
		/// </summary>
		/// <remarks>
		/// By default, this will bind to a "Text" property, or <see cref="IListItem.Text"/> when implemented.
		/// </remarks>
		/// <value>The text binding.</value>
		[Obsolete("Since 2.1: Use ItemTextBinding instead")]
		public IIndirectBinding<string> TextBinding {
			get { return ItemTextBinding; }
			set { ItemTextBinding = value; }
		}
		
		/// <summary>
		/// Gets or sets the binding to get the key for each radio button.
		/// </summary>
		/// <remarks>
		/// By default, this will bind to a "Key" property, or <see cref="IListItem.Key"/> when implemented.
		/// </remarks>
		/// <value>The key binding.</value>
		[Obsolete("Since 2.1: Use ItemKeyBinding instead")]
		public IIndirectBinding<string> KeyBinding {
			get { return ItemKeyBinding; }
			set { ItemKeyBinding = value; }
		}

		static readonly object SelectedIndexChangedKey = new object();

		/// <summary>
		/// Occurs when the <see cref="SelectedIndex"/> changes.
		/// </summary>
		public event EventHandler<EventArgs> SelectedIndexChanged
		{
			add { Properties.AddEvent(SelectedIndexChangedKey, value); }
			remove { Properties.RemoveEvent(SelectedIndexChangedKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="SelectedIndexChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnSelectedIndexChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedIndexChangedKey, this, e);
			OnSelectedValueChanged(e);
			OnSelectedKeyChanged(e);
		}

		static readonly object SelectedValueChangedKey = new object();
		
		/// <summary>
		/// Occurs when <see cref="SelectedValue"/> changes.
		/// </summary>
		public event EventHandler<EventArgs> SelectedValueChanged
		{
			add { Properties.AddEvent(SelectedValueChangedKey, value); }
			remove { Properties.RemoveEvent(SelectedValueChangedKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="SelectedValueChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnSelectedValueChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedValueChangedKey, this, e);
		}

		static readonly object SelectedKeyChangedKey = new object();

		/// <summary>
		/// Occurs when <see cref="SelectedKey"/> changes.
		/// </summary>
		public event EventHandler<EventArgs> SelectedKeyChanged
		{
			add { Properties.AddEvent(SelectedKeyChangedKey, value); }
			remove { Properties.RemoveEvent(SelectedKeyChangedKey, value); }
		}

		/// <summary>
		/// Raises the <see cref="SelectedKeyChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnSelectedKeyChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedKeyChangedKey, this, e);
		}

		/// <summary>
		/// Gets or sets the selected key of the currently selected item using the <see cref="ItemKeyBinding"/>.
		/// </summary>
		/// <value>The selected key.</value>
		public string SelectedKey
		{
			get { return SelectedValue == null ? null : ItemKeyBinding.GetValue(SelectedValue); }
			set
			{
				if (SelectedValue == null || ItemKeyBinding.GetValue(SelectedValue) != value)
				{
					SetSelectedKey(value);
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.RadioButtonList"/> is enabled.
		/// </summary>
		/// <remarks>
		/// When the control is disabled, the user will not be able to change the selected radio button.
		/// However, you can still programatically change the selection.
		/// </remarks>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
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
		/// Gets or sets the selected value, which is the <see cref="ListItem"/> or object in your custom data store.
		/// </summary>
		/// <value>The selected value.</value>
		public object SelectedValue
		{
			get { return selectedButton == null ? null : selectedButton.Tag; }
			set
			{
				if (SelectedValue != value)
				{
					if (value != null)
						SetSelectedKey(ItemKeyBinding.GetValue(value));
					else
						SetSelected(null);
				}
			}
		}

		/// <summary>
		/// Gets or sets the index of the selected item.
		/// </summary>
		/// <value>The index of the selected item.</value>
		public int SelectedIndex
		{
			get { return selectedButton == null ? -1 : buttons.IndexOf(selectedButton); }
			set
			{
				EnsureButtons();
				SetSelected(buttons[value]);
			}
		}

		static readonly object TextColor_Key = new object();

		/// <summary>
		/// Gets or sets the color of the radio button text.
		/// </summary>
		/// <value>The color of the radio button text.</value>
		public Color TextColor
		{
			get { return Properties.Get(TextColor_Key, SystemColors.ControlText); }
			set
			{
				if (value != TextColor)
				{
					Properties.Set(TextColor_Key, value);
					foreach (var button in buttons)
					{
						button.TextColor = value;
					}
				}
			}
		}

		class ItemDataStore : EnumerableChangedHandler<object>
		{
			public RadioButtonList Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
				var key = Handler.SelectedKey;
				Handler.Recreate();
				Handler.SetSelectedKey(key, true);
			}

			public override void AddItem(object item)
			{
				var button = Handler.CreateButton(item);
				Handler.buttons.Add(button);
				Handler.LayoutButtons();
			}

			public override void InsertItem(int index, object item)
			{
				var button = Handler.CreateButton(item);
				Handler.buttons.Insert(index, button);
				Handler.LayoutButtons();
			}

			public override void RemoveItem(int index)
			{
				var button = Handler.buttons[index];
				var isSelected = Handler.selectedButton == button;
				Handler.buttons.RemoveAt(index);
				Handler.UnregisterButton(button);
				if (button == Handler.controller)
					Handler.Recreate();
				else
					Handler.LayoutButtons();

				if (isSelected)
					Handler.SetSelected(null, true);
			}

			public override void RemoveAllItems()
			{
				Handler.Recreate();
				Handler.SetSelected(null);
			}
		}

		/// <summary>
		/// Gets or sets the orientation of the radio buttons.
		/// </summary>
		/// <value>The radio button orientation.</value>
		public Orientation Orientation
		{
			get { return orientation; }
			set
			{
				if (orientation != value)
				{
					orientation = value;
					LayoutButtons();
				}
			}
		}

		/// <summary>
		/// Gets or sets the spacing between each radio button.
		/// </summary>
		/// <value>The spacing between radio buttons.</value>
		public Size Spacing
		{
			get { return spacing; }
			set
			{
				if (spacing != value)
				{
					spacing = value;
					LayoutButtons();
				}
			}
		}

		/// <summary>
		/// Gets the item collection, when adding items programatically.
		/// </summary>
		/// <remarks>
		/// This is used when you want to add items manually.  Use the <see cref="DataStore"/>
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
				dataStore = new ItemDataStore { Handler = this };
				dataStore.Register(value);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RadioButtonList"/> class.
		/// </summary>
		public RadioButtonList()
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
				LayoutButtons();
				SetSelected(selectedButton, true);
			}
		}

		void EnsureButtons()
		{
			if (DataStore == null)
				DataStore = CreateDefaultItems();
		}

		void LayoutButtons()
		{
			if (!Loaded)
				return;
			SuspendLayout();
			var layout = new DynamicLayout { Padding = Padding.Empty, Spacing = spacing };
			var horizontal = orientation == Orientation.Horizontal;
			if (horizontal)
				layout.BeginHorizontal();
			foreach (var button in buttons)
			{
				layout.Add(button);
			}
			layout.Add(null);
			if (horizontal)
				layout.EndHorizontal();
			Content = layout;
			ResumeLayout();
		}

		void Recreate()
		{
			foreach (var b in buttons)
				UnregisterButton(b);
			buttons.Clear();
			controller = null;
			Create();
			LayoutButtons();
		}

		void Create()
		{
			if (dataStore == null)
				return;
			foreach (var item in dataStore.Collection)
			{
				buttons.Add(CreateButton(item));
			}
		}

		void SetSelectedKey(string key, bool force = false)
		{
			EnsureButtons();
			SetSelected(buttons.FirstOrDefault(r => ItemKeyBinding.GetValue(r.Tag) == key), force);
		}

		void SetSelected(RadioButton button, bool force = false, bool sendEvent = true)
		{
			EnsureButtons();
			var changed = selectedButton != button;
			if (force || changed)
			{
				selectedButton = button;
				settingChecked = true;
				foreach (var r in buttons)
					r.Checked = object.ReferenceEquals(r, button);
				settingChecked = false;
				if (sendEvent && changed && Loaded)
					OnSelectedIndexChanged(EventArgs.Empty);
			}
		}

		RadioButton CreateButton(object item)
		{
			var button = new RadioButton(controller);
			if (Properties.ContainsKey(TextColor_Key))
				button.TextColor = TextColor;
			button.CheckedChanged += HandleCheckedChanged;
			button.Text = ItemTextBinding.GetValue(item);
			if (ItemToolTipBinding != null)
				button.ToolTip = ItemToolTipBinding.GetValue(item);
			button.Tag = item;
			button.Enabled = base.Enabled;
			if (controller == null)
				controller = button;
			return button;
		}

		void UnregisterButton(RadioButton button)
		{
			button.CheckedChanged -= HandleCheckedChanged;
		}

		void HandleCheckedChanged(object sender, EventArgs e)
		{
			var button = (RadioButton)sender;
			if (!settingChecked && button.Checked)
			{
				selectedButton = button;
				OnSelectedIndexChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Creates the default items.
		/// </summary>
		/// <returns>The default items.</returns>
		protected virtual ListItemCollection CreateDefaultItems()
		{
			return new ListItemCollection();
		}

		/// <summary>
		/// Gets a binding to the <see cref="SelectedValue"/> property.
		/// </summary>
		/// <value>The selected value binding.</value>
		public BindableBinding<RadioButtonList, object> SelectedValueBinding
		{
			get
			{
				return new BindableBinding<RadioButtonList, object>(
					this,
					c => c.SelectedValue,
					(c, v) => c.SelectedValue = v,
					(c, h) => c.SelectedValueChanged += h,
					(c, h) => c.SelectedValueChanged -= h
				);
			}
		}

		/// <summary>
		/// Gets a binding to the <see cref="SelectedIndex"/> property.
		/// </summary>
		/// <value>The selected index binding.</value>
		public BindableBinding<RadioButtonList, int> SelectedIndexBinding
		{
			get
			{
				return new BindableBinding<RadioButtonList, int>(
					this,
					c => c.SelectedIndex,
					(c, v) => c.SelectedIndex = v,
					(c, h) => c.SelectedIndexChanged += h,
					(c, h) => c.SelectedIndexChanged -= h
				);
			}
		}

		/// <summary>
		/// Gets a binding to the <see cref="SelectedKey"/> property.
		/// </summary>
		/// <value>The selected index binding.</value>
		public BindableBinding<RadioButtonList, string> SelectedKeyBinding
		{
			get
			{
				return new BindableBinding<RadioButtonList, string>(
					this,
					c => c.SelectedKey,
					(c, v) => c.SelectedKey = v,
					(c, h) => c.SelectedKeyChanged += h,
					(c, h) => c.SelectedKeyChanged -= h
				);
			}
		}
	}

	/// <summary>
	/// Orientation of buttons in a <see cref="RadioButtonList"/>
	/// </summary>
	[Obsolete("Since 2.1: Use Orientation instead")]
	public struct RadioButtonListOrientation
	{
		readonly Orientation orientation;

		RadioButtonListOrientation(Orientation orientation)
		{
			this.orientation = orientation;
		}

		/// <summary>
		/// Radio buttons are displayed horizontally.
		/// </summary>
		public static RadioButtonListOrientation Horizontal { get { return Orientation.Horizontal; } }

		/// <summary>
		/// Radio buttons are displayed vertically.
		/// </summary>
		public static RadioButtonListOrientation Vertical { get { return Orientation.Vertical; } }

		/// <summary>Converts to an Orientation</summary>
		public static implicit operator Orientation(RadioButtonListOrientation orientation)
		{
			return orientation.orientation;
		}

		/// <summary>Converts an Orientation to a RadioButtonListOrientation</summary>
		public static implicit operator RadioButtonListOrientation(Orientation orientation)
		{
			return new RadioButtonListOrientation(orientation);
		}

		/// <summary>Compares for equality</summary>
		/// <param name="orientation1">Orientation1.</param>
		/// <param name="orientation2">Orientation2.</param>
		public static bool operator ==(Orientation orientation1, RadioButtonListOrientation orientation2)
		{
			return orientation1 == orientation2.orientation;
		}

		/// <summary>Compares for inequality</summary>
		/// <param name="orientation1">Orientation1.</param>
		/// <param name="orientation2">Orientation2.</param>
		public static bool operator !=(Orientation orientation1, RadioButtonListOrientation orientation2)
		{
			return orientation1 != orientation2.orientation;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Eto.Forms.SliderOrientation"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Eto.Forms.SliderOrientation"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="Eto.Forms.SliderOrientation"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return (obj is RadioButtonListOrientation && (this == (RadioButtonListOrientation)obj))
				|| (obj is Orientation && (this == (Orientation)obj));
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="Eto.Forms.SliderOrientation"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return orientation.GetHashCode();
		}
	}
}

