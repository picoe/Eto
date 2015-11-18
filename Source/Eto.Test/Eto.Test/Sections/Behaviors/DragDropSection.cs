using System;
using Eto.Forms;
using System.Text;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Drag and Drop")]
	public class DragDropSection : Panel
	{
		public DragDropSection()
		{
			var buttonSource = new Button() { Text = "Source", AllowDrag = true };
			var panelSource = new Panel() { BackgroundColor = Colors.Red, AllowDrag = true, Width = 20, Height = 20 };
			var buttonDestination = new Button() { Text = "Drop here!", AllowDrop = true };
						
			buttonSource.MouseDown += (sender, e) =>
			{
				buttonSource.DoDragDrop(new DragDropData()
				{
					Text = "test button",
					Uris = new List<Uri>()
					{
						new Uri(@"c:\button.txt"),
						new Uri(@"c:\test2.txt"),
					}
				}, DragDropAction.Link);
			};

			panelSource.MouseDown += (sender, e) =>
			{
				panelSource.DoDragDrop(new DragDropData(), DragDropAction.Link);
			};

			buttonDestination.DragDrop += (sender, e) =>
			{
				var obj = "Uknown";

				if (e.Source != null)
				{
					obj = e.Source.GetType().ToString();
				}

				if (e.Data != null)
				{
					Log.Write(sender, "Dropped text data: " + e.Data.Text);
					Log.Write(sender, "Dropped URIs:");
					foreach (var uri in e.Data.Uris)
					{
						Log.Write(sender, uri.LocalPath);
					}
				}

				buttonDestination.Text = obj + " object dropped.";
			};

			buttonDestination.DragOver += (sender, e) =>
			{
				var obj = "Uknown";

				if (e.Source != null)
				{
					obj = e.Source.GetType().ToString();

					// As example allow drop of button objects but not panels
					if (e.Source is Button)
					{
						e.Effect = DragDropAction.Link;
					}
					else
					{
						e.Effect = DragDropAction.None;
					}
				}
				else
				{
					e.Effect = DragDropAction.Link;
				}

				buttonDestination.Text = obj + " object dragged over.";
			};

			var content = new TableLayout()
			{
				Rows = {
					new TableRow()
					{
						Cells = {
							new TableCell()
							{
								Control = buttonSource
							},
							new TableCell()
							{
								ScaleWidth = true
							},
							new TableCell()
							{
								Control = buttonDestination
							}
						}
					},
					new TableRow()
					{
						Cells = {
							new TableCell()
							{
								Control = panelSource
							},
							new TableCell()
							{
								ScaleWidth = true
							}
						}
					},
					new TableRow()
				},
				Padding = new Padding(10)
			};

			this.Content = content;
		}
	}
}
