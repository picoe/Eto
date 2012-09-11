using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sw = System.Windows;
using swd = System.Windows.Documents;
using swc = System.Windows.Controls;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Printing
{
	public class PrintDocumentHandler : WidgetHandler<swd.DocumentPaginator, PrintDocument>, IPrintDocument
	{
		public PrintDocumentHandler ()
		{
			Control = new Paginator { Handler = this };
		}

		class Paginator : swd.DocumentPaginator
		{
			public PrintDocumentHandler Handler { get; set; }

			public override swd.DocumentPage GetPage (int pageNumber)
			{
				return null;
			}

			public override bool IsPageCountValid
			{
				get { return true; }
			}

			public override int PageCount
			{
				get { return Handler.PageCount; }
			}

			public override sw.Size PageSize
			{
				get; set;
			}

			public override swd.IDocumentPaginatorSource Source
			{
				get { return null; }
			}
		}

		public void Print ()
		{
			var print = new swc.PrintDialog ();
			if (print.ShowDialog () == true)
				print.PrintDocument (Control, this.Name);
		}


		public string Name
		{
			get; set;
		}

		public Size PageSize { get; set; }

		public int PageCount { get; set; }


		public PrintSettings PrintSettings
		{
			get; set;
		}
	}
}
