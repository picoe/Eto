using System;
using Eto.Forms;
using SD = System.Drawing;
using SWF = System.Windows.Forms;

namespace Eto.Platform.Windows
{
	public interface IWindowsLayout
	{
		object LayoutObject { get; }
	}
	
	public abstract class WindowsLayout<T, W> : WidgetHandler<T, W>, ILayout, IWindowsLayout
		where W: Layout
	{

		public virtual object LayoutObject {
			get { return null; }
		}
		
		public virtual void OnPreLoad ()
		{
		}

		public virtual void OnLoad ()
		{
		}

		public virtual void OnLoadComplete ()
		{
		}
		
		public virtual void Update ()
		{
		}

		public virtual void AttachedToContainer ()
		{
		}
	}
}
