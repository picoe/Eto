using System;

namespace Eto.Forms
{
	public class EnumRadioButtonList<T> : RadioButtonList
		where T: struct
	{
		class EnumValue : IListItem
		{
			public string Text { get; set; }
			
			public string Key { get; set; }
		}
		
		#region Events
		
		/// <summary>
		/// Event to handle when a value of the enumeration is added to the combo box
		/// </summary>
		public event EventHandler<AddValueEventArgs<T>> AddValue;
		
		/// <summary>
		/// Handles the <see cref="AddValue"/> event
		/// </summary>
		protected virtual void OnAddValue (AddValueEventArgs<T> e)
		{
			if (AddValue != null)
				AddValue (this, e);
		}
		
		#endregion
		
		/// <summary>
		/// Gets or sets the currently selected enumeration value
		/// </summary>
		public new T SelectedValue
		{
			get { return (T)Enum.ToObject (typeof(T), Convert.ToInt32 (base.SelectedKey)); }
			set { base.SelectedKey = Convert.ToString (Convert.ToInt32 (value)); }
		}

		public EnumRadioButtonList ()
		{
		}

		protected override ListItemCollection CreateDefaultItems ()
		{
			var type = typeof (T);
			if (!type.IsEnum) throw new EtoException ("T must be an enumeration");
			
			var items = new ListItemCollection ();
			var values = Enum.GetValues (type);
			var names = Enum.GetNames (type);
			for (int i = 0; i < names.Length; i++) {
				
				var e = new AddValueEventArgs<T> ((T)values.GetValue (i), true);
				if (e.ShouldAdd) {
					items.Add (new EnumValue {
						Text = names[i],
						Key = Convert.ToString (Convert.ToInt32 (e.Value))
					});
				}
			}
			items.Sort ((x, y) => string.Compare (x.Text, y.Text, StringComparison.CurrentCultureIgnoreCase));
			return items;
		}
	}
}

