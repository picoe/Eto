using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Addin.Shared
{

	public class ProjectWizardPageView : BasePageView
	{
		public ProjectWizardPageView(ProjectWizardPageModel model)
		{
			var content = new TableLayout
			{
				Spacing = new Size(10, 10)
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

			if (model.SupportsPanelType)
			{
				var panelTypeList = new RadioButtonList
				{
					Orientation = Orientation.Horizontal,
					Spacing = new Size(0, 0),
				};

				panelTypeList.Items.Add(new ListItem { Text = "Code", Key = "code" });
				panelTypeList.SelectedKeyBinding.Convert(v => v == "code", v => v ? "code" : panelTypeList.SelectedKey).BindDataContext((ProjectWizardPageModel m) => m.UseCode);

				if (model.SupportsXeto)
				{
					panelTypeList.Items.Add(new ListItem { Text = "Xaml", Key = "xaml" });
					panelTypeList.SelectedKeyBinding.Convert(v => v == "xaml", v => v ? "xaml" : panelTypeList.SelectedKey).BindDataContext((ProjectWizardPageModel m) => m.UseXeto);
				}
				if (model.SupportsJeto)
				{
					panelTypeList.Items.Add(new ListItem { Text = "Json", Key = "json" });
					panelTypeList.SelectedKeyBinding.Convert(v => v == "json", v => v ? "json" : panelTypeList.SelectedKey).BindDataContext((ProjectWizardPageModel m) => m.UseJeto);
				}
				if (model.SupportsCodePreview)
				{
					panelTypeList.Items.Add(new ListItem { Text = "Code Preview", Key = "codePreview" });
					panelTypeList.SelectedKeyBinding.Convert(v => v == "codePreview", v => v ? "codePreview" : panelTypeList.SelectedKey).BindDataContext((ProjectWizardPageModel m) => m.UseCodePreview);
				}

				content.Rows.Add(new TableRow(new Label { Text = "Panel:", TextAlignment = TextAlignment.Right }, panelTypeList));
			}



			var informationLabel = new Label();
			informationLabel.TextBinding.BindDataContext((ProjectWizardPageModel m) => m.Information);
			Information = informationLabel;

			Content = content;
			DataContext = model;
		}
	}
}