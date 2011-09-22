using System;
using System.Collections.Generic;

namespace Eto
{
	public delegate void StyleWidgetHandler (InstanceWidget widget);
	
	public delegate void StyleWidgetHandler<W> (W widget)
		where W: InstanceWidget;

	public delegate void StyleWidgetControlHandler<W,C> (W widget,C control)
		where W: InstanceWidget;
	
	public static class Style
	{
		static Dictionary<string, StyleWidgetHandler> styleMap;

		static Style ()
		{
			styleMap = new Dictionary<string, StyleWidgetHandler> ();
		}
		
		public static event StyleWidgetHandler StyleWidget;
		
		internal static void OnStyleWidget (InstanceWidget widget)
		{
			StyleWidgetHandler style;
			if (widget != null && !string.IsNullOrEmpty (widget.Style) && styleMap.TryGetValue (widget.Style, out style)) {
				style (widget);
			}
			if (StyleWidget != null)
				StyleWidget (widget);
		}
		
		public static void Add (string style, StyleWidgetHandler handler)
		{
			styleMap [style] = handler;
		}

		public static void Add<T> (string style, StyleWidgetHandler<T> handler)
			where T: InstanceWidget
		{
			styleMap [style] = delegate(InstanceWidget widget){
				var control = widget as T;
				if (control != null)
					handler (control);
			};
		}

		public static void Add<W, C> (string style, StyleWidgetControlHandler<W, C> handler)
			where W: InstanceWidget
		{
			styleMap [style] = delegate(InstanceWidget widget){
				var control = widget as W;
				if (control != null)
					handler (control, (C)control.ControlObject);
			};
		}
		
	}
}

