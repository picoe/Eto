using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Event arguments when printing a page.
	/// </summary>
	public class PrintPageEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the graphics context to draw the page's content.
		/// </summary>
		/// <value>The graphics context.</value>
		public Graphics Graphics { get; private set; }

		/// <summary>
		/// Gets the size of the page, in device units.
		/// </summary>
		/// <remarks>
		/// This should be used to position elements on the page when using the <see cref="Graphics"/> to draw the page's
		/// content.
		/// </remarks>
		/// <value>The size of the page.</value>
		public SizeF PageSize { get; private set; }

		/// <summary>
		/// Gets the current page that is being printed.
		/// </summary>
		/// <value>The current page.</value>
		public int CurrentPage { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.PrintPageEventArgs"/> class.
		/// </summary>
		/// <param name="graphics">Graphics context to paint the page.</param>
		/// <param name="pageSize">Size of the page in device units.</param>
		/// <param name="currentPage">Current page that is being printed.</param>
		public PrintPageEventArgs (Graphics graphics, SizeF pageSize, int currentPage)
		{
			this.Graphics = graphics;
			this.PageSize = pageSize;
			this.CurrentPage = currentPage;
		}
	}
}

