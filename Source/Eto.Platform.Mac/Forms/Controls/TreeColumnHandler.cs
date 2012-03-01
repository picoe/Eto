using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class TreeColumnHandler : DataColumnHandler<NSTableColumn, TreeColumn>, ITreeColumn
	{

		public TreeColumnHandler ()
		{
		}
		
		protected override float GetRowWidth (ICellHandler cell, int row, System.Drawing.SizeF cellSize)
		{
			var width = base.GetRowWidth (cell, row, cellSize);
			if (Column == 0) {
				var outline = (NSOutlineView)base.Handler.Table;
				var level = outline.LevelForRow (row) + 1;
				width += level * outline.IndentationPerLevel;
			}
			return width;
		}
	}
}

