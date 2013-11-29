using System;

namespace Eto.Forms
{
	public interface ISearchBox : ITextBox
	{
	}

	public class SearchBox: TextBox
	{
		public SearchBox()
			: this((Generator)null)
		{
		}

		public SearchBox (Generator generator) 
			: this(generator, typeof(ISearchBox))
		{
			
		}
		protected SearchBox (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}
	}
}
