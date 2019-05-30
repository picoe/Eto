using System;
using Eto.Drawing;

namespace Eto.Forms.ThemedControls
{
	/// <summary>
	/// A themed handler for the <see cref="AboutDialog"/> dialog.
	/// </summary>
	public class ThemedAboutDialogHandler : WidgetHandler<Dialog, AboutDialog, AboutDialog.ICallback>, AboutDialog.IHandler
    {
		string[] designers, developers, documenters;
		string license;
		DynamicLayout layout;
		ImageView image;
		Label labelCopyright, labelProgramName, labelProgramDescription, labelVersion;
		LinkButton linkWebsite;
		Uri uriWebsite;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.ThemedControls.AboutDialogHandler"/> class.
		/// </summary>
		public ThemedAboutDialogHandler()
		{
			Control = new Dialog();
			Control.Width = 500;
			Control.Height = 350;

			layout = new DynamicLayout();
			layout.Padding = new Padding(10);

			var layout2 = new DynamicLayout();
			layout2.DefaultSpacing = new Size(5, 5);
			layout2.BeginHorizontal();

			image = new ImageView();
			layout2.Add(image);

			var layout3 = new DynamicLayout();
			layout3.DefaultSpacing = new Size(5, 5);

			labelProgramName = new Label();
			labelProgramName.Font = SystemFonts.Bold(SystemFonts.Default().Size + 4);
			labelProgramName.VerticalAlignment = VerticalAlignment.Center;
			layout3.Add(labelProgramName);

			labelVersion = new Label();
			labelVersion.VerticalAlignment = VerticalAlignment.Center;
			layout3.Add(labelVersion);

			labelProgramDescription = new Label();
			labelProgramDescription.VerticalAlignment = VerticalAlignment.Center;
			layout3.Add(labelProgramDescription);

			linkWebsite = new LinkButton();
			linkWebsite.Click += LinkWebsite_Click;
			layout3.Add(linkWebsite);

			layout3.Add(null, true, true);

			layout2.Add(layout3, true);

			layout.Add(layout2, true, true);

			labelCopyright = new Label();
			labelCopyright.VerticalAlignment = VerticalAlignment.Center;
			layout.Add(labelCopyright);
		}

		private void LinkWebsite_Click(object sender, EventArgs e)
		{
			if (uriWebsite != null)
				Application.Instance.Open(uriWebsite.ToString());
		}

		/// <summary>
		/// Gets or sets the copyright text.
		/// </summary>
		/// <value>The copyright text.</value>
		public string Copyright
		{
			get { return labelCopyright.Text; }
			set { labelCopyright.Text = value; }
		}

		/// <summary>
		/// Gets or sets the list of graphical designers.
		/// </summary>
		/// <value>The list of graphical designers.</value>
		/// <seealso cref="Developers"/>
		/// <seealso cref="Documenters"/>
		public string[] Designers
		{
			get { return designers; }
			set { designers = value; }
		}

		/// <summary>
		/// Gets or sets the list of developers.
		/// </summary>
		/// <value>The list of developers.</value>
		/// <seealso cref="Designers"/>
		/// <seealso cref="Documenters"/>
		public string[] Developers
		{
			get { return developers; }
			set { developers = value; }
		}

		/// <summary>
		/// Gets or sets the list of documenters.
		/// </summary>
		/// <value>The list of documenters.</value>
		/// <seealso cref="Designers"/>
		/// <seealso cref="Developers"/>
		public string[] Documenters
		{
			get { return documenters; }
			set { documenters = value; }
		}

		/// <summary>
		/// Gets or sets the license.
		/// </summary>
		/// <value>The license.</value>
		public string License
		{
			get { return license; }
			set { license = value; }
		}

		/// <summary>
		/// Gets or sets the application logo.
		/// </summary>
		/// <value>The application logo.</value>
		public Image Logo
		{
			get { return image.Image; }
			set { image.Image = value; }
		}

		/// <summary>
		/// Gets or sets the program description.
		/// </summary>
		/// <value>The program description.</value>
		public string ProgramDescription
		{
			get { return labelProgramDescription.Text; }
			set { labelProgramDescription.Text = value; }
		}

		/// <summary>
		/// Gets or sets the name of the program.
		/// </summary>
		/// <value>The name of the program.</value>
		public string ProgramName
		{
			get { return labelProgramName.Text; }
			set { labelProgramName.Text = value; }
		}

		/// <summary>
		/// Gets or sets the window title.
		/// </summary>
		/// <value>The window title.</value>
		public string Title
		{
			get { return Control.Title; }
			set { Control.Title = value; }
		}

		/// <summary>
		/// Gets or sets the application version.
		/// </summary>
		/// <value>The application version.</value>
		public string Version
		{
			get { return labelVersion.Text; }
			set { labelVersion.Text = value; }
		}

		/// <summary>
		/// Gets or sets the application website.
		/// </summary>
		/// <value>The application website.</value>
		public Uri Website
		{
			get { return uriWebsite; }
			set { uriWebsite = value; }
		}

		/// <summary>
		/// Gets or sets the application website label.
		/// </summary>
		/// <value>The application website label.</value>
		public string WebsiteLabel
		{
			get { return linkWebsite.Text; }
			set { linkWebsite.Text = value; }
		}

		/// <summary>
		/// Shows the dialog.
		/// </summary>
		/// <returns>The dialog result.</returns>
		/// <param name="parent">The parent window.</param>
		public DialogResult ShowDialog(Window parent)
		{
			var textArea1 = new TextArea();
			textArea1.ReadOnly = true;

			var textArea2 = new TextArea();
			textArea2.ReadOnly = true;

			if (developers != null && developers.Length != 0)
				textArea1.Text += Application.Instance.Localize(Widget, "Developers:") + Environment.NewLine + string.Join(Environment.NewLine, developers) + Environment.NewLine + Environment.NewLine;
			if (designers != null && designers.Length != 0)
				textArea1.Text += Application.Instance.Localize(Widget, "Designers:") + Environment.NewLine + string.Join(Environment.NewLine, designers) + Environment.NewLine + Environment.NewLine;
			if (documenters != null && documenters.Length != 0)
				textArea1.Text += Application.Instance.Localize(Widget, "Documenters:") + Environment.NewLine + string.Join(Environment.NewLine, documenters) + Environment.NewLine + Environment.NewLine;
			textArea1.Text = textArea1.Text.TrimEnd(Environment.NewLine.ToCharArray());

			if (!string.IsNullOrEmpty(license))
				textArea2.Text = license;

			layout.Parent?.Remove(layout);

			if (string.IsNullOrEmpty(textArea1.Text) && string.IsNullOrEmpty(textArea2.Text))
				Control.Content = layout;
			else
			{
				var tabControl = new TabControl();

				tabControl.Pages.Add(new TabPage(layout) { Text = Application.Instance.Localize(Widget, "About") });

				if (!string.IsNullOrEmpty(textArea1.Text))
					tabControl.Pages.Add(new TabPage(textArea1) { Text = Application.Instance.Localize(Widget, "Credits") });

				if (!string.IsNullOrEmpty(textArea2.Text))
					tabControl.Pages.Add(new TabPage(textArea2) { Text = Application.Instance.Localize(Widget, "License") });

				Control.Content = tabControl;
			}

			Control.ShowModal(parent);

			return DialogResult.Cancel;
		}
    }
}
