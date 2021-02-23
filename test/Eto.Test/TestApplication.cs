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

	class TestForm : Form
	{
     	public TestForm()
        {
            Title = "GM Kickout";
			Menu = new MenuBar();
            MinimumSize = new Size(200, 200);
            Size = new Size(600, 300);
            var root  = new DynamicLayout();
            root.BackgroundColor = Colors.White;

            var header = new DynamicLayout();
            header.Height = 70;
            header.BackgroundColor = Color.FromRgb(0x008080);

            root.Add(header);

            var content = new DynamicLayout();
            content.Padding = new Padding(0, 20, 0, 0);
           

            var layout1 = CreateMainContentItem("Content Editor", "Edit the Content files of the project.", null);
            content.Add(layout1);
            root.Add(content);
	    Content = root;

            Focus();
	}
        public static DynamicLayout CreateMainContentItem(string title, string description, Action click)
        {
            DynamicLayout layout = new DynamicLayout();
            
            StackLayout stackLayout = new StackLayout();
            stackLayout.Orientation = Orientation.Vertical;
            stackLayout.Padding = new Padding(0, 10);
            stackLayout.HorizontalContentAlignment = HorizontalAlignment.Center;
            stackLayout.MouseEnter += ((e, o) =>
            {
                stackLayout.BackgroundColor = Color.FromRgb(0xdfdfdf);
            });
            stackLayout.MouseDown += ((e, o) =>
            {
                stackLayout.BackgroundColor = Color.FromRgb(0x008080);
                if(click != null)
                click();
            });
            stackLayout.MouseLeave += ((e, o) =>
            {
                stackLayout.BackgroundColor = SystemColors.ControlBackground;
            });

            Label titleLbl = new Label();
            titleLbl.TextColor = Color.FromRgb(0xf88379);
            titleLbl.Font = new Font(SystemFont.Label, 20f);
            titleLbl.Text = title;

            Label descriptionLbl = new Label();
            descriptionLbl.Font = new Font(SystemFont.Label);
            descriptionLbl.Text = description;

            stackLayout.Items.Add(titleLbl);
            stackLayout.Items.Add(descriptionLbl);

            StackLayout dummy = new StackLayout();
            layout.Add(stackLayout);
            layout.Add(dummy);

            return layout;
        }		
	}
	
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
			// MainForm = new TestForm();

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

