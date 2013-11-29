using Eto.Drawing;

namespace Eto.Forms
{
	public interface IPageSettings : IInstanceWidget
	{
		RectangleF PrintableArea { get; set; }
	}

	public class PageSettings : InstanceWidget
	{
		public PageSettings()
			: this((Generator)null)
		{
		}

		public PageSettings (Generator generator)
			: base (generator, typeof (IPageSettings))
		{
		}

		public RectangleF PrintableArea { get; set; }
	}
}
