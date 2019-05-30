using System;

namespace Eto.Forms
{
	/// <summary>
	/// Represents a document that can be printed
	/// </summary>
	/// <remarks>
	/// A print document uses the <see cref="Drawing.Graphics"/> to render its output via the
	/// <see cref="PrintPage"/> event.
	/// </remarks>
	[Handler(typeof(PrintDocument.IHandler))]
	public class PrintDocument : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		#region Events

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Printing"/> event
		/// </summary>
		public const string PrintingEvent = "PrintDocument.Printing";

		/// <summary>
		/// Occurs before printing has started
		/// </summary>
		public event EventHandler<EventArgs> Printing
		{
			add { Properties.AddHandlerEvent(PrintingEvent, value); }
			remove { Properties.RemoveEvent(PrintingEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Printing"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnPrinting(EventArgs e)
		{
			Properties.TriggerEvent(PrintingEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Printed"/> event
		/// </summary>
		public const string PrintedEvent = "PrintDocument.Printed";

		/// <summary>
		/// Occurs after the document has been printed
		/// </summary>
		public event EventHandler<EventArgs> Printed
		{
			add { Properties.AddHandlerEvent(PrintedEvent, value); }
			remove { Properties.RemoveEvent(PrintedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Printed"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnPrinted(EventArgs e)
		{
			Properties.TriggerEvent(PrintedEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="PrintPage"/> event
		/// </summary>
		public const string PrintPageEvent = "PrintDocument.PrintPage";

		/// <summary>
		/// Occurs for each printed page
		/// </summary>
		public event EventHandler<PrintPageEventArgs> PrintPage
		{
			add { Properties.AddHandlerEvent(PrintPageEvent, value); }
			remove { Properties.RemoveEvent(PrintPageEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="PrintPage"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnPrintPage(PrintPageEventArgs e)
		{
			Properties.TriggerEvent(PrintPageEvent, this, e);
		}

		#endregion

		static PrintDocument()
		{
			EventLookup.Register<PrintDocument>(c => c.OnPrinting(null), PrintDocument.PrintingEvent);
			EventLookup.Register<PrintDocument>(c => c.OnPrinted(null), PrintDocument.PrintedEvent);
			EventLookup.Register<PrintDocument>(c => c.OnPrintPage(null), PrintDocument.PrintPageEvent);
		}

		/// <summary>
		/// Gets or sets the name of the document to show in the printer queue
		/// </summary>
		/// <value>The name of the document</value>
		public string Name
		{
			get { return Handler.Name; }
			set { Handler.Name = value; }
		}

		/// <summary>
		/// Prints this document immediately using the current <see cref="PrintSettings"/>
		/// </summary>
		/// <remarks>
		/// This skips the print dialog, so if you want the user to adjust settings before printing, use
		/// <see cref="PrintDialog.ShowDialog(Control,PrintDocument)"/>.
		/// </remarks>
		public void Print()
		{
			Handler.Print();
		}

		/// <summary>
		/// Gets or sets the print settings for the document when printing.
		/// </summary>
		/// <remarks>
		/// You can adjust the settings using the <see cref="PrintDialog"/>, or use <see cref="PrintDialog.ShowDialog(Control,PrintDocument)"/>
		/// to allow the user to adjust the settings before printing.
		/// </remarks>
		/// <value>The print settings.</value>
		public PrintSettings PrintSettings
		{
			get { return Handler.PrintSettings; }
			set { Handler.PrintSettings = value; }
		}

		/// <summary>
		/// Gets or sets the total number of pages available to be printed in this document.
		/// </summary>
		/// <remarks>
		/// This must be set to the number of pages your document contains before printing or showing the print dialog.
		/// </remarks>
		/// <value>The page count.</value>
		public virtual int PageCount
		{
			get { return Handler.PageCount; }
			set { Handler.PageCount = value; }
		}

		#region Callback

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Interface for handlers to trigger events
		/// </summary>
		public new interface ICallback : Widget.ICallback
		{
			/// <summary>
			/// Raises the printing event, which should occur before the document is printed.
			/// </summary>
			void OnPrinting(PrintDocument widget, EventArgs e);
			/// <summary>
			/// Raises the printed event, which should occur after the document is fully printed.
			/// </summary>
			void OnPrinted(PrintDocument widget, EventArgs e);
			/// <summary>
			/// Raises the print page event, which should be called for each page in the selected page range to render its contents.
			/// </summary>
			void OnPrintPage(PrintDocument widget, PrintPageEventArgs e);
		}

		/// <summary>
		/// Callback methods for handlers of <see cref="PrintDocument"/>
		/// </summary>
		protected class Callback : ICallback
		{
			/// <summary>
			/// Raises the printing event.
			/// </summary>
			public void OnPrinting(PrintDocument widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnPrinting(e);
			}
			/// <summary>
			/// Raises the printed event.
			/// </summary>
			public void OnPrinted(PrintDocument widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnPrinted(e);
			}
			/// <summary>
			/// Raises the print page event.
			/// </summary>
			public void OnPrintPage(PrintDocument widget, PrintPageEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnPrintPage(e);
			}
		}

		#endregion

		#region Handler

		/// <summary>
		/// Handler interface for the <see cref="PrintDocument"/> widget
		/// </summary>
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Prints this document immediately using the current <see cref="PrintSettings"/>
			/// </summary>
			/// <remarks>
			/// This should not show a print dialog, and should add the document to the print queue immediately using
			/// the current <see cref="PrintSettings"/>
			/// </remarks>
			void Print();

			/// <summary>
			/// Gets or sets the name of the document to show in the printer queue
			/// </summary>
			/// <value>The name of the document</value>
			string Name { get; set; }

			/// <summary>
			/// Gets or sets the print settings for the document when printing.
			/// </summary>
			/// <value>The print settings.</value>
			PrintSettings PrintSettings { get; set; }

			/// <summary>
			/// Gets or sets the total number of pages available to be printed in this document.
			/// </summary>
			/// <remarks>
			/// This will be set to the number of pages your document contains before printing or showing the print dialog.
			/// </remarks>
			/// <value>The page count.</value>
			int PageCount { get; set; }
		}

		#endregion
	}
}
