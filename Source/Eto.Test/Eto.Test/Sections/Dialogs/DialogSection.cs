using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Dialogs
{
	[Section("Dialogs", typeof(Dialog))]
	public class DialogSection : Scrollable
	{
		public bool UseAsync { get; set; }
		public bool AddButtons { get; set; }
		public bool SetResizable { get; set; }
		public WindowState? DialogWindowState { get; set; }
		public DialogDisplayMode? DisplayMode { get; set; }

		public DialogSection()
		{
			var layout = new DynamicLayout { Spacing = new Size(20, 20), DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddSeparateRow(null, UseAsyncCheckBox(), AddButtonsCheckBox(), ResizableCheckBox(), null);
			layout.AddSeparateRow(null, "DisplayMode", DisplayModeDropDown(), "WindowState", WindowStateDropDown(), null);
			layout.BeginVertical();
			layout.AddRow(null, SimpleDialogButton(), null);
			layout.AddRow(null, KitchenSink(), null);
			layout.EndVertical();

			layout.Add(null);

			Content = layout;
		}

		Control UseAsyncCheckBox()
		{
			var control = new CheckBox { Text = "Use Async" };
			control.CheckedBinding.Bind(this, c => c.UseAsync);
			return control;
		}

		Control DisplayModeDropDown()
		{
			var control = new EnumDropDown<DialogDisplayMode?>();
			control.SelectedValueBinding.Bind(this, c => c.DisplayMode);
			return control;
		}

		Control WindowStateDropDown()
		{
			var control = new EnumDropDown<WindowState?>();
			control.SelectedValueBinding.Bind(this, c => c.DialogWindowState);
			return control;
		}

		Control AddButtonsCheckBox()
		{
			var control = new CheckBox { Text = "Add positive && negative buttons" };
			control.CheckedBinding.Bind(this, c => c.AddButtons);
			return control;
		}

		Control ResizableCheckBox()
		{
			var control = new CheckBox { Text = "Resizable" };
			control.CheckedBinding.Bind(this, c => c.SetResizable);
			return control;
		}


		Dialog CreateDialog()
		{
			var dialog = new Dialog();
			if (DisplayMode != null)
				dialog.DisplayMode = DisplayMode.Value;

			if (DialogWindowState != null)
				dialog.WindowState = DialogWindowState.Value;

			if (SetResizable)
				dialog.Resizable = true;

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Size = new Size(400, 100) };

			layout.AddCentered(new Label { Text = "Content" }, yscale: true);

			if (AddButtons)
			{
				dialog.DefaultButton = new Button { Text = "Default Button" };
				dialog.PositiveButtons.Add(dialog.DefaultButton);
				dialog.DefaultButton.Click += delegate
				{
					MessageBox.Show("Default button clicked");
				};

				dialog.AbortButton = new Button { Text = "Abort Button" };
				dialog.NegativeButtons.Add(dialog.AbortButton);
				dialog.AbortButton.Click += delegate
				{
					MessageBox.Show("Abort button clicked");
					dialog.Close();
				};
			}

			dialog.Content = layout;

			return dialog;
		}

		Control SimpleDialogButton()
		{
			var control = new Button { Text = "Simple Dialog" };
			control.Click += delegate
			{
				var dialog = CreateDialog();
				dialog.Title = "Simple Dialog";
				Show(dialog, this);

			};
			return control;
		}

		Control KitchenSink()
		{
			var control = new Button { Text = "Kitchen Sink && Maximized" };
			control.Click += delegate
			{
				var dialog = new Dialog();
				if (DisplayMode != null)
					dialog.DisplayMode = DisplayMode.Value;
				if (Platform.IsDesktop)
				{
					dialog.Minimizable = true;
					dialog.Resizable = true;
					dialog.Maximizable = true;
					dialog.WindowState = WindowState.Maximized;
				}

				dialog.Title = "Kitchen Sink Dialog";
				dialog.Content = new Controls.KitchenSinkSection();
				Show(dialog, this);
			};

			return control;
		}

		public async void Show(Dialog dialog, Control parent)
		{
			if (UseAsync)
			{
				Log.Write(null, "Showing dialog async...");
				var dialogTask = dialog.ShowModalAsync(parent);
				Log.Write(null, "Waiting for dialog to close...");
				await dialogTask;
				Log.Write(null, "Dialog closed");
			}
			else
			{
				Log.Write(null, "Showing dialog (blocking)...");
				dialog.ShowModal(parent);
				Log.Write(null, "Dialog closed");
			}
		}
	}
}

