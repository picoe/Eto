using System;
using Eto.Forms;
using System.Text;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Drag and Drop")]
	public class DragDropSection : Panel
	{
		EnumDropDown<DragEffects?> dragOverEffect;
		CheckBox showDragOverEvents;
		CheckBox useDragImage;
		PointEntry imageOffset;
		TextBox descriptionTextBox;
		TextBox innerTextBox;
		CheckBox writeDataCheckBox;
		EnumDropDown<DragEffects> allowedEffectDropDown;

		public DragDropSection()
		{
			// drag data object

			showDragOverEvents = new CheckBox { Text = "Show DragOver Events" };
			var includeImageCheck = new CheckBox { Text = "Include Image" };
			descriptionTextBox = new TextBox { PlaceholderText = "Format", ToolTip = "Add {0} to insert inner text into the description, e.g. 'Move to {0}'" };
			innerTextBox = new TextBox { PlaceholderText = "Inner", ToolTip = "Highlighted text to insert into description" };
			var textBox = new TextBox { Text = "Some text" };
			allowedEffectDropDown = new EnumDropDown<DragEffects> { SelectedValue = DragEffects.All };
			dragOverEffect = new EnumDropDown<DragEffects?> { SelectedValue = DragEffects.Copy };
			writeDataCheckBox = new CheckBox { Text = "Write data to log" };
			useDragImage = new CheckBox { Text = "Use custom drag image" };
			imageOffset = new PointEntry { Value = new Point(80, 80) };
			imageOffset.Bind(c => c.Enabled, useDragImage, c => c.Checked);

			var htmlTextArea = new TextArea();
			var selectFilesButton = new Button { Text = "Select Files" };
			Uri[] fileUris = null;
			selectFilesButton.Click += (sender, e) =>
			{
				var ofd = new OpenFileDialog();
				ofd.MultiSelect = true;
				ofd.ShowDialog(this);
				fileUris = ofd.Filenames.Select(r => new Uri(r)).ToArray();
				if (fileUris.Length == 0)
					fileUris = null;
			};

			var urlTextBox = new TextBox();

			DataObject CreateDataObject()
			{
				var data = new DataObject();
				if (!string.IsNullOrEmpty(textBox.Text))
					data.Text = textBox.Text;
				var uris = new List<Uri>();
				if (fileUris != null)
					uris.AddRange(fileUris);
				if (Uri.TryCreate(urlTextBox.Text, UriKind.Absolute, out var uri))
					uris.Add(uri);
				if (uris.Count > 0)
					data.Uris = uris.ToArray();
				if (!string.IsNullOrEmpty(htmlTextArea.Text))
					data.Html = htmlTextArea.Text;
				if (includeImageCheck.Checked == true)
					data.Image = TestIcons.Logo;
				return data;
			}

			// sources

			var buttonSource = new Button { Text = "Source" };
			buttonSource.MouseDown += (sender, e) =>
			{
				if (e.Buttons != MouseButtons.None)
				{
					DoDragDrop(buttonSource, CreateDataObject());
					e.Handled = true;
				}
			};

			var panelSource = new Panel { BackgroundColor = Colors.Red, Size = new Size(50, 50) };
			panelSource.MouseMove += (sender, e) =>
			{
				if (e.Buttons != MouseButtons.None)
				{
					DoDragDrop(panelSource, CreateDataObject());
					e.Handled = true;
				}
			};

			var treeSource = new TreeGridView { Size = new Size(200, 200) };
			treeSource.SelectedItemsChanged += (sender, e) => Log.Write(treeSource, $"TreeGridView.SelectedItemsChanged (source) Rows: {string.Join(", ", treeSource.SelectedRows.Select(r => r.ToString()))}");
			treeSource.DataStore = CreateTreeData();
			SetupTreeColumns(treeSource);
			treeSource.MouseMove += (sender, e) =>
			{
				if (e.Buttons == MouseButtons.Primary && !treeSource.IsEditing)
				{
					var cell = treeSource.GetCellAt(e.Location);
					if (cell.Item == null || cell.ColumnIndex == -1)
						return;
					var data = CreateDataObject();
					var selected = treeSource.SelectedItems.OfType<TreeGridItem>().Select(r => (string)r.Values[0]);
					data.SetString(string.Join(";", selected), "my-tree-data");

					DoDragDrop(treeSource, data);
					e.Handled = true;
				}
			};

			var gridSource = new GridView {  };
			gridSource.SelectedRowsChanged += (sender, e) => Log.Write(gridSource, $"GridView.SelectedItemsChanged (source): {string.Join(", ", gridSource.SelectedRows.Select(r => r.ToString()))}");
			SetupGridColumns(gridSource);
			gridSource.DataStore = CreateGridData();
			gridSource.MouseMove += (sender, e) =>
			{
				if (e.Buttons == MouseButtons.Primary && !gridSource.IsEditing)
				{
					var cell = gridSource.GetCellAt(e.Location);
					if (cell.RowIndex == -1 || cell.ColumnIndex == -1)
						return;
					var data = CreateDataObject();
					var selected = gridSource.SelectedItems.OfType<GridItem>().Select(r => (string)r.Values[0]);
					data.SetString(string.Join(";", selected), "my-grid-data");

					DoDragDrop(gridSource, data);
					e.Handled = true;
				}
			};


			// destinations

			var buttonDestination = new Button { Text = "Drop here!", AllowDrop = true };
			buttonDestination.DragEnter += (sender, e) => buttonDestination.Text = "Now, drop it!";
			buttonDestination.DragLeave += (sender, e) => buttonDestination.Text = "Drop here!";
			LogEvents(buttonDestination);

			var drawableDest = new Drawable { BackgroundColor = Colors.Blue, AllowDrop = true, Size = new Size(50, 50) };
			LogEvents(drawableDest);
			drawableDest.DragEnter += (sender, e) =>
			{
				if (e.Effects != DragEffects.None)
					drawableDest.BackgroundColor = Colors.Green;
			};
			drawableDest.DragLeave += (sender, e) =>
			{
				if (e.Effects != DragEffects.None)
					drawableDest.BackgroundColor = Colors.Blue;
			};
			drawableDest.DragDrop += (sender, e) =>
			{
				if (e.Effects != DragEffects.None)
					drawableDest.BackgroundColor = Colors.Blue;
			};

			var dragMode = new RadioButtonList
			{
				Orientation = Orientation.Vertical,
				Items =
				{
					new ListItem { Text = "No Restriction", Key = ""},
					new ListItem { Text = "RestrictToOver", Key = "over"},
					new ListItem { Text = "RestrictToInsert", Key = "insert"},
					new ListItem { Text = "RestrictToNode", Key = "node"},
					new ListItem { Text = "No Node", Key = "none"}
				},
				SelectedIndex = 0
			};
			var treeDest = new TreeGridView { AllowDrop = true, Size = new Size(200, 200) };
			var treeDestData = CreateTreeData();
			treeDest.DataStore = treeDestData;
			treeDest.DragOver += (sender, e) =>
			{
				var info = treeDest.GetDragInfo(e);
				if (info == null)
					return; // not supported

				switch (dragMode.SelectedKey)
				{
					case "over":
						info.RestrictToOver();
						break;
					case "insert":
						info.RestrictToInsert();
						break;
					case "node":
						info.RestrictToNode(treeDestData[2]);
						break;
					case "none":
						info.Item = info.Parent = null;
						break;
				}
			};
			SetupTreeColumns(treeDest);
			LogEvents(treeDest);

			var gridDest = new GridView { AllowDrop = true };
			var gridDestData = CreateGridData();
			gridDest.DataStore = gridDestData;
			gridDest.DragOver += (sender, e) =>
			{
				var info = gridDest.GetDragInfo(e);
				if (info == null)
					return; // not supported

				switch (dragMode.SelectedKey)
				{
					case "over":
						info.RestrictToOver();
						break;
					case "insert":
						info.RestrictToInsert();
						break;
					case "node":
						info.Index = 2;
						info.Position = GridDragPosition.Over;
						break;
					case "none":
						info.Index = -1;
						break;
				}
			};
			SetupGridColumns(gridDest);
			LogEvents(gridDest);



			// layout

			var layout = new DynamicLayout { Padding = 10, DefaultSpacing = new Size(4, 4) };

			layout.BeginHorizontal();

			layout.BeginScrollable(BorderType.None);
			layout.BeginCentered();

			layout.AddSeparateRow(showDragOverEvents);
			layout.AddSeparateRow("AllowedEffect", allowedEffectDropDown, null);
			layout.BeginVertical();
			layout.AddRow("DropDescription", descriptionTextBox);
			layout.AddRow(new Panel(), innerTextBox);
			layout.EndVertical();
			layout.AddSeparateRow("DragOver Effect", dragOverEffect, null);
			layout.AddSeparateRow(useDragImage);
			layout.AddSeparateRow("Image offset:", imageOffset);
			layout.AddSeparateRow(writeDataCheckBox);

			layout.BeginGroup("DataObject", 10);
			layout.AddRow("Text", textBox);
			layout.AddRow("Html", htmlTextArea);
			layout.AddRow("Url", urlTextBox);
			layout.BeginHorizontal();
			layout.AddSpace();
			layout.BeginVertical();
			layout.AddCentered(includeImageCheck);
			layout.AddCentered(selectFilesButton);
			layout.EndVertical();
			layout.EndGroup();
			layout.Add(dragMode);
			layout.AddSpace();

			layout.EndCentered();
			layout.EndScrollable();

			layout.BeginVertical(xscale: true);
			layout.AddRange("Drag sources:", buttonSource, panelSource);
			layout.Add(treeSource, yscale: true);
			layout.Add(gridSource, yscale: true);
			layout.EndVertical();

			layout.BeginVertical(xscale: true);
			layout.AddRange("Drag destinations:", buttonDestination, drawableDest);
			layout.Add(treeDest, yscale: true);
			layout.Add(gridDest, yscale: true);
			layout.EndVertical();

			layout.EndHorizontal();

			Content = layout;
		}

		void DoDragDrop(Control control, DataObject data)
		{
			if (useDragImage.Checked == true)
			{
				var bmp = new Bitmap(100, 100, PixelFormat.Format32bppRgba);
				using (var g = new Graphics(bmp))
				{
					g.FillEllipse(Brushes.Blue, 0, 0, 100, 100);
				}
				control.DoDragDrop(data, allowedEffectDropDown.SelectedValue, bmp, imageOffset.Value);
			}
			else
			{
				control.DoDragDrop(data, allowedEffectDropDown.SelectedValue);
			}
		}

		void SetupTreeColumns(TreeGridView tree)
		{
			tree.ShowHeader = false;
			tree.AllowMultipleSelection = true;
			tree.Columns.Add(new GridColumn { DataCell = new TextBoxCell(0), Editable = true });
		}

		void SetupGridColumns(GridView grid)
		{
			grid.ShowHeader = false;
			grid.AllowMultipleSelection = true;
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell(0), Editable = true });
		}

		TreeGridItemCollection CreateTreeData()
		{
			var coll = new TreeGridItemCollection();
			CreateTreeData(coll, 0);
			return coll;
		}

		void CreateTreeData(TreeGridItemCollection coll, int level, string parentString = "Item")
		{
			for (int i = 0; i < 10; i++)
			{
				var itemText = $"{parentString}-{i}";
				var item = new TreeGridItem { Values = new[] { itemText } };
				if (level < 2 && (i % 4) != 0)
					CreateTreeData(item.Children, level + 1, itemText);
				coll.Add(item);
			}
		}

		IEnumerable<object> CreateGridData()
		{
			var data = new ObservableCollection<GridItem>();

			for (int i = 0; i < 100; i++)
			{
				var item = new GridItem { Values = new[] { $"Item {i}" } };
				data.Add(item);
			}
			return data;
		}

		string WriteDragInfo(object sender, DragEventArgs e)
		{
			var sb = new StringBuilder();
			var obj = e.Source?.GetType().ToString() ?? "Unknown";
			sb.Append($"Source: {obj}, AllowedEffects: {e.AllowedEffects}");
			sb.Append($"\n\tLocation: {e.Location}, Modifiers: {e.Modifiers}, Buttons: {e.Buttons}");
			var treeGridInfo = (sender as TreeGridView)?.GetDragInfo(e);
			if (treeGridInfo != null)
				sb.Append($"\n\tParent: {(treeGridInfo.Parent as TreeGridItem)?.Values[0]}, ChildIndex: {treeGridInfo.ChildIndex}, Item: {(treeGridInfo.Item as TreeGridItem)?.Values[0]}, Position: {treeGridInfo.Position}, InsertIndex: {treeGridInfo.InsertIndex}");

			var gridInfo = (sender as GridView)?.GetDragInfo(e);
			if (gridInfo != null)
				sb.Append($"\n\tItem: {(gridInfo.Item as GridItem)?.Values[0]}, Index: {gridInfo.Index}, Position: {gridInfo.Position}, InsertIndex: {gridInfo.InsertIndex}");

			var data = e.Data;
			if (data.Text != null)
				sb.Append($"\n\tText: {data.Text}");
			if (data.Html != null)
				sb.Append($"\n\tHtml: {data.Html}");
			if (data.Types != null)
				sb.Append($"\n\tTypes: {string.Join(", ", data.Types)}");
			var uris = data.Uris;
			if (uris != null)
				sb.Append($"\n\tUris: {string.Join(", ", uris.Select(r => r.AbsoluteUri))})");
			return sb.ToString();
		}

		void LogEvents(Control control)
		{
			control.DragEnter += (sender, e) =>
			{
				Log.Write(sender, $"DragEnter: {WriteDragInfo(sender, e)}");

				if (dragOverEffect.SelectedValue != null)
					e.Effects = dragOverEffect.SelectedValue.Value;
				if (!string.IsNullOrEmpty(descriptionTextBox.Text) && e.SupportsDropDescription)
					e.SetDropDescription(descriptionTextBox.Text, innerTextBox.Text);
				WriteData(e.Data);
			};
			control.DragLeave += (sender, e) =>
			{
				Log.Write(sender, $"DragLeave: {WriteDragInfo(sender, e)}");
				if (dragOverEffect.SelectedValue != null)
					e.Effects = dragOverEffect.SelectedValue.Value;

				WriteData(e.Data);
			};
			control.DragOver += (sender, e) =>
			{
				if (showDragOverEvents.Checked == true)
				{
					Log.Write(sender, $"DragOver: {WriteDragInfo(sender, e)}");

					if (!string.IsNullOrEmpty(descriptionTextBox.Text) && e.SupportsDropDescription)
						e.SetDropDescription(descriptionTextBox.Text + " (over)", innerTextBox.Text);

				}

				if (dragOverEffect.SelectedValue != null)
					e.Effects = dragOverEffect.SelectedValue.Value;

				if (showDragOverEvents.Checked == true)
					WriteData(e.Data);
			};
			control.DragDrop += (sender, e) =>
			{
				Log.Write(sender, $"DragDrop: {WriteDragInfo(sender, e)}");

				if (dragOverEffect.SelectedValue != null)
					e.Effects = dragOverEffect.SelectedValue.Value;

				WriteData(e.Data);
			};
		}

		void WriteData(DataObject data)
		{
			if (writeDataCheckBox.Checked != true)
				return;
			foreach (var format in data.Types)
			{
				try
				{
					var d = data.GetData(format);
					if (d != null)
					{
						var s = string.Join(",", d.Select(r => r.ToString()).Take(1000));
						Log.Write(null, $"\t{format}: {s}");
					}
					else
						Log.Write(null, $"\t{format}: {d}");
				}
				catch
				{

				}
			}
		}
	}
}
