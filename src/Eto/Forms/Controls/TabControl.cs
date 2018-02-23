using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	/// <summary>
	/// Control to present multiple pages with a tab selection
	/// </summary>
	/// <remarks>
	/// Some platforms (e.g. OS X) have limitations on how many tabs are visible.
	/// It is advised to utilize different methods (e.g. a listbox or combo box) to switch between many sections
	/// if there are too many tabs.
	/// </remarks>
	[ContentProperty("Pages")]
	[Handler(typeof(TabControl.IHandler))]
	public class TabControl : Container
	{
		TabPageCollection pages;

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets an enumeration of controls that are directly contained by this container
		/// </summary>
		/// <value>The contained controls.</value>
		public override IEnumerable<Control> Controls
		{
			get { return pages ?? Enumerable.Empty<Control>(); }
		}

		static readonly object SelectedIndexChangedEvent = new object();

		/// <summary>
		/// Occurs when the <see cref="SelectedIndex"/> is changed.
		/// </summary>
		public event EventHandler<EventArgs> SelectedIndexChanged
		{
			add { Properties.AddEvent(SelectedIndexChangedEvent, value); }
			remove { Properties.RemoveEvent(SelectedIndexChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="SelectedIndexChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnSelectedIndexChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedIndexChangedEvent, this, e);
			var page = SelectedPage;
			if (page != null)
				page.TriggerClick(e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TabControl"/> class.
		/// </summary>
		public TabControl()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TabControl"/> class with the specified handler.
		/// </summary>
		/// <param name="handler">Handler for the implementation of the tab control.</param>
		protected TabControl(IHandler handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Gets or sets the index of the selected tab.
		/// </summary>
		/// <value>The index of the selected tab.</value>
		public int SelectedIndex
		{
			get { return Handler.SelectedIndex; }
			set { Handler.SelectedIndex = value; }
		}

		/// <summary>
		/// Gets or sets the currently selected page.
		/// </summary>
		/// <value>The selected page.</value>
		public TabPage SelectedPage
		{
			get { return SelectedIndex < 0 ? null : Pages[SelectedIndex]; }
			set { SelectedIndex = pages.IndexOf(value); }
		}

		/// <summary>
		/// Gets the collection of tab pages.
		/// </summary>
		/// <value>The pages.</value>
		public Collection<TabPage> Pages
		{
			get { return pages ?? (pages = new TabPageCollection(this)); }
		}

		/// <summary>
		/// Remove the specified child from the container.
		/// </summary>
		/// <param name="child">Child to remove.</param>
		public override void Remove(Control child)
		{
			var page = child as TabPage;
			if (page != null)
			{
				Pages.Remove(page);
			}
		}

		/// <summary>
		/// Gets or sets the position of the tabs relative to the content.
		/// </summary>
		/// <remarks>
		/// Note that on some platforms the text is rotated when using Left or Right (e.g. OS X).
		/// This means that is is not suitable when you have a lot of tabs.
		/// Some platforms (mobile) may ignore this hint and display the tabs according to the platform.
		/// </remarks>
		public DockPosition TabPosition
		{
			get { return Handler.TabPosition; }
			set { Handler.TabPosition = value; }
		}

		/// <summary>
		/// Gets the binding for the <see cref="SelectedIndex"/> property.
		/// </summary>
		/// <value>The selected index binding.</value>
		public BindableBinding<TabControl, int> SelectedIndexBinding
		{
			get
			{
				return new BindableBinding<TabControl, int>(
					this, 
					c => c.SelectedIndex, 
					(c, v) => c.SelectedIndex = v, 
					(c, h) => c.SelectedIndexChanged += h, 
					(c, h) => c.SelectedIndexChanged -= h
				);
			}
		}

		class TabPageCollection : Collection<TabPage>
		{
			readonly TabControl control;

			internal TabPageCollection(TabControl control)
			{
				this.control = control;
			}

			protected override void InsertItem(int index, TabPage item)
			{
				base.InsertItem(index, item);
				control.SetParent(item, () => control.Handler.InsertTab(index, item));
			}

			protected override void ClearItems()
			{
				control.Handler.ClearTabs();
				base.ClearItems();
			}

			protected override void RemoveItem(int index)
			{
				var page = this[index];
				control.Handler.RemoveTab(index, page);
				control.RemoveParent(page);
				base.RemoveItem(index);
			}
		}

		#region Callback

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for the <see cref="TabControl"/>
		/// </summary>
		public new interface ICallback : Control.ICallback
		{
			/// <summary>
			/// Raises the selected index changed event.
			/// </summary>
			void OnSelectedIndexChanged(TabControl widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="TabControl"/>
		/// </summary>
		protected new class Callback : Control.Callback, ICallback
		{
			/// <summary>
			/// Raises the selected index changed event.
			/// </summary>
			public void OnSelectedIndexChanged(TabControl widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnSelectedIndexChanged(e);
			}
		}

		#endregion

		#region Handler

		/// <summary>
		/// Handler interface for the <see cref="TabControl"/>
		/// </summary>
		public new interface IHandler : Container.IHandler
		{
			/// <summary>
			/// Gets or sets the index of the selected tab.
			/// </summary>
			/// <value>The index of the selected tab.</value>
			int SelectedIndex { get; set; }

			/// <summary>
			/// Inserts a tab at the specified index.
			/// </summary>
			/// <param name="index">Index to insert the tab.</param>
			/// <param name="page">Page to insert.</param>
			void InsertTab(int index, TabPage page);

			/// <summary>
			/// Removes all tabs from the control.
			/// </summary>
			void ClearTabs();

			/// <summary>
			/// Removes the specified tab.
			/// </summary>
			/// <param name="index">Index of the page to remove.</param>
			/// <param name="page">Page to remove.</param>
			void RemoveTab(int index, TabPage page);

			/// <summary>
			/// Gets or sets the position of the tabs relative to the content.
			/// </summary>
			DockPosition TabPosition { get; set; }
		}

		#endregion
	}
}
