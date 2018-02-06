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
			if (model.SupportsAppName)
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

			if (model.SupportsSeparated)
			{
				var platformTypeList = new RadioButtonList
				{
					Orientation = Orientation.Vertical,
					Spacing = new Size(0, 0),
					Items =
					{
						new ListItem { Text = "Single Windows, Linux, and Mac desktop project", Key = "combined" },
						new ListItem { Text = "Separate projects for each platform", Key = "separate" }
					}
				};
				platformTypeList.SelectedKeyBinding
				                .Convert(v => v == "separate", v => v ? "separate" : "combined")
				                .BindDataContext((ProjectWizardPageModel m) => m.Separate);
				content.Rows.Add(new TableRow(new Label { Text = "Launcher:", TextAlignment = TextAlignment.Right, VerticalAlignment = VerticalAlignment.Center }, platformTypeList));
			}

			if (model.SupportsXamMac)
			{
				var cb = new CheckBox
				{
					Text = "Include Xamarin.Mac project",
					ToolTip = "This enables you to bundle mono with your app so your users don't have to install it separately.  You can only compile this on a Mac"
				};
				cb.CheckedBinding.BindDataContext((ProjectWizardPageModel m) => m.IncludeXamMac);
				content.Rows.Add(new TableRow(new TableCell(), cb));
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
				if (model.SupportsNetStandard)
				{
					sharedCodeList.Items.Add(new ListItem { Text = ".NET Standard", Key = "netstandard" });
					sharedCodeList.SelectedKeyBinding.Convert(v => v == "netstandard", v => v ? "netstandard" : sharedCodeList.SelectedKey).BindDataContext((ProjectWizardPageModel m) => m.UseNetStandard);
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
				panelTypeList.SelectedKeyBinding.BindDataContext((ProjectWizardPageModel m) => m.Mode);

				if (model.SupportsXeto)
				{
					panelTypeList.Items.Add(new ListItem { Text = "Xaml", Key = "xaml" });
				}
				if (model.SupportsJeto)
				{
					panelTypeList.Items.Add(new ListItem { Text = "Json", Key = "json" });
				}
				if (model.SupportsCodePreview)
				{
					panelTypeList.Items.Add(new ListItem { Text = "Code Preview", Key = "preview" });
				}

				content.Rows.Add(new TableRow(new Label { Text = "Form:", TextAlignment = TextAlignment.Right, VerticalAlignment = VerticalAlignment.Center }, panelTypeList));
			}

			if (model.SupportsBase)
			{
				var baseTypeList = new RadioButtonList
				{
					Orientation = Orientation.Horizontal,
					Spacing = new Size(0, 0),
				};

				baseTypeList.Items.Add(new ListItem { Text = "Panel", Key = "Panel" });
				baseTypeList.Items.Add(new ListItem { Text = "Dialog", Key = "Dialog" });
				baseTypeList.Items.Add(new ListItem { Text = "Form", Key = "Form" });
				baseTypeList.SelectedKeyBinding.BindDataContext((ProjectWizardPageModel m) => m.Base);

				content.Rows.Add(new TableRow(new Label { Text = "Base:", TextAlignment = TextAlignment.Right, VerticalAlignment = VerticalAlignment.Center }, baseTypeList));
			}

			var informationLabel = new Label();
			informationLabel.TextBinding.BindDataContext((ProjectWizardPageModel m) => m.Information);
			Information = informationLabel;

			Content = content;
			DataContext = model;
		}
	}
}