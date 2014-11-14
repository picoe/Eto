using Eto.Drawing;
using Eto.Forms;
using System.ComponentModel;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(LinkButton))]
	public class LinkButtonSection : Scrollable
	{
		public LinkButtonSection()
		{
			var layout = new DynamicLayout();

			layout.AddAutoSized(NormalButton(), centered: true);
			layout.AddAutoSized(LongerButton(), centered: true);
			layout.AddAutoSized(ColourButton(), centered: true);
			layout.AddAutoSized(DisabledButton(), centered: true);
			layout.AddAutoSized(DisabledButtonWithColor(), centered: true);
			layout.Add(StretchedButton());
			layout.Add(null);

			Content = layout;
		}

		Control NormalButton()
		{
			var control = new LinkButton { Text = "Click Me" };
			LogEvents(control);
			return control;
		}

		Control StretchedButton()
		{
			var control = new LinkButton { Text = "A stretched button" };
			LogEvents(control);
			return control;
		}

		Control LongerButton()
		{
			var control = new LinkButton { Text = "This is a long(er) button title" };
			LogEvents(control);
			return control;
		}

		Control ColourButton()
		{
			var control = new LinkButton { Text = "Button with Color", TextColor = Colors.Lime };
			LogEvents(control);
			return control;
		}

		Control DisabledButton()
		{
			var control = new LinkButton { Text = "Disabled Button", Enabled = false };
			LogEvents(control);
			return control;
		}

		Control DisabledButtonWithColor()
		{
			var control = new LinkButton { Text = "Disabled Button with color", DisabledTextColor = Colors.Yellow, Enabled = false };
			LogEvents(control);
			return control;
		}

		void LogEvents(LinkButton button)
		{
			button.Click += delegate
			{
				Log.Write(button, "Click");
			};
		}
	}
}

