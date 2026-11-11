namespace Eto.Wpf.Forms.Menu
{
	public class RadioMenuItemHandler : MenuItemHandler<swc.MenuItem, RadioMenuItem, RadioMenuItem.ICallback>, RadioMenuItem.IHandler
	{
		List<RadioMenuItem> group;
		RadioMenuItem controller;

		public RadioMenuItemHandler()
		{
			Control = new swc.MenuItem
			{
				IsCheckable = true
			};
		}

		protected override void OnClick()
		{
			UncheckGroup();
			Checked = true;
			base.OnClick();
		}

		void UncheckGroup()
		{
			if (group != null)
			{
				var checkedItem = group.FirstOrDefault(r => r.Checked && r != Widget);
				if (checkedItem != null)
					checkedItem.Checked = false;
			}
		}

		public bool Checked
		{
			get { return Control.IsChecked; }
			set
			{
				if (value)
					UncheckGroup();
				Control.IsChecked = value;
			}
		}

		public RadioMenuItem Controller
		{
			get => controller;
			set
			{
				controller = value;
				var controllerInner = value?.Handler as RadioMenuItemHandler;
				if (controllerInner?.group != null && ReferenceEquals(controllerInner.group, group))
					return;

				group?.Remove(Widget);
				group = null;
				if (controllerInner != null)
				{
					controllerInner.group ??= new List<RadioMenuItem> { value };
					group = controllerInner.group;
					group.Add(Widget);
				}

				if (Checked)
					UncheckGroup();
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case RadioMenuItem.CheckedChangedEvent:
					Widget.Properties.Set(swc.MenuItem.IsCheckedProperty, PropertyChangeNotifier.Register(swc.MenuItem.IsCheckedProperty, HandleIsCheckedChanged, Control));
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void HandleIsCheckedChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
		{
			Callback.OnCheckedChanged(Widget, EventArgs.Empty);
		}
	}
}
