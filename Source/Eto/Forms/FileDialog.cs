using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IFileDialogFilter
	{
		/// <summary>
		/// Gets the name of the filter
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// Gets the extensions
		/// </summary>
		/// <remarks>
		/// Each extension should include the period.  e.g. ".jpeg", ".png", etc.
		/// </remarks>
		string[] Extensions { get; }
	}
	
	public class FileDialogFilter : IFileDialogFilter
	{
		public string Name {
			get; set;
		}
		
		public string[] Extensions {
			get; set;
		}
	}
	
	public interface IFileDialog : ICommonDialog
	{
		string FileName { get; set; }
		IEnumerable<IFileDialogFilter> Filters { get; set; }
		int CurrentFilterIndex { get; set; }
		bool CheckFileExists { get; set; }
		string Title { get; set; }
		Uri Directory { get; set; }
	}
	
	public abstract class FileDialog : CommonDialog
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

		public IEnumerable<IFileDialogFilter> Filters
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
		
		public Uri Directory
		{
			get { return inner.Directory; }
			set { inner.Directory = value; }
		}
	}
}