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
		IOpenFileDialog handler;
		
		public OpenFileDialog ()
			: this(Generator.Current)
		{
		}

		public OpenFileDialog (Generator g) : this(g, typeof(IOpenFileDialog))
		{
		}
		
		protected OpenFileDialog (Generator g, Type type, bool initialize = true)
			: base(g, type, initialize)
		{
			handler = (IOpenFileDialog)Handler;
		}

		public bool MultiSelect { 
			get { return handler.MultiSelect; }
			set { handler.MultiSelect = value; }
		}
		
		public IEnumerable<string> Filenames { get { return handler.Filenames; } }
	}
}
