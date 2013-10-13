using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Eto.Forms;

namespace Eto.Test.Android
{
	[Activity(Label = "Eto.Test.Android", MainLauncher = true)]
	public class MainActivity : Activity
	{

		public class SimpleApplication : Forms.Application
		{
			public SimpleApplication(Generator generator = null)
				: base(generator)
			{
			}
			public override void OnInitialized(EventArgs e)
			{
				base.OnInitialized(e);
				MainForm = new Form { Content = new Label { Text = "Hello world", VerticalAlign = VerticalAlign.Middle, HorizontalAlign = HorizontalAlign.Center } };
				MainForm.Show();
			}
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			var generator = new Eto.Platform.Android.Generator();
			//new TestApplication(generator).Attach(this);
			new SimpleApplication(generator).Attach(this).Run();

			// Set our view from the "main" layout resource
			//SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			/*Button button = FindViewById<Button>(Resource.Id.myButton);
			
			button.Click += delegate
			{
				button.Text = string.Format("{0} clicks!", count++);
			};*/
		}
	}
}


