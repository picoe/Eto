using System;
using Eto.Forms;
using Eto.Drawing;

using av = Android.Views;

namespace Eto.Android
{
	internal class CheckMenuItemHandler : MenuItemHandler<av.IMenuItem, CheckMenuItem, CheckMenuItem.ICallback>, CheckMenuItem.IHandler
	{
		public CheckMenuItemHandler()
		{
		}

		public override void CreateControl(av.IMenu androidMenu, Int32 index)
		{
			Control = androidMenu.Add(av.Menu.None, index, index, new Java.Lang.String(Text));
			Control.SetCheckable(true);
			Control.SetChecked(Checked);
			//RegisterClickListener(Control);
		}

		public Image Image
		{
			get;
			set;
		}

		public void AddMenu(System.Int32 index, MenuItem item)
		{
			//throw new System.NotImplementedException();
		}

		public void RemoveMenu(MenuItem item)
		{
			//throw new System.NotImplementedException();
		}

		public void Clear()
		{
			//throw new System.NotImplementedException();
		}

		public override void PerformClick()
		{
			OnClick();
			Callback?.OnClick(Widget, EventArgs.Empty);
		}

		protected override Boolean OnClick()
		{
			Checked = !Checked;
			return true;
		}

		private Boolean _Checked;

		public Boolean Checked
		{
			get
			{
				return _Checked;
			}

			set
			{
				_Checked = value;
				Control?.SetChecked(value);
			}
		}
	}
}
