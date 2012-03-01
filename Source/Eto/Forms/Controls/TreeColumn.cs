using System;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	public class TreeColumnCollection : ObservableCollection<TreeColumn>
	{
		
	}
	
	public interface ITreeColumn : IDataColumn
	{
		
	}
	
	public class TreeColumn : DataColumn
	{
		public TreeColumn ()
			: this (Generator.Current)
		{
		}
		
		public TreeColumn (Generator generator)
			: base(generator, typeof(ITreeColumn), true)
		{
		}
	}
}

