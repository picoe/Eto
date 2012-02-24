using System;

namespace Eto.Forms.Controls
{
	public interface ITabbedMdiContainer : IMdiContainer
	{
	}
	
	public class TabbedMdiContainer : MdiContainer
	{
		public TabbedMdiContainer ()
			: this (Generator.Current)
		{
		}
		
		public TabbedMdiContainer (Generator generator)
			: base (generator, typeof(ITabbedMdiContainer), true)
		{
		}
	}
}

