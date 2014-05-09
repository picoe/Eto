using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	[Handler(typeof(OpenFileDialog.IHandler))]
	public class OpenFileDialog : FileDialog
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }
		
		public OpenFileDialog()
		{
		}

		[Obsolete("Use default constructor instead")]
		public OpenFileDialog (Generator generator) : this(generator, typeof(OpenFileDialog.IHandler))
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


		public interface IHandler : FileDialog.IHandler
		{
			bool MultiSelect { get; set; }

			IEnumerable<string> Filenames { get; }
		}
	}
}
