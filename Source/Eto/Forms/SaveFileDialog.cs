using System;

namespace Eto.Forms
{
	public interface ISaveFileDialog : IFileDialog
	{
	}

	[Handler(typeof(ISaveFileDialog))]
	public class SaveFileDialog : FileDialog
	{
		public SaveFileDialog()
		{
		}

		[Obsolete("Use default constructor instead")]
		public SaveFileDialog (Generator generator) : this (generator, typeof(ISaveFileDialog))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected SaveFileDialog (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}
	}
}
