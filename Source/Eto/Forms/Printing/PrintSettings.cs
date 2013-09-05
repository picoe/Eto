using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Eto.Forms
{
	public interface IPrintSettings : IInstanceWidget
	{
		int Copies { get; set; }

		Range MaximumPageRange { get; set; }

		Range SelectedPageRange { get; set; }

		PrintSelection PrintSelection { get; set; }

		PageOrientation Orientation { get; set; }

		bool Collate { get; set; }

		bool Reverse { get; set; }
	}

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

	public class PrintSettings : InstanceWidget
	{
		new IPrintSettings Handler { get { return (IPrintSettings)base.Handler; } }

		public PrintSettings ()
			: this (Generator.Current)
		{
		}

		public PrintSettings (Generator generator)
			: base (generator, typeof (IPrintSettings))
		{
		}

		public PrintSettings (Generator generator, IPrintSettings handler)
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
	}
}
