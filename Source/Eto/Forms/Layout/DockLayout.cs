using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


#if XAML
using System.Windows.Markup;
#endif

namespace Eto.Forms
{
	public interface IDockLayout : ILayout
	{
		Padding Padding { get; set; }

		Control Content { get; set; }
	}
	
	public static class DockLayoutExtensions
	{
		public static Container AddDockedControl (this Container container, Control control, Padding? padding = null)
		{
			var layout = container.Layout as DockLayout;
			if (layout == null)
				layout = new DockLayout (container);
			if (padding != null)
				layout.Padding = padding.Value;
			layout.Content = control;
			return container;
		}
	}
	
#if XAML
	[ContentProperty("Content")]
#endif
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
		
		public static Panel CreatePanel (Control control, Padding? padding = null)
		{
			var panel = new Panel ();
			panel.AddDockedControl (control, padding);
			return panel;
		}
		
		public DockLayout ()
			: this(null)
		{
		}

		public DockLayout (Container container)
			: base (container != null ? container.Generator : Generator.Current, container, typeof (IDockLayout))
		{
			inner = (IDockLayout)Handler;
		}
		
		[Obsolete ("Use Content property instead")]
		public void Add (Control control)
		{
			Content = control;
		}
		
		[Obsolete ("Use Content property instead")]
		public void Remove (Control control)
		{
			Content = null;
		}
		
		public Control Content {
			get { return inner.Content; }
			set {
				control = value;
				if (control != null) {
					control.SetParentLayout (this);
					var load = Loaded && !control.Loaded;
					if (load) {
						control.OnPreLoad (EventArgs.Empty);
						control.OnLoad (EventArgs.Empty);
					}
					inner.Content = control;
					if (load)
						control.OnLoadComplete (EventArgs.Empty);
				}
				else
					inner.Content = control;
			}
		}
		
		public Padding Padding {
			get { return inner.Padding; }
			set { inner.Padding = value; }
		}
	}
}
