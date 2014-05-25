using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Direct2D.Forms.Printing
{
	public class PrintDocumentHandler : Eto.WinForms.Forms.Printing.PrintDocumentHandler
	{
		protected override void HandlePrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			using (((Eto.Direct2D.Platform)Widget.Platform).BasePlatform.Context)
			{
				base.HandlePrintPage(sender, e);
			}
		}
	}
}
