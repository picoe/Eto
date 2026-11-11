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
		RadioButton controller;

		public RadioButtonHandler()
		{
			Control = new aw.RadioButton(Platform.AppContextThemed);
		}

		public bool Checked
		{
			get { return Control.Checked; }
			set { Control.Checked = value; }
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			group = new List<RadioButton> { Widget };
			Control.Click += control_RadioSwitch;
		}

		public RadioButton Controller
		{
			get => controller;
			set
			{
				controller = value;
				var controllerInner = value?.Handler as RadioButtonHandler;
				if (controllerInner != null && ReferenceEquals(controllerInner.group, group))
					return;

				group?.Remove(Widget);
				if (controllerInner != null)
				{
					controllerInner.group ??= new List<RadioButton> { value };
					group = controllerInner.group;
					group.Add(Widget);
				}
				else
					group = new List<RadioButton> { Widget };

				if (Checked)
				{
					foreach (var item in group)
					{
						if (!ReferenceEquals(item, Widget))
							item.Checked = false;
					}
				}
			}
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