using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android
{
	internal abstract class MenuItemHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, MenuItem.IHandler, IMenuItemHandler
		where TControl : av.IMenuItem
		where TCallback : MenuItem.ICallback
		where TWidget : MenuItem
	{
		public Keys Shortcut
		{
			get; set;
		}

		public void CreateFromCommand(Command command)
		{
		}

		public string Text
		{
			get; set;
		}

		public string ToolTip
		{
			get; set;
		}

		public bool Enabled
		{
			get; set;
		}

		public abstract void PerformClick();

		public av.ISubMenu SubControl { get; set; }

		av.IMenuItem IMenuItemHandler.Control
		{
			get => Control;
			set => Control = (TControl)value;
		}

		public bool Visible
		{
			get; set;
		}

		public abstract void CreateControl(av.IMenu androidMenu, int index);

		protected void RegisterClickListener(av.IMenuItem control) => control.SetOnMenuItemClickListener(new ClickListener(this));

		protected virtual bool OnClick() => false;

		private class ClickListener : Java.Lang.Object, av.IMenuItemOnMenuItemClickListener
		{
			private readonly MenuItemHandler<TControl, TWidget, TCallback> _Handler;
			public ClickListener(MenuItemHandler<TControl, TWidget, TCallback> handler) => _Handler = handler;
			public bool OnMenuItemClick(av.IMenuItem item) => _Handler?.OnClick() ?? false;
		}
	}

	internal interface IMenuItemHandler
	{
		av.IMenuItem Control { get; set; }
		av.ISubMenu SubControl { get; set; }
		void PerformClick();
		void CreateControl(av.IMenu androidMenu, int index);
	}
}
