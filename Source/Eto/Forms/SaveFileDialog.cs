using System;

namespace Eto.Forms
{
	public interface ISaveFileDialog : IFileDialog
	{
	}
	
	public class SaveFileDialog : FileDialog
	{
		public SaveFileDialog()
			: this((Generator)null)
		{
		}

		public SaveFileDialog (Generator generator) : this (generator, typeof(ISaveFileDialog))
		{
		}

		protected SaveFileDialog (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}
	}
}
