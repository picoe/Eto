using System;
using System.Collections.Generic;

namespace Eto.Forms
{
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
		/// Occurs when a <see cref="NavigationItem"/> is shown.
		/// </summary>
		public event EventHandler<EventArgs> ItemShown;

		/// <summary>
		/// Raises the <see cref="ItemShown"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnItemShown(EventArgs e)
		{
			if (ItemShown != null)
				ItemShown(this, e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Navigation"/> class.
		/// </summary>
		public Navigation()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Navigation"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public Navigation(Generator generator)
			: base(generator, typeof(IHandler))
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
			void OnItemShown(Navigation widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for the <see cref="Navigation"/> control.
		/// </summary>
		protected new class Callback : Container.Callback, ICallback
		{
			/// <summary>
			/// Raises the item shown event.
			/// </summary>
			public void OnItemShown(Navigation widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnItemShown(e));
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

