namespace Eto.Test.Sections
{

	[Section("Automated Tests", "Unit Tests")]
	public class UnitTestSection : Panel
	{
		static UnitTestPanel unitTestPanel;
		UnitTestRunner runner;

		public UnitTestSection()
		{
			if (unitTestPanel == null)
			{
				runner = new UnitTestRunner(((TestApplication)TestApplication.Instance).TestAssemblies);
				unitTestPanel = new UnitTestPanel(runner, false);
				unitTestPanel.Log += (sender, e) => Log.Write(null, e.Message);
			}

			Content = unitTestPanel;
		}
	}
}