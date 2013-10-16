using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sdp = System.Drawing.Printing;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Printing
{
	public class PrintSettingsHandler : WidgetHandler<sdp.PrinterSettings, PrintSettings>, IPrintSettings
	{
		public PrintSettingsHandler(sdp.PrinterSettings settings)
		{
			Control = settings;
		}

		public static sdp.PrinterSettings DefaultSettings()
		{
			return new sdp.PrinterSettings { MinimumPage = 1, MaximumPage = 1, FromPage = 1, ToPage = 1, Copies = 1, Collate = true };
		}

		public PrintSettingsHandler()
		{
			Control = DefaultSettings();
		}

		public int Copies
		{
			get { return Control.Copies; }
			set { Control.Copies = (short)value; }
		}

		public Range MaximumPageRange
		{
			get { return new Range(Control.MinimumPage, Control.MaximumPage - Control.MinimumPage + 1); }
			set
			{
				Control.MinimumPage = value.Start;
				Control.MaximumPage = value.End;
			}
		}

		public Range SelectedPageRange
		{
			get { return new Range(Control.FromPage, Control.ToPage - Control.FromPage + 1); }
			set
			{
				Control.FromPage = value.Start;
				Control.ToPage = value.End;
			}
		}

		public PageOrientation Orientation
		{
			get { return Control.DefaultPageSettings.Landscape ? PageOrientation.Landscape : PageOrientation.Portrait; }
			set { Control.DefaultPageSettings.Landscape = value == PageOrientation.Landscape; }
		}

		public PrintSelection PrintSelection
		{
			get { return Control.PrintRange.ToEto(); }
			set { Control.PrintRange = value.ToSDP(); }
		}

		public bool Collate
		{
			get { return Control.Collate; }
			set { Control.Collate = value; }
		}

		// not supported by winforms
		public bool Reverse
		{
			get;
			set;
		}

	}
}
