using System;
using Eto.Forms;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Keyboard")]
	public class KeyboardSection : Panel
	{
		Label label = new Label { TextAlignment = TextAlignment.Center };
		public KeyboardSection()
		{
			var layout = new DynamicLayout();

			layout.BeginCentered(yscale: true);

			layout.Add(new Label { Text = "Press a modifier key", TextAlignment = TextAlignment.Center });
			layout.Add(label);

			layout.EndCentered();

			Content = layout;

			Keyboard_ModifiersChanged(null, EventArgs.Empty);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Keyboard.ModifiersChanged += Keyboard_ModifiersChanged;
		}

		protected override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			Keyboard.ModifiersChanged -= Keyboard_ModifiersChanged;
		}

		private void Keyboard_ModifiersChanged(object sender, EventArgs e)
		{
			Log.Write(this, $"Keyboard.ModifiersChanged: {Keyboard.Modifiers}");
			label.Text = $"Modifiers: {Keyboard.Modifiers}";
			foreach (var lockKey in Keyboard.SupportedLockKeys)
			{
				label.Text += $"\n{lockKey}: {Keyboard.IsKeyLocked(lockKey)}";
			}
		}
	}
}

