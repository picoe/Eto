using System;
using Eto.Forms;
using Eto.Drawing;

namespace EmbedEtoInXamarinMac
{
	/// <summary>
	/// Eto.Forms panel to embed in an existing MonoMac app
	/// </summary>
	/// <remarks>
	/// This is used in <see cref="ViewController.AwakeFromNib"/> to add to the existing native app.
	/// </remarks>
	public class MyEtoPanel : Panel
	{
		public MyEtoPanel()
		{
			Content = new TableLayout
			{
				Spacing = new Size(5, 5),
				Rows =
				{
					new TableRow(new Label { Text = "An Eto.Forms control" }),
					new TableRow(new TextBox()),
					new TableRow(new ComboBox { Items = { "Item 1", "Item 2", "Item 3" } }),
					null
				}
			};
		}
	}
}

