using System;
using System.Collections.Generic;
using System.Collections;


namespace Eto.Forms
{
	/// <summary>
	/// Cell to present a combo box in a <see cref="Grid"/>.
	/// </summary>
	[Handler(typeof(ComboBoxCell.IHandler))]
	public class ComboBoxCell : SingleValueCell<object>
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Binding to get the text value for the items in the combo box.
		/// </summary>
		/// <value>The combo text binding.</value>
		public IIndirectBinding<string> ComboTextBinding { get; set; }

		/// <summary>
		/// Binding to get the key value for the items in the combo box.
		/// </summary>
		/// <value>The combo key binding.</value>
		public IIndirectBinding<string> ComboKeyBinding { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ComboBoxCell"/> class with the column index to bind.
		/// </summary>
		/// <param name="column">Column index to bind to.</param>
		public ComboBoxCell(int column)
			: this()
		{
			Binding = new ColumnBinding<object>(column);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ComboBoxCell"/> class with the specified property to bind to.
		/// </summary>
		/// <param name="property">Property to bind the value of the combo box to.</param>
		public ComboBoxCell(string property)
			: this()
		{
			Binding = Eto.Forms.Binding.Property<object>(property);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ComboBoxCell"/> class.
		/// </summary>
		public ComboBoxCell()
		{
			ComboTextBinding = new ListItemTextBinding();
			ComboKeyBinding = new ListItemKeyBinding();
		}

		/// <summary>
		/// Gets or sets the data store of the items in the combo box for this cell.
		/// </summary>
		/// <seealso cref="ComboTextBinding"/>
		/// <seealso cref="ComboKeyBinding"/>
		/// <value>The source data store for the items in the combo box.</value>
		public IEnumerable<object> DataStore
		{
			get { return Handler.DataStore; }
			set { Handler.DataStore = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="ComboBoxCell"/>.
		/// </summary>
		public new interface IHandler : SingleValueCell<object>.IHandler
		{
			/// <summary>
			/// Gets or sets the data store of the items in the combo box for this cell.
			/// </summary>
			/// <seealso cref="ComboTextBinding"/>
			/// <seealso cref="ComboKeyBinding"/>
			/// <value>The source data store for the items in the combo box.</value>
			IEnumerable<object> DataStore { get; set; }
		}
	}
}

