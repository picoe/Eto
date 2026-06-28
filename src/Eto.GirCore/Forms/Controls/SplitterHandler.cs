namespace Eto.GirCore.Forms.Controls
{
	public class SplitterHandler : GirContainer<Gtk.Paned, Splitter, Splitter.ICallback>, Splitter.IHandler
	{
		Control? panel1;
		Control? panel2;
		SplitterFixedPanel fixedPanel;
		int? position;
		double relativePosition = double.NaN;
		int panel1MinimumSize;
		int panel2MinimumSize;
		int suppressPositionEvents;

		public SplitterHandler()
		{
			Control = Gtk.Paned.New(Gtk.Orientation.Horizontal);
			GObject.Object.NotifySignal.Connect(Control, HandlePositionChanged, detail: "position");
		}

		void HandlePositionChanged(GObject.Object sender, GObject.Object.NotifySignalArgs args)
		{
			if (suppressPositionEvents > 0 || !Widget.Loaded)
				return;

			var newPosition = Control.Position;
			if (position == newPosition)
				return;

			var changing = new SplitterPositionChangingEventArgs(newPosition);
			Callback.OnPositionChangeStarted(Widget, EventArgs.Empty);
			Callback.OnPositionChanging(Widget, changing);
			if (changing.Cancel)
			{
				if (position.HasValue)
				{
					suppressPositionEvents++;
					Control.Position = position.Value;
					suppressPositionEvents--;
				}
				Callback.OnPositionChangeCompleted(Widget, EventArgs.Empty);
				return;
			}

			position = newPosition;
			UpdateRelativeFromPosition();
			Callback.OnPositionChanged(Widget, EventArgs.Empty);
			Callback.OnPositionChangeCompleted(Widget, EventArgs.Empty);
		}

		int GetAvailableSize()
		{
			var allocation = Control.GetAllocation();
			var size = Orientation == Orientation.Horizontal ? allocation.Width : allocation.Height;
			return Math.Max(0, size - SplitterWidth);
		}

		void ApplyFixedPanel()
		{
			Control.ResizeStartChild = fixedPanel != SplitterFixedPanel.Panel1;
			Control.ResizeEndChild = fixedPanel != SplitterFixedPanel.Panel2;
		}

		void UpdateRelativeFromPosition()
		{
			var currentPosition = Position;
			var size = GetAvailableSize();
			relativePosition = fixedPanel switch
			{
				SplitterFixedPanel.Panel1 => currentPosition,
				SplitterFixedPanel.Panel2 => Math.Max(0, size - currentPosition),
				_ => size <= 0 ? 0.5 : currentPosition / (double)size
			};
		}

		int RelativeToPosition(double value)
		{
			var size = GetAvailableSize();
			return fixedPanel switch
			{
				SplitterFixedPanel.Panel1 => Math.Max(0, (int)Math.Round(value)),
				SplitterFixedPanel.Panel2 => Math.Max(0, size - (int)Math.Round(value)),
				_ => size <= 0 ? 0 : Math.Max(0, Math.Min(size, (int)Math.Round(size * value)))
			};
		}

		void SetPanel(ref Control? field, Control? value, bool first)
		{
			field = value;
			var widget = value?.GetContainerWidget();
			if (first)
				Control.StartChild = widget ?? Gtk.Box.New(Gtk.Orientation.Vertical, 0);
			else
				Control.EndChild = widget ?? Gtk.Box.New(Gtk.Orientation.Vertical, 0);
		}

		public Orientation Orientation
		{
			get => Control.GetOrientation() == Gtk.Orientation.Horizontal ? Orientation.Horizontal : Orientation.Vertical;
			set => Control.SetOrientation(value == Orientation.Horizontal ? Gtk.Orientation.Horizontal : Gtk.Orientation.Vertical);
		}

		public SplitterFixedPanel FixedPanel
		{
			get => fixedPanel;
			set
			{
				fixedPanel = value;
				ApplyFixedPanel();
				UpdateRelativeFromPosition();
			}
		}

		public int Position
		{
			get => position ?? Control.Position;
			set
			{
				position = value;
				relativePosition = double.NaN;
				suppressPositionEvents++;
				Control.Position = value;
				suppressPositionEvents--;
			}
		}

		public double RelativePosition
		{
			get
			{
				if (double.IsNaN(relativePosition))
					UpdateRelativeFromPosition();
				return relativePosition;
			}
			set
			{
				relativePosition = value;
				Position = RelativeToPosition(value);
			}
		}

		public int SplitterWidth
		{
			get => Control.WideHandle ? 4 : 1;
			set => Control.WideHandle = value >= 4;
		}

		public Control? Panel1
		{
			get => panel1;
			set => SetPanel(ref panel1, value, true);
		}

		public Control? Panel2
		{
			get => panel2;
			set => SetPanel(ref panel2, value, false);
		}

		public int Panel1MinimumSize
		{
			get => panel1MinimumSize;
			set
			{
				panel1MinimumSize = value;
				Control.ShrinkStartChild = value <= 0;
			}
		}

		public int Panel2MinimumSize
		{
			get => panel2MinimumSize;
			set
			{
				panel2MinimumSize = value;
				Control.ShrinkEndChild = value <= 0;
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Splitter.PositionChangedEvent:
				case Splitter.PositionChangingEvent:
				case Splitter.PositionChangeStartedEvent:
				case Splitter.PositionChangeCompletedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
