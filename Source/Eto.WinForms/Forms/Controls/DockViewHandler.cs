using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using sd = System.Drawing;
using swf = System.Windows.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class DockViewHandler : WindowsControl<swf.ToolStripContainer, DockView, DockView.ICallback>, DockView.IHandler
	{
		Collection<Tuple<Control, swf.ToolStrip>> toolStripReference = new Collection<Tuple<Control, swf.ToolStrip>>();
		Control content;
		swf.Panel contentHolder;
		static readonly object minimumSizeKey = new object();

		public DockViewHandler()
		{
			this.Control = new swf.ToolStripContainer
			{
				LeftToolStripPanelVisible = true,
				Visible = true
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

		public void AddControl(Control control, int index)
		{
			if (Widget.Loaded)
				SuspendLayout();

			swf.ToolStrip toolStrip = control.ControlObject as swf.ToolStrip;
			
			if (toolStrip == null)
			{
				swf.ToolStripControlHost toolStripControlHost = new swf.ToolStripControlHost((swf.Control)control.ControlObject);
				toolStrip = new swf.ToolStrip(toolStripControlHost);
				toolStrip.Dock = (swf.DockStyle)Enum.Parse(typeof(swf.DockStyle), control.Dock.ToString());
			}
			
			switch (control.Dock)
			{
				case ControlDock.Bottom:
					Control.BottomToolStripPanel.Join(toolStrip, control.Location.ToSD());
					break;
				case ControlDock.Left:
					Control.LeftToolStripPanel.Join(toolStrip, control.Location.ToSD());
					break;
				case ControlDock.Right:
					Control.RightToolStripPanel.Join(toolStrip, control.Location.ToSD());
					break;
				case ControlDock.Top:
				default:
					Control.TopToolStripPanel.Join(toolStrip, control.Location.ToSD());
					break;
			}

			toolStripReference.Add(new Tuple<Control, swf.ToolStrip>(control, toolStrip));

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
					var contentHandler = content.GetWindowsHandler();
					contentHandler.BeforeAddControl(Widget.Loaded);
					SetContent(contentHandler.ContainerControl);
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

		public void RemoveControl(Control control)
		{
			if (Widget.Loaded)
				SuspendLayout();

			var reference = toolStripReference.Where(r => r.Item1 == control).SingleOrDefault();
			
			switch (control.Dock)
			{
				case ControlDock.Bottom:
					this.Control.BottomToolStripPanel.Controls.Remove(reference.Item2);
					break;
				case ControlDock.Left:
					this.Control.LeftToolStripPanel.Controls.Remove(reference.Item2);
					break;
				case ControlDock.Right:
					this.Control.RightToolStripPanel.Controls.Remove(reference.Item2);
					break;
				case ControlDock.Top:
				default:
					this.Control.TopToolStripPanel.Controls.Remove(reference.Item2);
					break;
			}

			toolStripReference.Remove(reference);

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

