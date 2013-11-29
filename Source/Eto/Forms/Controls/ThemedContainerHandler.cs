using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Base class for implementing Eto Container
	/// controls using Eto controls.
	/// </summary>
	/// <typeparam name="TControl">The Eto control used to create the custom implementation</typeparam>
	/// <typeparam name="TWidget">The container being implemented.</typeparam>
	public abstract class ThemedContainerHandler<TControl, TWidget> : ThemedControlHandler<TControl, TWidget>, IContainer
		where TControl: Container
		where TWidget : Control
	{
		public Size ClientSize
		{
			get { return Control.ClientSize; }
			set { Control.ClientSize = value; }
		}

		public virtual bool RecurseToChildren { get { return false; } }
	}
}
