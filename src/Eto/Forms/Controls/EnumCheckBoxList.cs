using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Check box list based on an enumeration
	/// </summary>
	public class EnumCheckBoxList<T> : CheckBoxList
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
		public new IEnumerable<T> SelectedValues
		{
			get { return base.SelectedValues.Cast<T>(); }
			set { base.SelectedValues = value?.Cast<object>().ToList(); }
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
		/// Gets a binding to the <see cref="SelectedValues"/> property.
		/// </summary>
		/// <value>The selected value binding.</value>
		public new BindableBinding<EnumCheckBoxList<T>, IEnumerable<T>> SelectedValuesBinding
		{
			get
			{
				return new BindableBinding<EnumCheckBoxList<T>, IEnumerable<T>>(
					this,
					c => c.SelectedValues,
					(c, v) => c.SelectedValues = v,
					(c, h) => c.SelectedValuesChanged += h,
					(c, h) => c.SelectedValuesChanged -= h
				);
			}
		}
	}
}

