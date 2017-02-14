using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections;

namespace Eto.Forms
{
	/// <summary>
	/// Arguments for the <see cref="DocumentControl"/> to get the current page.
	/// </summary>
	public class DocumentPageEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the document page.
		/// </summary>
		/// <value>The document page.</value>
		public DocumentPage Page { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.DocumentPageEventArgs"/> class.
		/// </summary>
		/// <param name="page">Page.</param>
		public DocumentPageEventArgs(DocumentPage page)
		{
			Page = page;
		}
	}

	/// <summary>
	/// Control to present multiple pages with a tab selection
	/// </summary>
	/// <remarks>
	/// Some platforms (e.g. OS X) have limitations on how many tabs are visible.
	/// It is advised to utilize different methods (e.g. a listbox or combo box) to switch between many sections
	/// if there are too many tabs.
	/// </remarks>
	[ContentProperty("Pages")]
	[Handler(typeof(DocumentControl.IHandler))]
	public class DocumentControl : Container
	{
		DocumentPageCollection pages;

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets an enumeration of controls that are directly contained by this container
		/// </summary>
		/// <value>The contained controls.</value>
		public override IEnumerable<Control> Controls
		{
			get { return pages ?? Enumerable.Empty<Control>(); }
		}

		static readonly object PageClosedEvent = new object();
		static readonly object SelectedIndexChangedEvent = new object();

		/// <summary>
		/// Occurs when the <see cref="DocumentPage"/> is closed.
		/// </summary>
		public event EventHandler<DocumentPageEventArgs> PageClosed
		{
			add { Properties.AddEvent(PageClosedEvent, value); }
			remove { Properties.RemoveEvent(PageClosedEvent, value); }
		}

		/// <summary>
		/// Occurs when the <see cref="SelectedIndex"/> is changed.
		/// </summary>
		public event EventHandler<EventArgs> SelectedIndexChanged
		{
			add { Properties.AddEvent(SelectedIndexChangedEvent, value); }
			remove { Properties.RemoveEvent(SelectedIndexChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="PageClosed"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnPageClosed(DocumentPageEventArgs e)
		{
			Properties.TriggerEvent(PageClosedEvent, this, e);
			var page = SelectedPage;
			if (page != null)
				page.TriggerClose(e);
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
		/// Initializes a new instance of the <see cref="DocumentControl"/> class.
		/// </summary>
		public DocumentControl()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DocumentControl"/> class with the specified handler.
		/// </summary>
		/// <param name="handler">Handler for the implementation of the tab control.</param>
		protected DocumentControl(IHandler handler)
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
		public DocumentPage SelectedPage
		{
			get { return SelectedIndex < 0 ? null : Pages[SelectedIndex]; }
			set { SelectedIndex = pages.IndexOf(value); }
		}

		/// <summary>
		/// Gets the collection of tab pages.
		/// </summary>
		/// <value>The pages.</value>
		public IList<DocumentPage> Pages
		{
			get { return pages ?? (pages = new DocumentPageCollection(this.Handler)); }
		}

		/// <summary>
		/// Remove the specified child from the container.
		/// </summary>
		/// <param name="child">Child to remove.</param>
		public override void Remove(Control child)
		{
			var page = child as DocumentPage;
			if (page != null)
			{
				Pages.Remove(page);
			}
		}

		/// <summary>
		/// Gets the binding for the <see cref="SelectedIndex"/> property.
		/// </summary>
		/// <value>The selected index binding.</value>
		public BindableBinding<DocumentControl, int> SelectedIndexBinding
		{
			get
			{
				return new BindableBinding<DocumentControl, int>(
					this,
					c => c.SelectedIndex,
					(c, v) => c.SelectedIndex = v,
					(c, h) => c.SelectedIndexChanged += h,
					(c, h) => c.SelectedIndexChanged -= h
				);
			}
		}

		class DocumentPageCollection : IList<DocumentPage>
		{
			readonly IHandler handler;

			internal DocumentPageCollection(IHandler handler)
			{
				this.handler = handler;
			}

			public DocumentPage this[int index]
			{
				get
				{
					return handler.GetPage(index);
				}
				set
				{
					handler.RemovePage(index);
					handler.InsertPage(index, value);
				}
			}

			public int Count
			{
				get { return handler.GetPageCount(); }
			}

			public bool IsReadOnly
			{
				get { return false; }
			}

			public void Add(DocumentPage item)
			{
				handler.InsertPage(Count, item);
			}

			public void Clear()
			{
				while (Count > 0)
					handler.RemovePage(0);
			}

			public bool Contains(DocumentPage item)
			{
				for (int i = 0; i < Count; i++)
					if (handler.GetPage(i) == item)
						return true;

				return false;
			}

			public void CopyTo(DocumentPage[] array, int arrayIndex)
			{
				for (int i = arrayIndex; i < arrayIndex + array.Length; i++)
					array[i] = handler.GetPage(i);
			}

			public IEnumerator<DocumentPage> GetEnumerator()
			{
				for (int i = 0; i < Count; i++)
					yield return handler.GetPage(i);
			}

			public int IndexOf(DocumentPage item)
			{
				for (int i = 0; i < Count; i++)
					if (handler.GetPage(i) == item)
						return i;

				return -1;
			}

			public void Insert(int index, DocumentPage item)
			{
				handler.InsertPage(index, item);
			}

			public bool Remove(DocumentPage item)
			{
				for (int i = 0; i < Count; i++)
				{
					if (handler.GetPage(i) == item)
					{
						handler.RemovePage(i);
						return true;
					}
				}

				return false;
			}

			public void RemoveAt(int index)
			{
				handler.RemovePage(index);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < Count; i++)
					yield return handler.GetPage(i);
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
		/// Callback interface for the <see cref="DocumentControl"/>
		/// </summary>
		public new interface ICallback : Control.ICallback
		{
			/// <summary>
			/// Raises the page closed event.
			/// </summary>
			void OnPageClosed(DocumentControl widget, DocumentPageEventArgs e);

			/// <summary>
			/// Raises the selected index changed event.
			/// </summary>
			void OnSelectedIndexChanged(DocumentControl widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="DocumentControl"/>
		/// </summary>
		protected new class Callback : Control.Callback, ICallback
		{
			/// <summary>
			/// Raises the page closed event.
			/// </summary>
			public void OnPageClosed(DocumentControl widget, DocumentPageEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnPageClosed(e));
			}

			/// <summary>
			/// Raises the selected index changed event.
			/// </summary>
			public void OnSelectedIndexChanged(DocumentControl widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnSelectedIndexChanged(e));
			}
		}

		#endregion

		#region Handler

		/// <summary>
		/// Handler interface for the <see cref="DocumentControl"/>
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
			void InsertPage(int index, DocumentPage page);

			/// <summary>
			/// Gets the tab.
			/// </summary>
			/// <returns>The tab.</returns>
			/// <param name="index">The tab index.</param>
			DocumentPage GetPage(int index);

			/// <summary>
			/// Gets the tab count.
			/// </summary>
			/// <returns>The tab count.</returns>
			int GetPageCount();

			/// <summary>
			/// Removes the specified tab.
			/// </summary>
			/// <param name="index">Index of the page to remove.</param>
			void RemovePage(int index);
		}

		#endregion
	}
}
