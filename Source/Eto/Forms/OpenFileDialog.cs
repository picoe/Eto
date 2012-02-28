using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IOpenFileDialog : IFileDialog
	{
		bool MultiSelect { get; set; }
		IEnumerable<string> Filenames { get; }
	}
	
	public class OpenFileDialog : FileDialog
	{
		IOpenFileDialog inner;
		
		public OpenFileDialog()
			: this(Generator.Current)
		{
		}

		public OpenFileDialog(Generator g) : base(g, typeof(IOpenFileDialog))
		{
			inner = (IOpenFileDialog)Handler;
		}

		public bool MultiSelect { 
			get { return inner.MultiSelect; }
			set { inner.MultiSelect = value; }
		}
		
		public IEnumerable<string> Filenames { get { return inner.Filenames; } }
	}
}
