using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Forms
{
	public interface IPrintSettings : IInstanceWidget
	{
	}

	public class PrintSettings : InstanceWidget
	{
		IPrintSettings handler;

		public PrintSettings ()
			: this (Generator.Current)
		{
		}

		public PrintSettings (Generator generator)
			: base (generator, typeof (IPrintSettings))
		{
			handler = (IPrintSettings)Handler;
		}
	}
}
