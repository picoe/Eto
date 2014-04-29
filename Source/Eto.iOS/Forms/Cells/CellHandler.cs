using System;
using Eto.Drawing;
using Eto.Forms;
using MonoTouch.UIKit;
using NSCell = MonoTouch.UIKit.UITableViewCell;

namespace Eto.iOS.Forms.Cells
{
	public interface ICellHandler
	{
		NSCell Control { get; }

		void Configure (object dataItem, UITableViewCell cell);

		string TitleForSection (object dataItem);

		void SetBackgroundColor(NSCell cell, Color color);

		Color GetBackgroundColor(NSCell cell);

		void SetForegroundColor(NSCell cell, Color color);

		Color GetForegroundColor(NSCell cell);
	}

	public abstract class CellHandler<T, W> : WidgetHandler<T, W>, ICell, ICellHandler
		where T: NSCell
		where W: Cell
	{
		public CellHandler ()
		{
		}

		public abstract void Configure (object dataItem, UITableViewCell cell);

		public abstract string TitleForSection (object dataItem);

		public new NSCell Control
		{
			get { return Control; }
		}

		public void SetBackgroundColor(NSCell cell, Color color)
		{
			cell.BackgroundColor = color.ToNSUI();
		}

		public Color GetBackgroundColor(NSCell cell)
		{
			return cell.BackgroundColor.ToEto();
		}

		public void SetForegroundColor(NSCell cell, Color color)
		{
			cell.TextLabel.TextColor = color.ToNSUI();
		}

		public Color GetForegroundColor(NSCell cell)
		{
			return cell.TextLabel.TextColor.ToEto();
		}
	}
}

