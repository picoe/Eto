using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;

namespace Eto.Forms
{
	/// <summary>
	/// Text alignment hint for items in a <see cref="ToolBar"/>
	/// </summary>
	/// <remarks>
	/// Note that some platforms may define the visual style of toolbar items and this just serves as a hint for platforms
	/// that support such features (e.g. windows).
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum ToolBarTextAlign
	{
		/// <summary>
		/// Text will be shown to the right of the toolbar item, if available
		/// </summary>
		Right,
		/// <summary>
		/// Text will be shown below the toolbar items
		/// </summary>
		Underneath
	}

	/// <summary>
	/// Docking mode hint for a <see cref="ToolBar"/>
	/// </summary>
	/// <remarks>
	/// Most platforms will not respect this, and is usually only necessary for constrained devices like iOS and Android.
	/// Each platform may have a different default docking mode, depending on the type of device.
	/// E.g. iPhone will by default show the toolbar on the bottom, whereas iPad and dekstop platforms will show it at
	/// the top by default.
	/// 
	/// Additionally, some platforms may choose to show the toolbar in a different way, e.g. the Navigation control
	/// on iPhone has a standard toolbar available, so if you are using one it will attempt to use its toolbar to provide 
	/// the best native experience.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum ToolBarDock
	{
		/// <summary>
		/// Toolbar will be shown at the top of the form
		/// </summary>
		Top,
		/// <summary>
		/// Toolbar will be shown at the bottom of the form.
		/// </summary>
		Bottom
	}

	/// <summary>
	/// Toolbar widget for use on a <see cref="Window"/>.
	/// </summary>
	/// <remarks>
	/// Only a single toolbar is currently supported for each window.
	/// </remarks>
	/// <seealso cref="Window.ToolBar"/>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[ContentProperty("Items")]
	[Handler(typeof(ToolBar.IHandler))]
	public class ToolBar : Tool, IBindableWidgetContainer
	{
		internal new IHandler Handler { get { return (IHandler)base.Handler; } }

		ToolItemCollection items;

		/// <summary>
		/// Gets or sets the docking hint for the toolbar.
		/// </summary>
		/// <remarks>
		/// Most platforms will not respect this, and is usually only necessary for constrained devices like iOS and Android.
		/// Each platform may have a different default docking mode, depending on the type of device.
		/// E.g. iPhone will by default show the toolbar on the bottom, whereas iPad and dekstop platforms will show it at
		/// the top by default.
		/// 
		/// Additionally, some platforms may choose to show the toolbar in a different way, e.g. the Navigation control
		/// on iPhone has a standard toolbar available, so if you are using one it will attempt to use its toolbar to provide 
		/// the best native experience.
		/// </remarks>
		/// <value>The dock hint.</value>
		public ToolBarDock Dock
		{
			get { return Handler.Dock; }
			set { Handler.Dock = value; }
		}

		/// <summary>
		/// Gets the collection of items in the toolbar.
		/// </summary>
		/// <value>The tool item collection.</value>
		public ToolItemCollection Items { get { return items ?? (items = new ToolItemCollection(this)); } }

		/// <summary>
		/// Gets or sets the text alignment hint.
		/// </summary>
		/// <remarks>
		/// Note that some platforms may define the visual style of toolbar items and this just serves as a hint for platforms
		/// that support such features (e.g. windows).
		/// </remarks>
		/// <value>The text alignment hint.</value>
		public ToolBarTextAlign TextAlign
		{
			get { return Handler.TextAlign; }
			set { Handler.TextAlign = value; }
		}

		IEnumerable<BindableWidget> IBindableWidgetContainer.Children => Items;

		/// <summary>
		/// Called when the tool item is loaded to be shown on the form.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		internal protected override void OnLoad(EventArgs e)
		{
			foreach (var item in Items)
				item.OnLoad(e);
		}

		/// <summary>
		/// Called when the tool item is removed from a form.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		internal protected override void OnUnLoad(EventArgs e)
		{
			foreach (var item in Items)
				item.OnUnLoad(e);
		}

		/// <summary>
		/// Called when the tool item is removed from a form.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		internal protected override void OnPreLoad(EventArgs e)
		{
			foreach (var item in Items)
				item.OnPreLoad(e);
		}

		/// <summary>
		/// Handler interface for the <see cref="ToolBar"/>.
		/// </summary>
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Adds a button at the specified index.
			/// </summary>
			/// <param name="button">Button to add.</param>
			/// <param name="index">Index of the button to add.</param>
			void AddButton(ToolItem button, int index);

			/// <summary>
			/// Removes the specified button.
			/// </summary>
			/// <param name="button">Button to remove.</param>
			/// <param name="index">Index of the button to remove.</param>
			void RemoveButton(ToolItem button, int index);

			/// <summary>
			/// Clears all buttons from the toolbar
			/// </summary>
			void Clear();

			/// <summary>
			/// Gets or sets the text alignment hint.
			/// </summary>
			/// <remarks>
			/// Note that some platforms may define the visual style of toolbar items and this just serves as a hint for platforms
			/// that support such features (e.g. windows).
			/// </remarks>
			/// <value>The text alignment hint.</value>
			ToolBarTextAlign TextAlign { get; set; }

			/// <summary>
			/// Gets or sets the docking hint for the toolbar.
			/// </summary>
			/// <remarks>
			/// Most platforms will not respect this, and is usually only necessary for constrained devices like iOS and Android.
			/// Each platform may have a different default docking mode, depending on the type of device.
			/// E.g. iPhone will by default show the toolbar on the bottom, whereas iPad and dekstop platforms will show it at
			/// the top by default.
			/// 
			/// Additionally, some platforms may choose to show the toolbar in a different way, e.g. the Navigation control
			/// on iPhone has a standard toolbar available, so if you are using one it will attempt to use its toolbar to provide 
			/// the best native experience.
			/// </remarks>
			/// <value>The dock hint.</value>
			ToolBarDock Dock { get; set; }
		}
	}
}
