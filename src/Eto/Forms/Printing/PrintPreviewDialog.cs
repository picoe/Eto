namespace Eto.Forms;

/// <summary>
/// Dialog to show a print preview dialog which the user can print from
/// </summary>
[Handler(typeof(PrintPreviewDialog.IHandler))]
public class PrintPreviewDialog : CommonDialog
{
	new IHandler Handler => (IHandler)base.Handler;
		
		
	/// <summary>
	/// Gets the document the preview dialog is presenting
	/// </summary>
	/// <value></value>
	public PrintDocument Document { get; }
		
	/// <summary>
	/// Initializes a new instance of the PrintPreviewDialog for the specified <paramref name="document"/>.
	/// </summary>
	/// <param name="document">Print document to preview</param>
	public PrintPreviewDialog(PrintDocument document)
	{
		Document = document ?? throw new ArgumentNullException(nameof(document));
	}

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
	/// Shows the print preview dialog with the specified <paramref name="parent"/>
	/// </summary>
	/// <param name="parent">Parent window</param>
	/// <returns>The dialog resultP</returns>
	public override DialogResult ShowDialog(Window parent)
	{
		Document.OnBeforePrint();
		PrintSettings = Document.PrintSettings;
		PrintSettings.MaximumPageRange = new Range<int>(1, Document.PageCount);
		Handler.Document = Document;
		var result = base.ShowDialog(parent);
		Document.OnAfterPrint();
		return result;
	}

	/// <summary>
	/// Handler for the <see cref="PrintPreviewDialog"/>.
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
	}
}