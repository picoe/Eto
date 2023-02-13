using Eto.Forms;

using aw = Android.Widget;

namespace Eto.Android.Forms.ToolBar
{
	public class SeparatorToolItemHandler : ToolItemHandler<aw.Space, SeparatorToolItem>, SeparatorToolItem.IHandler
	{
		protected override aw.Space GetInnerControl(ToolBarHandler handler)
		{
			if (!HasControl)
			{
				Control = new aw.Space(Platform.AppContextThemed);
			}

			return Control;
		}

		public override void SetText(string text) { }

		public SeparatorToolItemType Type { get; set; }
	}
}
