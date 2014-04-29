using Eto.Forms;
using sw = Windows.UI.Xaml;

namespace Eto.WinRT.Forms
{
	/// <summary>
	/// Layout handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class WpfLayout<TControl, TWidget> : WpfContainer<TControl, TWidget>, ILayout
		where TControl: sw.FrameworkElement
		where TWidget: Layout
	{

		public virtual void Update()
		{
			Control.UpdateLayout();
		}
	}
}
