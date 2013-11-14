using System;

namespace Eto.Forms
{
	public interface IPanel : IDockContainer
	{
	}

	public class Panel : DockContainer
	{
		public Panel()
			: this((Generator)null)
		{
		}

		public Panel(Generator generator)
			: this(generator, typeof(IPanel))
		{
		}

		protected Panel(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{

		}

	}
}
