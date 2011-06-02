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
		
		public ComboBox() : this(Generator.Current)
		{
		}

		public ComboBox(Generator g) : base(g, typeof(IComboBox))
		{
			//inner = (IComboBox)base.InnerControl;
		}
	}

}
