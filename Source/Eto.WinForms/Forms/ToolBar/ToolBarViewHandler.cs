using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using sd = System.Drawing;
using swf = System.Windows.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class ToolBarViewHandler : WindowsControl<swf.ToolStripContainer, ToolBarView, ToolBarView.ICallback>, ToolBarView.IHandler
	{
		Control content;
		swf.Panel contentHolder;
		static readonly object minimumSizeKey = new object();

		public ToolBarViewHandler()
		{
			this.Control = new swf.ToolStripContainer
			{
				LeftToolStripPanelVisible = true,
				Visible = true,
				Width = 50,
				Height = 100
			};

			contentHolder = new swf.Panel
			{
				Font = sd.SystemFonts.DefaultFont,
				Dock = swf.DockStyle.Fill,
				ForeColor = sd.SystemColors.ControlText,
				AutoSize = true,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink
			};
			this.Control.ContentPanel.Controls.Add(contentHolder);
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		public void AddToolBar(Eto.Forms.ToolBar toolBar, int index)
		{
			if (Widget.Loaded)
				SuspendLayout();

			switch(toolBar.Dock)
			{
				case ToolBarDock.Bottom:
					this.Control.BottomToolStripPanel.Controls.Add((swf.ToolStrip)toolBar.ControlObject);
					break;
				case ToolBarDock.Left:
					this.Control.LeftToolStripPanel.Controls.Add((swf.ToolStrip)toolBar.ControlObject);
					break;
				case ToolBarDock.Right:
					this.Control.RightToolStripPanel.Controls.Add((swf.ToolStrip)toolBar.ControlObject);
					break;
				case ToolBarDock.Top:
				default:
					this.Control.TopToolStripPanel.Controls.Add((swf.ToolStrip)toolBar.ControlObject);
					break;
			}

			if (Widget.Loaded)
				ResumeLayout();
		}

		public void Clear()
		{
			if (Widget.Loaded)
				SuspendLayout();

			this.Control.BottomToolStripPanel.Controls.Clear();
			this.Control.LeftToolStripPanel.Controls.Clear();
			this.Control.RightToolStripPanel.Controls.Clear();
			this.Control.TopToolStripPanel.Controls.Clear();

			if (Widget.Loaded)
				ResumeLayout();
		}


		public Control Content
		{
			get { return content; }
			set
			{
				if (Widget.Loaded)
					SuspendLayout();
				
				if (content != null)
				{
					var contentHandler = content.GetWindowsHandler();
					contentHandler.SetScale(false, false);
					contentHandler.ContainerControl.Parent = null;
				}

				content = value;

				if (content != null)
				{
					SetContentScale(XScale, YScale);
					var childHandler = content.GetWindowsHandler();
					childHandler.BeforeAddControl(Widget.Loaded);
					SetContent(childHandler.ContainerControl);
				}
				
				if (Widget.Loaded)
					ResumeLayout();
			}
		}

		public Size MinimumSize
		{
			get { return Widget.Properties.Get<Size?>(minimumSizeKey) ?? Size.Empty; }
			set
			{
				if (value != MinimumSize)
				{
					Widget.Properties[minimumSizeKey] = value;
					SetMinimumSize(useCache: true);
				}
			}
		}
		
		public virtual Padding Padding
		{
			get { return contentHolder.Padding.ToEto(); }
			set { contentHolder.Padding = value.ToSWF(); }
		}

		public void RemoveToolBar(Eto.Forms.ToolBar toolBar)
		{
			if (Widget.Loaded)
				SuspendLayout();

			switch (toolBar.Dock)
			{
				case ToolBarDock.Bottom:
					this.Control.BottomToolStripPanel.Controls.Remove((swf.ToolStrip)toolBar.ControlObject);
					break;
				case ToolBarDock.Left:
					this.Control.LeftToolStripPanel.Controls.Remove((swf.ToolStrip)toolBar.ControlObject);
					break;
				case ToolBarDock.Right:
					this.Control.RightToolStripPanel.Controls.Remove((swf.ToolStrip)toolBar.ControlObject);
					break;
				case ToolBarDock.Top:
				default:
					this.Control.TopToolStripPanel.Controls.Remove((swf.ToolStrip)toolBar.ControlObject);
					break;
			}

			if (Widget.Loaded)
				ResumeLayout();
		}


		protected virtual void SetContent(swf.Control contentControl)
		{
			contentControl.Dock = swf.DockStyle.Fill;
			contentHolder.Controls.Add(contentControl);
		}

		protected virtual void SetContentScale(bool xscale, bool yscale)
		{
			if (content != null)
				content.SetScale(xscale, yscale);
		}

		public bool RecurseToChildren
		{
			get { return true; }
		}
	}
}

