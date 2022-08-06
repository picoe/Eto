using Eto.Forms;
using Eto.Drawing;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms
{
	public class PixelLayoutHandler : AndroidContainer<aw.FrameLayout, PixelLayout, PixelLayout.ICallback>, PixelLayout.IHandler
	{
		public PixelLayoutHandler()
		{
			Control = new aw.FrameLayout(Platform.AppContextThemed);
		}
		
		public void Add(Control child, int x, int y)
		{
			var childHandler = child.GetAndroidControl();
			var childControl = childHandler.ContainerControl;

			var lp = childControl.LayoutParameters = new aw.FrameLayout.LayoutParams(child.Width, child.Height)
			{
				LeftMargin = x,
				TopMargin = y
			};

			Control.AddView(childControl, lp);
			childControl.BringToFront();
		}

		public void Move(Control child, int x, int y)
		{
			var childHandler = child.GetAndroidControl();
			var childControl = childHandler.ContainerControl;

			childControl.LayoutParameters = new aw.FrameLayout.LayoutParams(av.ViewGroup.LayoutParams.WrapContent, av.ViewGroup.LayoutParams.WrapContent)
			{
				Gravity = av.GravityFlags.NoGravity,
				LeftMargin = x,
				TopMargin = y
			};
		}

		public void Move(Control child, int x, int y, int w, int h)
		{
			var childHandler = child.GetAndroidControl();
			var childControl = childHandler.ContainerControl;

			childControl.LayoutParameters = new aw.FrameLayout.LayoutParams(w, h)
			{
				Gravity = av.GravityFlags.NoGravity,
				LeftMargin = x,
				TopMargin = y
			};
		}

		public void Remove(Control child)
		{
			var childHandler = child.GetAndroidControl();
			var childControl = childHandler.ContainerControl;

			Control.RemoveView(childControl);
		}

		public void Update()
		{
			Control.ForceLayout();
		}

		public override av.View ContainerControl
		{
			get
			{
				return Control;
			}
		}
	}
}
