using System;
using Eto.Forms;
using SD = System.Drawing;
using MonoMac.Foundation;

namespace Eto.Platform.Mac
{
	public interface IMacLayout {
		object LayoutObject { get; }
		void SizeToFit();
	}
	
	public abstract class MacLayout<T, W> : MacObject<T, W>, ILayout, IMacLayout
		where T: NSObject
		where W: Layout
	{
		public virtual object LayoutObject
		{
			get { return null; }
		}
		
		public abstract void SizeToFit();

	}
}
