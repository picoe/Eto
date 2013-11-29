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
		new IOpenFileDialog Handler { get { return (IOpenFileDialog)base.Handler; } }
		
		public OpenFileDialog()
			: this((Generator)null)
		{
		}

		public OpenFileDialog (Generator generator) : this(generator, typeof(IOpenFileDialog))
		{
		}
		
		protected OpenFileDialog (Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		public bool MultiSelect { 
			get { return Handler.MultiSelect; }
			set { Handler.MultiSelect = value; }
		}
		
		public IEnumerable<string> Filenames { get { return Handler.Filenames; } }
	}
}
