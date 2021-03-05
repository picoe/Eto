using System;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Menu item for a submenu
	/// </summary>
	/// <remarks>
	/// This will always show as a submenu, even when no items have been added.
	/// The <see cref="SubMenuItem.Opening" /> can be used to populate or modify the submenu before it opens.
	/// </remarks>
	[ContentProperty("Items")]
	[Handler(typeof(SubMenuItem.IHandler))]
	public class SubMenuItem : ButtonMenuItem, ISubmenu, IBindableWidgetContainer
	{
		new IHandler Handler => (IHandler)base.Handler;

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="SubMenuItem.Opening"/> event.
		/// </summary>
		public const string OpeningEvent = "SubMenuItem.Opening";

		/// <summary>
		/// Occurs when the sub menu is opening, before it is shown.
		/// </summary>
		public event EventHandler<EventArgs> Opening
		{
			add { Properties.AddHandlerEvent(OpeningEvent, value); }
			remove { Properties.RemoveEvent(OpeningEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="SubMenuItem.Opening"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnOpening(EventArgs e)
		{
			Properties.TriggerEvent(OpeningEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Closed"/> event.
		/// </summary>
		public const string ClosedEvent = "SubMenuItem.Closed";

		/// <summary>
		/// Occurs when the sub menu is closed/dismissed, after the menu item has been selected and its click 
		/// event is triggered.
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

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Closing"/> event.
		/// </summary>
		public const string ClosingEvent = "SubMenuItem.Closing";

		/// <summary>
		/// Occurs before the sub menu is closed/dismissed when the user clicks an item, but before the menu item's
		/// click event is triggered.
		/// </summary>
		public event EventHandler<EventArgs> Closing
		{
			add { Properties.AddHandlerEvent(ClosingEvent, value); }
			remove { Properties.RemoveEvent(ClosingEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Closing"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnClosing(EventArgs e)
		{
			Properties.TriggerEvent(ClosingEvent, this, e);
		}

		static SubMenuItem()
		{
			RegisterEvent<SubMenuItem>(c => c.OnOpening(null), OpeningEvent);
			RegisterEvent<SubMenuItem>(c => c.OnClosing(null), ClosingEvent);
			RegisterEvent<SubMenuItem>(c => c.OnClosed(null), ClosedEvent);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SubMenuItem"/> class.
		/// </summary>
		public SubMenuItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SubMenuItem"/> class with the specified <paramref name="items" />.
		/// </summary>
		public SubMenuItem(params MenuItem[] items)
		{
			Items.AddRange(items);
		}

		static readonly object callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() => callback;

		/// <summary>
		/// Callback interface for instances of <see cref="SubMenuItem"/>
		/// </summary>
		public new interface ICallback : ButtonMenuItem.ICallback
		{
			/// <summary>
			/// Raises the <see cref="SubMenuItem.Opening"/> event.
			/// </summary>
			void OnOpening(SubMenuItem widget, EventArgs e);

			/// <summary>
			/// Raises the <see cref="Closed"/> event.
			/// </summary>
			void OnClosed(SubMenuItem widget, EventArgs e);

			/// <summary>
			/// Raises the <see cref="Closing"/> event.
			/// </summary>
			void OnClosing(SubMenuItem widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="SubMenuItem"/>
		/// </summary>
		protected new class Callback : ButtonMenuItem.Callback, ICallback
		{
			/// <summary>
			/// Raises the <see cref="Opening"/> event.
			/// </summary>
			public void OnOpening(SubMenuItem widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnOpening(e);
			}

			/// <summary>
			/// Raises the <see cref="Closed"/> event.
			/// </summary>
			public void OnClosed(SubMenuItem widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnClosed(e);
			}

			/// <summary>
			/// Raises the <see cref="Closing"/> event.
			/// </summary>
			public void OnClosing(SubMenuItem widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnClosing(e);
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="SubMenuItem"/>.
		/// </summary>
		public new interface IHandler : ButtonMenuItem.IHandler
		{
		}
	}
}