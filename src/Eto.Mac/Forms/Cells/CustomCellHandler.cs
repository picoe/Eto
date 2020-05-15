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
			var column = -1;// TODO: lookup!
			var args = new MutableCellEventArgs(ColumnHandler?.DataViewHandler as Grid, Widget, row, column, dataItem, CellStates.None, null);
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
			args.SetControl(widthCell);
			Callback.OnConfigureCell(Widget, args, widthCell);
			widthCell.GetMacControl()?.InvalidateMeasure();

			var result = widthCell.GetPreferredSize(SizeF.PositiveInfinity).Width;

			widthCell.DataContext = null;
			return result;
			//return Callback.OnGetPreferredWidth(Widget, new CellEventArgs(row, dataItem, CellStates.None));
		}


		public class EtoCustomCellView : NSTableCellView
		{
			public CustomCellHandler Handler { get; set;}
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

			public void Setup()
			{
				EtoControl.GotFocus += ControlGotFocus;
				EtoControl.LostFocus += ControlLostFocus;
			}

			bool losingFocus;

			private void ControlLostFocus(object sender, EventArgs e)
			{
				if (losingFocus)
					return;

				losingFocus = true;
				var h = Handler;
				var ee = MacConversions.CreateCellEventArgs(h.ColumnHandler.Widget, null, Args.Row, Args.Column, Args.Item);
				if (!h.ColumnHandler.DataViewHandler.IsCancellingEdit)
				{
					h.Callback.OnCommitEdit(h.Widget, Args);
					h.ColumnHandler.DataViewHandler.OnCellEdited(ee);
				}
				else
				{
					h.Callback.OnCancelEdit(h.Widget, Args);
				}
				losingFocus = false;
			}

			private void ControlGotFocus(object sender, EventArgs e)
			{
				var h = Handler;
				h.Callback.OnBeginEdit(h.Widget, Args);
				var ee = MacConversions.CreateCellEventArgs(h.ColumnHandler.Widget, null, Args.Row, Args.Column, Args.Item);
				h.ColumnHandler.DataViewHandler.OnCellEditing(ee);
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
			var column = -1; // TODO: get index or lookup when needed.
			var args = new MutableCellEventArgs(ColumnHandler.DataViewHandler as Grid, Widget, row, column, item, state, null);
			var identifier = tableColumn.Identifier;
			var id = Callback.OnGetIdentifier(Widget, args);
			if (!string.IsNullOrEmpty(id))
				identifier += "_" + id;

			var view = tableView.MakeView(identifier, tableView) as EtoCustomCellView;
			if (view == null)
			{
				var control = Callback.OnCreateCell(Widget, args);
				args.SetControl(control);

				view = new EtoCustomCellView
				{ 
					Handler = this,
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
					view.Setup();
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

