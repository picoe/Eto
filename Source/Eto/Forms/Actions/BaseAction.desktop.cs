#if DESKTOP

using System;
namespace Eto.Forms
{
	public abstract partial class BaseAction
	{
		public virtual MenuItem GenerateMenuItem(Generator generator)
		{
			throw new NotImplementedException();
		}
	}
}
#endif