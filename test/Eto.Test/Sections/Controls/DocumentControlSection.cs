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
			var allowReorder = new CheckBox { Text = "AllowReordering" };
			allowReorder.CheckedBinding.Bind(tabControl, c => c.AllowReordering);

			var enabled = new CheckBox { Text = "Enabled" };
			enabled.CheckedBinding.Bind(tabControl, c => c.Enabled);

			return new StackLayout
			{
				Spacing = 5,
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Items =
				{
					new StackLayout
					{
						Orientation = Orientation.Horizontal,
						Items = { AddPage(), RemovePage(), SelectPage(), allowReorder, enabled, null }
					},
					new StackLayoutItem(tabControl, expand: true)
				}
			};
		}

		Control AddPage()
		{
			var control = new Button { Text = "Add Page" };
			control.Click += (s, e) =>
			{
				var tab = new DocumentPage
				{
					Text = "Tab " + (tabControl.Pages.Count + 1),
					Content = tabControl.Pages.Count % 2 == 0 ? TabOne() : TabTwo(),
					Image = tabControl.Pages.Count % 3 == 0 ? TestIcons.Logo.WithSize(32, 32) : null
				};
				LogEvents(tab);

				tabControl.Pages.Add(tab);
			};
			return control;
		}

		Control RemovePage()
		{
			var control = new Button { Text = "Remove Page" };
			control.Click += (s, e) =>
			{
				if (tabControl.SelectedIndex >= 0)
					tabControl.Pages.RemoveAt(tabControl.SelectedIndex);
			};
			return control;
		}

		Control SelectPage()
		{
			var control = new Button { Text = "Select Page" };
			var rnd = new Random();
			control.Click += (s, e) =>
			{
				if (tabControl.Pages.Count > 0)
					tabControl.SelectedIndex = rnd.Next(tabControl.Pages.Count);
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
				Image = TestIcons.TestIcon.WithSize(16, 16),
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
			control.SelectedIndexChanged += (sender, e) => Log.Write(control, $"SelectedIndexChanged, Index: {control.SelectedIndex}");

			control.PageClosed += (sender, e) => Log.Write(control, $"PageClosed, Title: {e.Page.Text}");

			control.PageReordered += (sender, e) => Log.Write(control, $"PageReordered, Title: {e.Page.Text}, OldIndex: {e.OldIndex}, NewIndex: {e.NewIndex}");
		}

		void LogEvents(DocumentPage control)
		{
			control.Click += (sender, e) => Log.Write(control, $"Click, Item: {control.Text}");

			control.Closed += (sender, e) => Log.Write(control, $"Closed, Title: {control.Text}");
		}
	}
}

