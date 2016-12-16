using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using System.Reflection;

namespace Eto.WinForms.Forms.Controls
{
	public class TextStepperHandler : TextBoxHandler<TextStepperHandler.EtoUpDown, TextStepper, TextStepper.ICallback>, TextStepper.IHandler
	{
		public class EtoUpDown : swf.UpDownBase
		{
			public event EventHandler DownButtonClicked;
			public event EventHandler UpButtonClicked;

			static FieldInfo DefaultButtonsWidthField = typeof(swf.UpDownBase).GetField("defaultButtonsWidth", BindingFlags.Static | BindingFlags.NonPublic);
			static FieldInfo TextBoxField = typeof(swf.UpDownBase).GetField("upDownEdit", BindingFlags.Instance | BindingFlags.NonPublic);
			static FieldInfo UpDownButtonsField = typeof(swf.UpDownBase).GetField("upDownButtons", BindingFlags.Instance | BindingFlags.NonPublic);

			public swf.TextBox TextBox => TextBoxField?.GetValue(this) as swf.TextBox;

			public swf.Control UpDownButtons => UpDownButtonsField?.GetValue(this) as swf.Control;

			static int DefaultButtonsWidth
			{
				get { return (int?)DefaultButtonsWidthField?.GetValue(null) ?? 0; }
				set { DefaultButtonsWidthField?.SetValue(null, value); }
			}

			public EtoUpDown()
			{
				TextBox.MaxLength = 0;
			}

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

			public override sd.Size GetPreferredSize(sd.Size proposedSize)
			{
				// fix preferred size from being different from actual size, which causes the control
				// to grow/shrink each time it has a layout.. not sure why this fixes it, but oh well!
				return SizeFromClientSize(ClientSize) + Padding.Size;
			}

			protected override void OnLayout(swf.LayoutEventArgs e)
			{
				if (!UpDownButtons.Visible)
				{
					var oldVal = DefaultButtonsWidth;
					DefaultButtonsWidth = 0;
					base.OnLayout(e);
					DefaultButtonsWidth = oldVal;
					return;
				}
				base.OnLayout(e);
			}
		}

		public override EtoTextBox EtoTextBox => null;

		public override swf.TextBox SwfTextBox => Control.TextBox;

		public TextStepperHandler()
		{
			Control = new EtoUpDown();
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
				Control.PerformLayout();
			}
		}

		public bool ShowStepper
		{
			get { return Control.UpDownButtons.Visible; }
			set
			{
				Control.UpDownButtons.Visible = value;
				Control.PerformLayout();
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
