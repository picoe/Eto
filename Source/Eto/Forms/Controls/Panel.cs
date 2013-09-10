using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IPanel : IDockContainer
	{
	}
	
	public class Panel : DockContainer
	{
		
		public Panel () : this (Generator.Current)
		{
		}
		
		public Panel (Generator g) : this (g, typeof(IPanel))
		{
		}
		
		protected Panel (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			
		}

	}
}
