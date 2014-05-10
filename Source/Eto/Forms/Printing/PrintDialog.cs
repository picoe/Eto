using System.ComponentModel;
using System;

namespace Eto.Forms
{
	/// <summary>
	/// Dialog to show when printing a document or adjusting print settings
	/// </summary>
	[Handler(typeof(PrintDialog.IHandler))]
	public class PrintDialog : CommonDialog
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.PrintDialog"/> class.
		/// </summary>
		public PrintDialog()
		{
		}

		[Obsolete("Use default constructor instead")]
		public PrintDialog(Generator generator)
			: base (generator, typeof (IHandler))
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

		public new interface IHandler : CommonDialog.IHandler
		{
			PrintDocument Document { get; set; }

			PrintSettings PrintSettings { get; set; }

			bool AllowPageRange { get; set; }

			bool AllowSelection { get; set; }
		}
	}
}
