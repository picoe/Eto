using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Contains an item for the <see cref="DockView"/>.
	/// </summary>
	/// <copyright>(c) 2015 by Nicolas Pöhlmann</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[ContentProperty("Items")]
	[Handler(typeof(DockViewItem.IHandler))]
	public class DockViewItem : Control
	{
		/// <summary>
		/// Gets or sets the docking position for the control.
		/// </summary>
		/// <remarks>
		/// Most platforms will not respect this, and is usually only necessary for constrained devices like iOS and Android.
		/// Each platform may have a different default docking mode, depending on the type of device.
		/// </remarks>
		/// <value>The dock hint.</value>
		public DockPosition Dock { get; set; }

		/// <summary>
		/// Gets or sets the order of the control when adding to the <see cref="DockViewItemCollection"/>.
		/// </summary>
		/// <value>The order when adding the item.</value>
		public int Order { get; set; }

		/// <summary>
		/// Gets or sets the content of the control.
		/// </summary>
		/// <value>The content to set.</value>
		public Control Content { get; set; }

		/// <summary>
		/// Gets or sets the position of the control in the <see cref="DockViewItemCollection"/>.
		/// </summary>
		/// <value>The postition to set.</value>
		public Point Position { get; set; }

		/// <summary>
		/// Handler interface for the <see cref="DockViewItem"/>
		/// </summary>
		public new interface IHandler : Control.IHandler
		{
		}
	}
}
