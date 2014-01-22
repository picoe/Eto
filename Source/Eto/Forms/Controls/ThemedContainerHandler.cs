using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Base class for implementing Eto Container control handlers using Eto controls.
	/// </summary>
	/// <typeparam name="TControl">The Eto control used to create the custom implementation</typeparam>
	/// <typeparam name="TWidget">The container being implemented.</typeparam>
	public abstract class ThemedContainerHandler<TControl, TWidget> : ThemedControlHandler<TControl, TWidget>, IContainer
		where TControl: Container
		where TWidget : Control
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
		/// This is mainly used when you want to use Eto controls in your handler, such as with the <see cref="ThemedContainerHandler{TContainer,TWidget}"/>
		/// </remarks>
		/// <value><c>true</c> to recurse events to children; otherwise, <c>false</c>.</value>
		public virtual bool RecurseToChildren { get { return false; } }
	}
}
