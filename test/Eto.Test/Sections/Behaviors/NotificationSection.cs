using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", typeof(Notification))]
    public class NotificationSection : Panel
    {
		TextBox entryTitle, entryMessage;
		Button buttonShowNot;

        public NotificationSection()
        {
			var layout = new DynamicLayout();
			layout.DefaultSpacing = new Size(15, 6);
			layout.DefaultPadding = new Padding(10);
			layout.BeginVertical();

			layout.BeginHorizontal();
			layout.Add(null, false, false);
			layout.Add(null, true, true);
			layout.EndHorizontal();

			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Title:", VerticalAlignment = VerticalAlignment.Center }, false, false);
			entryTitle = new TextBox();
			entryTitle.Text = "Hello World";
			layout.Add(entryTitle, true, false);
			layout.EndHorizontal();

			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Message:", VerticalAlignment = VerticalAlignment.Center }, false, false);
			entryMessage = new TextBox();
			entryMessage.Text = "This is just a sample notification message.";
			layout.Add(entryMessage, true, false);
			layout.EndHorizontal();

			layout.BeginHorizontal();
			layout.Add(null, false, false);
			buttonShowNot = new Button();
			buttonShowNot.Text = "Show Notification";
			layout.Add(buttonShowNot, true, false);
			layout.EndHorizontal();

			layout.BeginHorizontal();
			layout.Add(null, false, false);
			layout.Add(null, true, true);
			layout.EndHorizontal();

			Content = layout;

			buttonShowNot.Click += ButtonShowNot_Click;
        }

		private void ButtonShowNot_Click(object sender, EventArgs e)
		{
			var notification = new Notification();
			notification.Title = entryTitle.Text;
			notification.Message = entryMessage.Text;
			notification.Icon = TestIcons.TestIcon;
			notification.Activated += delegate {
				Log.Write(notification, "Default Action Activated");
			};

			notification.Show(MainForm.tray);
		}
	}
}
