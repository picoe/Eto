using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Forms
{
	public interface ISearchBox : ITextBox
	{
	}

	public class SearchBox: TextBox
	{
		public SearchBox () : this(Generator.Current)
		{
		}
		
		public SearchBox (Generator g) 
			: this(g, typeof(ISearchBox))
		{
			
		}
		protected SearchBox (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
		}
	}
}
