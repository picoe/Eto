using System;
using Eto.Drawing;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	/// <summary>
	/// Control for a page in a <see cref="TabControl"/>
	/// </summary>
	[Handler(typeof(TabPage.IHandler))]
	public class TabPage : Panel
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TabPage"/> class.
		/// </summary>
		/// <param name="control">Control.</param>
		/// <param name="padding">Padding.</param>
		[Obsolete("Use initializer pattern instead")]
		public TabPage(Control control, Padding? padding = null)
		{
			if (padding != null)
				this.Padding = padding.Value;
			this.Content = control;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TabPage"/> class.
		/// </summary>
		public TabPage()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TabPage"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public TabPage(Generator generator)
			: this(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TabPage"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected TabPage(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Occurs when the tab is clicked to select it.
		/// </summary>
		public event EventHandler<EventArgs> Click;

		/// <summary>
		/// Raises the <see cref="Click"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnClick(EventArgs e)
		{
			if (Click != null)
				Click(this, e);
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
		/// Gets or sets the key of the tab.
		/// </summary>
		/// <value>The key.</value>
		[Obsolete]
		public virtual string Key { get; set; }

		/// <summary>
		/// Handler interface for the <see cref="TabPage"/>
		/// </summary>
		public new interface IHandler : Panel.IHandler
		{
			/// <summary>
			/// Gets or sets the title text of the page.
			/// </summary>
			/// <value>The title text.</value>
			string Text { get; set; }

			/// <summary>
			/// Gets or sets the image of the page.
			/// </summary>
			/// <value>The tab's image.</value>
			Image Image { get; set; }
		}
	}
}
