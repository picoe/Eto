using System;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	[Obsolete("Use Command apis instead")]
	public class ActionCollection : KeyedCollection<string, BaseAction>
	{
		readonly Generator generator;

		public ActionCollection()
			: this((Generator)null)
		{
		}

		public ActionCollection (Generator generator)
			: this(generator, null)
		{
		}
		
		public ActionCollection (Generator generator, Control control)
		{
			this.generator = generator;
			if (control != null)
				control.KeyDown += control_KeyDown;
		}

		public Generator Generator {
			get { return generator; }
		}
		
		protected override string GetKeyForItem (BaseAction item)
		{
			return item.ID;
		}
		
		protected override void RemoveItem (int index)
		{
			var item = this [index];
			item.Remove ();
			base.RemoveItem (index);
		}
		
		protected override void ClearItems ()
		{
			foreach (BaseAction a in this) {
				a.Remove ();
			}
			base.ClearItems ();
		}
		
		public BaseAction Find (string key)
		{
			return Contains(key) ? this[key] : null;
		}
		
		public bool RemoveHandler (string actionID, EventHandler<EventArgs> activatedHandler)
		{
			var action = this [actionID];
			if (action != null) {
				action.Activated -= activatedHandler;
				return true;
			}
			return false;
		}
		
		void control_KeyDown (object sender, KeyEventArgs e)
		{
			//Console.WriteLine("key: {0}, sender: {1}", e.KeyData.ToString(), sender.GetType().ToString());
			foreach (var action in this) {
				if (!action.Enabled)
					continue;
				if (action.Accelerators == null)
					continue;
				foreach (Keys key in action.Accelerators) {
					if (e.KeyData == key) {
						//Console.WriteLine("action: {0}, key: {1}, sender: {2}", action.Text, e.KeyData.ToString(), sender.GetType().ToString());
						e.Handled = true;
						action.Activate ();
						//break; // go through all that match
					}
				}
				if (e.Handled)
					break;
			}
		}
	}
}

