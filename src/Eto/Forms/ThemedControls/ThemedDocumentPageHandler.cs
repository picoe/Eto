using System;
using Eto.Drawing;

namespace Eto.Forms.ThemedControls
{
	/// <summary>
	/// A themed handler for the <see cref="DocumentPage"/> control.
	/// </summary>
	public class ThemedDocumentPageHandler : ThemedContainerHandler<Panel, DocumentPage, DocumentPage.ICallback>, DocumentPage.IHandler
    {
		internal ThemedDocumentControlHandler DocControl;
		bool closable;
		string text;
		Image image;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.ThemedControls.ThemedDocumentPageHandler"/> class.
		/// </summary>
        public ThemedDocumentPageHandler()
        {
			closable = true;
			Control = new Panel();
        }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.ThemedControls.ThemedDocumentPageHandler"/> is closable.
		/// </summary>
		/// <value><c>true</c> if closable; otherwise, <c>false</c>.</value>
		public bool Closable
		{
			get { return closable; }
			set
			{
				closable = value;
				Update();
			}
		}

		/// <summary>
		/// Gets or sets the content.
		/// </summary>
		/// <value>The content.</value>
		public Control Content
		{
			get { return Control.Content; }
			set
			{
				Control.Content = value;
				Update();
			}
		}

		/// <summary>
		/// Gets or sets the context menu.
		/// </summary>
		/// <value>The context menu.</value>
		public ContextMenu ContextMenu
		{
			get { return Control.ContextMenu; }
			set { Control.ContextMenu = value; }
		}

		/// <summary>
		/// Gets or sets the tab image.
		/// </summary>
		/// <value>The tab image.</value>
		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				Update();
			}
		}

		/// <summary>
		/// Gets or sets the minimum size.
		/// </summary>
		/// <value>The minimum size.</value>
		public Size MinimumSize
		{
			get { return Control.MinimumSize; }
			set { Control.MinimumSize = value; }
		}

		/// <summary>
		/// Gets or sets the padding.
		/// </summary>
		/// <value>The padding.</value>
		public Padding Padding
		{
			get { return Control.Padding; }
			set { Control.Padding = value; }
		}

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				Update();
			}
		}

		/// <summary>
		/// Gets a value indicating whether <see cref="Control.PreLoad"/>/<see cref="Control.Load"/>/<see cref="Control.LoadComplete"/>/<see cref="Control.UnLoad"/>
		/// events are propagated to the inner control
		/// </summary>
		public override bool PropagateLoadEvents => base.PropagateLoadEvents;

		internal RectangleF Rect { get; set; }

		internal RectangleF CloseRect { get; set; }

		internal RectangleF TextRect { get; set; }

		void Update() => DocControl?.Update(this);
	}
}
