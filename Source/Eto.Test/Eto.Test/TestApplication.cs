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
		public static IEnumerable<Assembly> DefaultTestAssemblies()
		{
#if PCL
			yield return typeof(TestApplication).GetTypeInfo().Assembly;
#else
			yield return typeof(TestApplication).Assembly;
#endif
		}

		public List<Assembly> TestAssemblies { get; private set; }

		public TestApplication(Platform platform)
			: base(platform)
		{
			TestAssemblies = DefaultTestAssemblies().ToList();
			this.Name = "Test Application";
			this.Style = "application";
		}

		protected override void OnInitialized(EventArgs e)
		{
			MainForm = new MainForm(TestSections.Get(TestAssemblies));

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

