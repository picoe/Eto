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

		public virtual Gtk.Container MainContainerControl
		{
			get { return (Gtk.Container)base.ContainerControl; }
		}

		public sealed override Gtk.Widget ContainerControl
		{
			get { return MainContainerControl; }
		}


		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
#if GTK2
			ContainerControl.SizeRequested += delegate(object o, Gtk.SizeRequestedArgs args) {
				if (MinimumSize != null) {
					var alloc = args.Requisition;
					if (MinimumSize.Value.Width > 0) alloc.Width = Math.Max (alloc.Width, MinimumSize.Value.Width);
					if (MinimumSize.Value.Height > 0) alloc.Height = Math.Max (alloc.Height, MinimumSize.Value.Height);
					args.Requisition = alloc;
				}
			};
#endif
			/*
			Control.SizeAllocated += delegate(object o, Gtk.SizeAllocatedArgs args) {
				if (MinimumSize != null) {
					var alloc = args.Allocation;
					if (MinimumSize.Value.Width > 0) alloc.Width = Math.Max (alloc.Width, MinimumSize.Value.Width);
					if (MinimumSize.Value.Height > 0) alloc.Height = Math.Max (alloc.Height, MinimumSize.Value.Height);
					if (alloc.Width != args.Allocation.Width || alloc.Height != args.Allocation.Height) {
						Control.SetSizeRequest(alloc.Width, alloc.Height);
						Control.Allocation = alloc; //.SetSizeRequest(alloc.Width, alloc.Height);
					}
				}
			};*/
		}
		
		public override Size Size {
			get {
				if (ContainerControl.Visible) 
					return ContainerControl.Allocation.Size.ToEto ();
				else
					return ContainerControl.SizeRequest ().ToEto (); 
			}
			set {
				ContainerControl.SetSizeRequest (value.Width, value.Height);
			}
		}
		
		public Size? MinimumSize {
			get; set;
		}
	}
}
