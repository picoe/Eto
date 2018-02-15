using System;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;

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
		public AddValueEventArgs(T value, bool shouldAdd)
		{
			this.Value = value;
			this.ShouldAdd = shouldAdd;
		}
	}

	/// <summary>
	/// Combo box for an enumeration
	/// </summary>
	/// <typeparam name="T">Enumeration type to fill the values with</typeparam>
	public class EnumDropDown<T> : DropDown
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
		protected virtual void OnAddValue(AddValueEventArgs<T> e)
		{
			if (AddValue != null) AddValue(this, e);
		}

		#endregion

		Type EnumType => Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

		/// <summary>
		/// Gets or sets the currently selected enumeration value
		/// </summary>
		public new T SelectedValue
		{
			get
			{
				if (string.IsNullOrEmpty(SelectedKey))
					return default(T);
				return (T)Enum.ToObject(EnumType, Convert.ToInt32(SelectedKey, CultureInfo.InvariantCulture));
			}
			set
			{
				if (value == null)
					SelectedKey = null;
				else
					SelectedKey = Convert.ToString(Convert.ToInt32(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
			}
		}

		/// <summary>
		/// Gets or sets a delegate used to get the text value for each item.
		/// </summary>
		/// <remarks>
		/// You can use this delegate to provide translated or custom text for each enumeration.
		/// Otherwise, the name of the enum is used.
		/// </remarks>
		public Func<T, string> GetText { get; set; }

		/// <summary>
		/// Gets or sets a value indicating that the items in the list are sorted alphabetically, instead of by numerical value of the enumeration
		/// </summary>
		public bool SortAlphabetically { get; set; }

		/// <summary>
		/// Creates the default data store for the list.
		/// </summary>
		/// <remarks>This is used to create a data store if one is not specified by the user.
		/// This can be used by subclasses to provide default items to populate the list.</remarks>
		/// <returns>The default data store.</returns>
		protected override IEnumerable<object> CreateDefaultDataStore()
		{
			var type = EnumType;
			if (!type.IsEnum())
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "T must be an enumeration"));

			var items = new ListItemCollection();
			if (typeof(T) != type)
				items.Add(new EnumValue { Key = null, Text = null });
			var values = Enum.GetValues(type);
			var names = Enum.GetNames(type);
			for (int i = 0; i < names.Length; i++)
			{
				var e = new AddValueEventArgs<T>((T)values.GetValue(i), true);
				OnAddValue(e);
				if (e.ShouldAdd)
				{
					items.Add(new EnumValue
					{
						Text = GetText != null ? GetText(e.Value) : names[i],
						Key = Convert.ToString(Convert.ToInt32(e.Value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture)
					});
				}
			}
			if (SortAlphabetically)
				items.Sort((x, y) => string.Compare(x.Text, y.Text, StringComparison.CurrentCultureIgnoreCase));
			return items;
		}

		/// <summary>
		/// Gets a new binding for the <see cref="SelectedValue"/> property.
		/// </summary>
		/// <value>A new selected value binding.</value>
		public new BindableBinding<EnumDropDown<T>, T> SelectedValueBinding
		{
			get
			{
				return new BindableBinding<EnumDropDown<T>, T>(
					this,
					c => c.SelectedValue,
					(c, v) => c.SelectedValue = v,
					(c, h) => c.SelectedValueChanged += h,
					(c, h) => c.SelectedValueChanged -= h
				)
				{
					SettingNullValue = default(T)
				};
			}
		}
	}
}

