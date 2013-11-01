using System;

namespace Eto.Forms
{
	public interface IPrintDocument : IInstanceWidget
	{
		void Print ();

		string Name { get; set; }

		PrintSettings PrintSettings { get; set; }

		int PageCount { get; set; }
	}

	public class PrintDocument : InstanceWidget
	{
		new IPrintDocument Handler { get { return (IPrintDocument)base.Handler; } }

		#region Events

		public const string PrintingEvent = "PrintDocument.Printing";
		EventHandler<EventArgs> printing;

		public event EventHandler<EventArgs> Printing {
			add {
				HandleEvent (PrintingEvent);
				printing += value;
			}
			remove { printing -= value; }
		}

		public virtual void OnPrinting (EventArgs e)
		{
			if (printing != null)
				printing (this, e);
		}

		public const string PrintedEvent = "PrintDocument.Printed";
		EventHandler<EventArgs> printed;

		public event EventHandler<EventArgs> Printed {
			add {
				HandleEvent (PrintedEvent);
				printed += value;
			}
			remove { printed -= value; }
		}

		public virtual void OnPrinted (EventArgs e)
		{
			if (printed != null)
				printed (this, e);
		}

		public const string PrintPageEvent = "PrintDocument.PrintPage";
		EventHandler<PrintPageEventArgs> _PrintPage;

		public event EventHandler<PrintPageEventArgs> PrintPage {
			add {
				HandleEvent (PrintPageEvent);
				_PrintPage += value;
			}
			remove { _PrintPage -= value; }
		}

		public virtual void OnPrintPage (PrintPageEventArgs e)
		{
			if (_PrintPage != null)
				_PrintPage (this, e);
		}

		#endregion

		static PrintDocument()
		{
			EventLookup.Register(typeof(PrintDocument), "OnPrinting", PrintDocument.PrintingEvent);
			EventLookup.Register(typeof(PrintDocument), "OnPrinted", PrintDocument.PrintedEvent);
			EventLookup.Register(typeof(PrintDocument), "OnPrintPage", PrintDocument.PrintPageEvent);
		}

		public PrintDocument ()
			: this (Generator.Current)
		{
		}

		public PrintDocument (Generator generator)
			: base (generator, typeof (IPrintDocument))
		{
		}

		public string Name
		{
			get { return Handler.Name; }
			set { Handler.Name = value; }
		}

		public void Print ()
		{
			Handler.Print ();
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
