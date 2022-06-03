using System;
using Eto.Forms;

namespace Eto.Mac.Forms
{
	public class FloatingFormHandler : FormHandler<NSPanel>, FloatingForm.IHandler
	{
		public FloatingFormHandler()
		{
		}

		public FloatingFormHandler(NSPanel panel)
			: base(panel)
		{
		}

		public FloatingFormHandler(NSWindowController panelController)
			: base(panelController)
		{
		}

		protected override void Initialize()
		{
			base.Initialize();
			Maximizable = false;
			Minimizable = false;
			ShowInTaskbar = false;
		}


		static readonly object LastOwner_Key = new object();

		protected override NSWindowLevel TopmostWindowLevel => NSWindowLevel.Floating;

		public override void SetOwner(Window owner)
		{
			base.SetOwner(owner);

			// When this is true, the NSPanel would hide the panel and owner if they aren't key.
			// So, only hide on deactivate if it is an ownerless form.
			Control.HidesOnDeactivate = owner == null;

			SetLevelAdjustment();
		}

		void SetLevelAdjustment()
		{
			// only need to adjust level when window style is not utility and we actually want it to be topmost (default for FloatingForm).
			var wantsTopmost = Widget.Properties.Get<bool>(Topmost_Key, true);
			var owner = Widget.Owner;
			var needsLevelAdjust = wantsTopmost && WindowStyle != WindowStyle.Utility && owner != null;

			var lastOwner = Widget.Properties.Get<Window>(LastOwner_Key);

			if (!needsLevelAdjust)
			{
				if (lastOwner != null)
				{
					// no longer need window level adjustments, unregister
					lastOwner.GotFocus -= Owner_GotFocus;
					lastOwner.LostFocus -= Owner_LostFocus;
					Widget.Closed -= Widget_Closed;
					Widget.Properties.Set<Window>(LastOwner_Key, null);
				}
				if (wantsTopmost)
				{
					SetAsTopmost();
				}
				return;
			}

			if (!ReferenceEquals(lastOwner, owner))
			{
				Widget.Properties.Set(LastOwner_Key, owner);
				if (lastOwner != null)
				{
					lastOwner.GotFocus -= Owner_GotFocus;
					lastOwner.LostFocus -= Owner_LostFocus;
					Widget.Closed -= Widget_Closed;
				}
				if (owner != null)
				{
					owner.GotFocus += Owner_GotFocus;
					owner.LostFocus += Owner_LostFocus;
					Widget.Closed += Widget_Closed;
				}
			}

			if (lastOwner == null || lastOwner.HasFocus)
				SetAsTopmost();
		}

		private void Widget_Closed(object sender, EventArgs e)
		{
			var lastOwner = Widget.Properties.Get<Window>(LastOwner_Key);
			if (lastOwner != null)
			{
				// when closed we need to disconnect from owner to prevent leaks
				lastOwner.GotFocus -= Owner_GotFocus;
				lastOwner.LostFocus -= Owner_LostFocus;
			}
		}

		static readonly object Topmost_Key = new object();

		public override bool Topmost
		{
			get => base.Topmost;
			set
			{
				base.Topmost = value;
				// need to remember the preferred state as it can be changed on us when setting the owner
				Widget.Properties.Set(Topmost_Key, value, true);
				SetLevelAdjustment();
			}
		}

		public override WindowStyle WindowStyle
		{
			get => base.WindowStyle;
			set
			{
				base.WindowStyle = value;
				SetLevelAdjustment();
			}
		}

		private void Owner_GotFocus(object sender, EventArgs e) => SetAsTopmost();
		
		void SetAsTopmost()
		{
			Control.Level = TopmostWindowLevel;
			Control.HidesOnDeactivate = true;
		}

		private void Owner_LostFocus(object sender, EventArgs e)
		{
			Control.Level = NSWindowLevel.Normal;
			if (Control.IsVisible)
				Control.OrderFront(Control);
		}

		protected override NSPanel CreateControl()
		{
			var panel = new EtoPanel(new CGRect(0, 0, 200, 200),
				NSWindowStyle.Resizable | NSWindowStyle.Closable | NSWindowStyle.Titled,
				NSBackingStore.Buffered, false);
				
			panel.CanFocus = true;
			panel.FloatingPanel = true;
			panel.BecomesKeyOnlyIfNeeded = true;

			return panel;
		}
	}
}
