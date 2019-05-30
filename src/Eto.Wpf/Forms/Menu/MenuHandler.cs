using Eto.Forms;
using System.Collections.Generic;
using swc = System.Windows.Controls;
using sw = System.Windows;
using System.Linq;

namespace Eto.Wpf.Forms.Menu
{
	public abstract class MenuHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>
		where TWidget : Eto.Forms.Menu
		where TCallback : Eto.Forms.Menu.ICallback
	{
		protected void RemoveKeyBindings(sw.FrameworkElement element)
		{
			if (element == null)
				return;
			var parentHost = Widget.Parents.Select(r => r.Handler).OfType<IInputBindingHost>().LastOrDefault();
			if (parentHost == null)
				return;
			element.InputBindings.RemoveKeyBindings(element);
		}

		protected void AddKeyBindings(sw.FrameworkElement element)
		{
			if (element == null)
				return;
			var parentHost = Widget.Parents.Select(r => r.Handler).OfType<IInputBindingHost>().LastOrDefault();
			if (parentHost == null)
				return;
			parentHost.InputBindings.AddKeyBindings(element);
		}
	}
}
