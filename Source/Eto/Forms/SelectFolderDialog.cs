using System;

namespace Eto.Forms
{
	[Handler(typeof(SelectFolderDialog.IHandler))]
	public class SelectFolderDialog : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }
		
		public SelectFolderDialog()
		{
		}

		[Obsolete("Use default constructor instead")]
		public SelectFolderDialog (Generator generator) : this (generator, typeof(SelectFolderDialog.IHandler))
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

		public interface IHandler : Widget.IHandler
		{
			string Title { get; set; }

			string Directory { get; set; }

			DialogResult ShowDialog (Window parent);
		}
	}
}

