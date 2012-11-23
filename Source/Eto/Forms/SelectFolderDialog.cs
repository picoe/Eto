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
		ISelectFolderDialog handler;
		
		public SelectFolderDialog () : this (Generator.Current)
		{
		}
		
		public SelectFolderDialog (Generator g) : this (g, typeof(ISelectFolderDialog))
		{
		}
		
		protected SelectFolderDialog (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
			handler = (ISelectFolderDialog)Handler;
		}
		
		public string Title {
			get { return handler.Title; }
			set { handler.Title = value; }
		}
		
		public string Directory {
			get { return handler.Directory; }
			set { handler.Directory = value; }
		}
		
		public DialogResult ShowDialog (Window parent)
		{
			return handler.ShowDialog (parent);
		}
	}
}

