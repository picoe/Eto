using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Forms
{
	public interface IPrintSettings : IInstanceWidget
	{
		int Copies { get; set; }

		Range PageRange { get; set; }

		PageOrientation Orientation { get; set; }

		bool Collate { get; set; }
	}

	public enum PageOrientation
	{
		Portrait,
		Landscape
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

		public int Copies
		{
			get { return Handler.Copies; }
			set { Handler.Copies = value; }
		}

		public Range PageRange
		{
			get { return Handler.PageRange; }
			set { Handler.PageRange = value; }
		}

		public PageOrientation Orientation
		{
			get { return Handler.Orientation; }
			set { Handler.Orientation = value; }
		}

		public bool Collate
		{
			get { return Handler.Collate; }
			set { Handler.Collate = value; }
		}
	}
}
