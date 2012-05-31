using System;

namespace Eto.Forms
{
	public interface ISaveFileDialog : IFileDialog
	{
	}
	
	public class SaveFileDialog : FileDialog
	{
		public SaveFileDialog ()
			: this (Generator.Current)
		{
		}

		public SaveFileDialog (Generator g) : this (g, typeof(ISaveFileDialog))
		{
		}

		protected SaveFileDialog (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
		}
	}
}
