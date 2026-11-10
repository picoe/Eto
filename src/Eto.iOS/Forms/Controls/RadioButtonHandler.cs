using UIKit;
namespace Eto.iOS.Forms.Controls
{
	// TODO: use UISwitch instead with custom state images, better accessibility
	public class RadioButtonHandler : IosButton<UIButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
		List<RadioButtonHandler> group;
		RadioButton controller;

		public RadioButtonHandler()
		{
			Control = new UIButton(UIButtonType.RoundedRect);
			group = new List<RadioButtonHandler> { this };
		}

		public RadioButton Controller
		{
			get => controller;
			set
			{
				controller = value;
				var controllerHandler = value?.Handler as RadioButtonHandler;
				if (controllerHandler != null && ReferenceEquals(controllerHandler.group, group))
					return;

				group.Remove(this);
				group = controllerHandler?.group ?? new List<RadioButtonHandler>();
				group.Add(this);

				if (Checked)
					UncheckOthers();
			}
		}

		void UncheckOthers()
		{
			foreach (var b in group.Where(r => r != this))
			{
				b.Checked = false;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.ShowsTouchWhenHighlighted = false;
			Control.TouchUpInside += (sender, e) => ApplicationHandler.Instance.AsyncInvoke(() =>
			{
				Checked = true;
				Callback.OnClick(Widget, EventArgs.Empty);
			});
		}

		public bool Checked
		{
			get { return Control.Selected; }
			set
			{ 
				if (Control.Selected != value)
				{
					Control.Selected = value;
					if (!Platform.IsIos7)
						Control.Highlighted = value;
					if (value)
						UncheckOthers();
					Callback.OnCheckedChanged(Widget, EventArgs.Empty);
				}
			}
		}
	}
}

