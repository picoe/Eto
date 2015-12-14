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
	public class TestForm : Form
	{
		SizeF min;
		public TestForm()
		{
			const string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

			/**/
			var myTable = new TableLayout2
			{
				//Padding = new Padding(20),
				//Spacing = new Size(5, 5),
				Rows =
				{
					new TableRow2 { Height = TableLength.Auto, Cells = { new Button { Text = "Hello!" } } },
					new TableRow2 { Height = TableLength.Star(1), Cells = { text } },
					new TableRow2 { new TextBox(), "Hello!", "There!" }
				},
				Columns =
				{
					new TableColumn { Width = TableLength.Star(1) },
					new TableColumn { Width = TableLength.Auto },
					new TableColumn { Width = TableLength.Auto },
				}
			};
			for (int i = 0; i < 0; i++)
			{
				myTable.Rows.Add(new TableRow2 { Height = TableLength.Auto, Cells = { new TextBox(), "Hello!", "There!" } });
			}
			Content = myTable;
			/**
			var myTable = new TableLayout
			{
				Padding = new Padding(20),
				Rows =
				{
					new TableRow { Cells = { new TableCell { Control = new Button { Text = "Hello!" }, ScaleWidth = true } } },
					new TableRow { ScaleHeight = true, Cells = { text } },
					new TableRow { Cells = { new TextBox(), "Hello!", "There!" } }
				},
				};
			for (int i = 0; i < 100; i++)
			{
				myTable.Rows.Add(new TableRow { Cells = { new TextBox(), "Hello!", "There!" } });
			}
			Content = myTable;
			/**/

			#if blah
			var button = new Button { Text = "Click Me!" };
			button.Click += (sender, e) => button.Text = "Click me this is a longer title!";
			var content = new FlowLayout
			{
				Contents =
				{
					button, new Label { Text = "Some other text that won't always wrap", VerticalAlignment = Forms.VerticalAlignment.Center },
					new Label { Text = text, Wrap = WrapMode.Character },
					new TextBox { Text = "Hello!" }
				}
			};

			/*Content = new TabControl
			{
				Pages =
				{
					new TabPage { Text = "Tab 1", Content = new Scrollable { Content = content } }
				}
			};*/
			Content = content; //new Scrollable { Content = content };
			#endif

			/*
			//Menu = new MenuBar();
			Width = 600;
			//Height = 300;

			const string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum";
			var label = new Label
				{
					Wrap = WrapMode.Word,
					Text = text
				};

			Content = new TableLayout(
				new TableRow(new TableLayout(new TableRow(new TableCell(label, true), new Button { Text = "Hello!"}))) { ScaleHeight = true },
				new TextBox()
			);
			*/

		}

		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);

			var size = Content.GetPreferredSize(new SizeF(400, 600));
			var diff = Size - ClientSize;
			//MinimumSize = Size.Round(size + diff);
			Debug.WriteLine("Preferred Size: {0}", size);
			ClientSize = Size.Round(size);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			var diff = Size - ClientSize;
			//MinimumSize = Size.Round(Content.GetPreferredSize(ClientSize) + diff);
			Debug.WriteLine("Size: {0}, ClientSize: {1}", Size, ClientSize);
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			/*var size = Content.GetPreferredSize(new Size(200, int.MaxValue));
			Debug.WriteLine("Preferred Size: {0}", size);*/
		}
	}

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
			/*Eto.Style.Add<TableLayout2>(null, c => c.LoadComplete += (sender, ee) =>
			{
				if (!c.Columns.Any(r => r.Width.IsStar))
					c.Columns[c.Columns.Count - 1].Width = TableLength.Star(1);
				if (!c.Rows.Any(r => r.Height.IsStar))
					c.Rows[c.Rows.Count - 1].Height = TableLength.Star(1);
			});*/

			//MainForm = new MainForm(TestSections.Get(TestAssemblies));
			MainForm = new TestForm();
			//MainForm = new Form{ Content = new Sections.Layouts.TableLayoutSection.ScalingSection() };//.Show();

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

