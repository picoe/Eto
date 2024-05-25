using Eto.Mac.Forms.Controls;


namespace Eto.Mac.Forms.Cells
{
	public class CheckBoxCellHandler : CellHandler<CheckBoxCell, CheckBoxCell.ICallback>, CheckBoxCell.IHandler
	{

		static readonly NSColor defaultColor = new CellView().Cell.BackgroundColor;

		public override void SetObjectValue(object dataItem, NSObject value)
		{
			if (Widget.Binding != null && !ColumnHandler.DataViewHandler.SuppressUpdate)
			{
				var num = value as NSNumber;
				if (num != null)
				{
					var state = (NSCellStateValue)num.Int32Value;
					bool? boolValue;
					switch (state)
					{
						default:
							boolValue = null;
							break;
						case NSCellStateValue.On:
							boolValue = true;
							break;
						case NSCellStateValue.Off:
							boolValue = false;
							break;
					}
					Widget.Binding.SetValue(dataItem, boolValue);
				}
			}
		}

		public override NSObject GetObjectValue(object dataItem)
		{
			if (Widget.Binding != null)
			{
				NSCellStateValue state = NSCellStateValue.Off;
				var val = Widget.Binding.GetValue(dataItem);
				state = val != null ? val.Value ? NSCellStateValue.On : NSCellStateValue.Off : NSCellStateValue.Mixed;
				return new NSNumber((int)state);
			}
			return new NSNumber((int)NSCellStateValue.Off);
		}

		public class EtoButton : NSButton
		{
			public event EventHandler Focussed;

			public EtoButton()
			{
			}
			public EtoButton(IntPtr handle)
				: base(handle)
			{
			}

			public override void LockFocus()
			{
				base.LockFocus();
				if (Focussed != null)
					Focussed(this, EventArgs.Empty);
			}
		}

		public override Color GetBackgroundColor(NSView view) => ((CellView)view).Cell.BackgroundColor.ToEto();

		public override void SetBackgroundColor(NSView view, Color color) => ((CellView)view).Cell.BackgroundColor = color.ToNSUI();
#if __MACOS__
		static IntPtr selBackgroundColor_Handle = Selector.GetHandle("setBackgroundColor:");
#endif
		
		private void SetDefaults(CellView view)
		{
#if __MACOS__
			// doesn't support null currently..
			Messaging.void_objc_msgSend_IntPtr(view.Cell.Handle, selBackgroundColor_Handle, defaultColor?.Handle ?? IntPtr.Zero);
#elif MONOMAC
			view.Cell.BackgroundColor = defaultColor;
#endif
		}
		

		class CellView : NSButton
		{
			[Export("item")]
			public NSObject Item { get; set; }
			public CellView() { }
			public CellView(IntPtr handle) : base(handle) { }
		}

		static NSString enabledBinding = new NSString("enabled");

		public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row, NSObject obj, Func<NSObject, int, object> getItem)
		{
			var view = tableView.MakeView(tableColumn.Identifier, tableView) as CellView;
			if (view == null)
			{
				view = new CellView { Title = string.Empty };
				view.Identifier = tableColumn.Identifier;
				view.SetButtonType(NSButtonType.Switch);
				view.Bind(enabledBinding, tableColumn, "editable", null);

				var col = Array.IndexOf(tableView.TableColumns(), tableColumn);
				view.Activated += (sender, e) =>
				{
					var colHandler = ColumnHandler;
					if (colHandler == null)
						return;
					var control = (CellView)sender;
					var r = (int)control.Tag;
					var item = getItem(control.Item, r);
					var ee = MacConversions.CreateCellEventArgs(colHandler.Widget, tableView, r, col, item);
					colHandler.DataViewHandler?.OnCellEditing(ee);
					SetObjectValue(item, control.ObjectValue);
					control.ObjectValue = GetObjectValue(item);

					colHandler.DataViewHandler?.OnCellEdited(ee);
				};
			}
			view.Tag = row;
			view.Item = obj;
			SetDefaults(view);
			var args = new MacCellFormatArgs(ColumnHandler.Widget, getItem(obj, row), row, view);
			ColumnHandler.DataViewHandler.OnCellFormatting(args);
			return view;
		}
	}
}

