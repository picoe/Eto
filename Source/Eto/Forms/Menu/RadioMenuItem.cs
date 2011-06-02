using System;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IRadioMenuItem : IMenuActionItem
	{
		void Create(RadioMenuItem controller);
		bool Checked { get; set; }
	}
	
	public class RadioMenuItem : MenuActionItem
	{
		IRadioMenuItem inner;

		public RadioMenuItem(Generator g, RadioMenuItem controller) : base(g, typeof(IRadioMenuItem))
		{
			inner = (IRadioMenuItem)base.Handler;
			inner.Create(controller);
		}


		public bool Checked
		{
			get { return inner.Checked; }
			set { inner.Checked = value; }
		}
	}
}
