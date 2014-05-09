using System.ComponentModel;
using System;

namespace Eto.Forms
{
	public enum PageOrientation
	{
		Portrait,
		Landscape
	}

	public enum PrintSelection
	{
		AllPages,
		Selection,
		SelectedPages
	}

	[Handler(typeof(PrintSettings.IHandler))]
	public class PrintSettings : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		public PrintSettings()
		{
		}

		public PrintSettings(IHandler handler)
			: base(handler)
		{
		}

		[Obsolete("Use default constructor instead")]
		public PrintSettings (Generator generator)
			: base (generator, typeof (IHandler))
		{
		}

		[Obsolete("Use PrintSettings(IPrintSettings) instead")]
		public PrintSettings (Generator generator, IHandler handler)
			: base (generator, handler)
		{
		}

		[DefaultValue(1)]
		public int Copies {
			get { return Handler.Copies; }
			set { Handler.Copies = value; }
		}

		public Range MaximumPageRange {
			get { return Handler.MaximumPageRange; }
			set { Handler.MaximumPageRange = value; }
		}

		public Range SelectedPageRange {
			get { return Handler.SelectedPageRange; }
			set { Handler.SelectedPageRange = value; }
		}

		public PageOrientation Orientation {
			get { return Handler.Orientation; }
			set { Handler.Orientation = value; }
		}

		public PrintSelection PrintSelection {
			get { return Handler.PrintSelection; }
			set { Handler.PrintSelection = value; }
		}

		[DefaultValue(true)]
		public bool Collate {
			get { return Handler.Collate; }
			set { Handler.Collate = value; }
		}

		public bool Reverse {
			get { return Handler.Reverse; }
			set { Handler.Reverse = value; }
		}

		public interface IHandler : Widget.IHandler
		{
			int Copies { get; set; }

			Range MaximumPageRange { get; set; }

			Range SelectedPageRange { get; set; }

			PrintSelection PrintSelection { get; set; }

			PageOrientation Orientation { get; set; }

			bool Collate { get; set; }

			bool Reverse { get; set; }
		}

	}
}
