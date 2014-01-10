using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Platform.Direct2D.Forms.Printing
{
	public class PrintDocumentHandler : Eto.Platform.Windows.Forms.Printing.PrintDocumentHandler
	{
		protected override void Initialize()
		{
			base.Initialize();
			// set to base generator to use gdi graphics
			Generator = ((Eto.Platform.Direct2D.Generator)Generator).BaseGenerator;
		}
	}
}
