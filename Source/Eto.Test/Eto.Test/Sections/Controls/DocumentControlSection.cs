using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(DocumentControl))]
	public class DocumentControlSection : Panel
	{
		DocumentControl tabControl;

		protected override void OnPreLoad(EventArgs e)
		{
			Content = Create();
			base.OnPreLoad(e);
		}

		public virtual Control Create()
		{
			tabControl = DefaultTabs();

			return new StackLayout
			{
				Spacing = 5,
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Items =
				{
					new StackLayout
					{
						Orientation = Orientation.Horizontal,
						Items = { AddTab(), null }
					},
					new StackLayoutItem(tabControl, expand: true)
				}
			};
		}

		Control AddTab()
		{
			var control = new Button { Text = "Add Tab" };
			control.Click += (s, e) =>
			{
				var tab = new DocumentPage
				{
					Text = "Tab " + (tabControl.Pages.Count + 1),
					Content = tabControl.Pages.Count % 2 == 0 ? TabOne() : TabTwo()
				};
				LogEvents(tab);

				tabControl.Pages.Add(tab);
			};
			return control;
		}

		DocumentControl DefaultTabs()
		{
			var control = CreateDocumentControl();
			LogEvents(control);

			control.Pages.Add(new DocumentPage { Text = "Tab 1", Content = TabOne() });

			control.Pages.Add(new DocumentPage
			{
				Text = "Tab 2",
				Image = TestIcons.TestIcon,
				Content = TabTwo()
			});

			control.Pages.Add(new DocumentPage { Text = "Tab 3", Closable = false });

			foreach (var page in control.Pages)
				LogEvents(page);

			return control;

		}

		protected virtual DocumentControl CreateDocumentControl()
		{
			return new DocumentControl();
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

		void LogEvents(DocumentControl control)
		{
			control.SelectedIndexChanged += delegate
			{
				Log.Write(control, "SelectedIndexChanged, Index: {0}", control.SelectedIndex);
			};

			control.PageClosed += (sender, e) =>
			{
				Log.Write(control, "PageClosed, Title: {0}", e.Page.Text);
			};
		}

		void LogEvents(DocumentPage control)
		{
			control.Click += delegate
			{
				Log.Write(control, "Click, Item: {0}", control.Text);
			};

			control.Closed += (sender, e) =>
			{
				Log.Write(control, "Closed, Title: {0}", control.Text);
			};
		}
	}
}

