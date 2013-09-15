using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Base class for implementing Eto Container
	/// controls using Eto controls.
	/// </summary>
	/// <typeparam name="T">The Eto control used to create the custom implementation</typeparam>
	/// <typeparam name="W">The container being implemented.</typeparam>
	public abstract class ThemedContainerHandler<T, W> : ThemedControlHandler<T, W>, IContainer
		where T: Container
		where W : Control
	{
		/*
#if DESKTOP
		public Size MinimumSize
		{
			get { return Control.MinimumSize; }
			set { Control.MinimumSize = value; }
		}
#endif
*/
		public Size ClientSize
		{
			get { return Control.ClientSize; }
			set { Control.ClientSize = value; }
		}

		public object ContainerObject
		{
			get { return Control; }
		}
	}
}
