using System;
using Eto.Drawing;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	/// <summary>
	/// Control for a page in a <see cref="DocumentControl"/>
	/// </summary>
	[Handler(typeof(DocumentPage.IHandler))]
	public class DocumentPage : Panel
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DocumentPage"/> class.
		/// </summary>
		/// <param name="control">Control.</param>
		/// <param name="padding">Padding.</param>
		public DocumentPage(Control control, Padding? padding = null)
		{
			if (padding != null)
				this.Padding = padding.Value;
			this.Content = control;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DocumentPage"/> class.
		/// </summary>
		public DocumentPage()
		{
		}

		static object CloseEvent = new object();

		static object ClickEvent = new object();

		/// <summary>
		/// Occurs when the tab is closed.
		/// </summary>
		public event EventHandler<EventArgs> Closed
		{
			add { Properties.AddEvent(CloseEvent, value); }
			remove { Properties.RemoveEvent(CloseEvent, value); }
		}

		/// <summary>
		/// Occurs when the tab is clicked to select it.
		/// </summary>
		public event EventHandler<EventArgs> Click
		{
			add { Properties.AddEvent(ClickEvent, value); }
			remove { Properties.RemoveEvent(ClickEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Closed"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnClose(EventArgs e)
		{
			Properties.TriggerEvent(CloseEvent, this, e);
		}

		/// <summary>
		/// Raises the <see cref="Click"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnClick(EventArgs e)
		{
			Properties.TriggerEvent(ClickEvent, this, e);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.DocumentPage"/> is closable.
		/// </summary>
		/// <value><c>true</c> if closable; otherwise, <c>false</c>.</value>
		public bool Closable
		{
			get { return Handler.Closable; }
			set { Handler.Closable = value; }
		}

		/// <summary>
		/// Gets or sets the image of the page.
		/// </summary>
		/// <remarks>
		/// It is usally good to use an <see cref="Icon"/> for the image with multiple sizes, so that scaling won't be needed
		/// to fit the image in the space.
		/// Usually you'd need 16x16 (desktop), 32x32 (iOS), and 64x64 (iOS Retina) variations.
		/// </remarks>
		/// <value>The tab's image to display.</value>
		public Image Image
		{
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}

		/// <summary>
		/// Gets or sets the title text of the page.
		/// </summary>
		/// <value>The title text.</value>
		public string Text
		{
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="DocumentPage"/>
		/// </summary>
		public new interface IHandler : Panel.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.DocumentPage.IHandler"/> is closable.
			/// </summary>
			/// <value><c>true</c> if closable; otherwise, <c>false</c>.</value>
			bool Closable { get; set; }

			/// <summary>
			/// Gets or sets the image of the page.
			/// </summary>
			/// <value>The tab's image.</value>
			Image Image { get; set; }

			/// <summary>
			/// Gets or sets the title text of the page.
			/// </summary>
			/// <value>The title text.</value>
			string Text { get; set; }
		}

		internal void TriggerClose(EventArgs e)
		{
			OnClose(e);
		}

		internal void TriggerClick(EventArgs e)
		{
			OnClick(e);
		}
	}
}
