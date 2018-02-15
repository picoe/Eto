using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Dialogs
{
	[Section("Dialogs", typeof(AboutDialog))]
	public class AboutDialogSection : Panel
	{
		TextBox entryTitle, entryProgramName, entryVersion, entryCopyright, entryLicense, entryWebsiteURL, entryWebsiteLabel;
		TextArea entryDescription, entryDevelopers, entryDesigners, entryDocumenters;
		Button buttonShowDialog;

		public AboutDialogSection()
		{
			var layout = new DynamicLayout();
			layout.DefaultSpacing = new Size(5, 5);
			layout.DefaultPadding = new Padding(10);
			layout.BeginHorizontal();

			var layout4 = new DynamicLayout();

			var layout2 = new DynamicLayout();
			layout2.DefaultSpacing = new Size(5, 5);
			layout2.BeginVertical();

			layout2.BeginHorizontal();
			layout2.Add(new Label { Text = "Title: ", VerticalAlignment = VerticalAlignment.Center }, false, false);
			entryTitle = new TextBox();
			entryTitle.Text = "About";
			layout2.Add(entryTitle, false, false);
			layout2.EndHorizontal();

			layout2.BeginHorizontal();
			layout2.Add(new Label { Text = "Program Name: ", VerticalAlignment = VerticalAlignment.Center }, false, false);
			entryProgramName = new TextBox();
			entryProgramName.Text = "Awesome App #02";
			layout2.Add(entryProgramName, false, false);
			layout2.EndHorizontal();

			layout2.BeginHorizontal();
			layout2.Add(new Label { Text = "Version: ", VerticalAlignment = VerticalAlignment.Center }, false, false);
			entryVersion = new TextBox();
			entryVersion.Text = "1.0.0";
			layout2.Add(entryVersion, false, false);
			layout2.EndHorizontal();

			layout2.BeginHorizontal();
			layout2.Add(new Label { Text = "Copyright: ", VerticalAlignment = VerticalAlignment.Center }, false, false);
			entryCopyright = new TextBox();
			entryCopyright.Text = "(C) Copyright 2012 - 2016";
			layout2.Add(entryCopyright, false, false);
			layout2.EndHorizontal();

			layout2.BeginHorizontal();
			layout2.Add(new Label { Text = "License: ", VerticalAlignment = VerticalAlignment.Center }, false, false);
			entryLicense = new TextBox();
			entryLicense.Text = "MIT License";
			layout2.Add(entryLicense, false, false);
			layout2.EndHorizontal();

			layout2.BeginHorizontal();
			layout2.Add(new Label { Text = "Website URL: ", VerticalAlignment = VerticalAlignment.Center }, false, false);
			entryWebsiteURL = new TextBox();
			entryWebsiteURL.Text = "https://github.com/picoe/Eto";
			layout2.Add(entryWebsiteURL, false, false);
			layout2.EndHorizontal();

			layout2.BeginHorizontal();
			layout2.Add(new Label { Text = "Website Label: ", VerticalAlignment = VerticalAlignment.Center }, false, false);
			entryWebsiteLabel = new TextBox();
			entryWebsiteLabel.Text = "Website";
			layout2.Add(entryWebsiteLabel, false, false);
			layout2.EndHorizontal();

			layout2.Add(null, true, true);

			layout4.Add(layout2);

			layout4.Add(null, true, true);

			buttonShowDialog = new Button();
			buttonShowDialog.Text = "Show Dialog";
			buttonShowDialog.Click += ButtonShowDialog_Click;
			layout4.Add(buttonShowDialog);

			layout.Add(layout4);

			var layout3 = new DynamicLayout();
			layout3.DefaultSpacing = new Size(5, 5);

			layout3.Add(new Label { Text = "Program Description: " });
			entryDescription = new TextArea();
			entryDescription.Text = "Application for bla bla bla bla";
			layout3.Add(entryDescription, true, true);

			layout3.Add(new Label { Text = "Developers: " });
			entryDevelopers = new TextArea();
			entryDevelopers.Text = "Developer 01" + Environment.NewLine + "Developer 02";
			layout3.Add(entryDevelopers, true, true);

			layout3.Add(new Label { Text = "Designers: " });
			entryDesigners = new TextArea();
			entryDesigners.Text = "Designer 01" + Environment.NewLine + "Designer 02";
			layout3.Add(entryDesigners, true, true);

			layout3.Add(new Label { Text = "Documenters: " });
			entryDocumenters = new TextArea();
			entryDocumenters.Text = "Documenter 01" + Environment.NewLine + "Documenter 02";
			layout3.Add(entryDocumenters, true, true);

			layout.Add(layout3);

			Content = layout;
		}

		private void ButtonShowDialog_Click(object sender, EventArgs e)
		{
			var dialog = new AboutDialog();
			dialog.Logo = TestIcons.Logo;
			dialog.Title = entryTitle.Text;
			dialog.ProgramName = entryProgramName.Text;
			dialog.Version = entryVersion.Text;
			dialog.Copyright = entryCopyright.Text;
			dialog.License = entryLicense.Text;

			try
			{
				dialog.Website = new Uri(entryWebsiteURL.Text);
				dialog.WebsiteLabel = entryWebsiteLabel.Text;
			}
			catch { }

			dialog.ProgramDescription = entryDescription.Text;
			dialog.Designers = entryDesigners.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
			dialog.Developers = entryDevelopers.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
			dialog.Documenters = entryDocumenters.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			dialog.ShowDialog(this);
		}
	}
}
