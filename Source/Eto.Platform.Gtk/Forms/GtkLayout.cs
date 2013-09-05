using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	internal interface IGtkLayout
	{
		Gtk.Widget ContainerObject { get; }
	}
	
	public abstract class GtkLayout<T, W> : WidgetHandler<T, W>, ILayout, IGtkLayout
		where W: Layout
		where T: Gtk.Widget
	{
		bool setlayout;
		
		protected override void Initialize ()
		{
			base.Initialize ();
			
			if (Widget.Container != null) {
				IGtkContainer container = Widget.Container.Handler as IGtkContainer;
				container.SetLayout (Widget);
			}
			else
				setlayout = true;
		}
		
		public virtual Gtk.Widget ContainerObject {
			get { return (Gtk.Widget)Control; }
		}

		public virtual void OnPreLoad ()
		{
		}
		
		public virtual void OnLoad ()
		{
		}

		public virtual void OnLoadComplete ()
		{
			if (setlayout && Widget.Container != null) {
				IGtkContainer container = Widget.Container.Handler as IGtkContainer;
				container.SetLayout (Widget);
			}
		}

		public virtual void OnUnLoad ()
		{
		}

		public virtual void Update ()
		{
		}

		public virtual void AttachedToContainer ()
		{
		}
	}
}
