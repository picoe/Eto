using System;
using Eto.Drawing;
using Eto.Forms;
using UIKit;
using NSCell = UIKit.UITableViewCell;

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

		Font GetFont(NSCell cell);

		void SetFont(NSCell cell, Font font);
	}

	public abstract class CellHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, Cell.IHandler, ICellHandler
		where TControl: NSCell
		where TWidget: Cell
	{
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

		public Font GetFont(NSCell cell)
		{
			return cell.TextLabel.Font.ToEto();
		}

		public void SetFont(NSCell cell, Font font)
		{
			cell.TextLabel.Font = font.ToUI();
		}
	}
}

