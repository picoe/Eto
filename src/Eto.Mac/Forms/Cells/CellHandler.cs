using Eto.Mac.Forms.Controls;


namespace Eto.Mac.Forms.Cells
{
	public interface ICellHandler
	{
		Eto.Platform Platform { get; }

		IDataColumnHandler ColumnHandler { get; set; }

		NSObject GetObjectValue(object dataItem);

		void SetObjectValue(object dataItem, NSObject val);

		nfloat GetPreferredWidth(object value, CGSize cellSize, int row, object dataItem);

		void HandleEvent(string handler, bool defaultEvent = false);

		Font GetFont(NSView view);

		void SetFont(NSView view, Font font);

		Color GetBackgroundColor(NSView view);

		void SetBackgroundColor(NSView view, Color color);

		Color GetForegroundColor(NSView view);

		void SetForegroundColor(NSView view, Color color);

		bool Editable { get; set; }

		void EnabledChanged(bool value);

		NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row, NSObject obj, Func<NSObject, int, object> getItem);

		void ViewRemoved(NSView view);
	}

	public class EtoCellTextField : EtoTextField
	{
		public EtoCellTextField()
		{
		}

		public EtoCellTextField(IntPtr handle)
			: base(handle)
		{
		}

		bool isFirstResponder;
		public event EventHandler<EventArgs> BecameFirstResponder;
		public event EventHandler<EventArgs> ResignedFirstResponder;

		public override bool BecomeFirstResponder()
		{
			var ret = base.BecomeFirstResponder();
			if (ret && BecameFirstResponder != null)
				BecameFirstResponder(this, EventArgs.Empty);
			if (ret)
				isFirstResponder = true;
			return ret;
		}

		public override void DidEndEditing(NSNotification notification)
		{
			base.DidEndEditing(notification);
			isFirstResponder = false;
		}

		public override bool ResignFirstResponder()
		{
			var ret = base.ResignFirstResponder();
			if (ret && isFirstResponder && ResignedFirstResponder != null)
			{
				ResignedFirstResponder(this, EventArgs.Empty);
				isFirstResponder = false;
			}
			return ret;
		}
	}

	public abstract class CellHandler<TWidget, TCallback> : MacObject<NSObject, TWidget, TCallback>, Cell.IHandler, ICellHandler
		where TWidget: Cell
	{
		public IDataColumnHandler ColumnHandler { get; set; }

		public virtual bool Editable { get; set; }

		public virtual NSObject GetObjectValue(object dataItem)
		{
			return null;
		}

		Eto.Platform ICellHandler.Platform
		{
			get { return Widget.Platform; }
		}

		public abstract void SetObjectValue(object dataItem, NSObject value);

		public virtual nfloat GetPreferredWidth(object value, CGSize cellSize, int row, object dataItem)
		{
			return 20;
		}

		public virtual void EnabledChanged(bool value)
		{
		}

		public virtual Font GetFont(NSView view)
		{
			return null;
		}

		public virtual void SetFont(NSView view, Font font)
		{
		}

		public virtual Color GetBackgroundColor(NSView view)
		{
			return Colors.Transparent;
		}

		public virtual void SetBackgroundColor(NSView view, Color color)
		{
		}

		public virtual Color GetForegroundColor(NSView view)
		{
			return NSColor.ControlText.ToEto();
		}

		public virtual void SetForegroundColor(NSView view, Color color)
		{
		}
		
		public virtual void ViewRemoved(NSView view)
		{
		}

		protected virtual void ReloadColumnData()
		{
			var handler = ColumnHandler?.DataViewHandler;
			if (handler?.Loaded == true)
			{
				var column = handler.Widget.Columns.IndexOf(ColumnHandler.Widget);
				var rows = NSIndexSet.FromNSRange(new NSRange(0, handler.Table.RowCount));
				handler.Table.ReloadData(rows, new NSIndexSet(column));
			}
		}

		public abstract NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row, NSObject obj, Func<NSObject, int, object> getItem);
	}
}

