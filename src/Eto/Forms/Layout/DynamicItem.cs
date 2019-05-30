using sc = System.ComponentModel;
using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Base item for the <see cref="DynamicLayout"/>
	/// </summary>
	[sc.TypeConverter(typeof(DynamicItemConverter))]
	public abstract class DynamicItem
	{
		/// <summary>
		/// Gets or sets the horizontal scale of the item
		/// </summary>
		/// <remarks>
		/// When true, the control takes up all remaining horizontal space available.
		/// If there is more than one item with this set to true, then they share the remaining space equally.
		/// </remarks>
		/// <value>The horizontal scale.</value>
		public bool? XScale { get; set; }

		/// <summary>
		/// Gets or sets the vertical scale of the item
		/// </summary>
		/// <remarks>
		/// When true, the control takes up all remaining vertical space available.
		/// If there is more than one item with this set to true, then they share the remaining space equally.
		/// </remarks>
		/// <value>The vertical scale.</value>
		public bool? YScale { get; set; }

		/// <summary>
		/// Creates the content for this item
		/// </summary>
		/// <param name="layout">Top level layout the item is being created for</param>
		public abstract Control Create(DynamicLayout layout);

		/// <summary>
		/// Create the item and add to the specified layout and co-ordinates
		/// </summary>
		/// <param name="layout">Top level layout</param>
		/// <param name="parent">Parent table to add this item to</param>
		/// <param name="x">The x coordinate in the table to add to</param>
		/// <param name="y">The y coordinate in the table to add to</param>
		public virtual void Create(DynamicLayout layout, TableLayout parent, int x, int y)
		{
			var c = Create(layout);
			if (c != null)
				parent.Add(c, x, y);
			if (XScale != null)
				parent.SetColumnScale(x, XScale.Value);
			if (YScale != null)
				parent.SetRowScale(y, YScale.Value);
		}

		/// <summary>
		/// Converts a control to a DynamicItem implicitly
		/// </summary>
		/// <param name="control">Control to convert</param>
		public static implicit operator DynamicItem(Control control)
		{
			return new DynamicControl { Control = control };
		}

		/// <summary>
		/// Converts a string to a DynamicItem with a Label implicitly
		/// </summary>
		/// <param name="label">Label string to convert to a DynamicItem.</param>
		public static implicit operator DynamicItem(string label)
		{
			return new DynamicControl { Control = new Label { Text = label } };
		}

		internal abstract IEnumerable<Control> Controls { get; }

		internal abstract void SetParent(DynamicTable table);

		internal abstract void SetLayout(DynamicLayout layout);
	}
}
