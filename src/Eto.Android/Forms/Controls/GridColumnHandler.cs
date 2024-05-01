using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms.Controls
{
	public class GridColumnHandler : WidgetHandler<object, GridColumn>, GridColumn.IHandler
	{
		public string HeaderText { get; set; }
		public bool Resizable { get; set; }
		public bool Sortable { get; set; }
		public bool AutoSize { get; set; }
		public int Width { get; set; }
		public Cell DataCell { get; set; }
		public bool Editable { get; set; }
		public bool Visible { get; set; }
		public bool Expand { get; set; }
		public TextAlignment HeaderTextAlignment { get; set; }
		public int MinWidth { get; set; }
		public int MaxWidth { get; set; }
		public int DisplayIndex { get; set; }
		public string HeaderToolTip { get; set; }
		public IIndirectBinding<string> CellToolTipBinding { get; set; }
	}
}