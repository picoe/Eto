using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swf = System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace Eto.WinForms.Forms.Controls
{
	public class StepperHandler : WindowsControl<StepperHandler.EtoUpDown, Stepper, Stepper.ICallback>, Stepper.IHandler
	{
		public class EtoUpDown : swf.UpDownBase
		{
			public event EventHandler DownButtonClicked;
			public event EventHandler UpButtonClicked;

			public override void DownButton()
			{
				DownButtonClicked?.Invoke(this, EventArgs.Empty);
			}

			public override void UpButton()
			{
				UpButtonClicked?.Invoke(this, EventArgs.Empty);
			}

			protected override Size DefaultSize => new Size(18, base.DefaultSize.Height);

			protected override void UpdateEditText()
			{
				
			}
		}

		public StepperHandler()
		{
			Control = new EtoUpDown();
			Control.InterceptArrowKeys = true;
			Control.Padding = new swf.Padding(0);
			Control.ReadOnly = true;
			var upDownEdit = Control.GetType().GetField("upDownEdit", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(Control) as swf.Control;
			//var upDownButtons = Control.GetType().GetField("upDownButtons", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(Control) as swf.Control;
			upDownEdit.Visible = false;
		}

		public StepperValidDirections ValidDirection
		{
			get { return StepperValidDirections.Both; }
			set
			{
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Stepper.StepEvent:
					Control.DownButtonClicked += (sender, e) => Callback.OnStep(Widget, new StepperEventArgs(StepperDirection.Down));
					Control.UpButtonClicked += (sender, e) => Callback.OnStep(Widget, new StepperEventArgs(StepperDirection.Up));
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}


	}
}
