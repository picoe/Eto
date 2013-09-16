using System;
using Eto.Platform.iOS.Forms.Controls;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Linq;
using Eto.Drawing;

namespace Eto.Platform.iOS.Forms
{
	public interface IiosDockContainer : IiosContainer
	{

	}

	public class iosDockContainer<T, W> : iosContainer<T, W>, IContainer, IiosDockContainer
		where T: UIView
		where W: DockContainer
	{
		Control child;
		Padding padding;

		public iosDockContainer()
		{
		}

		public virtual void SetContentSize(System.Drawing.SizeF size)
		{
			
		}
		/*
		protected override Size GetNaturalSize()
		{
			var layout = 
				Widget != null && Widget.Layout != null && Widget.Layout.InnerLayout != null
				? Widget.Layout.InnerLayout.Handler as IiosLayout : null;
			if (layout != null)
				return layout.GetPreferredSize(Size.MaxValue);
			else
				return base.GetNaturalSize();
		}*/

		public Eto.Drawing.Padding Padding {
			get { return padding; }
			set {
				padding = value;
				if (this.Widget.Loaded)
					Layout ();
			}
		}

		public override Eto.Drawing.Size GetPreferredSize (Size availableSize)
		{
			return child.GetPreferredSize (availableSize);
		}

		public void LayoutChildren ()
		{
			if (child == null) return;

			UIView parent = this.Control;

			UIView childControl = (UIView)child.ControlObject;
			var frame = parent.Frame;

			frame.Y = padding.Top;
			frame.X = padding.Left;
			frame.Width -= padding.Horizontal;
			frame.Height -= padding.Vertical;

			childControl.Frame = frame;
		}


		bool disposed;

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			/*if (!this.Control.Frame.IsEmpty)
				Layout();
			Widget.SizeChanged += (sender, ev) =>
			{
				Layout();
			};*/
		}

		public Control Content
		{
			get
			{
				return child;
			}
			set
			{
				if (child != null)
				{ 
					((UIView)child.ControlObject).RemoveFromSuperview();
					child = null; 
				}
				if (value != null)
				{
					child = value;
					var childControl = child.GetContainerView();
					childControl.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
					if (this.Widget.Loaded)
						Layout();

					Widget.AddSubview(child, true);
				}

			}
		}
	}
}

