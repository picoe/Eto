using System;
using System.Globalization;

namespace Eto.Forms
{
	/// <summary>
	/// Radio button list based on an enumeration
	/// </summary>
	public class EnumRadioButtonList<T> : RadioButtonList
		where T : struct
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
			if (AddValue != null)
				AddValue(this, e);
		}

		#endregion

		/// <summary>
		/// Gets or sets the currently selected enumeration value
		/// </summary>
		public new T SelectedValue
		{
			get { return (T)Enum.ToObject(typeof(T), Convert.ToInt32(SelectedKey, CultureInfo.InvariantCulture)); }
			set { SelectedKey = Convert.ToString(Convert.ToInt32(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture); }
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
		/// Creates the default items.
		/// </summary>
		/// <returns>The default items.</returns>
		protected override ListItemCollection CreateDefaultItems()
		{
			var type = typeof(T);
			if (!type.IsEnum())
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "T must be an enumeration"));

			var items = new ListItemCollection();
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
		/// Gets a binding to the <see cref="SelectedValue"/> property.
		/// </summary>
		/// <value>The selected value binding.</value>
		public new BindableBinding<EnumRadioButtonList<T>, T> SelectedValueBinding
		{
			get
			{
				return new BindableBinding<EnumRadioButtonList<T>, T>(
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

