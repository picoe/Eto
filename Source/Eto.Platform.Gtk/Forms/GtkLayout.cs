using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	internal interface IGtkLayout
	{
		object ContainerObject { get; }
	}
	
	public abstract class GtkLayout<T, W> : WidgetHandler<T, W>, ILayout, IGtkLayout
		where W: Layout
	{
		bool setlayout;
		
		public override void Initialize ()
		{
			base.Initialize ();
			
			if (Widget.Container != null) {
				IGtkContainer container = Widget.Container.Handler as IGtkContainer;
				container.SetLayout (Widget);
			}
			else
				setlayout = true;
		}
		
		public virtual object ContainerObject {
			get { return Control; }
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

		public virtual void Update ()
		{
		}

		public virtual void AttachedToContainer ()
		{
		}
	}
}
