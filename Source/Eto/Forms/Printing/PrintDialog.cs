using System.ComponentModel;
using System;

namespace Eto.Forms
{
	public interface IPrintDialog : ICommonDialog
	{
		PrintDocument Document { get; set; }

		PrintSettings PrintSettings { get; set; }

		bool AllowPageRange { get; set; }

		bool AllowSelection { get; set; }
	}

	[Handler(typeof(IPrintDialog))]
	public class PrintDialog : CommonDialog
	{
		new IPrintDialog Handler { get { return (IPrintDialog)base.Handler; } }

		public PrintDialog()
		{
		}

		[Obsolete("Use default constructor instead")]
		public PrintDialog(Generator generator)
			: base (generator, typeof (IPrintDialog))
		{
		}

		public PrintSettings PrintSettings
		{
			get { return Handler.PrintSettings; }
			set { Handler.PrintSettings = value; }
		}

		public bool AllowSelection
		{
			get { return Handler.AllowSelection; }
			set { Handler.AllowSelection = value; }
		}

		[DefaultValue(true)]
		public bool AllowPageRange
		{
			get { return Handler.AllowPageRange; }
			set { Handler.AllowPageRange = value; }
		}

		public DialogResult ShowDialog(Control parent, PrintDocument document)
		{
			PrintSettings = document.PrintSettings;
			PrintSettings.MaximumPageRange = new Range(1, document.PageCount);
			Handler.Document = document;
			var result = ShowDialog(parent);
			if (result == DialogResult.Ok)
			{
				document.Print();
			}
			return result;
		}
	}
}
