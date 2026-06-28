namespace Eto.GirCore.Forms.Controls
{
	public class TabControlHandler : GirContainer<Gtk.Notebook, TabControl, TabControl.ICallback>, TabControl.IHandler
	{
		bool suppressSelectionChanged;

		public TabControlHandler()
		{
			Control = Gtk.Notebook.New();
			Control.OnSwitchPage += HandleSwitchPage;
		}

		void HandleSwitchPage(Gtk.Notebook sender, Gtk.Notebook.SwitchPageSignalArgs args)
		{
			if (!suppressSelectionChanged && Widget.Loaded)
				Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
		}

		static Gtk.PositionType ToGtk(DockPosition position) => position switch
		{
			DockPosition.Left => Gtk.PositionType.Left,
			DockPosition.Right => Gtk.PositionType.Right,
			DockPosition.Bottom => Gtk.PositionType.Bottom,
			_ => Gtk.PositionType.Top
		};

		static DockPosition ToEto(Gtk.PositionType position) => position switch
		{
			Gtk.PositionType.Left => DockPosition.Left,
			Gtk.PositionType.Right => DockPosition.Right,
			Gtk.PositionType.Bottom => DockPosition.Bottom,
			_ => DockPosition.Top
		};

		public int SelectedIndex
		{
			get => Control.GetCurrentPage();
			set
			{
				suppressSelectionChanged = true;
				Control.SetCurrentPage(value);
				suppressSelectionChanged = false;
			}
		}

		public void InsertTab(int index, TabPage page)
		{
			var pageHandler = (TabPageHandler)page.Handler;
			if (index < 0 || index >= Control.GetNPages())
				Control.AppendPage(pageHandler.ContainerControl, pageHandler.LabelControl);
			else
				Control.InsertPage(pageHandler.ContainerControl, pageHandler.LabelControl, index);
		}

		public void ClearTabs()
		{
			suppressSelectionChanged = true;
			while (Control.GetNPages() > 0)
				Control.RemovePage(0);
			suppressSelectionChanged = false;
		}

		public void RemoveTab(int index, TabPage page)
		{
			suppressSelectionChanged = true;
			Control.RemovePage(index);
			suppressSelectionChanged = false;
		}

		public DockPosition TabPosition
		{
			get => ToEto(Control.TabPos);
			set => Control.TabPos = ToGtk(value);
		}
	}
}
