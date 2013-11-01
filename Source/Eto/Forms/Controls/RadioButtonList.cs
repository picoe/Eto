using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;

namespace Eto.Forms
{
	public enum RadioButtonListOrientation
	{
		Horizontal,
		Vertical
	}

	public class RadioButtonList : Panel
	{
		RadioButtonListOrientation orientation;
		ItemDataStore dataStore;
		readonly List<RadioButton> buttons = new List<RadioButton> ();
		RadioButton controller;
		RadioButton selectedButton;
		Size spacing = TableLayout.DefaultSpacing;
		bool settingChecked;

		public event EventHandler<EventArgs> SelectedIndexChanged;
		
		public virtual void OnSelectedIndexChanged (EventArgs e)
		{
			if (SelectedIndexChanged != null)
				SelectedIndexChanged (this, e);
			OnSelectedValueChanged (e);
		}
		
		public event EventHandler<EventArgs> SelectedValueChanged;
		
		public virtual void OnSelectedValueChanged (EventArgs e)
		{
			if (SelectedValueChanged != null)
				SelectedValueChanged (this, e);
		}

		public string SelectedKey
		{
			get { return SelectedValue == null ? null : SelectedValue.Key; }
			set
			{
				if (SelectedValue == null || SelectedValue.Key != value) {
					SetSelectedKey (value);
				}
			}
		}

		public override bool Enabled
		{
			get { return base.Enabled; }
			set
			{
				base.Enabled = value;
				foreach (var button in buttons) {
					button.Enabled = value;
				}
			}
		}

		public IListItem SelectedValue
		{
			get { return selectedButton == null ? null : selectedButton.Tag as IListItem; }
			set
			{
				if (SelectedValue != value) {
					if (value != null)
						SetSelectedKey (value.Key);
					else
						SetSelected (null);
				}
			}
		}

		public int SelectedIndex
		{
			get { return selectedButton == null ? -1 : buttons.IndexOf(selectedButton); }
			set
			{
				EnsureButtons ();
				SetSelected (buttons [value]);
			}
		}

		class ItemDataStore : DataStoreChangedHandler<IListItem, IListStore>
		{
			public RadioButtonList Handler { get; set; }

			public override void AddRange (IEnumerable<IListItem> items)
			{
				var key = Handler.SelectedKey;
				Handler.Recreate ();
				Handler.SetSelectedKey (key, true);
			}

			public override void AddItem (IListItem item)
			{
				var button = Handler.CreateButton (item);
				Handler.buttons.Add (button);
				Handler.LayoutButtons ();
			}

			public override void InsertItem (int index, IListItem item)
			{
				var button = Handler.CreateButton (item);
				Handler.buttons.Insert (index, button);
				Handler.LayoutButtons ();
			}

			public override void RemoveItem (int index)
			{
				var button = Handler.buttons [index];
				var isSelected = Handler.selectedButton == button;
				Handler.buttons.RemoveAt (index);
				Handler.UnregisterButton (button);
				if (button == Handler.controller)
					Handler.Recreate ();
				else
					Handler.LayoutButtons ();

				if (isSelected)
					Handler.SetSelected (null, true);
			}

			public override void RemoveAllItems ()
			{
				Handler.Recreate ();
				Handler.SetSelected (null);
			}
		}

		public RadioButtonListOrientation Orientation
		{
			get { return orientation; }
			set
			{
				if (orientation != value) {
					orientation = value;
					LayoutButtons ();
				}
			}
		}

		public Size Spacing
		{
			get { return spacing; }
			set {
				if (spacing != value) {
					spacing = value;
					LayoutButtons ();
				}
			}
		}

		public ListItemCollection Items
		{
			get
			{
				var items = (ListItemCollection)DataStore;
				if (items == null) {
					items = CreateDefaultItems ();
					DataStore = items;
				}
				return items;
			}
		}
		
		public IListStore DataStore
		{
			get { return dataStore == null ? null : dataStore.Collection; }
			set
			{
				dataStore = new ItemDataStore { Handler = this };
				dataStore.Register (value);
			}
		}

		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			if (DataStore == null)
				DataStore = CreateDefaultItems();
			else {
				LayoutButtons ();
				SetSelected (selectedButton, true);
			}
		}

		void EnsureButtons ()
		{
			if (DataStore == null)
				DataStore = CreateDefaultItems();
		}

		void LayoutButtons ()
		{
			if (!Loaded)
				return;
			SuspendLayout();
			var layout = new DynamicLayout (Padding.Empty, spacing);
			var horizontal = orientation == RadioButtonListOrientation.Horizontal;
			if (horizontal)
				layout.BeginHorizontal ();
			foreach (var button in buttons) {
				layout.Add (button);
			}
			layout.Add (null);
			if (horizontal)
				layout.EndHorizontal ();
			Content = layout;
			ResumeLayout();
		}

		void Recreate ()
		{
			buttons.ForEach(UnregisterButton);
			buttons.Clear ();
			controller = null;
			Create ();
			LayoutButtons ();
		}

		void Create ()
		{
			if (dataStore == null)
				return;
			foreach (var item in dataStore) {
				buttons.Add (CreateButton (item));
			}
		}

		void SetSelectedKey (string key, bool force = false)
		{
			EnsureButtons ();
			SetSelected (buttons.FirstOrDefault (r => ((IListItem)r.Tag).Key == key), force);
		}

		void SetSelected (RadioButton button, bool force = false, bool sendEvent = true)
		{
			EnsureButtons ();
			var changed = selectedButton != button;
			if (force || changed) {
				selectedButton = button;
				settingChecked = true;
				buttons.ForEach (r => r.Checked = object.ReferenceEquals (r, button));
				settingChecked = false;
				if (sendEvent && changed && Loaded)
					OnSelectedIndexChanged (EventArgs.Empty);
			}
		}

		RadioButton CreateButton (IListItem item)
		{
			var button = new RadioButton (controller);
			button.CheckedChanged += HandleCheckedChanged;
			button.Text = item.Text;
			button.Tag = item;
			button.Enabled = base.Enabled;
			if (controller == null) 
				controller = button;
			return button;
		}

		void UnregisterButton (RadioButton button)
		{
			button.CheckedChanged -= HandleCheckedChanged;
		}

		void HandleCheckedChanged (object sender, EventArgs e)
		{
			var button = (RadioButton)sender;
			if (!settingChecked && button.Checked) {
				selectedButton = button;
				OnSelectedIndexChanged (EventArgs.Empty);
			}
		}

		protected virtual ListItemCollection CreateDefaultItems ()
		{
			return new ListItemCollection ();
		}

		public ObjectBinding<RadioButtonList, IListItem> SelectedValueBinding
		{
			get
			{
				return new ObjectBinding<RadioButtonList, IListItem>(
					this, 
					c => c.SelectedValue, 
					(c, v) => c.SelectedValue = v, 
					(c, h) => c.SelectedValueChanged += h, 
					(c, h) => c.SelectedValueChanged -= h
					);
			}
		}
	}
}

