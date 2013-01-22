using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class TabControlSection : Panel
	{
		public TabControlSection ()
		{
			Add();			
		}

		protected virtual void Add()
		{
			var layout = new DynamicLayout(this);

			layout.Add(DefaultTabs());
		}
		
		Control DefaultTabs ()
		{
			var control = new TabControl ();
			LogEvents (control);
			
			var page = new TabPage { Text = "Tab 1" };
			page.AddDockedControl (TabOne ());
			control.TabPages.Add (page);
			
			LogEvents (page);
			
			page = new TabPage { 
				Text = "Tab 2",
				Image = Icon.FromResource ("Eto.Test.TestIcon.ico") 
			};
			LogEvents (page);
			page.AddDockedControl (TabTwo ());
			control.TabPages.Add (page);

			page = new TabPage { 
				Text = "Tab 3"
			};
			LogEvents (page);
			control.TabPages.Add (page);
			
			return control;
			
		}
		
		Control TabOne ()
		{
			var control = new Panel ();
			
			control.AddDockedControl (new LabelSection());
			
			return control;
		}

		Control TabTwo ()
		{
			var control = new Panel ();
			
			control.AddDockedControl (new TextAreaSection { Border = BorderType.None });
			
			return control;
		}
		
		void LogEvents (TabControl control)
		{
			control.SelectedIndexChanged += delegate {
				Log.Write (control, "SelectedIndexChanged, Index: {0}", control.SelectedIndex);	
			};
		}
		
		void LogEvents (TabPage control)
		{
			control.Click += delegate {
				Log.Write (control, "Click, Item: {0}", control.Text);
			};
		}
	}

	public class ThemedTabControlSection : TabControlSection
	{
		protected override void Add()
		{
			// Clone the current generator and add handlers
			// for TabControl and TabPage. Create a TabControlSection
			// using the new generator and then restore the previous generator.
			var currentGenerator = Generator.Current;
			var generator = Activator.CreateInstance(currentGenerator.GetType()) as Generator;
			Generator.Initialize(generator);

			generator.Add<ITabControl>(() => new Eto.Test.Handlers.TabControlHandler());
			generator.Add<ITabPage>(() => new Eto.Test.Handlers.TabPageHandler());

			base.Add();
			
			Generator.Initialize(currentGenerator); // restore
		}		
	}
}

