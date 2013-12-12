#if DESKTOP

using System;
namespace Eto.Forms
{
#if MENU_TOOLBAR_REFACTORING
	public abstract partial class BaseAction
	{
		public virtual MenuItem GenerateMenuItem(Generator generator)
		{
			throw new NotImplementedException();
		}
	}
#endif
}
#endif