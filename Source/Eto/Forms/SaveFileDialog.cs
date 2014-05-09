using System;

namespace Eto.Forms
{
	[Handler(typeof(SaveFileDialog.IHandler))]
	public class SaveFileDialog : FileDialog
	{
		public SaveFileDialog()
		{
		}

		[Obsolete("Use default constructor instead")]
		public SaveFileDialog (Generator generator) : this (generator, typeof(SaveFileDialog.IHandler))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected SaveFileDialog (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		public interface IHandler : FileDialog.IHandler
		{
		}
	}
}
