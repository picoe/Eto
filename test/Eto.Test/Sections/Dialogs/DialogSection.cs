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
		public bool SetMinimizable { get; set; }
		public bool SetMaximizable { get; set; }
		public bool SetOwner { get; set; } = true;
		public WindowState? DialogWindowState { get; set; }
		public DialogDisplayMode? DisplayMode { get; set; }

		public DialogSection()
		{
			var layout = new DynamicLayout { Spacing = new Size(20, 20), DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddSeparateRow(null, UseAsyncCheckBox(), AddButtonsCheckBox(), SetOwnerCheckBox(), null);
			layout.AddSeparateRow(null, ResizableCheckBox(), MinimizableCheckBox(), MaximizableCheckBox(), null);
			layout.AddSeparateRow(null, "DisplayMode", DisplayModeDropDown(), "WindowState", WindowStateDropDown(), null);
			layout.BeginVertical();
			layout.AddRow(null, SimpleDialogButton(), null);
			layout.AddRow(null, KitchenSink(), null);
			layout.EndVertical();

			layout.Add(null);

			Content = layout;
		}

		Control SetOwnerCheckBox()
		{
			var control = new CheckBox { Text = "Set owner" };
			control.CheckedBinding.Bind(this, c => c.SetOwner);
			return control;
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
			var control = new CheckBox { Text = "Add buttons" };
			control.CheckedBinding.Bind(this, c => c.AddButtons);
			return control;
		}

		Control ResizableCheckBox()
		{
			var control = new CheckBox { Text = "Resizable" };
			control.CheckedBinding.Bind(this, c => c.SetResizable);
			return control;
		}

		Control MinimizableCheckBox()
		{
			var control = new CheckBox { Text = "Minimizable" };
			control.CheckedBinding.Bind(this, c => c.SetMinimizable);
			return control;
		}

		Control MaximizableCheckBox()
		{
			var control = new CheckBox { Text = "Maximizable" };
			control.CheckedBinding.Bind(this, c => c.SetMaximizable);
			return control;
		}

		void SetOptions(Dialog dialog)
		{
			if (SetMinimizable)
				dialog.Minimizable = true;
			if (SetMaximizable)
				dialog.Maximizable = true;
			if (SetResizable)
				dialog.Resizable = true;
			if (DisplayMode != null)
				dialog.DisplayMode = DisplayMode.Value;
			if (DialogWindowState != null)
				dialog.WindowState = DialogWindowState.Value;
			
			if (AddButtons)
			{
				var openChildButton = new Button { Text = "Open child Dialog" };
				openChildButton.Click += (sender, e) => CreateDialog().ShowModal(dialog);
				dialog.PositiveButtons.Add(openChildButton);

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

			if (DisplayMode == DialogDisplayMode.Attached)
			{
				var closeDialogButton = new Button { Text = "Close" };
				closeDialogButton.Click += (sender, e) => dialog.Close();
				dialog.NegativeButtons.Add(closeDialogButton);
			}
		}

		Dialog CreateDialog()
		{
			var dialog = new Dialog { Title = "Simple Dialog" };
			SetOptions(dialog);

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };

			layout.Add(new Label
			{
				Text = "Content",
				Size = new Size(100, 100),
				VerticalAlignment = VerticalAlignment.Center,
				TextAlignment = TextAlignment.Center
			});

			dialog.Content = layout;

			return dialog;
		}

		Control SimpleDialogButton()
		{
			var control = new Button { Text = "Simple Dialog" };
			control.Click += delegate
			{
				var dialog = CreateDialog();
				Show(dialog, this);

			};
			return control;
		}

		Control KitchenSink()
		{
			var control = new Button { Text = "Kitchen Sink" };
			control.Click += delegate
			{
				var dialog = new Dialog
				{
					Title = "Kitchen Sink Dialog",
					Content = new Controls.KitchenSinkSection()
				};
				SetOptions(dialog);
				Show(dialog, this);
			};

			return control;
		}

		public async void Show(Dialog dialog, Control parent)
		{
			if (UseAsync)
			{
				Log.Write(null, "Showing dialog async...");
				var dialogTask = SetOwner ? dialog.ShowModalAsync(parent) : dialog.ShowModalAsync();
				Log.Write(null, "Waiting for dialog to close...");
				await dialogTask;
				Log.Write(null, "Dialog closed");
			}
			else
			{
				Log.Write(null, "Showing dialog (blocking)...");
				if (SetOwner)
					dialog.ShowModal(parent);
				else
					dialog.ShowModal();
				Log.Write(null, "Dialog closed");
			}
		}
	}
}

