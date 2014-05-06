using Eto.Drawing;
using System;

namespace Eto.Forms
{
	public interface IPageSettings : IWidget
	{
		RectangleF PrintableArea { get; set; }
	}

	[Handler(typeof(IPageSettings))]
	public class PageSettings : Widget
	{
		public PageSettings()
		{
		}

		[Obsolete("Use default constructor instead")]
		public PageSettings (Generator generator)
			: base (generator, typeof (IPageSettings))
		{
		}

		public RectangleF PrintableArea { get; set; }
	}
}
