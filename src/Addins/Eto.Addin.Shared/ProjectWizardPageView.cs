using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Designer;

namespace Eto.Addin.Shared
{

	public class ProjectWizardPageView : BasePageView
	{
		Label HeadingLabel(string text)
		{
			return new Label { Text = text, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Right };
		}

		public ProjectWizardPageView(ProjectWizardPageModel model)
		{

			var radioSpacing = Platform.IsGtk ? Size.Empty : new Size(2, 2);

			var content = new DynamicLayout
			{
				Spacing = new Size(10, 10)
			};

			if (model.SupportsAppName)
			{
				var nameBox = new TextBox();
				nameBox.TextBinding.BindDataContext((ProjectWizardPageModel m) => m.AppName);
				Application.Instance.AsyncInvoke(nameBox.Focus);

				var nameValid = new Label { TextColor = Global.Theme.ErrorForeground };
				nameValid.BindDataContext(c => c.Visible, (ProjectWizardPageModel m) => m.AppNameInvalid);
				nameValid.BindDataContext(c => c.Text, (ProjectWizardPageModel m) => m.AppNameValidationText);


				content.BeginHorizontal();
				content.Add(HeadingLabel((model.IsLibrary ? "Library" : "App") + " Name:"));
				content.AddColumn(nameBox, nameValid);
				content.EndHorizontal();
			}
			else if (!string.IsNullOrEmpty(model.AppName))
			{
				var label = new Label { Text = model.AppName, VerticalAlignment = VerticalAlignment.Center };
				content.AddRow(HeadingLabel((model.IsLibrary ? "Library" : "App") + " Name:"), label);
			}

			if (model.SupportsCombined)
			{
				var platformTypeList = new RadioButtonList
				{
					Orientation = Orientation.Vertical,
					Spacing = radioSpacing,
					Items =
					{
						new ListItem { Text = "Separate projects for each platform", Key = "separate" },
						new ListItem { Text = "Single Windows, Linux, and Mac desktop project", Key = "combined" }
					}
				};
				platformTypeList.BindDataContext(c => c.Enabled, (ProjectWizardPageModel m) => m.AllowCombined);
				platformTypeList.SelectedKeyBinding
				                .Convert(v => v == "combined", v => v ? "combined" : "separate")
				                .BindDataContext((ProjectWizardPageModel m) => m.Combined);
				content.AddRow(HeadingLabel("Launcher:"), platformTypeList);
			}

			if (model.SupportsXamMac)
			{
				var cb = new CheckBox
				{
					Text = "Include Xamarin.Mac project",
					ToolTip = "This enables you to bundle mono with your app so your users don't have to install it separately.  You can only compile this on a Mac"
				};
				cb.CheckedBinding.BindDataContext((ProjectWizardPageModel m) => m.IncludeXamMac);
				content.AddRow(HeadingLabel(string.Empty), cb);
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

			if (model.SupportsFramework)
			{
				var frameworkCheckBoxes = new CheckBoxList();
				frameworkCheckBoxes.BindDataContext(c => c.DataStore, (ProjectWizardPageModel m) => m.SupportedFrameworks);
				frameworkCheckBoxes.ItemTextBinding = Binding.Property((ProjectWizardPageModel.FrameworkInfo i) => i.Text);
				frameworkCheckBoxes.ItemKeyBinding = Binding.Property((ProjectWizardPageModel.FrameworkInfo i) => i.Value);
				frameworkCheckBoxes.SelectedValuesBinding.BindDataContext((ProjectWizardPageModel m) => m.SelectedFrameworks);

				content.AddRow(HeadingLabel("Framework:"), frameworkCheckBoxes);
			}

			if (model.SupportsProjectType)
			{
				var sharedCodeList = new RadioButtonList
				{
					Orientation = Orientation.Vertical,
					Spacing = radioSpacing,
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

				content.AddRow(new Label { Text = model.IsLibrary ? "Type:" : "Shared Code:", TextAlignment = TextAlignment.Right }, sharedCodeList);
			}

			if (model.SupportsPanelType)
			{
				var panelTypeList = new RadioButtonList
				{
					Orientation = Orientation.Horizontal,
					Spacing = radioSpacing,
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

				content.AddRow(HeadingLabel("Form:"), panelTypeList);
			}

			if (model.SupportsBase)
			{
				var baseTypeList = new RadioButtonList
				{
					Orientation = Orientation.Horizontal,
					Spacing = radioSpacing,
				};

				baseTypeList.Items.Add(new ListItem { Text = "Panel", Key = "Panel" });
				baseTypeList.Items.Add(new ListItem { Text = "Dialog", Key = "Dialog" });
				baseTypeList.Items.Add(new ListItem { Text = "Form", Key = "Form" });
				baseTypeList.SelectedKeyBinding.BindDataContext((ProjectWizardPageModel m) => m.Base);

				content.AddRow(HeadingLabel("Base:"), baseTypeList);
			}

#if DEBUG
			var showColorsButton = new Button { Text = "Show all themed colors" };
			showColorsButton.Click += (sender, e) => new ThemedColorsDialog().ShowModal(this);
			content.AddRow(new Panel(), showColorsButton);
#endif

			var informationLabel = new Label();
			informationLabel.TextBinding.BindDataContext((ProjectWizardPageModel m) => m.Information);
			Information = informationLabel;

			Content = content;
			DataContext = model;
		}
	}
}