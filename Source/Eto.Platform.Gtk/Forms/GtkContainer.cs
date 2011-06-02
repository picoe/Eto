using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp
{
	public interface IGtkContainer
	{
		void SetLayout(Layout inner);
		
		object ContainerObject { get; }
		
	}
	
	public abstract class GtkContainer<T, W> : GtkControl<T, W>, IContainer, IGtkContainer
		where T: Gtk.Widget
		where W: Container
			
	{
		public abstract object ContainerObject { get; }
		
		public virtual Size ClientSize {
			get { return this.Size; }
			set {
				this.Size = value;
			}
		}
		
	}
}
