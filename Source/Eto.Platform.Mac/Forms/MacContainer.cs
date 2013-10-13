using System;
using Eto.Forms;
using SD = System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Linq;
using Eto.Drawing;

#if IOS
using NSResponder = MonoTouch.UIKit.UIResponder;
using Eto.Platform.iOS.Forms;
#endif

namespace Eto.Platform.Mac.Forms
{
	public interface IMacContainer : IMacAutoSizing
	{
		void SetContentSize(SD.SizeF contentSize);

		void LayoutParent(bool updateSize = true);

		void LayoutChildren();

		void LayoutAllChildren();

		bool InitialLayout { get; }
	}

	public abstract class MacContainer<TControl, TWidget> : 
#if OSX
		MacView<TControl, TWidget>,
#elif IOS
		IosView<TControl, TWidget>,
#endif
		IContainer, IMacContainer
		where TControl: NSResponder
		where TWidget: Container
	{
		public virtual Size ClientSize { get { return Size; } set { Size = value; } }

		public override bool Enabled { get; set; }

		public bool InitialLayout { get; private set; }

		public virtual void Update()
		{
			LayoutChildren();	
		}

		public virtual void SetContentSize(SD.SizeF contentSize)
		{
		}

		public virtual void LayoutChildren()
		{
		}

		public void LayoutAllChildren()
		{
			LayoutChildren();
			foreach (var child in Widget.Controls.Select (r => r.GetMacContainer()).Where(r => r != null))
			{
				child.LayoutAllChildren();
			}
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);

			var parent = Widget.Parent.GetMacContainer();
			if (parent == null || parent.InitialLayout)
			{
				InitialLayout = true;
				LayoutAllChildren();
			}
		}

		public void LayoutParent(bool updateSize = true)
		{
			var container = Widget.Parent.GetMacContainer();
			if (container != null)
			{
				// traverse up the tree to update everything we own
				if (updateSize)
				{
					if (!AutoSize)
					{
						foreach (var child in Widget.Controls.Select (r => r.GetMacContainer()).Where(r => r != null))
						{
							var size = child.GetPreferredSize(Size.MaxValue);
							child.SetContentSize(size.ToSDSizeF());
						}
						updateSize = false;
					}
				}
				container.LayoutParent(updateSize);
				return;
			} 
			if (updateSize)
			{
				if (AutoSize)
				{
					var size = GetPreferredSize(Size.MaxValue);
					SetContentSize(size.ToSDSizeF());
				}
				else
				{
					foreach (var child in Widget.Controls.Select (r => r.GetMacContainer()).Where(r => r != null))
					{
						var size = child.GetPreferredSize(Size.MaxValue);
						child.SetContentSize(size.ToSDSizeF());
					}
				}
			}

			// layout everything!
			LayoutAllChildren();
		}
	}
}
