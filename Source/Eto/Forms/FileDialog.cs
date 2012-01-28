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
		public FileDialogFilter ()
		{
		}

		public FileDialogFilter (string name, params string[] extensions)
		{
			this.Name = name;
			this.Extensions = extensions;
		}

		public string Name {
			get; set;
		}
		
		public string[] Extensions {
			get; set;
		}

		public override string ToString ()
		{
			return Name;
		}

		public override int GetHashCode ()
		{
			int hash;
			if (Name != null) hash = Name.GetHashCode ();
			else hash = string.Empty.GetHashCode();
			if (Extensions != null) hash ^= Extensions.GetHashCode ();
			return hash;
		}
	}
	
	public interface IFileDialog : ICommonDialog
	{
		string FileName { get; set; }
		IEnumerable<IFileDialogFilter> Filters { get; set; }
		int CurrentFilterIndex { get; set; }
		IFileDialogFilter CurrentFilter { get; set; }
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

		public IFileDialogFilter CurrentFilter
		{
			get { return inner.CurrentFilter; }
			set { inner.CurrentFilter = value; }
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