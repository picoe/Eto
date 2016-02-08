using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WPF
using sw = System.Windows;
#elif WINFORMS
using sw = System.Windows.Forms;
#endif

using eto = Eto.Forms;
using System.Collections.Specialized;

namespace Eto
{
	public static class DragDropConversions
	{
		public static sw.DragDropEffects ToPlatformDropAction(this eto.DragDropAction action)
		{
			var resultAction = sw.DragDropEffects.None;

			if ((action & eto.DragDropAction.Copy) > 0)
			{
				resultAction |= sw.DragDropEffects.Copy;
			}

			if ((action & eto.DragDropAction.Move) > 0)
			{
				resultAction |= sw.DragDropEffects.Move;
			}

			if ((action & eto.DragDropAction.Link) > 0)
			{
				resultAction |= sw.DragDropEffects.Link;
			}

			return resultAction;
		}

		public static eto.DragDropAction ToEtoDropAction(this sw.DragDropEffects effects)
		{
			eto.DragDropAction action = (eto.DragDropAction)0;

			if ((effects & sw.DragDropEffects.Copy) != 0)
			{
				action |= eto.DragDropAction.Copy;
			}

			if ((effects & sw.DragDropEffects.Move) != 0)
			{
				action |= eto.DragDropAction.Move;
			}

			if ((effects & sw.DragDropEffects.Link) != 0)
			{
				action |= eto.DragDropAction.Link;
			}

			return action;
		}

		public static sw.DataObject ToPlatformDataObject(this eto.DragDropData data)
		{
			var resultData = new sw.DataObject();

			if (!string.IsNullOrEmpty(data.Text))
			{
				resultData.SetText(data.Text);
			}

			if (data.Uris != null && data.Uris.Count > 0)
			{
				var uris = new StringCollection();
				foreach (var uri in data.Uris)
				{
					uris.Add(uri.LocalPath);
				}

				resultData.SetFileDropList(uris);
			}

			return resultData;
		}

		public static eto.DragDropData ToEtoDataObject(this sw.DataObject data)
		{
			var resultData = new eto.DragDropData();
			resultData.Text = data.GetText();

			if (data.ContainsFileDropList())
			{
				resultData.Uris = new List<Uri>();

				foreach (var file in data.GetFileDropList())
				{
					resultData.Uris.Add(new Uri(file));
				}
			}

			return resultData;
		}
	}
}
