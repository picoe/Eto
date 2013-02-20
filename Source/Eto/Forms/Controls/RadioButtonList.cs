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
		List<RadioButton> buttons = new List<RadioButton> ();
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
			get { return SelectedValue != null ? SelectedValue.Key : null; }
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
			get { return selectedButton != null ? selectedButton.Tag as IListItem : null; }
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
			get { return selectedButton != null ? buttons.IndexOf (selectedButton) : -1; }
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
				Handler.SetSelectedKey (key, true, false);
			}

			public override void AddItem (IListItem item)
			{
				var button = Handler.CreateButton (item);
				Handler.buttons.Add (button);
				Handler.Layout ();
			}

			public override void InsertItem (int index, IListItem item)
			{
				var button = Handler.CreateButton (item);
				Handler.buttons.Insert (index, button);
				Handler.Layout ();
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
					Handler.Layout ();

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
					Layout ();
				}
			}
		}

		public Size Spacing
		{
			get { return spacing; }
			set {
				if (spacing != value) {
					spacing = value;
					Layout ();
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
					this.DataStore = items;
				}
				return items;
			}
		}
		
		public IListStore DataStore
		{
			get { return dataStore != null ? dataStore.Collection : null; }
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
				this.DataStore = CreateDefaultItems ();
			else {
				Layout ();
				SetSelected (selectedButton, true);
			}
		}

		void EnsureButtons ()
		{
			if (this.DataStore == null)
				this.DataStore = CreateDefaultItems ();
		}

		public RadioButtonList ()
		{
		}

		void Layout ()
		{
			if (!Loaded)
				return;
			this.SuspendLayout ();
			var layout = new DynamicLayout (new Panel (), Padding.Empty, spacing);
			var horizontal = orientation == RadioButtonListOrientation.Horizontal;
			if (horizontal)
				layout.BeginHorizontal ();
			foreach (var button in buttons) {
				layout.Add (button);
			}
			layout.Add (null);
			if (horizontal)
				layout.EndHorizontal ();
			this.AddDockedControl (layout.Container);
			this.ResumeLayout ();
		}

		void Recreate ()
		{
			buttons.ForEach (r => UnregisterButton (r));
			buttons.Clear ();
			controller = null;
			Create ();
			Layout ();
		}

		void Create ()
		{
			if (dataStore == null)
				return;
			foreach (var item in dataStore) {
				buttons.Add (CreateButton (item));
			}
		}

		void SetSelectedKey (string key, bool force = false, bool sendEvent = true)
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
			var button = sender as RadioButton;
			if (!settingChecked && button.Checked) {
				selectedButton = button;
				OnSelectedIndexChanged (EventArgs.Empty);
			}
		}

		protected virtual ListItemCollection CreateDefaultItems ()
		{
			return new ListItemCollection ();
		}
	}
}

