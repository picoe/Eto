using System;
using Eto.Forms;
using System.ComponentModel;
using System.Diagnostics;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Eto.Test
{
	public class TestApplication : Application
	{
		List<Assembly> testAssemblies;

		public static IEnumerable<Assembly> DefaultTestAssemblies()
		{ 
			#if PCL
			yield return typeof(TestApplication).GetTypeInfo().Assembly;
			#else
			yield return typeof(TestApplication).Assembly;
			#endif
		}

		public TestApplication(Platform platform, params Assembly[] additionalTestAssemblies)
			: base(platform)
		{
			testAssemblies = DefaultTestAssemblies().Union(additionalTestAssemblies ?? Enumerable.Empty<Assembly>()).ToList();
			this.Name = "Test Application";
			this.Style = "application";

			Eto.Style.Add<TableLayout>(null, table =>
			{
				table.Padding = new Padding(5);
				table.Spacing = new Size(5, 5);
			});
		}

		protected override void OnInitialized(EventArgs e)
		{
			MainForm = new MainForm(TestSections.Get(testAssemblies));

			base.OnInitialized(e);

			/*
			int count = 100000;
			var start = DateTime.Now;
			for (int i = 0; i < count; i++)
			{
				new Button();
			}
			var end = DateTime.Now;
			Debug.WriteLine("Time: {0}", end - start);
			*/

			// show the main form
			MainForm.Show();
		}

		protected override void OnTerminating(CancelEventArgs e)
		{
			base.OnTerminating(e);
			Log.Write(this, "Terminating");
			
			var result = MessageBox.Show(MainForm, "Are you sure you want to quit?", MessageBoxButtons.YesNo, MessageBoxType.Question);
			if (result == DialogResult.No)
				e.Cancel = true;
		}
	}
}

