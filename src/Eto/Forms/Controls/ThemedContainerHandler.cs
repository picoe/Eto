using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Base class for implementing Eto Container control handlers using Eto controls.
	/// </summary>
	/// <typeparam name="TControl">The Eto control used to create the custom implementation</typeparam>
	/// <typeparam name="TWidget">The container being implemented.</typeparam>
	/// <typeparam name="TCallback">The callback inferface for the control, e.g. TabControl.ICallback</typeparam>
	public abstract class ThemedContainerHandler<TControl, TWidget, TCallback> : ThemedControlHandler<TControl, TWidget, TCallback>, Container.IHandler
		where TControl: Container
		where TWidget : Container
		where TCallback : Container.ICallback
	{
		/// <summary>
		/// Gets or sets the size of the client.
		/// </summary>
		/// <value>The size of the client.</value>
		public Size ClientSize
		{
			get { return Control.ClientSize; }
			set { Control.ClientSize = value; }
		}

		/// <summary>
		/// Gets a value indicating whether PreLoad/Load/LoadComplete/Unload events are propegated to the children controls
		/// </summary>
		/// <remarks>
		/// This is mainly used when you want to use Eto controls in your handler, such as with the <see cref="ThemedContainerHandler{TContainer,TWidget,TCallback}"/>
		/// </remarks>
		/// <value><c>true</c> to recurse events to children; otherwise, <c>false</c>.</value>
		public virtual bool RecurseToChildren { get { return false; } }
	}
}
