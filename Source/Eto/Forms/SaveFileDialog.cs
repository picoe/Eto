using System;

namespace Eto.Forms
{
	public interface ISaveFileDialog : IFileDialog
	{
	}
	
	public class SaveFileDialog : FileDialog
	{
		//ISaveFileDialog inner;

		public SaveFileDialog()
			: this(Generator.Current)
		{
		}

		public SaveFileDialog(Generator g) : base(g, typeof(ISaveFileDialog))
		{
			//inner = (ISaveFileDialog)InnerControl;
		}

	}
}
