using System;
using System.Collections.ObjectModel;

namespace Eto.Forms.Controls
{
	public interface IMdiChild : IContainer
	{
		// bool AllowClose { get; set; }
	}
	
	public class MdiChild : Container
	{
		public MdiChild ()
			: this (Generator.Current)
		{
		}
		
		public MdiChild (Generator generator)
			: base (generator, typeof(IMdiChild), true)
		{
		}
	}
}

