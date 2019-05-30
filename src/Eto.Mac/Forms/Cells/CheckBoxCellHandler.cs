using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Forms.Controls;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms.Cells
{
	public class CheckBoxCellHandler : CellHandler<CheckBoxCell, CheckBoxCell.ICallback>, CheckBoxCell.IHandler
	{

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

		public override Color GetBackgroundColor(NSView view)
		{
			return ((CellView)view).Cell.BackgroundColor.ToEto();
		}

		public override void SetBackgroundColor(NSView view, Color color)
		{
			var field = ((CellView)view).Cell;
			field.BackgroundColor = color.ToNSUI();
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
					var control = (CellView)sender;
					var r = (int)control.Tag;
					var item = getItem(control.Item, r);
					var ee = MacConversions.CreateCellEventArgs(ColumnHandler.Widget, tableView, r, col, item);
					ColumnHandler.DataViewHandler.OnCellEditing(ee);
					SetObjectValue(item, control.ObjectValue);
					control.ObjectValue = GetObjectValue(item);

					ColumnHandler.DataViewHandler.OnCellEdited(ee);
				};
			}
			view.Tag = row;
			view.Item = obj;
			var args = new MacCellFormatArgs(ColumnHandler.Widget, getItem(obj, row), row, view);
			ColumnHandler.DataViewHandler.OnCellFormatting(args);
			return view;
		}
	}
}

