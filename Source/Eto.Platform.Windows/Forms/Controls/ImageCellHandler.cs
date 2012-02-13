using System;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class ImageCellHandler : CellHandler<swf.DataGridViewImageCell, ImageCell>, IImageCell
	{
		public ImageCellHandler ()
		{
			Control = new swf.DataGridViewImageCell ();
		}
	}
}

