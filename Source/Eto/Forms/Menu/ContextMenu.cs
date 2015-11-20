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
	[ContentProperty("Items")]
	[Handler(typeof(ContextMenu.IHandler))]
	public class ContextMenu : Menu, ISubmenu
	{
		MenuItemCollection items;

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets the menu items in the context menu
		/// </summary>
		/// <value>The items.</value>
		public MenuItemCollection Items { get { return items ?? (items = new MenuItemCollection(Handler, this)); } }

		/// <summary>
		/// Gets a value indicating whether this sub menu should trim its child menu items when loaded onto a form
		/// </summary>
		/// <remarks>Trimming will collapse any duplicate splitter items. This is done so that you can easily merge your menus.</remarks>
		/// <value><c>true</c> to trim the child menu items; otherwise, <c>false</c>.</value>
		public bool Trim { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ContextMenu"/> class.
		/// </summary>
		public ContextMenu()
		{
			Trim = true;
			Closed += HandleClosed;
		}

		void HandleClosed(object sender, EventArgs e)
		{
			OnUnLoad(EventArgs.Empty);
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
		/// Show the context menu relative to the specified control
		/// </summary>
		/// <param name="relativeTo">Control to show the menu relative to</param>
		public void Show(Control relativeTo)
		{
			if (Trim)
				Items.Trim();
			OnPreLoad(EventArgs.Empty);
			OnLoad(EventArgs.Empty);
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
		/// Event identifier for handlers when attaching the <see cref="ContextMenu.Opening"/> event.
		/// </summary>
		public const string OpeningEvent = "ContextMenu.Opening";

		/// <summary>
		/// Occurs when the context menu is opening, before it is shown.
		/// </summary>
		public event EventHandler<EventArgs> Opening
		{
			add { Properties.AddHandlerEvent(OpeningEvent, value); }
			remove { Properties.RemoveEvent(OpeningEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="ContextMenu.Opening"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnOpening(EventArgs e)
		{
			Properties.TriggerEvent(OpeningEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Closed"/> event.
		/// </summary>
		public const string ClosedEvent = "ContextMenu.Closed";

		/// <summary>
		/// Occurs when the context menu is closed/dismissed.
		/// </summary>
		public event EventHandler<EventArgs> Closed
		{
			add { Properties.AddHandlerEvent(ClosedEvent, value); }
			remove { Properties.RemoveEvent(ClosedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Closed"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnClosed(EventArgs e)
		{
			Properties.TriggerEvent(ClosedEvent, this, e);
		}

		static readonly object callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback()
		{
			return callback;
		}

		/// <summary>
		/// Callback interface for instances of <see cref="ContextMenu"/>
		/// </summary>
		public new interface ICallback : Menu.ICallback
		{
			/// <summary>
			/// Raises the <see cref="ContextMenu.Opening"/> event.
			/// </summary>
			void OnOpening(ContextMenu widget, EventArgs e);

			/// <summary>
			/// Raises the <see cref="Closed"/> event.
			/// </summary>
			void OnClosed(ContextMenu widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="ContextMenu"/>
		/// </summary>
		protected class Callback : ICallback
		{
			/// <summary>
			/// Raises the <see cref="Opening"/> event.
			/// </summary>
			public void OnOpening(ContextMenu widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnOpening(e));
			}

			/// <summary>
			/// Raises the <see cref="Closed"/> event.
			/// </summary>
			public void OnClosed(ContextMenu widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnClosed(e));
			}
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