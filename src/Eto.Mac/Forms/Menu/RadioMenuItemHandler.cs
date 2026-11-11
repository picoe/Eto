namespace Eto.Mac.Forms.Menu
{
	public class RadioMenuItemHandler : MenuHandler<NSMenuItem, RadioMenuItem, RadioMenuItem.ICallback>, RadioMenuItem.IHandler, IMenuActionHandler
	{
		List<RadioMenuItem> radioGroup;
		RadioMenuItem controller;

		protected override NSMenuItem CreateControl()
		{
			return new NSMenuItem();
		}

		protected override void Initialize()
		{
			Enabled = true;
			//control.SetButtonType(NSButtonType.Radio);
			Control.Target = new MenuActionHandler { Handler = this };
			Control.Action = MenuActionHandler.selActivate;

			base.Initialize();
		}

		public override void Activate()
		{
			Checked = true;
			Callback.OnClick(Widget, EventArgs.Empty);
		}

		void UncheckGroup()
		{
			if (radioGroup == null)
				return;
			foreach (var item in radioGroup)
			{
				if (!ReferenceEquals(item, Widget))
					item.Checked = false;
			}
		}

		public RadioMenuItem Controller
		{
			get => controller;
			set
			{
				controller = value;
				var controllerInner = value?.Handler as RadioMenuItemHandler;
				if (controllerInner?.radioGroup != null && ReferenceEquals(controllerInner.radioGroup, radioGroup))
					return;

				radioGroup?.Remove(Widget);
				radioGroup = null;
				if (controllerInner != null)
				{
					controllerInner.radioGroup ??= new List<RadioMenuItem> { value };
					radioGroup = controllerInner.radioGroup;
					radioGroup.Add(Widget);
				}

				if (Checked)
					UncheckGroup();
			}
		}

		public string Text
		{
			get	{ return Control.Title; }
			set => Control.Title = MacConversions.StripAmpersands(value ?? string.Empty);
		}

		public string ToolTip
		{
			get { return Control.ToolTip; }
			set { Control.ToolTip = value ?? string.Empty; }
		}

		public Keys Shortcut
		{
			get { return KeyMap.Convert(Control.KeyEquivalent, Control.KeyEquivalentModifierMask); }
			set
			{ 
				Control.KeyEquivalent = KeyMap.KeyEquivalent(value);
				Control.KeyEquivalentModifierMask = KeyMap.KeyEquivalentModifierMask(value);
			}
		}

		public bool Checked
		{
			get { return Control.State == NSCellStateValue.On; }
			set
			{ 
				if (Checked != value)
				{
					if (value)
						UncheckGroup();
					Control.State = value ? NSCellStateValue.On : NSCellStateValue.Off; 
					Callback.OnCheckedChanged(Widget, EventArgs.Empty);
				}
			}
		}

		MenuItem IMenuActionHandler.Widget
		{
			get { return Widget; }
		}

		MenuItem.ICallback IMenuActionHandler.Callback
		{
			get { return Callback; }
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case RadioMenuItem.CheckedChangedEvent:
					break;
				case MenuItem.ValidateEvent:
					// handled in MenuActionHandler
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
