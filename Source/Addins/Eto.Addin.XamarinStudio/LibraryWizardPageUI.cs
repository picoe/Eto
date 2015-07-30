using System;
using MonoDevelop.Ide.Templates;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Addin.XamarinStudio
{
	class LibraryWizardPageUI : Panel
	{
		public LibraryWizardPageUI(LibraryWizardPage page)
		{
			BackgroundColor = Color.FromArgb(225, 228, 232);

			var nameBox = new TextBox();
			nameBox.TextBinding.BindDataContext((LibraryWizardPage m) => m.LibraryName);
			Application.Instance.AsyncInvoke(nameBox.Focus);

			var sharedCodeList = new RadioButtonList
			{
				Orientation = Orientation.Vertical,
				Spacing = new Size(0, 0),
				Items =
				{ 
					new ListItem { Text = "Portable Class Library", Key = "pcl" }, 
					new ListItem { Text = "Full .NET", Key = "net" }
				}
			};
			sharedCodeList.SelectedKeyBinding.Convert(v => v == "pcl", v => v ? "pcl" : sharedCodeList.SelectedKey).BindDataContext((LibraryWizardPage m) => m.UsePCL);
			sharedCodeList.SelectedKeyBinding.Convert(v => v == "net", v => v ? "net" : sharedCodeList.SelectedKey).BindDataContext((LibraryWizardPage m) => m.UseNET);

			var content = new TableLayout
			{
				Spacing = new Size(10, 10),
				Rows =
				{
					new TableRow(new Label { Text = "Library Name", TextAlignment = TextAlignment.Right, VerticalAlignment = VerticalAlignment.Center }, nameBox),
				}
			};

			if (page.Wizard.ShowLibraryType)
			{
				content.Rows.Add(new TableRow(new Label { Text = "Library Type", TextAlignment = TextAlignment.Right }, sharedCodeList));
			}

			Content = new StackLayout
			{
				Orientation = Orientation.Horizontal,
				VerticalContentAlignment = VerticalAlignment.Stretch,
				Items =
				{
					new Panel { Size = new Size(50, 0) },
					new StackLayoutItem(content, VerticalAlignment.Center, expand: true),
					new Panel { Size = new Size(50, 0) },
					new Panel { BackgroundColor = Colors.White, Size = new Size(280, 200) }

				}
			};
			
			DataContext = page;
		}
	}
}