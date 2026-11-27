using Eto.Forms.ThemedControls;

namespace Eto.WinForms.Forms
{
	/// <summary>
	/// Themed popover that uses a color key for the area around it, since WinForms windows can't be partly see-through.
	/// </summary>
	public class PopoverHandler : ThemedPopoverHandler
	{
		static readonly Color s_transparencyKey = Color.FromArgb(255, 0, 255);

		public PopoverHandler()
		{
			// anything blended with the key color would show as a colored fringe
			ShadowSize = 0;
			AntiAlias = false;
			BorderColor = Color.Blend(SystemColors.Control, new Color(SystemColors.ControlText, 0.25f));
		}

		protected override Form CreateWindow()
		{
			var form = new Form
			{
				WindowStyle = WindowStyle.None,
				Resizable = false,
				Minimizable = false,
				Maximizable = false,
				ShowInTaskbar = false,
				BackgroundColor = s_transparencyKey
			};
			if (form.Handler is FormHandler handler)
				handler.TransparencyKey = s_transparencyKey;
			return form;
		}
	}
}
