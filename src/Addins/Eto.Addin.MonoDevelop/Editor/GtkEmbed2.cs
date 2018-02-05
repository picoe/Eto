#if Mac
using System;
using AppKit;
using MonoDevelop.Components.Mac;
using CoreGraphics;
using Gtk;
using Foundation;
using Gdk;
using System.Collections.Generic;

namespace Eto.Addin.MonoDevelop.Editor
{
	public class NSViewContainer2 : Container
	{
		//
		// Static Fields
		//
		static Dictionary<NSView, NSViewContainer2> containers = new Dictionary<NSView, NSViewContainer2>();

		//
		// Fields
		//
		List<Gtk.Widget> children = new List<Gtk.Widget>();

		NSView nsview;
		NSView child;

		//
		// Properties
		//
		public NSView NSView
		{
			get
			{
				if (this.nsview == null)
				{
					base.Realize();
				}
				return this.nsview;
			}
		}

		//
		// Constructors
		//
		public NSViewContainer2(NSView child = null)
		{
			WidgetFlags |= WidgetFlags.NoWindow;
			this.child = child;
		}

		//
		// Static Methods
		//
		internal static NSViewContainer2 GetContainer(NSView v)
		{
			while (v != null)
			{
				NSViewContainer2 result;
				if (containers.TryGetValue(v, out result))
				{
					return result;
				}
				v = v.Superview;
			}
			return null;
		}

		//
		// Methods
		//
		void ConnectSubviews(NSView v)
		{
			if (v is GtkEmbed2)
			{
				((GtkEmbed2)v).Connect(this);
			}
			else {
				NSView[] subviews = v.Subviews;
				for (int i = 0; i < subviews.Length; i++)
				{
					NSView nSView = subviews[i];
					this.ConnectSubviews(nSView);
				}
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			if (this.nsview != null)
			{
				containers.Remove(this.nsview);
			}
		}

		protected override void ForAll(bool include_internals, Callback cb)
		{
			Gtk.Widget[] array = children.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				Gtk.Widget widget = array[i];
				cb(widget);
			}
		}

		protected override void OnAdded(Gtk.Widget widget)
		{
			widget.Parent = this;
			this.children.Add(widget);
		}

		protected override void OnRealized()
		{
			base.OnRealized();
			this.nsview = GtkMacInterop.GetNSView(this);
			containers[this.nsview] = this;

			if (child != null)
			{
				child.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
				child.Frame = nsview.Bounds; 
				nsview.AddSubview(child);
			}
			this.ConnectSubviews(this.nsview);
		}

		protected override void OnRemoved(Gtk.Widget widget)
		{
			this.children.Remove(widget);
		}

		protected override void OnSizeAllocated(Rectangle allocation)
		{
			base.OnSizeAllocated(allocation);
		}
	}
	public class WidgetWithNativeWindow2 : EventBox
	{
		// Fields
		GtkEmbed2 embedParent;

		// Constructors
		public WidgetWithNativeWindow2 (GtkEmbed2 embed)
		{
			embedParent = embed;
		}

		// Methods
		protected override void OnRealized ()
		{
			WidgetFlags |= WidgetFlags.Realized;
			WindowAttr attributes = default (WindowAttr);
			attributes.X = Allocation.X;
			attributes.Y = Allocation.Y;
			attributes.Height = Allocation.Height;
			attributes.Width = Allocation.Width;
			attributes.WindowType = Gdk.WindowType.Child;
			attributes.Wclass = WindowClass.InputOutput;
			attributes.Visual = Visual;
			attributes.TypeHint = (WindowTypeHint)100;
			attributes.Colormap = Colormap;
			attributes.EventMask = (int)(Events | EventMask.ExposureMask | EventMask.Button1MotionMask | EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.KeyPressMask | EventMask.KeyReleaseMask);
			WindowAttributesType attributes_mask = WindowAttributesType.X | WindowAttributesType.Y | WindowAttributesType.Colormap | WindowAttributesType.Visual;
			GdkWindow = new Gdk.Window (ParentWindow, attributes, (int)attributes_mask);
			GdkWindow.UserData = Handle;
			Style = Style.Attach (GdkWindow);
			Style.SetBackground (GdkWindow, State);
			WidgetFlags &= ~WidgetFlags.NoWindow;
			NSView nSView = GtkMacInterop.GetNSView (this);
			nSView.RemoveFromSuperview ();
			embedParent.AddSubview (nSView);
			nSView.Frame = new CGRect (0, 0, embedParent.Frame.Width, embedParent.Frame.Height);
		}
	}
	
	public class GtkEmbed2 : NSView
	{
		WidgetWithNativeWindow2 cw;
		NSViewContainer2 container;
		Gtk.Widget embeddedWidget;

		public GtkEmbed2(Gtk.Widget w)
		{
			if (!GtkMacInterop.SupportsGtkIntoNSViewEmbedding())
				throw new NotSupportedException("GTK/NSView embedding is not supported by the installed GTK");

			embeddedWidget = w;
			var s = w.SizeRequest();
			SetFrameSize(new CGSize(s.Width, s.Height));
			WatchForFocus(w);
		}

		internal void Connect(NSViewContainer2 container)
		{
			this.container = container;
			cw = new WidgetWithNativeWindow2(this);
			cw.Add(embeddedWidget);
			container.Add(cw);
			cw.Show();
		}

		void WatchForFocus(Gtk.Widget widget)
		{
			widget.FocusInEvent += (o, args) =>
			{
				var view = GtkMacInterop.GetNSView(widget);
				if (view != null)
					view.Window.MakeFirstResponder(view);
			};

			if (widget is Gtk.Container)
			{
				var c = (Gtk.Container)widget;
				foreach (var w in c.Children)
					WatchForFocus(w);
			}
		}

		[Export("isGtkView")]
		public bool isGtkView()
		{
			return true;
		}

		void UpdateAllocation()
		{
			if (container.GdkWindow == null || cw.GdkWindow == null)
				return;

			var gw = GtkMacInterop.GetNSView(cw);
			gw.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
			var rect = GetRelativeAllocation(GtkMacInterop.GetNSView(container), gw);

			var allocation = new Gdk.Rectangle
			{
				X = (int)rect.Left,
				Y = (int)rect.Top,
				Width = (int)rect.Width,
				Height = (int)rect.Height
			};

			cw.SizeAllocate(allocation);
		}

		CGRect GetRelativeAllocation(NSView ancestor, NSView child)
		{
			if (child == null)
				return CGRect.Empty;
			if (child.Superview == ancestor)
				return child.Frame;
			var f = GetRelativeAllocation(ancestor, child.Superview);
			var cframe = child.Frame;
			return new CGRect(cframe.X + f.X, cframe.Y + f.Y, cframe.Width, cframe.Height);
		}

		public override CGRect Frame
		{
			get
			{
				return base.Frame;
			}
			set
			{
				base.Frame = value;
				UpdateAllocation();
			}
		}

		public override void ViewDidMoveToSuperview()
		{
			base.ViewDidMoveToSuperview();
			var c = NSViewContainer2.GetContainer(Superview);
			if (c != null)
				Connect(c);
		}

		public override void RemoveFromSuperview()
		{
			base.RemoveFromSuperview();
			if (container != null)
			{
				container.Remove(cw);
				container = null;
			}
		}	}
}

#endif