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
		EnumDropDown<DragEffects> dragOverEffect;
		CheckBox showDragOverEvents;

		public DragDropSection()
		{
			// drag data object

			showDragOverEvents = new CheckBox { Text = "Show DragOver Events" };
			var includeImageCheck = new CheckBox { Text = "Include Image" };
			var textBox = new TextBox { Text = "Some text" };
			var allowedEffectDropDown = new EnumDropDown<DragEffects> { SelectedValue = DragEffects.All };
			dragOverEffect = new EnumDropDown<DragEffects> { SelectedValue = DragEffects.Copy };

			var htmlTextArea = new TextArea();
			var selectFilesButton = new Button { Text = "Select Files" };
			Uri[] uris = null;
			selectFilesButton.Click += (sender, e) =>
			{
				var ofd = new OpenFileDialog();
				ofd.MultiSelect = true;
				ofd.ShowDialog(this);
				uris = ofd.Filenames.Select(r => new Uri(r)).ToArray();
				if (uris.Length == 0)
					uris = null;
			};

			Func<DataObject> createDataObject = () =>
			{
				var data = new DataObject();
				if (!string.IsNullOrEmpty(textBox.Text))
					data.Text = textBox.Text;
				if (uris != null)
					data.Uris = uris;
				if (!string.IsNullOrEmpty(htmlTextArea.Text))
					data.Html = htmlTextArea.Text;
				if (includeImageCheck.Checked == true)
					data.Image = TestIcons.Logo;
				return data;
			};

			// sources

			var buttonSource = new Button { Text = "Source" };
			buttonSource.MouseDown += (sender, e) =>
			{
				buttonSource.DoDragDrop(createDataObject(), allowedEffectDropDown.SelectedValue);
				e.Handled = true;
			};

			var panelSource = new Panel { BackgroundColor = Colors.Red, Size = new Size(50, 50) };
			panelSource.MouseDown += (sender, e) =>
			{
				panelSource.DoDragDrop(createDataObject(), allowedEffectDropDown.SelectedValue);
				e.Handled = true;
			};

			var treeSource = new TreeGridView { Size = new Size(200, 200) };
			treeSource.DataStore = CreateTreeData();
			SetupTreeColumns(treeSource);
			treeSource.MouseMove += (sender, e) =>
			{
				if (e.Buttons == MouseButtons.Primary)
				{
					var cell = treeSource.GetCellAt(e.Location);
					if (cell.Item == null || cell.ColumnIndex == -1)
						return;
					var data = createDataObject();
					var selected = treeSource.SelectedItems.OfType<TreeGridItem>().Select(r => (string)r.Values[0]);
					data.SetString(string.Join(";", selected), "my-tree-data");

					treeSource.DoDragDrop(data, allowedEffectDropDown.SelectedValue);
					e.Handled = true;
				}
			};

			var gridSource = new GridView {  };
			SetupGridColumns(gridSource);
			gridSource.DataStore = CreateGridData();
			gridSource.MouseMove += (sender, e) =>
			{
				if (e.Buttons == MouseButtons.Primary)
				{
					var data = createDataObject();
					var selected = gridSource.SelectedItems.OfType<GridItem>().Select(r => (string)r.Values[0]);
					data.SetString(string.Join(";", selected), "my-grid-data");

					gridSource.DoDragDrop(data, allowedEffectDropDown.SelectedValue);
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

			layout.BeginCentered();

			layout.AddSeparateRow(showDragOverEvents);
			layout.AddSeparateRow("AllowedEffect", allowedEffectDropDown, null);
			layout.AddSeparateRow("DragOver Effect", dragOverEffect, null);

			layout.BeginGroup("DataObject", 10);
			layout.AddRow("Text", textBox);
			layout.AddRow("Html", htmlTextArea);
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

		void SetupTreeColumns(TreeGridView tree)
		{
			tree.ShowHeader = false;
			tree.AllowMultipleSelection = true;
			tree.Columns.Add(new GridColumn { DataCell = new TextBoxCell(0) });
		}

		void SetupGridColumns(GridView grid)
		{
			grid.ShowHeader = false;
			grid.AllowMultipleSelection = true;
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell(0) });
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
				e.Effects = dragOverEffect.SelectedValue;
			};
			control.DragLeave += (sender, e) =>
			{
				Log.Write(sender, $"DragLeave: {WriteDragInfo(sender, e)}");
				e.Effects = dragOverEffect.SelectedValue;
			};
			control.DragOver += (sender, e) =>
			{
				if (showDragOverEvents.Checked == true)
					Log.Write(sender, $"DragOver: {WriteDragInfo(sender, e)}");

				e.Effects = dragOverEffect.SelectedValue;
			};
			control.DragDrop += (sender, e) =>
			{
				Log.Write(sender, $"DragDrop: {WriteDragInfo(sender, e)}");
				e.Effects = dragOverEffect.SelectedValue;
			};
		}
	}
}
