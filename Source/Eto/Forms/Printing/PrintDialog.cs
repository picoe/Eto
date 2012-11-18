using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

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
		new IPrintDialog Handler { get { return (IPrintDialog)base.Handler; } }

		public PrintDialog ()
			: this (Generator.Current)
		{
		}

		public PrintDialog (Generator generator)
			: base (generator, typeof (IPrintDialog))
		{
		}

		public PrintSettings PrintSettings
		{
			get { return Handler.PrintSettings; }
			set { Handler.PrintSettings = value; }
		}

		public bool AllowSelection {
			get { return Handler.AllowSelection; }
			set { Handler.AllowSelection = value; }
		}

		[DefaultValue(true)]
		public bool AllowPageRange {
			get { return Handler.AllowPageRange; }
			set { Handler.AllowPageRange = value; }
		}

		public DialogResult ShowDialog (Control parent, PrintDocument document)
		{
			this.PrintSettings.MaximumPageRange = new Range (1, document.PageCount);
			this.PrintSettings = document.PrintSettings;
			var result = this.ShowDialog (parent);
			if (result == DialogResult.Ok) {
				document.Print ();
			}
			return result;
		}

	}
}
