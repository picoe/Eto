using System;
using Eto.Forms;
using SD = System.Drawing;
using MonoMac.AppKit;
using System.Linq;
using Eto.Drawing;

#if IOS
using NSResponder = MonoTouch.UIKit.UIResponder;
using Eto.Platform.iOS.Forms;
#endif

namespace Eto.Platform.Mac.Forms
{
	public interface IMacContainer : IMacViewHandler
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
			if (ApplicationHandler.QueueResizing)
			{
				ApplicationHandler.Instance.AsyncInvoke(update ?? Update);
				return true;
			}
			return false;
		}

		public virtual void SetContentSize(SD.SizeF contentSize)
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
			if (updateSize)
			{
				if (AutoSize)
				{
					var size = GetPreferredSize(Size.MaxValue);
					SetContentSize(size.ToSD());
				}
			}

			// layout everything!
			LayoutAllChildren();
		}
	}
}
