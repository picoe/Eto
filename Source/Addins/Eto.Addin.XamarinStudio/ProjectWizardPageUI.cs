using System;
using MonoDevelop.Ide.Templates;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Addin.XamarinStudio
{
	class ProjectWizardPageUI : Panel
	{
		public ProjectWizardPageUI(ProjectWizardPage page)
		{
			BackgroundColor = Color.FromArgb(225, 228, 232);

			var nameBox = new TextBox();
			nameBox.TextBinding.BindDataContext((ProjectWizardPage m) => m.AppName);
			Application.Instance.AsyncInvoke(nameBox.Focus);

			var platformTypeList = new RadioButtonList
			{
				Orientation = Orientation.Vertical,
				Spacing = new Size(0, 0),
				Items =
				{ 
					new ListItem { Text = "Combined windows && linux, separate mac", Key = "combined" }, 
					new ListItem { Text = "Separate platforms", Key = "separate" }
				}
			};
			platformTypeList.SelectedKeyBinding.Convert(v => v == "combined", v => v ? "combined" : "separate").BindDataContext((ProjectWizardPage m) => m.GenerateCombined);

			var sharedCodeList = new RadioButtonList
			{
				Orientation = Orientation.Vertical,
				Spacing = new Size(0, 0),
			};
			if (page.Wizard.SupportsPCL)
			{
				sharedCodeList.Items.Add(new ListItem { Text = "Portable Class Library", Key = "pcl" });
				sharedCodeList.SelectedKeyBinding.Convert(v => v == "pcl", v => v ? "pcl" : sharedCodeList.SelectedKey).BindDataContext((ProjectWizardPage m) => m.UsePCL);
			}
			sharedCodeList.Items.Add(new ListItem { Text = "Shared", Key = "sal" });
			sharedCodeList.Items.Add(new ListItem { Text = "Full .NET", Key = "net" });

			sharedCodeList.SelectedKeyBinding.Convert(v => v == "sal", v => v ? "sal" : sharedCodeList.SelectedKey).BindDataContext((ProjectWizardPage m) => m.UseSAL);
			sharedCodeList.SelectedKeyBinding.Convert(v => v == "net", v => v ? "net" : sharedCodeList.SelectedKey).BindDataContext((ProjectWizardPage m) => m.UseNET);

			var content = new TableLayout
			{
				Spacing = new Size(10, 10),
				Rows =
				{
					new TableRow(new Label { Text = "App Name", TextAlignment = TextAlignment.Right, VerticalAlignment = VerticalAlignment.Center }, nameBox)
				}
			};
			if (page.Wizard.ShowGenerateCombined)
			{
				content.Rows.Add(new TableRow(new Label { Text = "Platforms", TextAlignment = TextAlignment.Right }, platformTypeList));
			}
			if (page.Wizard.ShowSharedCode)
			{
				content.Rows.Add(new TableRow(new Label { Text = "Shared Code", TextAlignment = TextAlignment.Right }, sharedCodeList));
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