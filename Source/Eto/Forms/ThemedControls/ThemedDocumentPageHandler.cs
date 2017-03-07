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
		Control control;
		string text;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.ThemedControls.ThemedDocumentPageHandler"/> class.
		/// </summary>
        public ThemedDocumentPageHandler()
        {
			closable = true;
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
				DocControl?.Update(this);
			}
		}

		/// <summary>
		/// Gets or sets the content.
		/// </summary>
		/// <value>The content.</value>
		public Control Content
		{
			get { return control; }
			set
			{
				control = value;
				DocControl?.Update(this);
			}
		}

		/// <summary>
		/// Gets or sets the context menu.
		/// </summary>
		/// <value>The context menu.</value>
		public ContextMenu ContextMenu { get; set; }

		/// <summary>
		/// Gets or sets the tab image.
		/// </summary>
		/// <value>The tab image.</value>
		public Image Image { get; set; }

		/// <summary>
		/// Gets or sets the minimum size.
		/// </summary>
		/// <value>The minimum size.</value>
		public Size MinimumSize { get; set; }

		/// <summary>
		/// Gets or sets the padding.
		/// </summary>
		/// <value>The padding.</value>
		public Padding Padding { get; set; }

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
				DocControl?.Update(this);
			}
		}

		internal RectangleF Rect { get; set; }

		internal bool CloseSelected { get; set; }
	}
}
