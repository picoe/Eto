using Eto.Forms;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using swi = System.Windows.Input;
using System.Windows;
using System;

namespace Eto.Wpf.Forms.Menu
{
	public class SubMenuItemHandler : MenuItemHandler<swc.MenuItem, SubMenuItem, SubMenuItem.ICallback>, SubMenuItem.IHandler
	{
		swc.MenuItem hiddenItem;
		public SubMenuItemHandler()
		{
			Control = new swc.MenuItem();

			hiddenItem = new swc.MenuItem();
			Control.Items.Add(hiddenItem);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case SubMenuItem.OpeningEvent:
					// handled with HandleContextMenuOpening
					break;
				case SubMenuItem.ClosedEvent:
					Control.SubmenuClosed += Control_SubmenuClosed;
					break;
				case SubMenuItem.ClosingEvent:
					// wpf's Closed event fires after, so look at IsSubmenuOpen property changes instead
					Widget.Properties.Set(swc.MenuItem.IsSubmenuOpenProperty, PropertyChangeNotifier.Register(swc.MenuItem.IsSubmenuOpenProperty, HandleIsOpenChanged, Control));
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void HandleIsOpenChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (!Control.IsSubmenuOpen)
				Callback.OnClosing(Widget, EventArgs.Empty);
		}

		private void Control_SubmenuClosed(object sender, RoutedEventArgs e)
		{
			Callback.OnClosed(Widget, EventArgs.Empty);
		}

		protected override void HandleContextMenuOpening(object sender, RoutedEventArgs e)
		{
			Callback.OnOpening(Widget, EventArgs.Empty);
			base.HandleContextMenuOpening(sender, e);
		}

		public override void AddMenu(int index, MenuItem item)
		{
			if (hiddenItem != null)
			{
				Control.Items.Remove(hiddenItem);
				hiddenItem = null;
			}
			base.AddMenu(index, item);
		}

		public override void RemoveMenu(MenuItem item)
		{
			base.RemoveMenu(item);

			if (Control.Items.Count == 0)
			{
				hiddenItem = hiddenItem ?? new swc.MenuItem();
				Control.Items.Add(hiddenItem);
			}
		}

		public override void Clear()
		{
			base.Clear();
			hiddenItem = hiddenItem ?? new swc.MenuItem();
			Control.Items.Add(hiddenItem);
		}
	}
}
