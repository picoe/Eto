using System;

namespace Eto.Forms
{
	[Handler(typeof(PrintDocument.IHandler))]
	public class PrintDocument : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		#region Events

		public const string PrintingEvent = "PrintDocument.Printing";

		public event EventHandler<EventArgs> Printing
		{
			add { Properties.AddHandlerEvent(PrintingEvent, value); }
			remove { Properties.RemoveEvent(PrintingEvent, value); }
		}

		protected virtual void OnPrinting(EventArgs e)
		{
			Properties.TriggerEvent(PrintingEvent, this, e);
		}

		public const string PrintedEvent = "PrintDocument.Printed";

		public event EventHandler<EventArgs> Printed
		{
			add { Properties.AddHandlerEvent(PrintedEvent, value); }
			remove { Properties.RemoveEvent(PrintedEvent, value); }
		}

		protected virtual void OnPrinted(EventArgs e)
		{
			Properties.TriggerEvent(PrintedEvent, this, e);
		}

		public const string PrintPageEvent = "PrintDocument.PrintPage";

		public event EventHandler<PrintPageEventArgs> PrintPage
		{
			add { Properties.AddHandlerEvent(PrintPageEvent, value); }
			remove { Properties.RemoveEvent(PrintPageEvent, value); }
		}

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

		public PrintDocument()
		{
		}

		[Obsolete("Use default constructor instead")]
		public PrintDocument(Generator generator)
			: base(generator, typeof(IHandler))
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

		static readonly object callback = new Callback();
		protected override object GetCallback() { return callback; }

		public interface ICallback : Widget.ICallback
		{
			void OnPrinting(PrintDocument widget, EventArgs e);
			void OnPrinted(PrintDocument widget, EventArgs e);
			void OnPrintPage(PrintDocument widget, PrintPageEventArgs e);
		}

		protected class Callback : ICallback
		{
			public void OnPrinting(PrintDocument widget, EventArgs e)
			{
				widget.OnPrinting(e);
			}
			public void OnPrinted(PrintDocument widget, EventArgs e)
			{
				widget.OnPrinted(e);
			}
			public void OnPrintPage(PrintDocument widget, PrintPageEventArgs e)
			{
				widget.OnPrintPage(e);
			}
		}

		public interface IHandler : Widget.IHandler
		{
			void Print();

			string Name { get; set; }

			PrintSettings PrintSettings { get; set; }

			int PageCount { get; set; }
		}

	}
}
