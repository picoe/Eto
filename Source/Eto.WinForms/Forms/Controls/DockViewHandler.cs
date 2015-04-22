using System;
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
		Collection<Tuple<DockViewItem, swf.ToolStrip>> toolStripReference = new Collection<Tuple<DockViewItem, swf.ToolStrip>>();
		Control content;
		swf.Panel contentHolder;
		DockPosition dock = DockPosition.None;
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

		public void AddItem(DockViewItem item, int index)
		{
			if (Widget.Loaded)
				SuspendLayout();

			swf.ToolStrip toolStrip = item.Content.ControlObject as swf.ToolStrip;
			
			if (toolStrip == null)
			{
				swf.ToolStripControlHost toolStripControlHost = new swf.ToolStripControlHost((swf.Control)item.Content.ControlObject);
				toolStrip = new swf.ToolStrip(toolStripControlHost);
				toolStrip.Dock = (swf.DockStyle)Enum.Parse(typeof(swf.DockStyle), item.Dock.ToString());
			}
			else if (item.Content is ToolBarView)
			{
				toolStrip.Dock = (swf.DockStyle)Enum.Parse(typeof(swf.DockStyle), ((ToolBarView)item.Content).Dock.ToString());
			}

			switch (item.Dock)
			{
				case DockPosition.Bottom:
					Control.BottomToolStripPanel.Join(toolStrip, item.Position.ToSD());
					break;
				case DockPosition.Left:
					Control.LeftToolStripPanel.Join(toolStrip, item.Position.ToSD());
					break;
				case DockPosition.Right:
					Control.RightToolStripPanel.Join(toolStrip, item.Position.ToSD());
					break;
				case DockPosition.Top:
				default:
					Control.TopToolStripPanel.Join(toolStrip, item.Position.ToSD());
					break;
			}

			toolStripReference.Add(new Tuple<DockViewItem, swf.ToolStrip>(item, toolStrip));

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

		public DockPosition Dock
		{
			get { return dock; }
			set { dock = value; }
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

		public void RemoveItem(DockViewItem item)
		{
			if (Widget.Loaded)
				SuspendLayout();

			var reference = toolStripReference.Where(r => r.Item1 == item).SingleOrDefault();

			switch (item.Dock)
			{
				case DockPosition.Bottom:
					this.Control.BottomToolStripPanel.Controls.Remove(reference.Item2);
					break;
				case DockPosition.Left:
					this.Control.LeftToolStripPanel.Controls.Remove(reference.Item2);
					break;
				case DockPosition.Right:
					this.Control.RightToolStripPanel.Controls.Remove(reference.Item2);
					break;
				case DockPosition.Top:
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

