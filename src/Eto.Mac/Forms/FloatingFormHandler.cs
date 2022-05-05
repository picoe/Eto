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

		public override void SetOwner(Window owner)
		{
			base.SetOwner(owner);
			
			// When this is true, the NSPanel would hide the panel and owner if they aren't key.
			// So, only hide on deactivate if it is an ownerless form.
			Control.HidesOnDeactivate = owner == null;
		}

		protected override NSPanel CreateControl()
		{
			var panel = new EtoPanel(new CGRect(0, 0, 200, 200), 
				NSWindowStyle.Resizable | NSWindowStyle.Closable | NSWindowStyle.Titled, 
				NSBackingStore.Buffered, false);

			panel.FloatingPanel = true;
			panel.BecomesKeyOnlyIfNeeded = true;
				
			return panel;
		}
	}
}
