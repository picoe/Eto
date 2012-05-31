using System;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface ISeparatorMenuItem : IMenuItem
	{
	}
	
	public class SeparatorMenuItem : MenuItem
	{
		
		public SeparatorMenuItem () : this (Generator.Current)
		{
		}

		public SeparatorMenuItem (Generator g) : this (g, typeof(ISeparatorMenuItem))
		{
		}
		
		protected SeparatorMenuItem (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			
		}

	}
}
