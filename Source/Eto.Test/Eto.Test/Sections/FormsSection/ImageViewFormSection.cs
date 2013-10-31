using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.FormsSection
{
	class ImageViewFormSection : WindowSectionMethod
	{
		protected override Eto.Forms.Window GetWindow()
		{
			return new Form
			{
				Content = new ImageView { Image = TestIcons.TestIcon },
				Size = new Size(640, 400),
			};
		}
	}
}
