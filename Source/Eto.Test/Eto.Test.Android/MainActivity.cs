using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Eto.Forms;
using System.Linq;
using System.Collections.ObjectModel;

namespace Eto.Test.Android
{
	[Activity(Label = "Eto.Test.Android", MainLauncher = true)]
	public class MainActivity : Activity
	{
		public class SimpleApplication : Forms.Application
		{
			public SimpleApplication(Platform platform = null)
				: base(platform)
			{
			}

			protected override void OnInitialized(EventArgs e)
			{
				base.OnInitialized(e);
				/**
				var layout = new DynamicLayout();
				layout.Add(new Label { Text = "Hello world", VerticalAlign = VerticalAlign.Middle, HorizontalAlign = HorizontalAlign.Center });
				layout.Add(new Label { Text = "Hello world2", VerticalAlign = VerticalAlign.Middle, HorizontalAlign = HorizontalAlign.Center });
				layout.Add(new Eto.Forms.Button { Text = "Hello world3" });
				layout.Add(new Eto.Forms.Spinner { Enabled = true });
				layout.Add(null);

				var layout2 = new Eto.Forms.TableLayout(
					new Eto.Forms.TableRow(null, new Label { Text = "Hello world", VerticalAlign = VerticalAlign.Middle, HorizontalAlign = HorizontalAlign.Center }, null),
					new Eto.Forms.TableRow(null, new Label { Text = "Hello world2", VerticalAlign = VerticalAlign.Middle, HorizontalAlign = HorizontalAlign.Center }, null),
					new Eto.Forms.TableRow(null, new Eto.Forms.Button { Text = "Hello world3" }, null),
					new Eto.Forms.TableRow(null, new Eto.Forms.Spinner { Enabled = true }, null)
				);

				MainForm = new Form { Content = new Panel { Content = layout2 } };
				MainForm.Show();
				/**
				var gv = new Eto.Forms.GridView();
				gv.Columns.Add(new GridColumn { DataCell = new TextBoxCell { Binding = new DelegateBinding<string, string>(r => r) }, HeaderText = "Col 1" });
				gv.DataStore = new ObservableCollection<string>(Enumerable.Range(0, 1000).Select(r => "Woo" + r));

				MainForm = new Form { Content = gv };//new Eto.Test.Sections.Controls.ButtonSection() };
				MainForm.Show();
				/**/
				MainForm = new Form { Content = new Eto.Test.Sections.UnitTestSection() };
				MainForm.Show();
				/**/
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			var platform = new Eto.Android.Platform();
			new TestApplication(platform).Attach(this).Run();
			//new SimpleApplication(platform).Attach(this).Run();
		}
	}
}


