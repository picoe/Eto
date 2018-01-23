using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", typeof(Notification))]
    public class NotificationSection : Panel
    {
		static int count;

		public bool IncludeImage { get; set; }

		public NotificationSection()
		{
			var entryId = new TextBox();
			entryId.TextBinding.BindDataContext((Notification n) => n.ID);

			var entryTitle = new TextBox();
			entryTitle.TextBinding.BindDataContext((Notification n) => n.Title);

			var entryMessage = new TextBox();
			entryMessage.TextBinding.BindDataContext((Notification n) => n.Message);

			var entryUserData = new TextArea();
			entryUserData.TextBinding.BindDataContext((Notification n) => n.UserData);

			var showNotificationButton = new Button();
			showNotificationButton.Text = "Show Notification";
			showNotificationButton.Click += ShowNotificationButton_Click;

			var includeImageCheck = new CheckBox { Text = "Include Image", Checked = true };
			includeImageCheck.CheckedBinding.Bind(this, t => t.IncludeImage);

			Content = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = 10,
				Rows = {
					null,
					new TableRow(new Label { Text = "ID:", VerticalAlignment = VerticalAlignment.Center }, entryId),
					new TableRow(new Label { Text = "Title:", VerticalAlignment = VerticalAlignment.Center }, entryTitle),
					new TableRow(new Label { Text = "Message:", VerticalAlignment = VerticalAlignment.Center }, entryMessage),
					new TableRow(new Label { Text = "UserData:", VerticalAlignment = VerticalAlignment.Center }, entryUserData),
					new TableRow(new TableCell(), includeImageCheck),
					new TableRow(new TableCell(), TableLayout.AutoSized(showNotificationButton)),
					null
				}
			};

			CreateNotification();
        }

		void CreateNotification()
		{
			count++;
			DataContext = new Notification
			{
				ID = "my_identifier_" + count, // optional, but can be useful
				Title = "Hello World " + count,
				Message = "This is just a sample notification message."
			};
		}

		void ShowNotificationButton_Click(object sender, EventArgs e)
		{
			var notification = DataContext as Notification;

			if (IncludeImage)
				notification.ContentImage = TestIcons.TestIcon;

			notification.Show();

			CreateNotification();
		}
	}
}
