using Eto.Drawing;
using System;

namespace Eto.Forms
{
	public interface IPageSettings : IInstanceWidget
	{
		RectangleF PrintableArea { get; set; }
	}

	[Handler(typeof(IPageSettings))]
	public class PageSettings : InstanceWidget
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
