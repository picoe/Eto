using System;

namespace Eto.Forms
{
	public class EnumComboBox<T> : ComboBox
		where T: struct, IConvertible
	{
		class EnumValue : IListItem
		{
			public string Text { get; set; }

			public string Key { get; set; }
		}
		
		public new T SelectedValue {
			get {
				return (T)Enum.ToObject (typeof(T), Convert.ToInt32 (base.SelectedKey));
			}
			set
			{
				base.SelectedKey = Convert.ToString (Convert.ToInt32 (value));
			}
		}
		
		public EnumComboBox ()
			: this (Generator.Current)
		{
		}
		
		protected EnumComboBox (Generator generator)
			: base (generator)
		{
			var type = typeof(T);
			if (!type.IsEnum) throw new EtoException("T must be an enumeration");
			
			var items = new ListItemCollection();
			var values = Enum.GetValues (type);
			var names = Enum.GetNames (type);
			for (int i = 0; i<names.Length; i++)
			{
				items.Add (new EnumValue{
					Text = names[i],
					Key = Convert.ToString (Convert.ToInt32 (values.GetValue (i)))
				});
			}
			items.Sort ((x, y) => string.Compare (x.Text, y.Text, StringComparison.CurrentCultureIgnoreCase));
			this.DataStore = items;
		}
	}
}

