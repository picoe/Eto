using System;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IRadioMenuItem : IMenuActionItem
	{
		void Create (RadioMenuItem controller);

		bool Checked { get; set; }
	}
	
	public class RadioMenuItem : MenuActionItem
	{
		IRadioMenuItem handler;

		public RadioMenuItem (RadioMenuItem controller = null) : this (Generator.Current, controller)
		{
		}
		
		public RadioMenuItem (Generator g, RadioMenuItem controller = null)
			: this (g, typeof(IRadioMenuItem), controller)
		{
		}

		protected RadioMenuItem (Generator generator, Type type, RadioMenuItem controller, bool initialize = true)
			: base (generator, type, false)
		{
			handler = (IRadioMenuItem)base.Handler;
			handler.Create (controller);
			if (initialize)
				Initialize ();
		}

		public bool Checked {
			get { return handler.Checked; }
			set { handler.Checked = value; }
		}
	}
}
