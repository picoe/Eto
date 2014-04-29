using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms
{
	public class SplitterHandler : WindowsControl<swf.SplitContainer, Splitter>, ISplitter
	{
		Control panel1;
		Control panel2;
		int? position;

		public bool RecurseToChildren { get { return true; } }

		public override Size? DefaultSize
		{
			get
			{
				var size = base.DefaultSize;
				if (size == null && Control.AutoSize)
				{
					var min = Control.MinimumSize;
					Control.MinimumSize = sd.Size.Empty;
					size = Control.GetPreferredSize(sd.Size.Empty).ToEto();
					Control.MinimumSize = min;
				}
				return size;
			}
		}

		public class EtoSplitContainer : swf.SplitContainer
		{
			public SplitterHandler Handler { get; set; }

			public override sd.Size GetPreferredSize(sd.Size proposedSize)
			{
				var size = new sd.Size();
				var size1 = Handler.panel1.GetPreferredSize();
				var size2 = Handler.panel2.GetPreferredSize();
				if (Orientation == swf.Orientation.Vertical)
				{
					size1.Width = Handler.position ?? size1.Width;
					size.Width = size1.Width + size2.Width + SplitterWidth;
					size.Height = Math.Max(size1.Height, size2.Height);
				}
				else
				{
					size1.Height = Handler.position ?? size1.Height;
					size.Height = size1.Height + size2.Height + SplitterWidth;
					size.Width = Math.Max(size1.Width, size2.Width);
				}
				return size;
			}
		}

		public SplitterHandler()
		{
			Control = new EtoSplitContainer {
				Handler = this,
				AutoSize = true,
				FixedPanel = swf.FixedPanel.Panel1,
				Panel1MinSize = 0,
				Panel2MinSize = 0,
			};
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Splitter.PositionChangedEvent:
					// Hook SplitterMoving, not SplitterMoved,
					// because the latter fires even when the
					// splitter distance is changed programmatically.
					Control.SplitterMoving += (s, e) => Widget.OnPositionChanged(e);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public int Position
		{
			get { return Control.SplitterDistance; }
			set
			{
				position = value;
				if (Control.IsHandleCreated)
					SetPosition(value);
			}
		}

		void SetPosition(int position)
		{
			if (Control.Orientation == swf.Orientation.Vertical)
			{
				Control.SplitterDistance = Math.Max(0, Math.Min(Control.Width - Control.Panel2MinSize - 1, position));
			}
			else
			{
				Control.SplitterDistance = Math.Max(0, Math.Min(Control.Height - Control.Panel2MinSize - 1, position));
			}
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			Control.HandleCreated += (sender, ee) =>
			{
				if (position != null)
				{
					SetPosition(position.Value);
					position = null;
				}
				else
				{
					switch (FixedPanel)
					{
						case SplitterFixedPanel.Panel1:
							var size1 = panel1.GetPreferredSize();
							SetPosition(Control.Orientation == swf.Orientation.Vertical ? size1.Width : size1.Height);
							break;
						case SplitterFixedPanel.Panel2:
							var size2 = panel2.GetPreferredSize();
							int pos;
							if (Control.Orientation == swf.Orientation.Vertical)
								pos = Control.Width - size2.Width;
							else
								pos = Control.Height - size2.Height;
							SetPosition(pos);
							break;
					}
				}
			};
			Control.Panel1Collapsed = panel1 == null || !(panel1.GetWindowsHandler()).InternalVisible;
			Control.Panel2Collapsed = panel2 == null || !(panel2.GetWindowsHandler()).InternalVisible;
		}

		public SplitterFixedPanel FixedPanel
		{
			get
			{
				switch (Control.FixedPanel)
				{
					case swf.FixedPanel.None:
						return SplitterFixedPanel.None;
					case swf.FixedPanel.Panel1:
						return SplitterFixedPanel.Panel1;
					case swf.FixedPanel.Panel2:
						return SplitterFixedPanel.Panel2;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				switch (value)
				{
					case SplitterFixedPanel.None:
						Control.FixedPanel = swf.FixedPanel.None;
						break;
					case SplitterFixedPanel.Panel1:
						Control.FixedPanel = swf.FixedPanel.Panel1;
						break;
					case SplitterFixedPanel.Panel2:
						Control.FixedPanel = swf.FixedPanel.Panel2;
						break;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public SplitterOrientation Orientation
		{
			get
			{
				switch (Control.Orientation)
				{
					case swf.Orientation.Horizontal:
						return SplitterOrientation.Vertical;
					case swf.Orientation.Vertical:
						return SplitterOrientation.Horizontal;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				switch (value)
				{
					case SplitterOrientation.Horizontal:
						Control.Orientation = swf.Orientation.Vertical;
						break;
					case SplitterOrientation.Vertical:
						Control.Orientation = swf.Orientation.Horizontal;
						break;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public Control Panel1
		{
			get { return panel1; }
			set
			{
				if (panel1 != value)
				{
					Control.SuspendLayout();
					var old = panel1.GetContainerControl();
					if (old != null)
					{
						old.VisibleChanged -= c1_VisibleChanged;
					}
					panel1 = value;
					Control.Panel1.Controls.Clear();
					if (panel1 != null)
					{
						var control = panel1.GetContainerControl();
						control.Dock = swf.DockStyle.Fill;
						control.VisibleChanged += c1_VisibleChanged;
						Control.Panel1.Controls.Add(control);
					}
					if (Widget.Loaded)
						Control.Panel1Collapsed = panel1 == null || !(panel1.GetWindowsHandler()).InternalVisible;
					Control.ResumeLayout();
				}
			}
		}

		public Control Panel2
		{
			get { return panel2; }
			set
			{
				if (panel2 != value)
				{
					Control.SuspendLayout();
					var old = panel2.GetContainerControl();
					if (old != null)
					{
						old.VisibleChanged -= c2_VisibleChanged;
					}
					panel2 = value;
					Control.Panel2.Controls.Clear();
					if (panel2 != null)
					{
						var control = panel2.GetContainerControl();
						control.Dock = swf.DockStyle.Fill;
						control.VisibleChanged += c2_VisibleChanged;
						Control.Panel2.Controls.Add(control);
					}
					if (Widget.Loaded)
						Control.Panel2Collapsed = panel2 == null || !(panel2.GetWindowsHandler()).InternalVisible;
					Control.ResumeLayout();
				}
			}
		}

		void c1_VisibleChanged(object sender, EventArgs e)
		{
			if (panel1 != null && (panel1.GetWindowsHandler()).InternalVisible)
				Control.Panel1Collapsed = false;
			else
				Control.Panel1Collapsed = true;
		}

		void c2_VisibleChanged(object sender, EventArgs e)
		{
			if (panel2 != null && (panel2.GetWindowsHandler()).InternalVisible)
				Control.Panel2Collapsed = false;
			else
				Control.Panel2Collapsed = true;
		}
	}
}
