using System;
using Eto.Forms;

namespace Eto.Test
{
	public static class DynamicLayoutExtensions
	{
		public static void AddLabelledSection (this DynamicLayout layout, string text, Control control)
		{
			var label = new Label { Text = text, VerticalAlign = VerticalAlign.Middle };
#if DESKTOP
			layout.AddRow (label, control);
#elif MOBILE
			layout.BeginVertical ();
			layout.Add (label);
			layout.Add (control);
			layout.EndVertical ();
#endif
		}
	}
}

