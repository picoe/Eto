using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sdp = System.Drawing.Printing;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Printing
{
	public static class PrintSettingsExtensions
	{
		public static PrintSettings ToEto (this sdp.PrinterSettings settings, Eto.Generator generator)
		{
			if (settings == null)
				return null;
			return new PrintSettings (generator, new PrintSettingsHandler (settings));
		}

		public static sdp.PrinterSettings ToSD (this PrintSettings settings)
		{
			if (settings == null)
				return PrintSettingsHandler.DefaultSettings ();
			else
				return ((PrintSettingsHandler)settings.Handler).Control;
		}
	}

	public class PrintSettingsHandler : WidgetHandler<sdp.PrinterSettings, PrintSettings>, IPrintSettings
	{
		public PrintSettingsHandler (sdp.PrinterSettings settings)
		{
			Control = settings;
		}

		public static sdp.PrinterSettings DefaultSettings ()
		{
			return new sdp.PrinterSettings { MinimumPage = 1, MaximumPage = 1, FromPage = 1, ToPage = 1, Copies = 1 };
		}

		public PrintSettingsHandler ()
		{
			Control = DefaultSettings ();
		}

		public int Copies
		{
			get { return Control.Copies; }
			set { Control.Copies = (short)value; }
		}

		public Range PageRange
		{
			get { return new Range (Control.MinimumPage, Control.MaximumPage - Control.MinimumPage + 1); }
			set
			{
				Control.MinimumPage = value.Location;
				Control.MaximumPage = value.Location + value.Length - 1;
			}
		}

		public PageOrientation Orientation
		{
			get { return Control.DefaultPageSettings.Landscape ? PageOrientation.Landscape : PageOrientation.Portrait; }
			set { Control.DefaultPageSettings.Landscape = value == PageOrientation.Landscape; }
		}

		public bool Collate
		{
			get { return Control.Collate; }
			set { Control.Collate = value; }
		}
	}
}
