using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Forms
{
	public interface IPrintDialog : ICommonDialog
	{
		PrintSettings PrintSettings { get; set; }

		bool AllowPageRange { get; set; }
		bool AllowSelection { get; set; }
	}

	public class PrintDialog : CommonDialog
	{
		IPrintDialog handler;

		public PrintDialog ()
			: this (Generator.Current)
		{
		}

		public PrintDialog (Generator generator)
			: base (generator, typeof (IPrintDialog))
		{
			handler = (IPrintDialog)Handler;
		}

		public PrintSettings PrintSettings
		{
			get { return handler.PrintSettings; }
			set { handler.PrintSettings = value; }
		}
	}
}
