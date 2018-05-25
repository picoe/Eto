using Eto.Forms;
using Eto.Drawing;
using System.Linq;
using System;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "ContextMenu")]
	public class ContextMenuSection : Panel
	{
		public bool RelativeToLabel { get; set; }
		public bool UseLocation { get; set; }

		PointF location = new PointF(100, 100);

		public ContextMenuSection() : this(false)
		{
		}

		ContextMenuSection(bool inDialog)
		{
			var relativeToLabelCheckBox = new CheckBox { Text = "Relative to label" };
			relativeToLabelCheckBox.CheckedBinding.Bind(this, c => c.RelativeToLabel);

			var useLocationCheckBox = new CheckBox { Text = "Use Location" };
			useLocationCheckBox.CheckedBinding.Bind(this, c => c.UseLocation);

			var locationInput = PointControl(() => location, p => location = p);

			var contextMenuLabel = CreateContextMenuLabel();

			var showInDialog = new Button { Text = "Show in Dialog" };
			showInDialog.Click += (sender, e) =>
			{
				var dlg = new Dialog { Content = new ContextMenuSection(true) };
				dlg.ShowModal(this);
			};

			// layout

			var layout = new DynamicLayout();

			layout.BeginCentered();
			layout.AddAutoSized(relativeToLabelCheckBox);
			layout.AddSeparateRow(useLocationCheckBox, locationInput);
			if (!inDialog)
				layout.AddAutoSized(showInDialog);
			layout.EndCentered();

			layout.AddCentered(contextMenuLabel, yscale: true);

			Content = layout;
		}

		Control PointControl(Func<PointF> getValue, Action<PointF> setValue)
		{
			var xpoint = new NumericStepper();
			xpoint.ValueBinding.Bind(() => getValue().X, v =>
			{
				var p = getValue();
				p.X = (float)v;
				setValue(p);
			});

			var ypoint = new NumericStepper();
			ypoint.ValueBinding.Bind(() => getValue().Y, v =>
			{
				var p = getValue();
				p.Y = (float)v;
				setValue(p);
			});

			return new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Items = { "X:", xpoint, "Y:", ypoint }
			};
		}


		ContextMenu _menu;

		ContextMenu CreateMenu()
		{
			if (_menu != null)
				return _menu;
			
			_menu = new ContextMenu();

			_menu.Opening += (sender, e) => Log.Write(sender, "Opening");
			_menu.Closed += (sender, e) => Log.Write(sender, "Closed");
			_menu.Closing += (sender, e) => Log.Write(sender, "Closing");

			_menu.Items.Add(new ButtonMenuItem { Text = "Item 1" });
			_menu.Items.Add(new ButtonMenuItem { Text = "Item 2", Shortcut = Keys.Control | Keys.I });
			_menu.Items.Add(new ButtonMenuItem { Text = "Item 3", Shortcut = Keys.Shift | Keys.I });
			_menu.Items.Add(new ButtonMenuItem { Text = "Item 4", Shortcut = Keys.Alt | Keys.I });
			_menu.Items.Add(new ButtonMenuItem { Text = "Disabled Item", Enabled = false });

			var subMenu = _menu.Items.GetSubmenu("Sub Menu");
			subMenu.Items.Add(new ButtonMenuItem { Text = "Item 5", Shortcut = Keys.Application | Keys.I });
			subMenu.Items.Add(new ButtonMenuItem { Text = "Item 6", Shortcut = Keys.I });
			subMenu.Items.Add(new ButtonMenuItem { Text = "Disabled Item 2", Enabled = false });

			_menu.Items.AddSeparator();
			RadioMenuItem radioController;
			_menu.Items.Add(radioController = new RadioMenuItem { Text = "Radio 1" });
			_menu.Items.Add(new RadioMenuItem(radioController) { Text = "Radio 2", Checked = true });
			_menu.Items.Add(new RadioMenuItem(radioController) { Text = "Radio 3", Shortcut = Keys.R });
			_menu.Items.Add(new RadioMenuItem(radioController) { Text = "Radio 4" });
			_menu.Items.Add(new RadioMenuItem(radioController) { Text = "Radio 5 Disabled", Enabled = false });

			_menu.Items.AddSeparator();
			_menu.Items.Add(new CheckMenuItem { Text = "Check 1" });
			_menu.Items.Add(new CheckMenuItem { Text = "Check 2", Shortcut = Keys.Control | Keys.Alt | Keys.G, Checked = true });
			_menu.Items.Add(new CheckMenuItem { Text = "Check 3", Shortcut = Keys.Control | Keys.Shift | Keys.G });
			_menu.Items.Add(new CheckMenuItem { Text = "Check 4", Shortcut = Keys.Control | Keys.Application | Keys.G });
			_menu.Items.Add(new CheckMenuItem { Text = "Check 5", Shortcut = Keys.Shift | Keys.Alt | Keys.G });
			_menu.Items.Add(new CheckMenuItem { Text = "Check 6", Shortcut = Keys.Shift | Keys.Application | Keys.G });
			_menu.Items.Add(new CheckMenuItem { Text = "Check 7", Shortcut = Keys.Alt | Keys.Application | Keys.G });
			_menu.Items.Add(new CheckMenuItem { Text = "Disabled Check", Checked = true, Enabled = false });

			LogEvents(_menu);
			return _menu;
		}

		Control CreateContextMenuLabel()
		{
			var label = new Label
			{
				Size = new Size(100, 100),
				BackgroundColor = Colors.Blue,
				TextColor = Colors.White,
				TextAlignment = TextAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Text = "Click on me!"
			};
			label.MouseDown += (sender, e) =>
			{
				var menu = CreateMenu();

				if (UseLocation)
				{
					if (RelativeToLabel)
						menu.Show(label, location);
					else
						menu.Show(location);
				}
				else if (RelativeToLabel)
					menu.Show(label);
				else
					menu.Show();
			};
			return label;
		}

		void LogEvents(ISubmenu menu)
		{
			if (menu == null)
				return;
			foreach (var item in menu.Items)
			{
				LogClickEvents(item);
			}
		}

		void LogEvents(CheckMenuItem item)
		{
			if (item == null)
				return;
			item.CheckedChanged += delegate
			{
				Log.Write(item, "CheckedChanged, Item: {0}, Checked: {1}", item.Text, item.Checked);
			};
		}

		void LogEvents(RadioMenuItem item)
		{
			if (item == null)
				return;
			item.CheckedChanged += delegate
			{
				Log.Write(item, "CheckedChanged, Item: {0}, Checked: {1}", item.Text, item.Checked);
			};
		}

		void LogClickEvents(MenuItem item)
		{
			LogEvents(item as ISubmenu);
			LogEvents(item as CheckMenuItem);
			LogEvents(item as RadioMenuItem);
			item.Click += delegate
			{
				Log.Write(item, "Click, Item: {0}", item.Text);
			};
		}
	}
}