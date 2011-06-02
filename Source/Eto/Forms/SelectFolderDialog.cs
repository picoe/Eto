using System;

namespace Eto.Forms
{
	public interface ISelectFolderDialog : IWidget
	{
		string Title { get; set; }
		string Directory { get; set; }
		DialogResult ShowDialog(Window parent);
	}
	
	public class SelectFolderDialog : Widget
	{
		ISelectFolderDialog inner;
		
		public SelectFolderDialog()
			: this(Generator.Current)
		{
		}
		
		public SelectFolderDialog (Generator g)
			: base(g, typeof(ISelectFolderDialog))
		{
			inner = (ISelectFolderDialog)Handler;
		}
		
		public string Title
		{
			get { return inner.Title; }
			set { inner.Title = value; }
		}
		
		public string Directory
		{
			get { return inner.Directory; }
			set { inner.Directory = value; }
		}
		
		public DialogResult ShowDialog(Window parent)
		{
			return inner.ShowDialog(parent);
		}
	}
}

