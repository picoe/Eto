using System.ComponentModel;
using System;

namespace Eto.Forms
{
	/// <summary>
	/// Orientation of the printed page.
	/// </summary>
	public enum PageOrientation
	{
		/// <summary>
		/// Print the page in portrait mode.
		/// </summary>
		Portrait,
		/// <summary>
		/// Print the page in landscape mode.
		/// </summary>
		Landscape
	}

	/// <summary>
	/// Selection mode when printing
	/// </summary>
	public enum PrintSelection
	{
		/// <summary>
		/// Print all pages
		/// </summary>
		AllPages,
		/// <summary>
		/// Print the selection (defined by the application)
		/// </summary>
		Selection,
		/// <summary>
		/// Print the selected pages from <see cref="PrintSettings.SelectedPageRange"/>
		/// </summary>
		SelectedPages
	}

	/// <summary>
	/// Settings for printing a <see cref="PrintDocument"/>
	/// </summary>
	/// <remarks>
	/// This defines the parameters for printing such as how many copies, page range, orientation, etc.
	/// </remarks>
	[Handler(typeof(PrintSettings.IHandler))]
	public class PrintSettings : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.PrintSettings"/> class.
		/// </summary>
		public PrintSettings()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.PrintSettings"/> class.
		/// </summary>
		/// <param name="handler">Handler.</param>
		public PrintSettings(IHandler handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Gets or sets the number of copies to print.
		/// </summary>
		/// <value>The number of copies.</value>
		[DefaultValue(1)]
		public int Copies
		{
			get { return Handler.Copies; }
			set { Handler.Copies = value; }
		}

		/// <summary>
		/// Gets or sets the maximum page range the user can select.
		/// </summary>
		/// <value>The maximum page range.</value>
		public Range<int> MaximumPageRange
		{
			get { return Handler.MaximumPageRange; }
			set { Handler.MaximumPageRange = value; }
		}

		/// <summary>
		/// Gets or sets the user's selected page range.
		/// </summary>
		/// <remarks>
		/// This will control which pages get rendered with the <see cref="PrintDocument.PrintPage"/>
		/// </remarks>
		/// <value>The selected page range.</value>
		public Range<int> SelectedPageRange
		{
			get { return Handler.SelectedPageRange; }
			set { Handler.SelectedPageRange = value; }
		}

		/// <summary>
		/// Gets or sets the orientation of the page when printing
		/// </summary>
		/// <value>The page orientation.</value>
		public PageOrientation Orientation
		{
			get { return Handler.Orientation; }
			set { Handler.Orientation = value; }
		}

		/// <summary>
		/// Gets or sets the print selection mode
		/// </summary>
		/// <value>The print selection.</value>
		public PrintSelection PrintSelection
		{
			get { return Handler.PrintSelection; }
			set { Handler.PrintSelection = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to collate the copies.
		/// </summary>
		/// <remarks>
		/// When <c>true</c>, all pages of the document will be printed together for each copy. e.g. 123, 123, 123.
		/// When <c>false</c>, each page will print all copies together, e.g. 111, 222, 333.
		/// </remarks>
		/// <value><c>true</c> if collate; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool Collate
		{
			get { return Handler.Collate; }
			set { Handler.Collate = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to print in reverse.
		/// </summary>
		/// <remarks>
		/// Printing in reverse will typically finish the job with the first page of the document at the top, since most
		/// printers stack the pages from bottom to top.
		/// </remarks>
		/// <value><c>true</c> to print in reverse; otherwise, <c>false</c>.</value>
		public bool Reverse
		{
			get { return Handler.Reverse; }
			set { Handler.Reverse = value; }
		}

		#region Handler

		/// <summary>
		/// Handler interface for the <see cref="PrintSettings"/> class.
		/// </summary>
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Gets or sets the number of copies to print.
			/// </summary>
			/// <value>The copies.</value>
			int Copies { get; set; }

			/// <summary>
			/// Gets or sets the maximum page range the user can select.
			/// </summary>
			/// <value>The maximum page range.</value>
			Range<int> MaximumPageRange { get; set; }

			/// <summary>
			/// Gets or sets the user's selected page range.
			/// </summary>
			/// <value>The selected page range.</value>
			Range<int> SelectedPageRange { get; set; }

			/// <summary>
			/// Gets or sets the print selection mode
			/// </summary>
			/// <value>The print selection.</value>
			PrintSelection PrintSelection { get; set; }

			/// <summary>
			/// Gets or sets the orientation of the page when printing
			/// </summary>
			/// <value>The page orientation.</value>
			PageOrientation Orientation { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether to collate the copies.
			/// </summary>
			/// <remarks>
			/// When <c>true</c>, all pages of the document will be printed together for each copy. e.g. 123, 123, 123.
			/// When <c>false</c>, each page will print all copies together, e.g. 111, 222, 333.
			/// </remarks>
			/// <value><c>true</c> if collate; otherwise, <c>false</c>.</value>
			bool Collate { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether to print in reverse.
			/// </summary>
			/// <remarks>
			/// Printing in reverse will typically finish the job with the first page of the document at the top, since most
			/// printers stack the pages from bottom to top.
			/// 
			/// Each platform can have a different default for this, depending on the system settings or usual defaults of other applications.
			/// </remarks>
			/// <value><c>true</c> to print in reverse; otherwise, <c>false</c>.</value>
			bool Reverse { get; set; }
		}

		#endregion
	}
}
