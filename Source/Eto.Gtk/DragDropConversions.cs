using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eto = Eto.Forms;

namespace Eto.GtkSharp
{
	public static class DragDropConversions
	{
		public static Gdk.DragAction ToPlatformDropAction(this eto.DragDropAction dragAction)
		{
			Gdk.DragAction action = (Gdk.DragAction)0;

			if ((dragAction & eto.DragDropAction.Copy) != 0)
			{
				action |= Gdk.DragAction.Copy;
			}

			if ((dragAction & eto.DragDropAction.Move) != 0)
			{
				action |= Gdk.DragAction.Move;
			}

			if ((dragAction & eto.DragDropAction.Link) != 0)
			{
				action |= Gdk.DragAction.Link;
			}

			return action;
		}

		public static eto.DragDropAction ToEtoDropAction(this Gdk.DragAction dragAction)
		{
			eto.DragDropAction action = (eto.DragDropAction)0;

			if ((dragAction & Gdk.DragAction.Copy) != 0)
			{
				action |= eto.DragDropAction.Copy;
			}

			if ((dragAction & Gdk.DragAction.Move) != 0)
			{
				action |= eto.DragDropAction.Move;
			}

			if ((dragAction & Gdk.DragAction.Link) != 0)
			{
				action |= eto.DragDropAction.Link;
			}

			return action;
		}
	}
}
