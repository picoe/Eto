using System;
using System.Collections;

namespace Eto.Forms
{
	public interface IComboBox : IListControl
	{
	}
	
	public class ComboBox : ListControl
	{
		//private IComboBox inner;
		
		public ComboBox () : this (Generator.Current)
		{
		}

		public ComboBox (Generator g) : this (g, typeof(IComboBox))
		{
		}
		
		protected ComboBox (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
			//inner = (IComboBox)base.InnerControl;
		}
	}

}
