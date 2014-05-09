using Eto.Drawing;
using System;

namespace Eto.Forms
{
	[Handler(typeof(PageSettings.IHandler))]
	public class PageSettings : Widget
	{
		public PageSettings()
		{
		}

		[Obsolete("Use default constructor instead")]
		public PageSettings (Generator generator)
			: base (generator, typeof (IHandler))
		{
		}

		public RectangleF PrintableArea { get; set; }

		public interface IHandler : Widget.IHandler
		{
			RectangleF PrintableArea { get; set; }
		}
	}
}
