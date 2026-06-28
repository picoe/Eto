using Gtk;

namespace Eto.GirCore.Forms.Menu
{
	public class RadioMenuItemHandler : MenuItemHandler<Gtk.CheckButton, RadioMenuItem, RadioMenuItem.ICallback>, RadioMenuItem.IHandler
	{
		RadioMenuItem? controller;
		bool suppressClick;

		protected override CheckButton CreateControl()
		{
			return Gtk.CheckButton.New();
		}

		public void Create(RadioMenuItem controller)
		{
			this.controller = controller;
			Control.UseUnderline = true;
			Control.OnToggled += (sender, e) =>
			{
				Callback.OnCheckedChanged(Widget, EventArgs.Empty);
				if (!suppressClick && Control.Active)
				{
					ParentMenu?.PrepareForChildActivation();
					Callback.OnClick(Widget, EventArgs.Empty);
					ParentMenu?.CloseHierarchy();
				}
			};

			if (controller?.Handler is RadioMenuItemHandler controllerHandler)
				Control.SetGroup(controllerHandler.Control);

			UpdateDisplay();
		}

		protected override void UpdateDisplay()
		{
			Control.Label = GirMenuHelper.ToMnemonic(Text);
			Control.UseUnderline = true;
		}

		public bool Checked
		{
			get => Control.Active;
			set
			{
				suppressClick = true;
				Control.Active = value;
				suppressClick = false;
			}
		}
	}
}
