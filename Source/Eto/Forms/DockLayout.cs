using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	public interface IDockLayout : ILayout
	{
		Padding Padding { get; set; }
		void Add(Control control);
		void Remove(Control control);
	}
	
	public static class DockLayoutExtensions
	{
		public static DockLayout AddDockedControl(this Container container, Control control, Padding? padding = null)
		{
			var layout = container.Layout as DockLayout;
			if (layout == null) layout = new DockLayout(container);
			if (padding != null) layout.Padding = padding.Value;
			layout.Add(control);
			return layout;
		}
	}
		
	
	public class DockLayout : Layout
	{
		IDockLayout inner;
		Control control;
		
		public static Padding DefaultPadding = Padding.Empty;
		
		public override IEnumerable<Control> Controls {
			get {
				if (control != null)
					yield return control;
				else
					yield break;
			}
		}

		public DockLayout(Container container)
			: base(container.Generator, container, typeof(IDockLayout))
		{
			inner = (IDockLayout)Handler;
		}
		
		public void Add(Control control)
		{
			this.control = control;
			control.SetParentLayout(this);
			inner.Add(control);
			if (Loaded) {
				control.OnLoad (EventArgs.Empty);
				control.OnLoadComplete (EventArgs.Empty);
			}
		}
		
		public void Remove(Control control)
		{
			this.control = null;
			inner.Remove (control);
			control.SetParentLayout(null);
		}
		
		public Padding Padding
		{
			get { return inner.Padding; }
			set { inner.Padding = value; }
		}
	}
}
