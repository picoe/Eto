using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
	public class LabelTests : TestBase
	{
		[Test, ManualTest]
		public void LabelWithDifferentFontSizesShouldShowCorrectly()
		{
			ManualForm("Labels should show correctly.", form =>
			{
				return new TableLayout
				{
					Rows =
					{
						new Panel {BackgroundColor = Colors.White, Height = 10},
						new TableLayout(new Label { Text = "Small Font", Wrap = WrapMode.None, Font = SystemFonts.Default(8) }),
						new Panel {BackgroundColor = Colors.White, Height = 10},
						new TableLayout(new Label { Text = "Normal Font", Wrap = WrapMode.None }),
						new Panel {BackgroundColor = Colors.White, Height = 10},
						new TableLayout(new Label { Text = "Large Font", Wrap = WrapMode.None, Font = SystemFonts.Default(48) }),
						new Panel {BackgroundColor = Colors.White, Height = 10},
					}
				};
			});
		}
	}
}
