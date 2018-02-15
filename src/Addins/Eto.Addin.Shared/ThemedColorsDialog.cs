using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Designer;
using System.Linq;

namespace Eto.Addin.Shared
{
	public partial class ThemedColorsDialog : Dialog
	{
		public ThemedColorsDialog()
		{
			Title = "All themed colors";
			Resizable = true;
			ClientSize = new Size(400, 350);

			var okButton = new Button { Text = "Ok" };
			okButton.Click += (sender, e) => Close();

			var colorsPanel = new GridView();
			colorsPanel.Columns.Add(new GridColumn
			{
				HeaderText = "Name",
				DataCell = new TextBoxCell {  Binding = Binding.Property((PlatformColor m) => m.Name) }
			});

			colorsPanel.Columns.Add(new GridColumn
			{
				HeaderText = "Name",
				Width = 40,
				AutoSize = false,
				DataCell = new CustomCell
				{
					CreateCell = e =>
					{
						var p = new Panel { Size = new Size(20, 20) };
						p.BindDataContext(c => c.BackgroundColor, (PlatformColor m) => m.Color);
						return p;
					}
				}
			});
			colorsPanel.DataStore = Global.Theme.AllColors.ToList();

			Content = new StackLayout
			{
				Padding = 10,
				Items =
				{
					new StackLayoutItem {
						Expand = true,
						HorizontalAlignment = HorizontalAlignment.Stretch,
						Control = new Scrollable { Content = colorsPanel }
					},

					new StackLayoutItem
					{
						HorizontalAlignment = HorizontalAlignment.Right,
						Control = okButton
					}
				}
			};

		}
	}
}
