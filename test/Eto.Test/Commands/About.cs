namespace Eto.Test.Commands
{
	public class About : Command
	{
		public About()
		{
			ID = "about";
			if (Platform.Instance.Supports<Icon>())
				Image = TestIcons.TestIcon;
			MenuText = "About Test Application";
			ToolBarText = "About";
			Shortcut = Keys.F11;
		}

		protected override void OnExecuted(EventArgs e)
		{
			base.OnExecuted(e);

			var about = new AboutDialog();
			about.Logo = TestIcons.TestIcon;
			about.WebsiteLabel = "Eto Forms Website";
			about.Website = new Uri("https://github.com/picoe/Eto");

			about.ShowDialog(Application.Instance.MainForm);
		}
	}
}

