using System;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using Eto.Mac.Forms.Controls;
using Eto.Shared;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
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
	public class CustomCellHandler : CellHandler<CustomCell, CustomCell.ICallback>, CustomCell.IHandler
	{
		public override void SetObjectValue(object dataItem, NSObject value)
		{
		}

		public override NSObject GetObjectValue(object dataItem)
		{
			return null;
		}

		Dictionary<string, Control> widthCells = new Dictionary<string, Eto.Forms.Control>();

		public override nfloat GetPreferredWidth(object value, CGSize cellSize, int row, object dataItem)
		{
			var args = new CellEventArgs(row, dataItem, CellStates.None);
			var identifier = Callback.OnGetIdentifier(Widget, args) ?? string.Empty;
			Control widthCell;
			if (!widthCells.TryGetValue(identifier, out widthCell))
			{
				widthCell = Callback.OnCreateCell(Widget, args);
				if (widthCell == null)
					return Callback.OnGetPreferredWidth(Widget, args);
				widthCell.AttachNative();
				widthCells.Add(identifier, widthCell);
			}
			Callback.OnConfigureCell(Widget, args, widthCell);
			widthCell.GetMacControl()?.InvalidateMeasure();

			var result = widthCell.GetPreferredSize(SizeF.PositiveInfinity).Width;

			widthCell.DataContext = null;
			return result;
			//return Callback.OnGetPreferredWidth(Widget, new CellEventArgs(row, dataItem, CellStates.None));
		}


		public class EtoCustomCellView : NSTableCellView
		{
			public MutableCellEventArgs Args { get; set; }

			public Control EtoControl { get; set; }

			public bool DrawsBackground { get; set; }

			public NSColor BackgroundColor { get; set; }

			public override void DrawRect(CGRect dirtyRect)
			{
				if (DrawsBackground && BackgroundColor != null && BackgroundColor.AlphaComponent > 0)
				{
					BackgroundColor.Set();
					NSGraphics.RectFill(dirtyRect);
				}
				base.DrawRect(dirtyRect);
			}

			static Color AlternateSelectedControlText = NSColor.AlternateSelectedControlText.ToEto();
			static Color ControlText = NSColor.ControlText.ToEto();

			public override NSBackgroundStyle BackgroundStyle
			{
				get { return base.BackgroundStyle; }
				set
				{
					base.BackgroundStyle = value;
					if (value == NSBackgroundStyle.Dark)
						Args.SetTextColor(AlternateSelectedControlText);
					else
						Args.SetTextColor(ControlText);
				}
			}
		}

		public override Color GetBackgroundColor(NSView view)
		{
			return ((EtoCustomCellView)view).BackgroundColor.ToEto();
		}

		public override void SetBackgroundColor(NSView view, Color color)
		{
			var field = ((EtoCustomCellView)view);
			field.BackgroundColor = color.ToNSUI();
			field.DrawsBackground = color.A > 0;
		}

		public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row, NSObject obj, Func<NSObject, int, object> getItem)
		{
			var item = getItem(obj, row);
			var state = CellStates.None;
			if (tableView.IsRowSelected(row))
				state |= CellStates.Selected;
			if (tableColumn.Editable)
				state |= CellStates.Editing;
			var args = new MutableCellEventArgs(row, item, state);
			var identifier = tableColumn.Identifier;
			var id = Callback.OnGetIdentifier(Widget, args);
			if (!string.IsNullOrEmpty(id))
				identifier += "_" + id;

			var view = tableView.MakeView(identifier, tableView) as EtoCustomCellView;
			if (view == null)
			{
				var control = Callback.OnCreateCell(Widget, args);

				view = new EtoCustomCellView
				{ 
					Args = args,
					EtoControl = control,
					Identifier = identifier,
					Frame = CGRect.Empty,
					AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable 
				};
				if (control != null)
				{
					var childView = control.ToNative(true);
					childView.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
					childView.Frame = view.Frame;
					view.AddSubview(childView);
				}
			}
			else
			{
				view.Args.SetEditable(args.IsEditing);
				view.Args.SetSelected(args.IsSelected);
				view.Args.SetItem(args.Item);
			}

			var formatArgs = new MacCellFormatArgs(ColumnHandler.Widget, item, row, view);
			ColumnHandler.DataViewHandler.OnCellFormatting(formatArgs);
			Callback.OnConfigureCell(Widget, view.Args, view.EtoControl);
			return view;
		}
	}
}

