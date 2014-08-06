using System;
using Eto.Forms;
using SD = System.Drawing;
using MonoMac.AppKit;
using System.Linq;
using Eto.Drawing;
using MonoTouch.Foundation;
using MonoMac.Foundation;
#if Mac64
using CGFloat = System.Double;
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
#else
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using NSPoint = System.Drawing.PointF;
using CGFloat = System.Single;
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
#endif

#if IOS
using NSResponder = MonoTouch.UIKit.UIResponder;
using NSView = MonoTouch.UIKit.UIView;
using Eto.iOS.Forms;
#endif

namespace Eto.Mac.Forms
{
	public interface IMacContainer : IMacControlHandler
	{
		void SetContentSize(NSSize contentSize);

		void LayoutParent(bool updateSize = true);

		void LayoutChildren();

		void LayoutAllChildren();

		bool InitialLayout { get; }
	}

	public abstract class MacContainer<TControl, TWidget, TCallback> : 
		MacView<TControl, TWidget, TCallback>,
		Container.IHandler, IMacContainer
		where TControl: NSObject
		where TWidget: Container
		where TCallback: Container.ICallback
	{
		public bool RecurseToChildren { get { return true; } }

		public virtual Size ClientSize { get { return Size; } set { Size = value; } }

		public override bool Enabled { get; set; }

		public bool InitialLayout { get; private set; }

		protected override void Initialize()
		{
			base.Initialize();
			Enabled = true;
		}

		public virtual void Update()
		{
			LayoutChildren();	
		}

		public bool NeedsQueue(Action update = null)
		{
			#if OSX
			if (ApplicationHandler.QueueResizing)
			{
				ApplicationHandler.Instance.AsyncInvoke(update ?? Update);
				return true;
			}
			#endif
			return false;
		}

		public virtual void SetContentSize(NSSize contentSize)
		{
		}

		public virtual void LayoutChildren()
		{
		}

		public void LayoutAllChildren()
		{
			//Console.WriteLine("Layout all children: {0}\n {1}", this.GetType().Name, new StackTrace());
			LayoutChildren();
			foreach (var child in Widget.Controls.Select (r => r.GetMacContainer()).Where(r => r != null))
			{
				child.LayoutAllChildren();
			}
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			var parent = Widget.Parent.GetMacContainer();
			if (parent == null || parent.InitialLayout)
			{
				InitialLayout = true;
				LayoutAllChildren();
			}
		}

		public void LayoutParent(bool updateSize = true)
		{
			if (NeedsQueue(() => LayoutParent(updateSize)))
				return;
			var container = Widget.Parent.GetMacContainer();
			if (container != null)
			{
				// traverse up the tree to update everything we own
				container.LayoutParent(updateSize);
				return;
			} 
			if (updateSize && !Widget.Loaded && AutoSize)
			{
				var size = GetPreferredSize(Size.MaxValue);
				SetContentSize(size.ToNS());
			}

			// layout everything!
			LayoutAllChildren();
		}
	}
}
