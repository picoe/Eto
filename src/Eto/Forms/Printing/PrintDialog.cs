using System.ComponentModel;
using System;

namespace Eto.Forms
{
	/// <summary>
	/// Dialog to show when printing a document or adjusting print settings
	/// </summary>
	[Handler(typeof(PrintDialog.IHandler))]
	public class PrintDialog : CommonDialog
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets or sets the print settings the print dialog is modifying.
		/// </summary>
		/// <value>The print settings.</value>
		public PrintSettings PrintSettings
		{
			get { return Handler.PrintSettings; }
			set { Handler.PrintSettings = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the user can select to print the current selection.
		/// </summary>
		/// <remarks>
		/// If this is true, the <see cref="Eto.Forms.PrintSettings.PrintSelection"/> can be set by the user to <see cref="PrintSelection.Selection"/>.
		/// You must handle this case and only generate the pages for your selected content.
		/// </remarks>
		/// <value><c>true</c> if the user can select to print the selection; otherwise, <c>false</c>.</value>
		public bool AllowSelection
		{
			get { return Handler.AllowSelection; }
			set { Handler.AllowSelection = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the user can select the page range.
		/// </summary>
		/// <value><c>true</c> to allow the user to select the page range; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool AllowPageRange
		{
			get { return Handler.AllowPageRange; }
			set { Handler.AllowPageRange = value; }
		}

		/// <summary>
		/// Shows the print dialog for the specified <paramref name="document"/>, printing after closed if the user selects to print.
		/// </summary>
		/// <returns>The result.</returns>
		/// <param name="parent">Parent of the dialog to make modal.</param>
		/// <param name="document">Document to print.</param>
		public DialogResult ShowDialog(Control parent, PrintDocument document)
		{
			PrintSettings = document.PrintSettings;
			PrintSettings.MaximumPageRange = new Range<int>(1, document.PageCount);
			Handler.Document = document;
			var result = ShowDialog(parent);
			if (result == DialogResult.Ok)
			{
				document.Print();
			}
			return result;
		}

		/// <summary>
		/// Handler for the <see cref="PrintDialog"/>.
		/// </summary>
		public new interface IHandler : CommonDialog.IHandler
		{
			/// <summary>
			/// Gets or sets the document that is being printed.
			/// </summary>
			/// <value>The document to print.</value>
			PrintDocument Document { get; set; }

			/// <summary>
			/// Gets or sets the print settings the print dialog is modifying.
			/// </summary>
			/// <remarks>
			/// This should always return an instance, and is not required to be set by the user.
			/// </remarks>
			/// <value>The print settings.</value>
			PrintSettings PrintSettings { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the user can select the page range.
			/// </summary>
			/// <value><c>true</c> to allow the user to select the page range; otherwise, <c>false</c>.</value>
			bool AllowPageRange { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the user can select to print the current selection.
			/// </summary>
			/// <remarks>
			/// If this is true, the <see cref="Eto.Forms.PrintSettings.PrintSelection"/> can be set by the user to <see cref="PrintSelection.Selection"/>.
			/// You must handle this case and only generate the pages for your selected content.
			/// </remarks>
			/// <value><c>true</c> if the user can select to print the selection; otherwise, <c>false</c>.</value>
			bool AllowSelection { get; set; }
		}
	}
}
