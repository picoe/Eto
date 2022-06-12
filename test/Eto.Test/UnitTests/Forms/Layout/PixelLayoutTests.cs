using System;
using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Layout
{
	[TestFixture]
	public class PixelLayoutTests : TestBase
	{
		[Test, ManualTest]
		public void SettingChildSizeShouldNotChangePosition()
		{
			ManualForm("Panels should be at all four corners,\ngreen on top, blue on bottom.\nThe boxes should not move when the form is resized", form =>
			{

				var layout = new PixelLayout { Size = new Size(300, 300) };

				var tl = new Panel { BackgroundColor = Colors.Green, Size = new Size(200, 200) };
				layout.Add(tl, 0, 0);

				var tr = new Panel { BackgroundColor = Colors.Green, Size = new Size(200, 200) };
				layout.Add(tr, 200, 0);

				var bl = new Panel { BackgroundColor = Colors.Blue, Size = new Size(200, 200) };
				layout.Add(bl, 0, 200);

				var br = new Panel { BackgroundColor = Colors.Blue, Size = new Size(200, 200) };
				layout.Add(br, 200, 200);

				tl.Size = tr.Size = bl.Size = br.Size = new Size(100, 100);

				return layout;
			});
		}

		[Test, ManualTest]
		public void LabelsShouldGetCorrectSize()
		{
			ManualForm("Labels should end with a period", form => {
				// note: this actually failed only when the form was the initial window, as it calculated its size before it was even shown.
				form.ClientSize = new Size(200, 200);

				var layout = new PixelLayout();
				layout.Add(new Label { Text = "Hello world.", BackgroundColor = Colors.Yellow, TextColor = Colors.Black }, 50, 50);
				layout.Add(new Label { Text = "Bonjour monde.", BackgroundColor = Colors.LightGreen, TextColor = Colors.Black }, 20, 20);
				form.Content = layout;
				return layout;
			});
		}
	}
}
