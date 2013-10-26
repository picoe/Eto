using System;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	public class TabControlSection : Panel
	{
		TabControl tabControl;

		public override void OnPreLoad(EventArgs e)
		{
			base.OnPreLoad(e);
			Content = Create();			
		}

		protected virtual Control Create()
		{
			var layout = new DynamicLayout();
			layout.AddSeparateRow(null, AddTab(), RemoveTab(), null);
			layout.AddSeparateRow(tabControl = DefaultTabs());
			return layout;
		}

		Control AddTab()
		{
			var control = new Button { Text = "Add Tab" };
			control.Click += (s, e) =>
			{
				var tab = new TabPage(tabControl.Generator) { Text = "Tab " + (tabControl.TabPages.Count + 1) };
				tabControl.TabPages.Add(tab);
			};
			return control;
		}

		Control RemoveTab()
		{
			var control = new Button { Text = "Remove Tab" };
			control.Click += (s, e) =>
			{
				if (tabControl.SelectedIndex >= 0 && tabControl.TabPages.Count > 0)
				{
					tabControl.TabPages.RemoveAt(tabControl.SelectedIndex);
				}
			};
			return control;
		}

		TabControl DefaultTabs()
		{
			var control = CreateTabControl();
			LogEvents(control);

			control.TabPages.Add(new TabPage { Text = "Tab 1", Content = TabOne() });

			control.TabPages.Add(new TabPage
			{ 
				Text = "Tab 2",
				Image = TestIcons.TestIcon,
				Content = TabTwo()
			});

			control.TabPages.Add(new TabPage { Text = "Tab 3" });

			foreach (var page in control.TabPages)
				LogEvents(page);
			
			return control;
			
		}

		protected virtual TabControl CreateTabControl()
		{
			return new TabControl();
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
		protected override Control Create()
		{
			// Clone the current generator and add handlers
			// for TabControl and TabPage. Create a TabControlSection
			// using the new generator and then restore the previous generator.
			var currentGenerator = Generator.Current;
			try
			{
				var generator = (Generator)Activator.CreateInstance(currentGenerator.GetType());
				Generator.Initialize(generator);

				generator.Add<ITabControl>(() => new Eto.Test.Handlers.TabControlHandler());
				generator.Add<ITabPage>(() => new Eto.Test.Handlers.TabPageHandler());

				return base.Create();
			}
			finally
			{			
				Generator.Initialize(currentGenerator); // restore
			}
		}
	}
}

