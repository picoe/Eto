namespace Eto.GirCore.Forms.Menu
{
	public interface IGirMenuChildHandler
	{
		Gtk.Widget MenuWidget { get; }

		void SetParentMenu(IGirMenuParentHandler? parent);

		void TriggerValidate();
	}

	public interface IGirMenuParentHandler
	{
		void PrepareForChildActivation();

		void CloseHierarchy();

		void ChildUpdated();
	}

	internal static class GirMenuHelper
	{
		public static void ClearBox(Gtk.Box box)
		{
			Gtk.Widget? child;
			while ((child = box.GetFirstChild()) != null)
				box.Remove(child);
		}

		public static Gtk.Widget? GetWidget(MenuItem item) => (item.Handler as IGirMenuChildHandler)?.MenuWidget;

		public static void SetParent(MenuItem item, IGirMenuParentHandler? parent)
		{
			(item.Handler as IGirMenuChildHandler)?.SetParentMenu(parent);
		}

		public static void ValidateItems(IEnumerable<MenuItem> items)
		{
			foreach (var item in items)
				(item.Handler as IGirMenuChildHandler)?.TriggerValidate();
		}

		public static string? ToMnemonic(string? text) => string.IsNullOrEmpty(text) ? text : text.ToPlatformMnemonic();
	}

	public abstract class MenuHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Eto.Forms.Menu.IHandler
		where TControl : class
		where TWidget : Eto.Forms.Menu
		where TCallback : Eto.Forms.Menu.ICallback
	{
	}

	public abstract class MenuItemHandler<TControl, TWidget, TCallback> : MenuHandler<TControl, TWidget, TCallback>, MenuItem.IHandler, IGirMenuChildHandler
		where TControl : Gtk.Widget
		where TWidget : MenuItem
		where TCallback : MenuItem.ICallback
	{
		IGirMenuParentHandler? parentMenu;
		string? text;
		string? toolTip;
		Keys shortcut;

		public Gtk.Widget MenuWidget => Control;

		public virtual string Text
		{
			get => text ?? string.Empty;
			set
			{
				text = value;
				UpdateDisplay();
			}
		}

		public virtual string ToolTip
		{
			get => toolTip ?? string.Empty;
			set => toolTip = value;
		}

		public virtual Keys Shortcut
		{
			get => shortcut;
			set => shortcut = value;
		}

		public virtual bool Enabled
		{
			get => Control.Sensitive;
			set => Control.Sensitive = value;
		}

		public virtual bool Visible
		{
			get => Control.Visible;
			set => Control.Visible = value;
		}

		public virtual void CreateFromCommand(Command command)
		{
		}

		public void SetParentMenu(IGirMenuParentHandler? parent)
		{
			parentMenu = parent;
		}

		protected IGirMenuParentHandler? ParentMenu => parentMenu;

		public virtual void TriggerValidate()
		{
			Callback.OnValidate(Widget, EventArgs.Empty);
		}

		protected void NotifyParentChanged()
		{
			parentMenu?.ChildUpdated();
		}

		protected abstract void UpdateDisplay();
	}
}
