using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
namespace Eto.Android.Forms.Cells
{
	public class CheckBoxCellHandler : CellHandler<CheckBoxCell>, CheckBoxCell.IHandler
	{
		public override av.View CreateView(av.View view, object item)
		{
			var tv = view as aw.TextView ?? new aw.TextView(Platform.AppContextThemed);

			tv.Text = Widget.Binding?.GetValue(item)?.ToString() ?? String.Empty;
			return tv;
		}
	}
}
