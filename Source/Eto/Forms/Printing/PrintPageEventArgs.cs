using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public class PrintPageEventArgs : EventArgs
	{
		public Graphics Graphics { get; private set; }

		public SizeF PageSize { get; private set; }

		public int CurrentPage { get; private set; }

		public PrintPageEventArgs (Graphics graphics, SizeF pageSize, int currentPage)
		{
			this.Graphics = graphics;
			this.PageSize = pageSize;
			this.CurrentPage = currentPage;
		}
	}
}

