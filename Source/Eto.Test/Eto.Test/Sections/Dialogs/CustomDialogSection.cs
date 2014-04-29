using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Dialogs
{
	public class CustomDialogSection : Panel
	{
		public CustomDialogSection()
		{
			var layout = new DynamicLayout(new Size(20, 20));

			layout.AddRow(null, Standard(), null);
			layout.AddRow(null, Resizable(), null);
			layout.AddRow(null, KitchenSink(), null);

			layout.Add(null);

			Content = layout;
		}

		Dialog CreateDialog()
		{
			var dialog = new Dialog();

			var layout = new DynamicLayout();

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
				dialog.ShowDialog(this);

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
				if (Generator.IsDesktop)
					dialog.Resizable = true;
				dialog.ShowDialog(this);
			};

			return control;
		}

		Control KitchenSink()
		{
			var control = new Button { Text = "Kitchen Sink && Maximized" };
			control.Click += delegate
			{
				var dialog = new Dialog();
				if (Generator.IsDesktop)
				{
					dialog.Minimizable = true;
					dialog.Resizable = true;
					dialog.Maximizable = true;
					dialog.WindowState = WindowState.Maximized;
				}

				dialog.Title = "Kitchen Sink Dialog";
				dialog.Content = new Controls.KitchenSinkSection();
				dialog.ShowDialog(this);
			};

			return control;
		}
	}
}

