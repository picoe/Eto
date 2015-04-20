using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// DockView item collection.
	/// </summary>
	/// <copyright>(c) 2015 by Nicolas Pöhlmann</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class DockViewCollection : Collection<Control>
	{
		readonly DockView parent;

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DockViewCollection"/> class.
		/// </summary>
		/// <param name="parent">Parent of the control.</param>
		protected internal DockViewCollection(DockView parent)
		{
			this.parent = parent;
		}
		
		/// <summary>
		/// Adds the specified item given its order.
		/// </summary>
		/// <remarks>
		/// This will add the item into the collection based on its <see cref="Control.Order"/>, keeping
		/// all items in their order.
		/// </remarks>
		/// <param name="control">Control to add.</param>
		public new void Add(Control control)
		{
			var previousItem = this.Where(c => c.Order <= control.Order).OrderBy(c => c.Order).LastOrDefault();
			var previous = this.IndexOf(previousItem);

			Insert(previous + 1, control);
		}

		/// <summary>
		/// Adds the specified controls to the collection.
		/// </summary>
		/// <param name="controls">Control to add.</param>
		public void AddRange(IEnumerable<Control> controls)
		{
			foreach (var control in controls)
			{
				Add(control);
			}
		}

		/// <summary>
		/// Called when the collection should be cleared.
		/// </summary>
		protected override void ClearItems()
		{
			base.ClearItems();
			parent.Handler.Clear();
		}

		/// <summary>
		/// Called when a control is inserted.
		/// </summary>
		/// <param name="index">Index of the control to insert.</param>
		/// <param name="control">Control to insert.</param>
		protected override void InsertItem(int index, Control control)
		{
			base.InsertItem(index, control);
			parent.Handler.AddControl(control, index);
		}

		/// <summary>
		/// Called when a control is removed from the collection.
		/// </summary>
		/// <param name="index">Index of the control being removed.</param>
		protected override void RemoveItem(int index)
		{
			var control = this[index];
			base.RemoveItem(index);
			parent.Handler.RemoveControl(control);
		}
	}
}
