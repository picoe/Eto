using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class TabControlSection : Panel
	{
		TabControl tabControl;

		public TabControlSection()
		{
			Create();			
		}

		protected virtual void Create()
		{
			var layout = new DynamicLayout();
			layout.AddSeparateRow(null, AddTab(), RemoveTab(), null);
			layout.AddSeparateRow(tabControl = DefaultTabs());
			Content = layout;
		}

		Control AddTab()
		{
			var control = new Button { Text = "Add Tab" };
			control.Click += (s, e) => {
				var tab = new TabPage(tabControl.Generator) { Text = "Tab " + (tabControl.TabPages.Count + 1) };
				tabControl.TabPages.Add(tab);
			};
			return control;
		}

		Control RemoveTab()
		{
			var control = new Button { Text = "Remove Tab" };
			control.Click += (s, e) => {
				if (tabControl.SelectedIndex >= 0 && tabControl.TabPages.Count > 0)
				{
					tabControl.TabPages.RemoveAt(tabControl.SelectedIndex);
				}
			};
			return control;
		}

		TabControl DefaultTabs()
		{
			var control = new TabControl();
			LogEvents(control);
			
			var page = new TabPage { Text = "Tab 1" };
			page.Content = TabOne();
			control.TabPages.Add(page);
			
			LogEvents(page);
			
			page = new TabPage
			{ 
				Text = "Tab 2",
				Image = TestIcons.TestIcon,
			};
			LogEvents(page);
			page.Content = TabTwo();
			control.TabPages.Add(page);

			page = new TabPage
			{ 
				Text = "Tab 3"
			};
			LogEvents(page);
			control.TabPages.Add(page);
			
			return control;
			
		}

		Control TabOne()
		{
			var control = new Panel();
			
			control.Content = new LabelSection();
			
			return control;
		}

		Control TabTwo()
		{
			var control = new Panel();
			
			control.Content = new TextAreaSection { Border = BorderType.None };
			
			return control;
		}

		void LogEvents(TabControl control)
		{
			control.SelectedIndexChanged += delegate
			{
				Log.Write(control, "SelectedIndexChanged, Index: {0}", control.SelectedIndex);	
			};
		}

		void LogEvents(TabPage control)
		{
			control.Click += delegate
			{
				Log.Write(control, "Click, Item: {0}", control.Text);
			};
		}
	}

	public class ThemedTabControlSection : TabControlSection
	{
		protected override void Create()
		{
			// Clone the current generator and add handlers
			// for TabControl and TabPage. Create a TabControlSection
			// using the new generator and then restore the previous generator.
			var currentGenerator = Generator.Current;
			try
			{
				var generator = Activator.CreateInstance(currentGenerator.GetType()) as Generator;
				Generator.Initialize(generator);

				generator.Add<ITabControl>(() => new Eto.Test.Handlers.TabControlHandler());
				generator.Add<ITabPage>(() => new Eto.Test.Handlers.TabPageHandler());

				base.Create();
			}
			finally
			{
			
				Generator.Initialize(currentGenerator); // restore
			}
		}
	}
}

