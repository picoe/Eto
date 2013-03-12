using System;
using Eto.Forms;

namespace Eto.Test.Sections.Behaviors
{
	public class MousePositionSection : Panel
	{
		Label mousePositionLabel;
		Label pointFromScreenLabel;
		Label pointToScreenLabel;
		Label buttonsLabel;

		public MousePositionSection ()
		{
			var layout = new DynamicLayout (this);

			layout.AddSeparateRow (null, new Label { Text = "Mouse Position (in screen co-ordinates)"}, MousePositionLabel (), null);
			layout.AddSeparateRow (null, new Label { Text = "PointFromScreen" }, PointFromScreen (), null);
			layout.AddSeparateRow (null, new Label { Text = "PointToScreen" }, PointToScreen (), null);
			layout.AddSeparateRow (null, new Label { Text = "Buttons" }, Buttons (), null);
			layout.Add (null);

			HandleEvent (Control.MouseMoveEvent, Control.MouseDownEvent, Control.MouseUpEvent);

			SetLabels ();
		}

		Control MousePositionLabel ()
		{
			return mousePositionLabel = new Label ();
		}

		Control PointFromScreen ()
		{
			return pointFromScreenLabel = new Label ();
		}

		Control PointToScreen ()
		{
			return pointToScreenLabel = new Label ();
		}

		Control Buttons ()
		{
			return buttonsLabel = new Label ();
		}

		void SetLabels ()
		{
			var position = Mouse.GetPosition();
			mousePositionLabel.Text = position.ToString ();

			// convert to control co-ordinates
			position = this.PointFromScreen (position);
			pointFromScreenLabel.Text = position.ToString ();

			// convert back to world co-ordinates
			position = this.PointToScreen (position);
			pointToScreenLabel.Text = position.ToString ();

			buttonsLabel.Text = Mouse.GetButtons ().ToString ();
		}

		public override void OnMouseMove (MouseEventArgs e)
		{
			base.OnMouseMove (e);
			SetLabels ();
		}

		public override void OnMouseDown (MouseEventArgs e)
		{
			base.OnMouseDown (e);
			SetLabels ();
		}

		public override void OnMouseUp (MouseEventArgs e)
		{
			base.OnMouseUp (e);
			SetLabels ();
		}
	}
}

