using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Direct2D.Forms.Printing
{
	public class PrintDocumentHandler : Eto.WinForms.Forms.Printing.PrintDocumentHandler
	{
		protected override void Initialize()
		{
			base.Initialize();
			// set to base generator to use gdi graphics
			Platform = ((Eto.Direct2D.Platform)Platform).BaseGenerator;
		}
	}
}
