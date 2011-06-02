using System;

namespace Eto.Forms
{
	
	public interface IFileDialog : IWidget
	{
		string FileName { get; set; }
		string[] Filters { get; set; }
		int CurrentFilterIndex { get; set; }
		bool CheckFileExists { get; set; }
		string Title { get; set; }
		DialogResult ShowDialog(Window parent);
	}
	
	public abstract class FileDialog : Widget
	{
		IFileDialog inner;
		protected FileDialog(Generator g, Type type) : base(g, type)
		{
			inner = (IFileDialog)Handler;
		}

		public string FileName
		{
			get { return inner.FileName; }
			set { inner.FileName = value; }
		}

		public string[] Filters
		{
			get { return inner.Filters; }
			set { inner.Filters = value; }
		}

		public int CurrentFilterIndex
		{
			get { return inner.CurrentFilterIndex; }
			set { inner.CurrentFilterIndex = value; }
		}

		public bool CheckFileExists 
		{
			get { return inner.CheckFileExists; }
			set { inner.CheckFileExists = value; }
		}

		public string Title
		{
			get { return inner.Title; }
			set { inner.Title = value; }
		}

		public DialogResult ShowDialog(Window parent)
		{
			return inner.ShowDialog(parent);
		}
	}
}