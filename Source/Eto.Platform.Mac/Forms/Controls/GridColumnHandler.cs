using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class GridColumnHandler : DataColumnHandler<NSTableColumn, GridColumn>, IGridColumn
	{

		public GridColumnHandler ()
		{
		}
	}
}

