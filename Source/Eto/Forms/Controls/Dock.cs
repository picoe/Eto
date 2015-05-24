using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Forms
{
	/// <summary>
	/// Docking mode hint for a <see cref="DockView"/>, <see cref="DockViewItem"/> or <see cref="ToolBarView"/>
	/// </summary>
	/// <remarks>
	/// Some platforms will not respect this, and is usually only necessary for constrained devices like iOS and Android.
	/// Each platform may have a different default docking mode, depending on the type of device.
	/// E.g. iPhone will by default show the toolbar on the bottom, whereas iPad and desktop platforms will show it at
	/// the top by default.
	/// </remarks>
	/// <copyright>(c) 2015 by Nicolas Pöhlmann</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum DockPosition
	{
		/// <summary>
		/// Control will be shown at the bottom.
		/// </summary>
		Bottom,
		/// <summary>
		/// Control will be shown at the left.
		/// </summary>
		Left,
		/// <summary>
		/// Control will be shown at it's position coordinates.
		/// </summary>
		None,
		/// <summary>
		/// Control will be shown at the right.
		/// </summary>
		Right,
		/// <summary>
		/// Control will be shown at the top.
		/// </summary>
		Top
	}
}
