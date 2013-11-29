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
		
		public SelectFolderDialog()
			: this((Generator)null)
		{
		}

		public SelectFolderDialog (Generator generator) : this (generator, typeof(ISelectFolderDialog))
		{
		}
		
		protected SelectFolderDialog (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
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

