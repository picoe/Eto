using System;
using Eto.Forms;

namespace Eto.Test.Sections
{

	[Section("Automated Tests", "Unit Tests")]
	public class UnitTestSection : Panel
	{
		UnitTestPanel unitTestPanel;
		UnitTestRunner runner;
		bool includeManualTests = true;
		public bool IncludeManualTests
		{
			get => includeManualTests;
			set
			{
				includeManualTests = value;
				SetFilters();
				unitTestPanel.Refresh();
			}
		}

		void SetFilters()
		{
			unitTestPanel.ExcludeCategories.Clear();
			if (!includeManualTests)
				unitTestPanel.ExcludeCategories.Add(Eto.Test.UnitTests.TestBase.ManualTestCategory);
		}

		public UnitTestSection()
		{
			runner = new UnitTestRunner(((TestApplication)TestApplication.Instance).TestAssemblies);
			runner.Log += (sender, e) => Application.Instance.Invoke(() => Log.Write(null, e.Message));
			unitTestPanel = new UnitTestPanel(runner, false);
			SetFilters();

			var includeManualTestsCheckBox = new CheckBox { Text = "Include Manual Tests" };
			includeManualTestsCheckBox.CheckedBinding.Bind(this, m => m.IncludeManualTests);

			unitTestPanel.Content = new StackLayout
			{
				Items = { includeManualTestsCheckBox }
			};

			Content = unitTestPanel;
		}
	}
}