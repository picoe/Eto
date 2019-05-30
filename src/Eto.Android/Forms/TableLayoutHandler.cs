using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;
using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms
{
	/// <summary>
	/// Handler for <see cref="TableLayout"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TableLayoutHandler : AndroidContainer<aw.TableLayout, TableLayout, TableLayout.ICallback>, TableLayout.IHandler
	{
		int lastColumnScale;
		bool[] columnScale;
		int lastRowScale;
		bool[] rowScale;

		public override av.View ContainerControl { get { return Control; } }

		public TableLayoutHandler()
		{
			Control = new aw.TableLayout(aa.Application.Context);
		}

		public void CreateControl(int cols, int rows)
		{
			lastColumnScale = cols - 1;
			columnScale = new bool[cols];
			lastRowScale = rows - 1;
			rowScale = new bool[rows];
			for (int y = 0; y < rows; y++)
			{
				var row = new aw.TableRow(aa.Application.Context);
				for (int x = 0; x < cols; x++)
				{
					row.AddView(new aw.FrameLayout(aa.Application.Context), new aw.TableRow.LayoutParams(x));
				}
				Control.AddView(row, new aw.TableLayout.LayoutParams(av.ViewGroup.LayoutParams.MatchParent, av.ViewGroup.LayoutParams.MatchParent, y == lastRowScale ? 1f : 0f));
			}
			Control.SetColumnStretchable(cols - 1, true);
		}

		public bool GetColumnScale(int column)
		{
			return columnScale[column];
		}

		public void SetColumnScale(int column, bool scale)
		{
			var lastScale = lastColumnScale;
			columnScale[column] = scale;
			Control.SetColumnStretchable(column, scale);
			lastColumnScale = columnScale.Any(r => r) ? -1 : columnScale.Length - 1;
			if (lastScale != lastColumnScale)
			{
				Control.SetColumnStretchable(columnScale.Length - 1, column == lastColumnScale || columnScale[columnScale.Length - 1]);
			}
		}

		public bool GetRowScale(int row)
		{
			return rowScale[row];
		}

		public void SetRowScale(int row, bool scale)
		{
			var lastScale = lastRowScale;
			rowScale[row] = scale;
			var layout = Control.GetChildAt(row).LayoutParameters as aw.TableLayout.LayoutParams;
			layout.Weight = scale ? 1f : 0f;
			lastRowScale = rowScale.Any(r => r) ? -1 : rowScale.Length - 1;
			if (lastScale != lastRowScale)
			{
				layout = Control.GetChildAt(rowScale.Length - 1).LayoutParameters as aw.TableLayout.LayoutParams;
				layout.Weight = row == lastRowScale || rowScale[rowScale.Length - 1] ? 1f : 0f;
			}
		}

		public Size Spacing { get; set; }

		public Padding Padding
		{
			get { return Control.GetPadding(); }
			set { Control.SetPadding(value); }
		}

		public void Add(Control child, int x, int y)
		{
			var row = (aw.TableRow)Control.GetChildAt(y);

			var cell = (aw.FrameLayout)row.GetChildAt(x);
			cell.RemoveAllViews();
			var control = child.GetContainerView();
			if (control != null)
			{
				control.LayoutParameters = new av.ViewGroup.LayoutParams(av.ViewGroup.LayoutParams.MatchParent, av.ViewGroup.LayoutParams.MatchParent);
				cell.AddView(control);
			}
		}

		public void Move(Control child, int x, int y)
		{
			Remove(child);
			Add(child, x, y);
		}

		public void Remove(Control child)
		{
			var control = child.GetContainerView();
			var row = control.Parent as aw.TableRow;
			if (row != null && object.ReferenceEquals(row.Parent, Control))
			{
				var x = row.IndexOfChild(control);
				row.RemoveView(control);
				row.AddView(new av.View(aa.Application.Context), new aw.TableRow.LayoutParams(av.ViewGroup.LayoutParams.MatchParent, av.ViewGroup.LayoutParams.MatchParent) { Column = x });
			}
		}

		public void Update()
		{
			Control.ForceLayout();
		}
	}
}

