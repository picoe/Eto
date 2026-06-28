namespace Eto.GirCore.Forms.Menu
{
	public class MenuBarHandler : MenuHandler<Gtk.Box, MenuBar, MenuBar.ICallback>, MenuBar.IHandler, Eto.Forms.Menu.ISubmenuHandler, IGirMenuParentHandler
	{
		MenuItem? quitItem;
		MenuItem? aboutItem;

		public MenuBarHandler()
		{
			Control = Gtk.Box.New(Gtk.Orientation.Horizontal, 0);
		}

		void Rebuild()
		{
			GirMenuHelper.ClearBox(Control);
			foreach (var item in Widget.Items)
			{
				GirMenuHelper.SetParent(item, this);
				var child = GirMenuHelper.GetWidget(item);
				if (child != null)
					Control.Append(child);
			}
		}

		public void AddMenu(int index, MenuItem item)
		{
			Rebuild();
		}

		public void RemoveMenu(MenuItem item)
		{
			GirMenuHelper.SetParent(item, null);
			Rebuild();
		}

		public void Clear()
		{
			foreach (var item in Widget.Items)
				GirMenuHelper.SetParent(item, null);
			Rebuild();
		}

		public void PrepareForChildActivation()
		{
		}

		public void CloseHierarchy()
		{
		}

		public void ChildUpdated()
		{
			Rebuild();
		}

		public void SetQuitItem(MenuItem item)
		{
			item.Order = 1000;
			if (quitItem != null)
				ApplicationMenu.Items.Remove(quitItem);
			else
				ApplicationMenu.Items.AddSeparator(999);
			ApplicationMenu.Items.Add(item);
			quitItem = item;
		}

		public void SetAboutItem(MenuItem item)
		{
			item.Order = 1000;
			if (aboutItem != null)
				HelpMenu.Items.Remove(aboutItem);
			else
				HelpMenu.Items.AddSeparator(999);
			HelpMenu.Items.Add(item);
			aboutItem = item;
		}

		public void CreateSystemMenu()
		{
		}

		public void CreateLegacySystemMenu()
		{
		}

		public IEnumerable<Command> GetSystemCommands()
		{
			yield break;
		}

		public ButtonMenuItem ApplicationMenu => Widget.Items.GetSubmenu(Application.Instance.Localize(Widget, "&File"), -100);

		public ButtonMenuItem HelpMenu => Widget.Items.GetSubmenu(Application.Instance.Localize(Widget, "&Help"), 1000);
	}
}
