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
	public class TextStepperHandler : TextBoxHandler<TextStepperHandler.EtoUpDown, TextStepper, TextStepper.ICallback>, TextStepper.IHandler
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

			protected override void UpdateEditText()
			{
				
			}
		}

		public override EtoTextBox EtoTextBox => null;

		public override swf.TextBox SwfTextBox => Control.GetType().GetField("upDownEdit", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(Control) as swf.TextBox;

		public TextStepperHandler()
		{
			Control = new EtoUpDown();
			Control.InterceptArrowKeys = false;
		}

		public StepperValidDirections ValidDirection { get; set; } = StepperValidDirections.Both;

		public override bool ReadOnly
		{
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}

		public override bool ShowBorder
		{
			get { return Control.BorderStyle != swf.BorderStyle.None; }
			set
			{
				Control.BorderStyle = value ? swf.BorderStyle.Fixed3D : swf.BorderStyle.None;
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextStepper.StepEvent:
					Control.DownButtonClicked += (sender, e) =>
					{
						if (ValidDirection.HasFlag(StepperValidDirections.Down))
							Callback.OnStep(Widget, new StepperEventArgs(StepperDirection.Down));
					};
					Control.UpButtonClicked += (sender, e) =>
					{
						if (ValidDirection.HasFlag(StepperValidDirections.Up))
							Callback.OnStep(Widget, new StepperEventArgs(StepperDirection.Up));
					};
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
