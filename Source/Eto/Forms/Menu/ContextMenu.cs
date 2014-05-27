using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Interface for widgets/handlers that implement a context menu
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IContextMenuHost
	{
		/// <summary>
		/// Gets or sets the context menu to show when the user right clicks or presses the menu key
		/// </summary>
		/// <value>The context menu to show, or null to have no menu</value>
		ContextMenu ContextMenu { get; set; }
	}

	/// <summary>
	/// Represents a context menu that can be shown typically when users right click or press the menu key on a control
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(ContextMenu.IHandler))]
	public class ContextMenu : Menu, ISubmenu
	{
		MenuItemCollection items;

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets the menu items in the context menu
		/// </summary>
		/// <value>The items.</value>
		public MenuItemCollection Items { get { return items ?? (items = new MenuItemCollection(Handler)); } }

		public bool Trim { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ContextMenu"/> class.
		/// </summary>
		public ContextMenu()
		{
			Trim = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ContextMenu"/> class.
		/// </summary>
		/// <param name="items">Items to populate the menu</param>
		public ContextMenu(IEnumerable<MenuItem> items)
			: this()
		{
			Items.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ContextMenu"/> class.
		/// </summary>
		/// <param name="items">Items to populate the menu</param>
		public ContextMenu(params MenuItem[] items)
			: this()
		{
			Items.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ContextMenu"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public ContextMenu(Generator generator) : this(generator, typeof(ContextMenu.IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ContextMenu"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ContextMenu(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			Trim = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ContextMenu"/> class.
		/// </summary>
		/// <param name="g">The green component.</param>
		/// <param name="items">Items.</param>
		[Obsolete("Use constructor without generator instead")]
		public ContextMenu(Generator g, IEnumerable<MenuItem> items) : this(g)
		{
			Items.AddRange(items);
		}

		/// <summary>
		/// Show the context menu relative to the specified control
		/// </summary>
		/// <param name="relativeTo">Control to show the menu relative to</param>
		public void Show(Control relativeTo)
		{
			if (Trim)
				Items.Trim();
			Handler.Show(relativeTo);
		}

		/// <summary>
		/// Called when the menu is assigned to a control/window
		/// </summary>
		/// <param name="e">Event arguments</param>
		internal protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			foreach (var item in Items)
				item.OnLoad(e);
		}

		/// <summary>
		/// Called when the menu is removed from a control/window
		/// </summary>
		/// <param name="e">Event arguments</param>
		internal protected override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			foreach (var item in Items)
				item.OnLoad(e);
		}

		/// <summary>
		/// Handler interface for the <see cref="ContextMenu"/>
		/// </summary>
		public new interface IHandler : Menu.IHandler, ISubmenuHandler
		{
			/// <summary>
			/// Show the context menu relative to the specified control
			/// </summary>
			/// <param name="relativeTo">Control to show the menu relative to</param>
			void Show(Control relativeTo);
		}
	}
}