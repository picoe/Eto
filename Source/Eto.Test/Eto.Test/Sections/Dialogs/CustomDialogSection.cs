using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Dialogs
{
	public class CustomDialogSection : Panel
	{
		public CustomDialogSection ()
		{
			var layout = new DynamicLayout (this, new Size (20, 20));

			layout.AddRow (null, Standard (), null);
			layout.AddRow (null, Resizable (), null);
			layout.AddRow (null, KitchenSink (), null);

			layout.Add (null);
		}

		Dialog CreateDialog ()
		{
			var dialog = new Dialog ();

			var layout = new DynamicLayout (dialog);

			layout.AddCentered (new Label { Text = "Content" }, yscale:true);

			dialog.DefaultButton = new Button { Text = "Default Button" };
			dialog.AbortButton = new Button { Text = "Abort Button" };

			dialog.DefaultButton.Click += delegate {
				MessageBox.Show ("Default button clicked");
			};

			dialog.AbortButton.Click += delegate {
				MessageBox.Show ("Abort button clicked");
			};

			layout.BeginVertical ();
			layout.AddRow (null, dialog.DefaultButton, dialog.AbortButton);
			layout.EndVertical ();
			return dialog;
		}

		Control Standard ()
		{
			var control = new Button { Text = "Standard Dialog" };
			control.Click += delegate {
				var dialog = CreateDialog ();
				dialog.Title = "Standard Dialog";
				dialog.ShowDialog (this);

			};
			return control;
		}

		Control Resizable ()
		{
			var control = new Button { Text = "Resizable" };
			control.Click += delegate {
				var dialog = CreateDialog ();
				dialog.Title = "Resizable Dialog";
				dialog.Resizable = true;
				dialog.ShowDialog (this);
			};

			return control;
		}

		Control KitchenSink ()
		{
			var control = new Button { Text = "Kitchen Sink && Maximized" };
			control.Click += delegate {
				var dialog = new Dialog ();
				dialog.State = WindowState.Maximized;
				dialog.Title = "Kitchen Sink Dialog";
				dialog.Resizable = true;
				dialog.AddDockedControl(new Controls.KitchenSinkSection());
				dialog.ShowDialog (this);
			};

			return control;
		}
	}
}

