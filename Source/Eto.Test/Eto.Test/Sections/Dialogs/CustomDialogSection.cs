using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Dialogs
{
	[Section("Dialogs", typeof(Dialog), "Custom Dialog")]
	public class CustomDialogSection : Scrollable
	{
		public bool UseAsync { get; set; }
		public DialogDisplayMode DisplayMode { get; set; }

		public CustomDialogSection()
		{
			var layout = new DynamicLayout { Spacing = new Size(20, 20), DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddSeparateRow(null, UseAsyncCheckBox(), DisplayModeDropDown(), null);
			layout.BeginVertical();
			layout.AddRow(null, Standard(), null);
			layout.AddRow(null, Resizable(), null);
			layout.AddRow(null, KitchenSink(), null);
			layout.EndVertical();

			layout.Add(null);

			Content = layout;
		}

		Control UseAsyncCheckBox()
		{
			var control = new CheckBox { Text = "Use Async" };
			control.CheckedBinding.Bind(() => UseAsync, val => UseAsync = val ?? false);
			return control;
		}
		Control DisplayModeDropDown()
		{
			var control = new EnumDropDown<DialogDisplayMode>();
			control.SelectedValueBinding.Bind(() => DisplayMode, val => DisplayMode = val);
			return control;
		}

		Dialog CreateDialog()
		{
			var dialog = new Dialog();
			dialog.DisplayMode = DisplayMode;

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };

			layout.AddCentered(new Label { Text = "Content" }, yscale: true);

			dialog.DefaultButton = new Button { Text = "Default Button" };
			dialog.AbortButton = new Button { Text = "Abort Button" };

			dialog.DefaultButton.Click += delegate
			{
				MessageBox.Show("Default button clicked");
			};

			dialog.AbortButton.Click += delegate
			{
				MessageBox.Show("Abort button clicked");
				dialog.Close();
			};

			layout.BeginVertical();
			layout.AddRow(null, dialog.DefaultButton, dialog.AbortButton);
			layout.EndVertical();

			dialog.Content = layout;

			return dialog;
		}

		Control Standard()
		{
			var control = new Button { Text = "Standard Dialog" };
			control.Click += delegate
			{
				var dialog = CreateDialog();
				dialog.Title = "Standard Dialog";
				Show(dialog, this);

			};
			return control;
		}

		Control Resizable()
		{
			var control = new Button { Text = "Resizable" };
			control.Click += delegate
			{
				var dialog = CreateDialog();
				dialog.Title = "Resizable Dialog";
				if (Platform.IsDesktop)
					dialog.Resizable = true;
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
				dialog.DisplayMode = DisplayMode;
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

