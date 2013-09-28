using System;

namespace Eto.Forms
{
	public interface ISelectFolderDialog : IInstanceWidget
	{
		string Title { get; set; }

		string Directory { get; set; }

		DialogResult ShowDialog (Window parent);
	}
	
	public class SelectFolderDialog : InstanceWidget
	{
		new ISelectFolderDialog Handler { get { return (ISelectFolderDialog)base.Handler; } }
		
		public SelectFolderDialog () : this (Generator.Current)
		{
		}
		
		public SelectFolderDialog (Generator g) : this (g, typeof(ISelectFolderDialog))
		{
		}
		
		protected SelectFolderDialog (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
		}
		
		public string Title {
			get { return Handler.Title; }
			set { Handler.Title = value; }
		}
		
		public string Directory {
			get { return Handler.Directory; }
			set { Handler.Directory = value; }
		}
		
		public DialogResult ShowDialog (Window parent)
		{
			return Handler.ShowDialog (parent);
		}
	}
}

