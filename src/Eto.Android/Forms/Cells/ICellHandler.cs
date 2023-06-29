using av = Android.Views;

namespace Eto.Android.Forms.Cells
{
	public interface ICellHandler
	{
		av.View CreateView(av.View view, object item);
	}
	
	public abstract class CellHandler<TWidget> : WidgetHandler<TWidget>, Cell.IHandler, ICellHandler
		where TWidget : Cell
	{
		public abstract av.View CreateView(av.View view, object item);
	}
}