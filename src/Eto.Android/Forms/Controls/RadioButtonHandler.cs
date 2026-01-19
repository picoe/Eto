using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms.Controls
{
	public class RadioButtonHandler : AndroidCommonControl<aw.RadioButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
		List<RadioButton> group;

		public RadioButtonHandler()
		{
			Control = new aw.RadioButton(Platform.AppContextThemed);
		}

		public bool Checked
		{
			get { return Control.Checked; }
			set { Control.Checked = value; }
		}
		
		public void Create(RadioButton controller)
		{
			if (controller != null)
			{
				var controllerInner = (RadioButtonHandler)controller.Handler;
				if (controllerInner.group == null)
				{
					controllerInner.group = new List<RadioButton>();
					controllerInner.group.Add(controller);
				}

				group = controllerInner.group;
				group.Add(Widget);
			}
			else
			{
				group = new List<RadioButton> { Widget };
			}

			Control.Click += control_RadioSwitch;
		}

		private void control_RadioSwitch(object sender, EventArgs e)
		{
			if (group != null)
			{
				foreach (RadioButton item in group)
				{
					item.Checked = (item.ControlObject == sender);
				}
			}

			if (ReferenceEquals(sender, Control))
				Callback.OnClick(Widget, EventArgs.Empty);
		}
	}
}