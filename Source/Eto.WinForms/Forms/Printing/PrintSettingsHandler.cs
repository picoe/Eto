using sdp = System.Drawing.Printing;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms.Forms.Printing
{
	public class PrintSettingsHandler : WidgetHandler<sdp.PrinterSettings, PrintSettings>, PrintSettings.IHandler
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

		public Range<int> MaximumPageRange
		{
			get { return new Range<int>(Control.MinimumPage, Control.MaximumPage); }
			set
			{
				Control.MinimumPage = value.Start;
				Control.MaximumPage = value.End;
			}
		}

		public Range<int> SelectedPageRange
		{
			get { return new Range<int>(Control.FromPage, Control.ToPage); }
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
