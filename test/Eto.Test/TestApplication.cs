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
		static Settings settings;

		public static Settings Settings => settings ?? (settings = Settings.Load());

		public static IEnumerable<Assembly> DefaultTestAssemblies()
		{
#if PCL
			yield return typeof(TestApplication).GetTypeInfo().Assembly;
#else
			yield return typeof(TestApplication).Assembly;
#endif
		}

		public List<Assembly> TestAssemblies { get; private set; }

		protected override void OnLocalizeString(LocalizeEventArgs e)
		{
			base.OnLocalizeString(e);
			//Console.WriteLine($"Localize {e.Source}:{e.Text}");
			//e.LocalizedText = e.Text + "_localized";
		}

		public TestApplication(Platform platform)
			: base(platform)
		{
			TestAssemblies = DefaultTestAssemblies().ToList();
			this.Name = "Test Application";
			this.Style = "application";

			if (Platform.Supports<Notification>())
			{
				NotificationActivated += (sender, e) => Log.Write(this, $"Notification: {e.ID}, userData: {e.UserData}");
			}
		}

		protected override void OnInitialized(EventArgs e)
		{
			MainForm = new MainForm(TestSections.Get(TestAssemblies));

			base.OnInitialized(e);

			/**
			Debug.WriteLine("Starting test...");
			const int count = 1000;
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < count; i++)
			{
				new Sections.Serialization.Xaml.Test();
			}
			sw.Stop();
			Debug.WriteLine("Time: {0}", sw.Elapsed);
			/**/

			// show the main form
			MainForm.Show();
#if NETSTANDARD2_0
//			var elapsedTime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
	//		Log.Write(this, $"Startup time: {elapsedTime}");
#endif
		}

		protected override void OnTerminating(CancelEventArgs e)
		{
			base.OnTerminating(e);
			Log.Write(this, "Terminating");
			Settings.Save();

			/*
			var result = MessageBox.Show(MainForm, "Are you sure you want to quit?", MessageBoxButtons.YesNo, MessageBoxType.Question);
			if (result == DialogResult.No)
				e.Cancel = true;
			*/
		}
	}
}

