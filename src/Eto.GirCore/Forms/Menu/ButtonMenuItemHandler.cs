namespace Eto.GirCore.Forms.Menu
{
	public class ButtonMenuItemHandler : ButtonMenuItemHandler<ButtonMenuItem, ButtonMenuItem.ICallback>, ButtonMenuItem.IHandler
	{
	}

	public class ButtonMenuItemHandler<TWidget, TCallback> : MenuItemHandler<Gtk.Box, TWidget, TCallback>, ButtonMenuItem.IHandler, Eto.Forms.Menu.ISubmenuHandler, IGirMenuParentHandler
		where TWidget : ButtonMenuItem
		where TCallback : ButtonMenuItem.ICallback
	{
		Gtk.Widget? buttonWidget;
		Gtk.Button? button;
		Gtk.Box? buttonContent;
		Gtk.Label? buttonLabel;
		Gtk.Image? imageView;
		Gtk.Popover? popover;
		Gtk.Box? submenuBox;
		bool pendingClosed;
		Image? image;

		protected virtual bool AlwaysHasSubmenu => false;

		protected bool HasSubmenu => AlwaysHasSubmenu || Widget.Items.Count > 0;

		public ButtonMenuItemHandler()
		{
			Control = Gtk.Box.New(Gtk.Orientation.Horizontal, 0);
		}

		protected override void Initialize()
		{
			base.Initialize();
			UpdatePresentation();
		}

		protected override void UpdateDisplay()
		{
			if (button != null)
			{
				buttonLabel!.SetTextWithMnemonic(GirMenuHelper.ToMnemonic(Text));
			}
		}

		protected virtual void OnButtonClicked()
		{
			if (HasSubmenu)
			{
				GirMenuHelper.ValidateItems(Widget.Items);
				EnsurePopover();
				popover!.Present();
			}
			else
			{
				ParentMenu?.PrepareForChildActivation();
				Callback.OnClick(Widget, EventArgs.Empty);
				ParentMenu?.CloseHierarchy();
			}
		}

		void UpdatePresentation()
		{
			if (buttonWidget != null)
				Control.Remove(buttonWidget);

			button = Gtk.Button.New();
			buttonContent = Gtk.Box.New(Gtk.Orientation.Horizontal, 6);
			buttonLabel = Gtk.Label.New(null);
			buttonLabel.SetTextWithMnemonic(GirMenuHelper.ToMnemonic(Text));
			buttonContent.Append(buttonLabel);
			button.SetChild(buttonContent);
			button.Sensitive = Enabled;
			button.Visible = Visible;
			button.OnClicked += (sender, e) => OnButtonClicked();
			buttonWidget = button;
			Control.Append(buttonWidget);
			UpdateImage();

			if (!HasSubmenu && popover != null)
			{
				popover.Popdown();
				popover.Unparent();
				popover = null;
				submenuBox = null;
			}
		}

		void EnsurePopover()
		{
			if (popover != null)
				return;

			popover = Gtk.Popover.New();
			popover.SetHasArrow(false);
			popover.SetParent(button!);
			popover.OnClosed += (sender, e) =>
			{
				if (Widget is SubMenuItem subMenu)
				{
					var callback = (SubMenuItem.ICallback)Callback;
					if (pendingClosed)
						Application.Instance.AsyncInvoke(() => callback.OnClosed(subMenu, EventArgs.Empty));
					else
						callback.OnClosed(subMenu, EventArgs.Empty);
				}
				pendingClosed = false;
			};

			submenuBox = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
			popover.SetChild(submenuBox);
			RebuildSubmenu();
		}

		protected void RebuildSubmenu()
		{
			if (submenuBox == null)
				return;

			GirMenuHelper.ClearBox(submenuBox);
			foreach (var item in Widget.Items)
			{
				GirMenuHelper.SetParent(item, this);
				var child = GirMenuHelper.GetWidget(item);
				if (child != null)
					submenuBox.Append(child);
			}
		}

		public Image Image
		{
			get => image!;
			set
			{
				image = value;
				UpdateImage();
			}
		}

		public void AddMenu(int index, MenuItem item)
		{
			GirMenuHelper.SetParent(item, this);
			UpdatePresentation();
			RebuildSubmenu();
			NotifyParentChanged();
		}

		public void RemoveMenu(MenuItem item)
		{
			GirMenuHelper.SetParent(item, null);
			UpdatePresentation();
			RebuildSubmenu();
			NotifyParentChanged();
		}

		public void Clear()
		{
			foreach (var item in Widget.Items)
				GirMenuHelper.SetParent(item, null);
			UpdatePresentation();
			RebuildSubmenu();
			NotifyParentChanged();
		}

		public void PrepareForChildActivation()
		{
			if (Widget is SubMenuItem subMenu)
				((SubMenuItem.ICallback)Callback).OnClosing(subMenu, EventArgs.Empty);

			pendingClosed = true;
			ParentMenu?.PrepareForChildActivation();
		}

		public void CloseHierarchy()
		{
			if (popover != null)
				popover.Popdown();
			ParentMenu?.CloseHierarchy();
		}

		public void ChildUpdated()
		{
			RebuildSubmenu();
		}

		void UpdateImage()
		{
			if (buttonContent == null)
				return;

			if (imageView != null)
			{
				buttonContent.Remove(imageView);
				imageView = null;
			}

			if (image != null)
			{
				imageView = Drawing.GirImageHelper.CreateImage(image, new Size(16, 16));
				buttonContent.Prepend(imageView);
			}
		}
	}
}
