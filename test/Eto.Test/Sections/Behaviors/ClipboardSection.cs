namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Clipboard")]
	public class ClipboardSection : Panel
	{
		const string CustomObjectType = "my.custom.object";
		Scrollable pasteData = new Scrollable();
		Clipboard monitoredClipboard;
		ToggleButton monitorChangesButton;
		int changeCount;

		public ClipboardSection()
		{
			var copyTextButton = new Button { Text = "Copy Text" };
			copyTextButton.Click += (sender, e) =>
			{
				new Clipboard().Text = "Some text";
				Update();
			};
			var copyHtmlButton = new Button { Text = "Copy Html" };
			copyHtmlButton.Click += (sender, e) =>
			{
				new Clipboard().Html = "Some <strong style='color:blue'>HTML</strong>";
				Update();
			};
			var copyImageButton = new Button { Text = "Copy Image" };
			copyImageButton.Click += (sender, e) =>
			{
				new Clipboard().Image = TestIcons.TestImage;
				Update();
			};
			var copyCustomButton = new Button { Text = "Copy Custom" };
			copyCustomButton.Click += (sender, e) =>
			{
				new Clipboard().SetString("my value", "my.custom.type");
				Update();
			};
			var copyObjectButton = new Button { Text = "Copy Object" };
			copyObjectButton.Click += (sender, e) =>
			{
				new Clipboard().SetObject(new DragDropSection.CustomSerializableType { Name = "Woot" }, CustomObjectType);
				Update();
			};

			var pasteTextButton = new Button { Text = "Paste" };
			pasteTextButton.Click += (sender, e) => Update();

			var clearButton = new Button { Text = "Clear" };
			clearButton.Click += (sender, e) =>
			{
				using var cb = new Clipboard();
				cb.Clear();
				Update();
			};

			monitorChangesButton = new ToggleButton { Text = "Monitor Changes" };
			monitorChangesButton.CheckedChanged += (sender, e) =>
			{
				if (monitorChangesButton.Checked == true)
					StartMonitoring();
				else
					StopMonitoring();
			};

			Content = new StackLayout
			{
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Spacing = 5,
				Items =
				{
					new StackLayout
					{
						Orientation = Orientation.Horizontal,
						VerticalContentAlignment = VerticalAlignment.Stretch,
						Spacing = 5,
						Padding = new Padding(10),
						Items = { copyTextButton, copyHtmlButton, copyImageButton, copyCustomButton, copyObjectButton, pasteTextButton, clearButton, monitorChangesButton }
					},
					new StackLayoutItem(pasteData, expand: true)
				}
			};
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (monitorChangesButton.Checked == true)
				StartMonitoring();
		}

		protected override void OnUnLoad(EventArgs e)
		{
			StopMonitoring();
			base.OnUnLoad(e);
		}

		void StartMonitoring()
		{
			if (monitoredClipboard != null)
				return;

			monitoredClipboard = new Clipboard();
			monitoredClipboard.Changed += Clipboard_Changed;
			Log.Write(monitoredClipboard, "Clipboard monitoring started");
		}

		void StopMonitoring()
		{
			if (monitoredClipboard != null)
			{
				monitoredClipboard.Changed -= Clipboard_Changed;
				monitoredClipboard.Dispose();
				monitoredClipboard = null;
				Log.Write(this, "Clipboard monitoring stopped");
			}
		}

		void Clipboard_Changed(object sender, EventArgs e)
		{
			changeCount++;
			Log.Write(sender, $"Clipboard changed ({changeCount})");
			Update();
		}

		TextArea ReadOnlyTextArea(string text) => new TextArea { Text = text, ReadOnly = true, Border = BorderType.None };

		void Update()
		{
			using var clipboard = new Clipboard();
			var panel = new StackLayout { Padding = new Padding(10), HorizontalContentAlignment = HorizontalAlignment.Stretch, Spacing = 5 };
			if (clipboard.Text != null)
			{
				panel.Items.Add(new Label { Text = "\nText:", Font = SystemFonts.Bold() });
				panel.Items.Add(ReadOnlyTextArea(clipboard.Text));
			}
			if (clipboard.Image != null)
			{
				panel.Items.Add(new Label { Text = "\nImage:", Font = SystemFonts.Bold() });
				panel.Items.Add(new ImageView
				{
					Image = clipboard.Image
				});
			}
			if (clipboard.Html != null)
			{
				panel.Items.Add(new Label { Text = "\nHtml:", Font = SystemFonts.Bold() });
				panel.Items.Add(ReadOnlyTextArea(clipboard.Html));
			}
			var uris = clipboard.Uris;
			if (uris != null)
			{
				panel.Items.Add(new Label { Text = "\nUris:", Font = SystemFonts.Bold() });
				panel.Items.Add(ReadOnlyTextArea(string.Join(", ", uris.Select(r => r.AbsoluteUri))));
			}

			var types = clipboard.Types;
			if (types != null)
			{
				foreach (var type in types)
				{
					panel.Items.Add(new Label { Text = $"\n{type}:", Font = SystemFonts.Bold() });
					string str = null;
					byte[] data = null;
					try
					{
						str = clipboard.GetString(type);
						if (str != null)
						{
							panel.Items.Add($"- String, Length: {str.Length}");
							panel.Items.Add(ReadOnlyTextArea(str));
						}
					}
					catch (Exception ex)
					{
						panel.Items.Add($"- Error getting string: {ex.Message}");
					}
					try
					{
						data = clipboard.GetData(type);
						if (data != null)
						{
							panel.Items.Add($"- Data, Length: {data.Length}");
							var hexString = BitConverter.ToString(data);
							panel.Items.Add(ReadOnlyTextArea(hexString.Substring(0, Math.Min(hexString.Length, 1000))));
						}
					}
					catch (Exception ex)
					{
						panel.Items.Add($"- Error getting data: {ex.Message}");
					}
					if (type == CustomObjectType)
					{
						try
						{
							var obj = clipboard.GetObject(type);
							if (obj != null)
							{
								panel.Items.Add($"- Object, Type: {obj.GetType()}:");
								panel.Items.Add(ReadOnlyTextArea(obj.ToString()));
							}
						}
						catch (Exception ex)
						{
							panel.Items.Add($"- Error getting object: {ex.Message}");
						}
					}
				}
			}
			pasteData.Content = panel;
		}
	}
}
