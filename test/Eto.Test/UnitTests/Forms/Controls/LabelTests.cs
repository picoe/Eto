using System;
using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class LabelTests : TestBase
	{
		[Test, ManualTest]
		public void ChangingTextAfterCreationShouldUpdateSize()
		{
			ManualForm("Label should be fully visible", form =>
			{
				var label = new Label { Text = "" };
				Application.Instance.AsyncInvoke(() => label.Text = "This label should end with a period.");
				var layout = new PixelLayout
				{
					Size = new Size(300, 200)
				};
				layout.Add(label, 0, 0);
				return layout;
			});
		}
	}
}
