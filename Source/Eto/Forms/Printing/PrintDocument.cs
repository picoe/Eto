using System;

namespace Eto.Forms
{
	public interface IPrintDocument : IInstanceWidget
	{
		void Print();

		string Name { get; set; }

		PrintSettings PrintSettings { get; set; }

		int PageCount { get; set; }
	}

	public class PrintDocument : InstanceWidget
	{
		new IPrintDocument Handler { get { return (IPrintDocument)base.Handler; } }

		#region Events

		public const string PrintingEvent = "PrintDocument.Printing";

		public event EventHandler<EventArgs> Printing
		{
			add { Properties.AddHandlerEvent(PrintingEvent, value); }
			remove { Properties.RemoveEvent(PrintingEvent, value); }
		}

		public virtual void OnPrinting(EventArgs e)
		{
			Properties.TriggerEvent(PrintingEvent, this, e);
		}

		public const string PrintedEvent = "PrintDocument.Printed";

		public event EventHandler<EventArgs> Printed
		{
			add { Properties.AddHandlerEvent(PrintedEvent, value); }
			remove { Properties.RemoveEvent(PrintedEvent, value); }
		}

		public virtual void OnPrinted(EventArgs e)
		{
			Properties.TriggerEvent(PrintedEvent, this, e);
		}

		public const string PrintPageEvent = "PrintDocument.PrintPage";

		public event EventHandler<PrintPageEventArgs> PrintPage
		{
			add { Properties.AddHandlerEvent(PrintPageEvent, value); }
			remove { Properties.RemoveEvent(PrintPageEvent, value); }
		}

		public virtual void OnPrintPage(PrintPageEventArgs e)
		{
			Properties.TriggerEvent(PrintPageEvent, this, e);
		}

		#endregion

		static PrintDocument()
		{
			EventLookup.Register(typeof(PrintDocument), "OnPrinting", PrintDocument.PrintingEvent);
			EventLookup.Register(typeof(PrintDocument), "OnPrinted", PrintDocument.PrintedEvent);
			EventLookup.Register(typeof(PrintDocument), "OnPrintPage", PrintDocument.PrintPageEvent);
		}

		public PrintDocument()
			: this((Generator)null)
		{
		}

		public PrintDocument(Generator generator)
			: base(generator, typeof(IPrintDocument))
		{
		}

		public string Name
		{
			get { return Handler.Name; }
			set { Handler.Name = value; }
		}

		public void Print()
		{
			Handler.Print();
		}

		public PrintSettings PrintSettings
		{
			get { return Handler.PrintSettings; }
			set { Handler.PrintSettings = value; }
		}

		public virtual int PageCount
		{
			get { return Handler.PageCount; }
			set { Handler.PageCount = value; }
		}
	}
}
