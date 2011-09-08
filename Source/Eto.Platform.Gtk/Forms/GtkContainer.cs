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
		where T: Gtk.Container
		where W: Container
			
	{
		public abstract object ContainerObject { get; }
		
		public virtual Size ClientSize {
			get { return this.Size; }
			set {
				this.Size = value;
			}
		}
		
		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			Control.SizeRequested += delegate(object o, Gtk.SizeRequestedArgs args) {
				if (MinimumSize != null) {
					var alloc = args.Requisition;
					alloc.Width = Math.Max (alloc.Width, MinimumSize.Value.Width);
					alloc.Height = Math.Max (alloc.Height, MinimumSize.Value.Height);
					args.Requisition = alloc;
				}
			};
			/*Control.SizeAllocated += delegate(object o, Gtk.SizeAllocatedArgs args) {
				if (MinimumSize != null) {
					var alloc = args.Allocation;
					alloc.Width = Math.Max (alloc.Width, MinimumSize.Value.Width);
					alloc.Height = Math.Max (alloc.Height, MinimumSize.Value.Height);
					if (alloc.Width != args.Allocation.Width || alloc.Height != args.Allocation.Height) {
						Control.SetSizeRequest(alloc.Width, alloc.Height);
						Control.Allocation = alloc; //.SetSizeRequest(alloc.Width, alloc.Height);
					}
				}
			};*/
		}
		
		public override Size Size {
			get {
				if (Control.Visible) 
					return Generator.Convert(Control.Allocation.Size);
				else
					return Generator.Convert (Control.SizeRequest ()); 
			}
			set {
				if (Control.Visible)
					Control.Allocation = new Gdk.Rectangle(Control.Allocation.Location, Generator.Convert (value));
				else
					Control.SetSizeRequest (value.Width, value.Height);
			}
		}
		
		public Size? MinimumSize { get; set; }
	}
}
