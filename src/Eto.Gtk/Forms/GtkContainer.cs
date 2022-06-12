using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Forms.Controls;
using System.Collections.Generic;
using System;

namespace Eto.GtkSharp.Forms
{
	public abstract class GtkContainer<TControl, TWidget, TCallback> : GtkControl<TControl, TWidget, TCallback>, Container.IHandler
		where TControl : Gtk.Widget
		where TWidget : Container
		where TCallback : Container.ICallback
	{
		public bool RecurseToChildren { get { return true; } }

		public virtual Size ClientSize
		{
			get { return Size; }
			set { Size = value; }
		}

		public override IEnumerable<Control> VisualControls => Widget.Controls;

		protected virtual void SetFocusChain()
		{
			var container = Control as Gtk.Container;
			if (container == null)
				return;
			var widgets = GetOrderedWidgets().Distinct().ToArray();
			container.FocusChain = widgets;
		}

		IEnumerable<Gtk.Widget> GetOrderedWidgets()
		{
			var parent = Widget.IsVisualControl ? Widget.LogicalParent : Widget;
			if (parent == null)
				yield break;
			foreach (var ctl in parent.Controls.OrderBy(r => r.TabIndex))
			{
				var widget = ctl.GetContainerWidget();
				while (widget != null && !ReferenceEquals(widget.Parent, Control))
				{
					widget = widget.Parent;
				}
				if (widget != null)
					yield return widget;
			}
		}

#if GTK2
		public override void TriggerEnabled(bool oldEnabled, bool newEnabled, bool force)
		{
			foreach (var child in Widget.VisualControls)
			{
				child.GetGtkControlHandler()?.TriggerEnabled(oldEnabled && child.Enabled, newEnabled);
			}

			base.TriggerEnabled(oldEnabled, newEnabled, force);
		}

		public override void SetBackgroundColor()
		{
			base.SetBackgroundColor();
			foreach (var child in Widget.VisualControls.Select(r => r.GetGtkControlHandler()).Where(r => r != null))
			{
				child.SetBackgroundColor();
			}
		}
#endif
	}
}
