using System;
using System.Globalization;

namespace Eto.Forms
{
	/// <summary>
	/// Arguments for controlling whether values should be added to a list or control
	/// </summary>
	public class AddValueEventArgs<T> : EventArgs
	{
		/// <summary>
		/// Value being added to the combo box
		/// </summary>
		public T Value { get; private set; }

		/// <summary>
		/// True if the value should be added, false otherwise
		/// </summary>
		public bool ShouldAdd { get; set; }

		/// <summary>
		/// Initializes a new instance of the AddValueEventArgs class
		/// </summary>
		/// <param name="value">value to be added</param>
		/// <param name="shouldAdd">true if by default the item will be added, false otherwise</param>
		public AddValueEventArgs (T value, bool shouldAdd)
		{
			this.Value = value;
			this.ShouldAdd = shouldAdd;
		}
	}

	/// <summary>
	/// Combo box for an enumeration
	/// </summary>
	/// <typeparam name="T">Enumeration type to fill the values with</typeparam>
	public class EnumComboBox<T> : ComboBox
		where T : struct, IConvertible
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
			if (AddValue != null) AddValue (this, e);
		}

		#endregion

		/// <summary>
		/// Gets or sets the currently selected enumeration value
		/// </summary>
		public new T SelectedValue
		{
			get { return (T)Enum.ToObject (typeof (T), Convert.ToInt32 (base.SelectedKey, CultureInfo.InvariantCulture)); }
			set { base.SelectedKey = Convert.ToString (Convert.ToInt32(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture); }
		}

		/// <summary>
		/// Initializes a new instance of the EnumComboBox
		/// </summary>
		public EnumComboBox ()
			: this (Generator.Current)
		{
		}

		/// <summary>
		/// Initializes a new instance of the EnumComboBox with the specified generator
		/// </summary>
		/// <param name="generator">platform generator</param>
		protected EnumComboBox (Generator generator)
			: base (generator)
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
						Key = Convert.ToString (Convert.ToInt32(e.Value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture)
					});
				}
			}
			items.Sort ((x, y) => string.Compare (x.Text, y.Text, StringComparison.CurrentCultureIgnoreCase));
			return items;
		}

		public new ObjectBinding<EnumComboBox<T>, T> SelectedValueBinding
		{
			get
			{
				return new ObjectBinding<EnumComboBox<T>, T>(
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

