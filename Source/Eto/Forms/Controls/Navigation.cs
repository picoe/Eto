using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Event arguments with a <see cref="NavigationItem"/> reference
	/// </summary>
	public sealed class NavigationItemEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the item that triggered the event
		/// </summary>
		/// <value>The item.</value>
		public INavigationItem Item { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.NavigationItemEventArgs"/> class.
		/// </summary>
		/// <param name="item">Item that triggered the event</param>
		public NavigationItemEventArgs(INavigationItem item)
		{
			Item = item;
		}
	}

	/// <summary>
	/// Control to show child panels in a hirarchical stack using a navigation button to go back to a previous panel.
	/// </summary>
	/// <remarks>
	/// Typically only available on mobile platforms, this allows you to show multiple panes of information in a
	/// hierarchical fashion, keeping the state of previous panes.
	/// </remarks>
	[Handler(typeof(Navigation.IHandler))]
	public class Navigation : Container
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets an enumeration of controls that are directly contained by this container
		/// </summary>
		/// <value>The contained controls.</value>
		public override IEnumerable<Control> Controls
		{
			get
			{
				yield break;
			}
		}

		/// <summary>
		/// Gets a value indicating that the Navigation control is supported by the current platform.
		/// </summary>
		/// <value><c>true</c> if supported; otherwise, <c>false</c>.</value>
		public static bool IsSupported
		{
			get { return Platform.Instance.Supports<IHandler>(); }
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="ItemShown"/> event
		/// </summary>
		public const string ItemShownEvent = "Navigation.ItemShown";

		/// <summary>
		/// Event to handle when an item is shown on the navigation stack
		/// </summary>
		public event EventHandler<NavigationItemEventArgs> ItemShown
		{
			add { Properties.AddHandlerEvent(ItemShownEvent, value); }
			remove { Properties.RemoveEvent(ItemShownEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="ItemShown"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnItemShown(NavigationItemEventArgs e)
		{
			Properties.TriggerEvent(ItemShownEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="ItemRemoved"/> event
		/// </summary>
		public const string ItemRemovedEvent = "Navigation.ItemRemoved";

		/// <summary>
		/// Event to handle when an item is removed from the navigation stack, either by the user or by code.
		/// </summary>
		public event EventHandler<NavigationItemEventArgs> ItemRemoved
		{
			add { Properties.AddHandlerEvent(ItemRemovedEvent, value); }
			remove { Properties.RemoveEvent(ItemRemovedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="ItemRemoved"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnItemRemoved(NavigationItemEventArgs e)
		{
			RemoveParent(e.Item.Content);
			Properties.TriggerEvent(ItemRemovedEvent, this, e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Navigation"/> class.
		/// </summary>
		public Navigation()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Navigation"/> class with the initial <paramref name="content"/> and <paramref name="title"/>.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="title">Title.</param>
		public Navigation(Control content, string title = null)
			: this()
		{
			Push(content, title);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Navigation"/> class with the initial navigation item.
		/// </summary>
		/// <param name="item">Item for the intial content of the navigation control.</param>
		public Navigation(INavigationItem item)
			: this()
		{
			Push(item);
		}

		/// <summary>
		/// Pushes a new pane onto the navigation stack with the specified <paramref name="content"/> and <paramref name="title"/>.
		/// </summary>
		/// <param name="content">Content for the new pane</param>
		/// <param name="title">Title of the pane</param>
		public void Push(Control content, string title = null)
		{
			Push(new NavigationItem { Content = content, Text = title });
		}

		/// <summary>
		/// Pushes a new navigation item onto the stack.
		/// </summary>
		/// <param name="item">Item to push onto the navigation stack.</param>
		public void Push(INavigationItem item)
		{
			SetParent(item.Content, () => Handler.Push(item), null);
		}

		/// <summary>
		/// Pops the last item from the navigation stack.
		/// </summary>
		public virtual void Pop()
		{
			Handler.Pop();
		}

		/// <summary>
		/// Removes the specified child.
		/// </summary>
		/// <param name="child">Child to remove.</param>
		public override void Remove(Control child)
		{
			//throw new NotImplementedException();
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
		/// Callback interface for the <see cref="Navigation"/> control.
		/// </summary>
		public new interface ICallback : Container.ICallback
		{
			/// <summary>
			/// Raises the item shown event.
			/// </summary>
			void OnItemShown(Navigation widget, NavigationItemEventArgs e);

			/// <summary>
			/// Raises the item removed event.
			/// </summary>
			void OnItemRemoved(Navigation widget, NavigationItemEventArgs e);
		}

		/// <summary>
		/// Callback implementation for the <see cref="Navigation"/> control.
		/// </summary>
		protected new class Callback : Container.Callback, ICallback
		{
			/// <summary>
			/// Raises the item shown event.
			/// </summary>
			public void OnItemShown(Navigation widget, NavigationItemEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnItemShown(e);
			}

			/// <summary>
			/// Raises the item removed event.
			/// </summary>
			public void OnItemRemoved(Navigation widget, NavigationItemEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnItemRemoved(e);
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="Navigation"/> control.
		/// </summary>
		public new interface IHandler : Container.IHandler
		{
			/// <summary>
			/// Pushes a new navigation item onto the stack.
			/// </summary>
			/// <param name="item">Item to push onto the navigation stack.</param>
			void Push(INavigationItem item);

			/// <summary>
			/// Pops the last item from the navigation stack.
			/// </summary>
			void Pop();
		}
	}
}

