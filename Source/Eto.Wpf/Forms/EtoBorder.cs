using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sw = System.Windows;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;

namespace Eto.Wpf.Forms
{
	/// <summary>
	/// Shared class to use as a container that properly handles measurement for Eto sizing rules
	/// </summary>
	/// <remarks>
	/// This should only be used if it is set to the <see cref="WpfFrameworkElement{TControl, TWidget, TCallback}.ContainerControl"/>.
	/// </remarks>
	public class EtoBorder : swc.Border
	{
		public IWpfFrameworkElement Handler { get; set; }

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}

}
