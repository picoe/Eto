using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Dialogs
{
	[Section("Dialogs", typeof(ColorDialog))]
	public class ColorDialogSection : Panel
	{
		public bool AllowAlpha { get; set; }

		public ColorDialogSection() : this(true)
		{

		}

		ColorDialogSection(bool showCreateDialog)
		{
			var layout = new DynamicLayout { Spacing = new Size(20, 20), DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			var btnCreateDialog = new Button { Text = "Use in Dialog" };
			btnCreateDialog.Click += BtnCreateDialog_Click;

			layout.BeginCentered();
			layout.Add(CreateAllowAlphaCheckBox());
			layout.Add(PickColor());
			layout.Add(PickColorWithStartingColor());
			if (showCreateDialog)
				layout.Add(btnCreateDialog);

			layout.EndCentered();
			layout.AddCentered(new ColorPicker());

			layout.AddCentered(new TextBox());

			layout.AddSpace();

			Content = layout;
		}

		private void BtnCreateDialog_Click(object sender, System.EventArgs e)
		{
			var dlg = new Dialog();
			dlg.Content = new ColorDialogSection(false);
			dlg.ShowModal(this);
		}

		Control CreateAllowAlphaCheckBox()
		{
			var control = new CheckBox { Text = "AllowAlpha" };
			control.CheckedBinding.Bind(this, c => c.AllowAlpha);
			return control;
		}

		Control PickColor()
		{
			var button = new Button { Text = "Pick Color" };
			button.Click += delegate
			{
				var dialog = new ColorDialog
				{
					AllowAlpha = AllowAlpha
				};
				dialog.ColorChanged += delegate
				{
					// you need to handle this event for OS X, where the dialog is a floating window
					Log.Write(dialog, "ColorChanged, Color: {0}", dialog.Color);
				};
				var result = dialog.ShowDialog(ParentWindow);
				if (result == DialogResult.Ok)
				{
					Log.Write(dialog, "Result: {0}, Color: {1}", result, dialog.Color);
				}
				else
					Log.Write(dialog, "Result: {0}", result);
			};
			return button;
		}

		Control PickColorWithStartingColor()
		{
			var button = new Button { Text = "Pick Color with initial starting color (green)" };
			button.Click += delegate
			{
				var dialog = new ColorDialog
				{
					Color = Colors.Lime,
					AllowAlpha = AllowAlpha
				};
				dialog.ColorChanged += delegate
				{
					// need to handle this event for OS X, where the dialog is a floating window
					Log.Write(dialog, "ColorChanged, Color: {0}", dialog.Color);
				};
				var result = dialog.ShowDialog(ParentWindow);
				if (result == DialogResult.Ok)
				{
					Log.Write(dialog, "Result: {0}, Color: {1}", result, dialog.Color);
				}
				else
					Log.Write(dialog, "Result: {0}", result);
			};
			return button;
		}
	}
}

