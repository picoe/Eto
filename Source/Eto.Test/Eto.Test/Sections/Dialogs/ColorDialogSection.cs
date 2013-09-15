using System;
using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Test.Sections.Dialogs
{
	public class ColorDialogSection : Panel
	{
		public ColorDialogSection()
		{
			var layout = new DynamicLayout(new Size(20, 20));

			layout.AddRow(null, PickColor(), null);
			layout.AddRow(null, PickColorWithStartingColor(), null);

			layout.Add(null);

			Content = layout;
		}

		Control PickColor()
		{
			var button = new Button { Text = "Pick Color" };
			button.Click += delegate
			{
				var dialog = new ColorDialog();
				dialog.ColorChanged += delegate
				{
					// you need to handle this event for OS X, where the dialog is a floating window
					Log.Write(dialog, "ColorChanged, Color: {0}", dialog.Color);
				};
				var result = dialog.ShowDialog(this.ParentWindow);
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
					Color = Colors.Lime
				};
				dialog.ColorChanged += delegate
				{
					// need to handle this event for OS X, where the dialog is a floating window
					Log.Write(dialog, "ColorChanged, Color: {0}", dialog.Color);
				};
				var result = dialog.ShowDialog(this.ParentWindow);
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

