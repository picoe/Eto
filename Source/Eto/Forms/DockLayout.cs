using System;
using Eto.Drawing;

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
		
		public static Padding DefaultPadding = new Padding(0);

		public DockLayout(Container container)
			: base(container.Generator, container, typeof(IDockLayout))
		{
			inner = (IDockLayout)Handler;
		}
		
		public void Add(Control control)
		{
			base.Container.InnerControls.Clear ();
			control.SetParentLayout(this);
			inner.Add(control);
			Container.InnerControls.Add(control);
			if (Loaded) {
				control.OnLoad (EventArgs.Empty);
				control.OnLoadComplete (EventArgs.Empty);
			}
		}
		
		public void Remove(Control control)
		{
			base.Container.InnerControls.Remove (control);
			inner.Remove (control);
			control.SetParentLayout(null);
			Container.InnerControls.Remove(control);
		}
		
		public Padding Padding
		{
			get { return inner.Padding; }
			set { inner.Padding = value; }
		}
	}
}
