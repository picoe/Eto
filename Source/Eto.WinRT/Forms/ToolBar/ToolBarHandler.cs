using System;
using Eto.Forms;
using swc = Windows.UI.Xaml.Controls;
using swi = Windows.UI.Xaml.Input;
using swm = Windows.UI.Xaml.Media;

namespace Eto.WinRT.Forms.ToolBar
{
	/// <summary>
	/// Control to hold a tool bar which can be displayed via <see cref="ToolBarView"/> control
	/// </summary>
	/// <copyright>(c) 2015 by Nicolas Pöhlmann</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ToolBarHandler : WpfControl<swc.CommandBar, Eto.Forms.ToolBar, Eto.Forms.ToolBar.ICallback>, Eto.Forms.ToolBar.IHandler
	{
		public ToolBarHandler()
		{
			this.Control = new swc.CommandBar
			{
				FontFamily = new swm.FontFamily("Segoe UI")
			};
		}

		public void AddButton(ToolItem button, int index)
		{
			Control.PrimaryCommands.Add((swc.AppBarButton)button.ControlObject);
		}

		public void RemoveButton(ToolItem button)
		{
			Control.PrimaryCommands.Remove((swc.AppBarButton)button.ControlObject);
		}

		public void Clear()
		{
			Control.PrimaryCommands.Clear();
		}

		public ToolBarTextAlign TextAlign
		{
			get
			{
				return ToolBarTextAlign.Underneath;
			}
			set
			{
			}
		}
	}
}