using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Addin.Shared
{

	public class ProjectWizardPageView : Panel
	{
		public ProjectWizardPageView(ProjectWizardPageModel model)
		{
			BackgroundColor = Color.FromArgb(225, 228, 232);

			var content = new TableLayout
			{
				Spacing = new Size(10, 10),
				Padding = new Padding(50, 0, 20, 0)
			};
			if (model.ShowAppName)
			{
				var nameBox = new TextBox();
				nameBox.TextBinding.BindDataContext((ProjectWizardPageModel m) => m.AppName);
				Application.Instance.AsyncInvoke(nameBox.Focus);

				content.Rows.Add(new TableRow(new Label { Text = (model.IsLibrary ? "Library" : "App") + " Name:", TextAlignment = TextAlignment.Right, VerticalAlignment = VerticalAlignment.Center }, nameBox));
			}
			else if (!string.IsNullOrEmpty(model.AppName))
			{
				var label = new Label { Text = model.AppName, VerticalAlignment = VerticalAlignment.Center };
				content.Rows.Add(new TableRow(new Label { Text = (model.IsLibrary ? "Library" : "App") + " Name:", TextAlignment = TextAlignment.Right, VerticalAlignment = VerticalAlignment.Center }, label));
			}

			if (model.SupportsCombined)
			{
				var platformTypeList = new RadioButtonList
				{
					Orientation = Orientation.Vertical,
					Spacing = new Size(0, 0),
					Items =
					{
						new ListItem { Text = "Combined Windows && Linux assembly, separate Mac", Key = "combined" },
						new ListItem { Text = "Separate platform assemblies", Key = "separate" }
					}
				};
				platformTypeList.SelectedKeyBinding.Convert(v => v == "combined", v => v ? "combined" : "separate").BindDataContext((ProjectWizardPageModel m) => m.Combined);
				content.Rows.Add(new TableRow(new Label { Text = "Launcher:", TextAlignment = TextAlignment.Right }, platformTypeList));
			}

			/*
			 * eventually select platforms to include?
			 * 
			var platformCheckBoxes = new DynamicLayout();
			platformCheckBoxes.BeginHorizontal();
            platformCheckBoxes.Add(new CheckBox { Text = "Gtk2" });
			platformCheckBoxes.Add(new CheckBox { Text = "Gtk3" });
			platformCheckBoxes.EndBeginHorizontal();
            platformCheckBoxes.Add(new CheckBox { Text = "WinForms" });
			platformCheckBoxes.Add(new CheckBox { Text = "Wpf" });
			platformCheckBoxes.Add(new CheckBox { Text = "Direct2D" });
			platformCheckBoxes.EndBeginHorizontal();
			platformCheckBoxes.Add(new CheckBox { Text = "Mac" });
			platformCheckBoxes.Add(new CheckBox { Text = "XamMac" });
			platformCheckBoxes.Add(new CheckBox { Text = "XamMac2" });
			platformCheckBoxes.EndHorizontal();

			content.Rows.Add(new TableRow(new Label { Text = "Platforms:", TextAlignment = TextAlignment.Right }, platformCheckBoxes));
			/**/

			if (model.SupportsProjectType)
			{
				var sharedCodeList = new RadioButtonList
				{
					Orientation = Orientation.Vertical,
					Spacing = new Size(0, 0),
				};
				if (model.SupportsPCL)
				{
					sharedCodeList.Items.Add(new ListItem { Text = "Portable Class Library", Key = "pcl" });
					sharedCodeList.SelectedKeyBinding.Convert(v => v == "pcl", v => v ? "pcl" : sharedCodeList.SelectedKey).BindDataContext((ProjectWizardPageModel m) => m.UsePCL);
				}
				if (model.SupportsSAL)
				{
					sharedCodeList.Items.Add(new ListItem { Text = "Shared Project", Key = "sal" });
					sharedCodeList.SelectedKeyBinding.Convert(v => v == "sal", v => v ? "sal" : sharedCodeList.SelectedKey).BindDataContext((ProjectWizardPageModel m) => m.UseSAL);
				}

				sharedCodeList.Items.Add(new ListItem { Text = "Full .NET", Key = "net" });
				sharedCodeList.SelectedKeyBinding.Convert(v => v == "net", v => v ? "net" : sharedCodeList.SelectedKey).BindDataContext((ProjectWizardPageModel m) => m.UseNET);

				content.Rows.Add(new TableRow(new Label { Text = model.IsLibrary ? "Type:" : "Shared Code:", TextAlignment = TextAlignment.Right }, sharedCodeList));
			}

			var information = new Label();
			information.TextBinding.BindDataContext((ProjectWizardPageModel m) => m.Information);

			Content = new StackLayout
			{
				Orientation = Orientation.Horizontal,
				VerticalContentAlignment = VerticalAlignment.Stretch,
				Items =
				{
					new StackLayoutItem(content, VerticalAlignment.Center, expand: true),
					new Panel { BackgroundColor = Colors.White, Size = new Size(280, 200), Content = information, Padding = new Padding(20) }
				}
			};

			DataContext = model;
		}
	}
}