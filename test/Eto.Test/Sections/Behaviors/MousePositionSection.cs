using Eto.Drawing;
using Eto.Forms;
using System;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Mouse Position", Requires = typeof(Mouse))]
	public class MousePositionSection : Panel
	{
		Label mousePositionLabel;
		Label pointFromScreenLabel;
		Label pointToScreenLabel;
		Label buttonsLabel;
		Label modifiersLabel;

		public MousePositionSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };

			layout.Add(null);
			layout.AddSeparateRow(null, "Mouse Position (in screen co-ordinates)", MousePositionLabel(), null);
			layout.AddSeparateRow(null, "PointFromScreen", PointFromScreen(), null);
			layout.AddSeparateRow(null, "PointToScreen", PointToScreen(), null);
			layout.AddSeparateRow(null, "Mouse.Buttons", Buttons(), null);
			layout.AddSeparateRow(null, "Keyboard.ModifierKeys", Modifiers(), null);
			layout.AddSeparateRow(null, SetMousePosition(), null);
			layout.Add(null);

			SetLabels();

			Content = layout;
		}

		Control SetMousePosition()
		{
			var rnd = new Random();
			var control = new Button { Text = "Set Mouse.Position" };
			control.Click += (sender, e) =>
			{
				var bounds = Screen.DisplayBounds;
				var position = bounds.TopLeft + new SizeF(rnd.Next((int)bounds.Width), rnd.Next((int)bounds.Height));
				Log.Write(this, $"Setting Mouse.Position to {position}");
				Mouse.Position = position;
			};
			return control;
		}

		Control MousePositionLabel()
		{
			return mousePositionLabel = new Label();
		}

		Control PointFromScreen()
		{
			return pointFromScreenLabel = new Label();
		}

		Control PointToScreen()
		{
			return pointToScreenLabel = new Label();
		}

		Control Buttons()
		{
			return buttonsLabel = new Label();
		}

		Control Modifiers()
		{
			return modifiersLabel = new Label();
		}

		void SetLabels()
		{
			var position = Mouse.Position;
			mousePositionLabel.Text = position.ToString();

			// convert to control co-ordinates
			position = PointFromScreen(position);
			pointFromScreenLabel.Text = position.ToString();

			// convert back to world co-ordinates
			position = PointToScreen(position);
			pointToScreenLabel.Text = position.ToString();

			buttonsLabel.Text = Mouse.Buttons.ToString();

			modifiersLabel.Text = Keyboard.Modifiers.ToString();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			SetLabels();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			SetLabels();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			SetLabels();
		}
	}
}

