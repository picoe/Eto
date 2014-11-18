using System;
using Eto.Forms;
using Eto.Drawing;

namespace EmbedEtoInWpf
{
	/// <summary>
	/// Eto.Forms panel to embed in an existing Wpf app
	/// </summary>
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
