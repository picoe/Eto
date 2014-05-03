using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IOpenFileDialog : IFileDialog
	{
		bool MultiSelect { get; set; }

		IEnumerable<string> Filenames { get; }
	}

	[Handler(typeof(IOpenFileDialog))]
	public class OpenFileDialog : FileDialog
	{
		new IOpenFileDialog Handler { get { return (IOpenFileDialog)base.Handler; } }
		
		public OpenFileDialog()
		{
		}

		[Obsolete("Use default constructor instead")]
		public OpenFileDialog (Generator generator) : this(generator, typeof(IOpenFileDialog))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
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
