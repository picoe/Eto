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

		public const string BeginPrintEvent = "PrintDocument.BeginPrint";
		EventHandler<EventArgs> _BeginPrint;

		public event EventHandler<EventArgs> BeginPrint {
			add {
				HandleEvent (BeginPrintEvent);
				_BeginPrint += value;
			}
			remove { _BeginPrint -= value; }
		}

		public virtual void OnBeginPrint (EventArgs e)
		{
			if (_BeginPrint != null)
				_BeginPrint (this, e);
		}

		public const string EndPrintEvent = "PrintDocument.EndPrint";
		EventHandler<EventArgs> _EndPrint;

		public event EventHandler<EventArgs> EndPrint {
			add {
				HandleEvent (EndPrintEvent);
				_EndPrint += value;
			}
			remove { _EndPrint -= value; }
		}

		public virtual void OnEndPrint (EventArgs e)
		{
			if (_EndPrint != null)
				_EndPrint (this, e);
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
