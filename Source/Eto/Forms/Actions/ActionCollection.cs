using System;
using Eto.Collections;
using Eto.Drawing;
using System.Reflection;

namespace Eto.Forms
{
	public class ActionCollection : LookupList<string, BaseAction>
	{
		Generator generator;

		public ActionCollection(Generator g, Control control)
		{
			this.generator = g;
			if (control != null) control.KeyDown += control_KeyDown;
		}

		public Generator Generator
		{
			get { return generator; }
		}
		
		protected override string GetLookupByValue(BaseAction value)
		{
			return value.ID;
		}

		protected override void OnRemoved (ListEventArgs<BaseAction> e)
		{
			base.OnRemoved (e);
			e.Item.Remove();
		}
		
		public override void Clear()
		{
			foreach (BaseAction a in this)
			{
				a.Remove();
			}
			base.Clear();
		}


		public bool RemoveHandler(string actionID, EventHandler<EventArgs> activatedHandler)
		{
			var action = Find(actionID);
			if (action != null)
			{
				action.Activated -= activatedHandler;
				return true;
			}
			return false;
		}
		
		private void control_KeyDown(object sender, KeyPressEventArgs e)
		{
			//Console.WriteLine("key: {0}, sender: {1}", e.KeyData.ToString(), sender.GetType().ToString());
			foreach (var action in this)
			{
				if (action.Accelerators == null) continue;
				foreach (Key key in action.Accelerators)
				{
					if (e.KeyData == key)
					{
						//Console.WriteLine("action: {0}, key: {1}, sender: {2}", action.Text, e.KeyData.ToString(), sender.GetType().ToString());
						e.Handled = true;
						action.Activate();
						//break; // go through all that match
					}
				}
				if (e.Handled) break;
			}
		}
	}
}

