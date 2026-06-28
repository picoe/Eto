using System;

namespace Eto.GirCore.Forms;

	public abstract class GirContainer<TControl, TWidget, TCallback> : GirControl<TControl, TWidget, TCallback>, Container.IHandler
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
			throw new NotImplementedException();
			/*
			var container = Control as Gtk.Container;
			if (container == null)
				return;
			var widgets = GetOrderedWidgets().Distinct().ToArray();
			container.FocusChain = widgets;
			*/
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

	}
