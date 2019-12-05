using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Wpf.Forms;
using sw = System.Windows;

namespace Eto.Wpf
{
	public static class DragDropConversions
	{
		public static sw.DragDropEffects ToWpf(this DragEffects action)
		{
			var resultAction = sw.DragDropEffects.None;
			
			if (action.HasFlag(DragEffects.Copy))
				resultAction |= sw.DragDropEffects.Copy;
			
			if (action.HasFlag(DragEffects.Move))
				resultAction |= sw.DragDropEffects.Move;

			if (action.HasFlag(DragEffects.Link))
				resultAction |= sw.DragDropEffects.Link;

			return resultAction;
		}

		public static DragEffects ToEto(this sw.DragDropEffects effects)
		{
			var action = DragEffects.None;

			if (effects.HasFlag(sw.DragDropEffects.Copy))
				action |= DragEffects.Copy;

			if (effects.HasFlag(sw.DragDropEffects.Move))
				action |= DragEffects.Move;

			if (effects.HasFlag(sw.DragDropEffects.Link))
				action |= DragEffects.Link;

			return action;
		}
		public static sw.DataObject ToWpf(this DataObject data) => DataObjectHandler.GetControl(data);

		public static DataObject ToEto(this sw.IDataObject data) => new DataObject(new DataObjectHandler(data));

		public static DataObject ToEto(this sw.DataObject data) => new DataObject(new DataObjectHandler(data));
	}
}
