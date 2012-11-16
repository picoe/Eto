using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

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
		IPrintDocument handler;
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
			handler = (IPrintDocument)Handler;
		}

		public string Name
		{
			get { return handler.Name; }
			set { handler.Name = value; }
		}

		public void Print ()
		{
			handler.Print ();
		}

		public DialogResult ShowPrintDialog (Control parent)
		{
			var dialog = new PrintDialog (Generator);
			this.PrintSettings.PageRange = new Range (1, PageCount);
			dialog.PrintSettings = this.PrintSettings;
			var result = dialog.ShowDialog (parent);
			if (result == DialogResult.Ok) {
				Print ();
			}
			return result;
		}

		public PrintSettings PrintSettings
		{
			get { return handler.PrintSettings; }
			set { handler.PrintSettings = value; }
		}

		public virtual int PageCount
		{
			get { return handler.PageCount; }
			set { handler.PageCount = value; }
		}
	}
}
