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
		}

		public bool MultiSelect { 
			get { return Handler.MultiSelect; }
			set { Handler.MultiSelect = value; }
		}
		
		public IEnumerable<string> Filenames { get { return Handler.Filenames; } }
	}
}
