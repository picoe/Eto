using System;

namespace Eto.Forms
{
	public interface ISelectFolderDialog : IWidget
	{
		string Title { get; set; }

		string Directory { get; set; }

		DialogResult ShowDialog (Window parent);
	}

	[Handler(typeof(ISelectFolderDialog))]
	public class SelectFolderDialog : Widget
	{
		new ISelectFolderDialog Handler { get { return (ISelectFolderDialog)base.Handler; } }
		
		public SelectFolderDialog()
		{
		}

		[Obsolete("Use default constructor instead")]
		public SelectFolderDialog (Generator generator) : this (generator, typeof(ISelectFolderDialog))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
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

