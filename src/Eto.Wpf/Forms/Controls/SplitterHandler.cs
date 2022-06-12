using System;
using System.Linq;
using Eto.Forms;
using swm = System.Windows.Media;
using sw = System.Windows;
using swc = System.Windows.Controls;
using Eto.Drawing;
using System.ComponentModel;
using System.Windows.Controls.Primitives;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoGrid : swc.Grid, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }
		
		internal sw.Size BaseMeasureOverride(sw.Size constraint) => base.MeasureOverride(constraint);

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}
	
	public class SplitterHandler : WpfContainer<EtoGrid, Splitter, Splitter.ICallback>, Splitter.IHandler
	{
		readonly swc.GridSplitter splitter;
		readonly swc.DockPanel pane1;
		readonly swc.DockPanel pane2;
		Orientation orientation;
		SplitterFixedPanel fixedPanel;
		int? position;
		int splitterWidth = 5;
		double relative = double.NaN;
		bool panel1Visible, panel2Visible;
		int panel1MinimumSize, panel2MinimumSize;
		Control panel1, panel2;
		PropertyChangeNotifier panel1VisibilityNotifier;
		PropertyChangeNotifier panel2VisibilityNotifier;


		public override sw.Size MeasureOverride(sw.Size constraint, Func<sw.Size, sw.Size> measure)
		{
			return base.MeasureOverride(constraint, BetterMeasure);
		}

		private sw.Size BetterMeasure(sw.Size constraint)
		{
			// call base measure override so everything gets set up correctly
			var size = Control.BaseMeasureOverride(constraint);
			
			if (FixedPanel == SplitterFixedPanel.Panel2)
			{
				// provide our own measuring when FixedPanel is Panel2, as WPF gets it wrong sometimes
				var panel1Size = panel1?.GetContainerControl()?.DesiredSize ?? sw.Size.Empty;
				var panel2Size = panel2?.GetContainerControl()?.DesiredSize ?? sw.Size.Empty;
				if (Orientation == Orientation.Horizontal)
					size.Width = Math.Max(size.Width, panel1Size.Width + panel2Size.Width + SplitterWidth);
				else
					size.Height = Math.Max(size.Height, panel1Size.Height + panel2Size.Height + SplitterWidth); 
			}
			return size;
		}

		public SplitterHandler()
		{
			Control = new EtoGrid { Handler = this };

			Control.ColumnDefinitions.Add(new swc.ColumnDefinition());
			Control.ColumnDefinitions.Add(new swc.ColumnDefinition() { Width = sw.GridLength.Auto });
			Control.ColumnDefinitions.Add(new swc.ColumnDefinition());

			Control.RowDefinitions.Add(new swc.RowDefinition());
			Control.RowDefinitions.Add(new swc.RowDefinition { Height = sw.GridLength.Auto });
			Control.RowDefinitions.Add(new swc.RowDefinition());

			splitter = new swc.GridSplitter
			{
				//Background = sw.SystemColors.ControlLightLightBrush,
				ResizeBehavior = swc.GridResizeBehavior.PreviousAndNext
			};
			pane1 = new swc.DockPanel { LastChildFill = true };
			pane2 = new swc.DockPanel { LastChildFill = true };


			Control.Children.Add(pane1);
			Control.Children.Add(splitter);
			Control.Children.Add(pane2);

			UpdateOrientation();
			Control.Loaded += Control_Loaded;
			Control.SizeChanged += (sender, e) => ResetMinMax();

			panel1VisibilityNotifier = new PropertyChangeNotifier(sw.UIElement.VisibilityProperty);
			panel1VisibilityNotifier.ValueChanged += HandlePanel1IsVisibleChanged;

			panel2VisibilityNotifier = new PropertyChangeNotifier(sw.UIElement.VisibilityProperty);
			panel2VisibilityNotifier.ValueChanged += HandlePanel2IsVisibleChanged;
		}

		private void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			// only set on initial load, subsequent loads should keep the last position
			Control.Loaded -= Control_Loaded;
			ResetMinMax();
			SetInitialPosition();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Splitter.PositionChangedEvent:
					PositionChangedEnabled++;
					AttachPropertyChanged(swc.RowDefinition.HeightProperty, HandlePositionChanged, Control.RowDefinitions[0], new object());
					AttachPropertyChanged(swc.RowDefinition.HeightProperty, HandlePositionChanged, Control.RowDefinitions[2], new object());
					AttachPropertyChanged(swc.ColumnDefinition.WidthProperty, HandlePositionChanged, Control.ColumnDefinitions[0], new object());
					AttachPropertyChanged(swc.ColumnDefinition.WidthProperty, HandlePositionChanged, Control.ColumnDefinitions[2], new object());
					PositionChangedEnabled--;
					break;
				case Splitter.PositionChangingEvent:
					splitter.DragStarted += splitter_DragStarted;
					splitter.DragCompleted += splitter_DragCompleted;
					splitter.DragDelta += splitter_DragDelta;
					break;
				case Splitter.PositionChangeStartedEvent:
				case Splitter.PositionChangeCompletedEvent:
					HandleEvent(Splitter.PositionChangingEvent);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void splitter_DragDelta(object sender, DragDeltaEventArgs e)
		{
			e.Handled = HandlePositionChanging(e.HorizontalChange, e.VerticalChange);
		}

		private void splitter_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			e.Handled = HandlePositionChanging(e.HorizontalChange, e.VerticalChange);
			Callback.OnPositionChangeCompleted(Widget, EventArgs.Empty);
		}

		private void splitter_DragStarted(object sender, DragStartedEventArgs e)
		{
			Callback.OnPositionChangeStarted(Widget, EventArgs.Empty);
			HandlePositionChanging(e.HorizontalOffset, e.VerticalOffset);
		}

		void HandlePositionChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
		{
			if (PositionChangedEnabled > 0)
				return;
			// we use actual width vs. width itself, so we have to use the value passed in
			var old = position;
			var pos = (sw.GridLength)e.NewValue;
			if (pos.GridUnitType != sw.GridUnitType.Pixel)
				return;
				
			var newPosition = (int)Math.Round(pos.Value);
			
			if (sender is PropertyChangeNotifier notifier && 
				(notifier.PropertySource == Control.ColumnDefinitions[2] || notifier.PropertySource == Control.RowDefinitions[2]))
			{
				// invert position, we are resizing the second pane, not the first
				newPosition = (int)Math.Round(GetAvailableSize() - newPosition);
			}
			if (newPosition != Position)
			{
				var args = new SplitterPositionChangingEventArgs(newPosition);
				// Console.WriteLine($"Source: PositionChanged: {newPosition}");
				Callback.OnPositionChanging(Widget, args);
				if (args.Cancel)
				{
					return;
				}
			}
			position = newPosition;
			Callback.OnPositionChanged(Widget, EventArgs.Empty);
			position = old;
		}

		bool HandlePositionChanging(double horizontal, double vertical, [System.Runtime.CompilerServices.CallerMemberName] string source = null)
		{
			var position = DoublePosition;
			if (orientation == Orientation.Horizontal)
				position += horizontal;
			else
				position += vertical;
				
			// restrict to control size
			position = Math.Max(0, Math.Min(GetAvailableSize(false), position));
			
			var intPosition = (int)Math.Round(position);

			if (intPosition == Position)
				return false;
			// Console.WriteLine($"Source: {source}, {intPosition}");
			var args = new SplitterPositionChangingEventArgs(intPosition);
			Callback.OnPositionChanging(Widget, args);
			if (args.Cancel)
				return true;
				
			if (fixedPanel == SplitterFixedPanel.None)
			{
				Callback.OnPositionChanged(Widget, EventArgs.Empty);
			}
			
			return false;
		}

		static object PositionChangedEnabled_Key = new object();
		int PositionChangedEnabled
		{
			get { return Widget.Properties.Get(PositionChangedEnabled_Key, 0); }
			set { Widget.Properties.Set(PositionChangedEnabled_Key, value, 0); }
		}

		void SetInitialPosition()
		{
			panel1Visible = panel1?.Visible ?? false;
			panel2Visible = panel2?.Visible ?? false;

			// controls should be stretched to fit panels
			SetStretch(panel1);
			SetStretch(panel2);
			UpdateColumnSizing(false);

			if (position != null)
			{
				var pos = position.Value;
				if (fixedPanel != SplitterFixedPanel.Panel1)
				{
					var size = GetAvailableSize(false);
					var want = GetAvailableSize(true);
					if (size != want)
					{
						if (FixedPanel == SplitterFixedPanel.Panel2)
							pos += (int)Math.Round(size - want);
						else
						{
							SetRelative(pos / (double)want);
							return;
						}
					}

				}
				SetPosition(pos);
			}
			else if (!double.IsNaN(relative))
			{
				SetRelative(relative);
			}
			else if (fixedPanel == SplitterFixedPanel.Panel1)
			{
				var size1 = panel1?.GetPreferredSize() ?? SizeF.Empty;
				SetRelative(orientation == Orientation.Horizontal ? size1.Width : size1.Height);
			}
			else if (fixedPanel == SplitterFixedPanel.Panel2)
			{
				var size2 = panel2?.GetPreferredSize() ?? SizeF.Empty;
				SetRelative(orientation == Orientation.Horizontal ? size2.Width : size2.Height);
			}
			else
			{
				var size1 = panel1?.GetPreferredSize() ?? SizeF.Empty;
				var size2 = panel2?.GetPreferredSize() ?? SizeF.Empty;
				SetRelative(orientation == Orientation.Horizontal
					? size1.Width / (double)(size1.Width + size2.Width)
					: size1.Height / (double)(size1.Height + size2.Height));
			}
		}

		static void SetStretch(Control panel)
		{
			if (panel != null)
			{
				var control = panel.GetContainerControl();
				control.VerticalAlignment = sw.VerticalAlignment.Stretch;
				control.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
				/*
				((sw.FrameworkElement)panel.ControlObject).Width = double.NaN;
				((sw.FrameworkElement)panel.ControlObject).Height = double.NaN;
				 * */
			}
		}

		void UpdateOrientation()
		{
			if (orientation == Orientation.Horizontal)
			{

				splitter.ResizeDirection = swc.GridResizeDirection.Columns;
				splitter.HorizontalAlignment = sw.HorizontalAlignment.Left;
				splitter.VerticalAlignment = sw.VerticalAlignment.Stretch;

				splitter.SetValue(swc.Grid.RowSpanProperty, 3);
				pane1.SetValue(swc.Grid.RowSpanProperty, 3);
				pane2.SetValue(swc.Grid.RowSpanProperty, 3);

				splitter.SetValue(swc.Grid.ColumnSpanProperty, 1);
				pane1.SetValue(swc.Grid.ColumnSpanProperty, 1);
				pane2.SetValue(swc.Grid.ColumnSpanProperty, 1);

				swc.Grid.SetColumn(splitter, 1);
				swc.Grid.SetRow(splitter, 0);
				swc.Grid.SetColumn(pane2, 2);
				swc.Grid.SetRow(pane2, 0);

				splitter.Width = splitterWidth;
				splitter.Height = double.NaN;
			}
			else
			{
				splitter.ResizeDirection = swc.GridResizeDirection.Rows;
				splitter.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
				splitter.VerticalAlignment = sw.VerticalAlignment.Top;
				pane2.VerticalAlignment = sw.VerticalAlignment.Stretch;

				splitter.SetValue(swc.Grid.RowSpanProperty, 1);
				pane1.SetValue(swc.Grid.RowSpanProperty, 1);
				pane2.SetValue(swc.Grid.RowSpanProperty, 1);

				splitter.SetValue(swc.Grid.ColumnSpanProperty, 3);
				pane1.SetValue(swc.Grid.ColumnSpanProperty, 3);
				pane2.SetValue(swc.Grid.ColumnSpanProperty, 3);

				swc.Grid.SetColumn(splitter, 0);
				swc.Grid.SetRow(splitter, 1);
				swc.Grid.SetColumn(pane2, 0);
				swc.Grid.SetRow(pane2, 2);

				splitter.Width = double.NaN;
				splitter.Height = splitterWidth;
			}
			UpdateColumnSizing(position.HasValue || !double.IsNaN(relative));
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		void UpdateColumnSizing(bool updatePosition)
		{
			if (updatePosition && position == null && double.IsNaN(relative))
				UpdateRelative();

			//SetLength(1, sw.GridLength.Auto);
			switch (FixedPanel)
			{
				case SplitterFixedPanel.Panel1:
					SetLength(0, new sw.GridLength(1, sw.GridUnitType.Star));
					break;
				case SplitterFixedPanel.Panel2:
					SetLength(0, new sw.GridLength(1, sw.GridUnitType.Star));
					break;
				case SplitterFixedPanel.None:
					SetLength(0, new sw.GridLength(1, sw.GridUnitType.Star));
					SetLength(2, new sw.GridLength(1, sw.GridUnitType.Star));
					break;
			}

			if (updatePosition)
			{
				SetPositionOrRelative();
			}
		}

		void SetPositionOrRelative()
		{
			if (position.HasValue)
				SetPosition(position.Value);
			else if (!double.IsNaN(relative))
				SetRelative(relative);
			else
				SetHiddenPanels();
		}

		public Orientation Orientation
		{
			get
			{
				switch (splitter.ResizeDirection)
				{
					case swc.GridResizeDirection.Columns:
						return Orientation.Horizontal;
					case swc.GridResizeDirection.Rows:
						return Orientation.Vertical;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				orientation = value;
				UpdateOrientation();
			}
		}

		public SplitterFixedPanel FixedPanel
		{
			get { return fixedPanel; }
			set
			{
				if (Control.IsLoaded)
				{
					if (HasHiddenPanels)
						UpdateRelativePosition(value);
					else
						position = Position;
				}
				else if (WasLoaded)
					UpdateRelativePosition(value);

				fixedPanel = value;
				
				if (fixedPanel == SplitterFixedPanel.None)
				{
					// positionchanged events get triggered here when the fixed panel is none
					HandleEvent(Splitter.PositionChangingEvent);
				}

				UpdateColumnSizing(Control.IsLoaded);
			}
		}

		private void UpdateRelativePosition(SplitterFixedPanel newFixedPanel)
		{
			if (double.IsNaN(relative))
				return;

			// translate relative position from old fixed panel to new fixed panel
			var width = orientation == Orientation.Horizontal ? Control.ActualWidth : Control.ActualHeight;
			switch (fixedPanel)
			{
				case SplitterFixedPanel.Panel1:
					switch (newFixedPanel)
					{
						case SplitterFixedPanel.Panel2:
							relative = width - relative - SplitterWidth;
							break;
						case SplitterFixedPanel.None:
							relative = relative / width;
							break;
					}
					break;
				case SplitterFixedPanel.Panel2:
					switch (newFixedPanel)
					{
						case SplitterFixedPanel.Panel1:
							relative = width - relative - SplitterWidth;
							break;
						case SplitterFixedPanel.None:
							relative = (width - relative) / width;
							break;
					}
					break;
				case SplitterFixedPanel.None:
					switch (newFixedPanel)
					{
						case SplitterFixedPanel.Panel1:
							relative = width * relative;
							break;
						case SplitterFixedPanel.Panel2:
							relative = width - (width * relative);
							break;
					}
					break;
			}
		}

		double DoublePosition
		{
			get
			{
				if (position != null)
					return position.Value;
				if (!Control.IsLoaded && !WasLoaded)
					return 0;
				if (splitter.ResizeDirection == swc.GridResizeDirection.Columns)
					return Control.ColumnDefinitions[0].ActualWidth;
				return Control.RowDefinitions[0].ActualHeight;
			}
		}

		public int Position
		{
			get => (int)Math.Round(DoublePosition);
			set
			{
				SetPosition(value);
			}
		}

		public int SplitterWidth
		{
			get { return splitterWidth; }
			set
			{
				if (splitterWidth == value)
					return;
				splitterWidth = value;
				if (orientation == Orientation.Horizontal)
					splitter.Width = value;
				else
					splitter.Height = value;
			}
		}

		double GetAvailableSize()
		{
			return GetAvailableSize(!Control.IsLoaded);
		}

		double GetAvailableSize(bool desired)
		{
			if (desired)
			{
				var size = UserPreferredSize;
				var pick = Orientation == Orientation.Horizontal ? size.Width : size.Height;
				if (pick >= 0)
					return pick - splitterWidth;
			}
			var width = Orientation == Orientation.Horizontal ? Control.ActualWidth : Control.ActualHeight;
			if (double.IsNaN(width))
				width = Orientation == Orientation.Horizontal ? Control.Width : Control.Height;
			return width - splitterWidth;
		}

		void UpdateRelative()
		{
			var pos = Position;
			if (fixedPanel == SplitterFixedPanel.Panel1)
				relative = pos;
			else
			{
				var sz = GetAvailableSize();
				if (fixedPanel == SplitterFixedPanel.Panel2)
					relative = sz <= 0 ? 0 : sz - pos;
				else
					relative = sz <= 0 ? 0.5 : pos / (double)sz;
			}
		}

		public double RelativePosition
		{
			get
			{
				if (double.IsNaN(relative) || Widget.Loaded)
					UpdateRelative();
				return relative;
			}
			set
			{
				if (relative == value)
					return;
				SetRelative(value);
				Callback.OnPositionChanged(Widget, EventArgs.Empty);
			}
		}

		double GetSize(int panel)
		{
			if (splitter.ResizeDirection == swc.GridResizeDirection.Columns)
				return Control.ColumnDefinitions[panel].ActualWidth;
			else
				return Control.RowDefinitions[panel].ActualHeight;
		}


		sw.GridLength GetLength(int panel)
		{
			if (splitter.ResizeDirection == swc.GridResizeDirection.Columns)
				return Control.ColumnDefinitions[panel].Width;
			else
				return Control.RowDefinitions[panel].Height;
		}

		void SetLength(int panel, sw.GridLength value)
		{
			if (splitter.ResizeDirection == swc.GridResizeDirection.Columns)
				Control.ColumnDefinitions[panel].Width = value;
			else
				Control.RowDefinitions[panel].Height = value;
		}

		bool HasHiddenPanels
		{
			get { return panel1 == null || !panel1.Visible || panel2 == null || !panel2.Visible; }
		}

		bool SetHiddenPanels()
		{
			if (!Widget.Loaded)
				return false;
			if (panel1 == null || !panel1.Visible)
			{
				SetLength(0, new sw.GridLength(0, sw.GridUnitType.Pixel));
				SetLength(1, new sw.GridLength(0, sw.GridUnitType.Pixel));
				SetLength(2, new sw.GridLength(1, sw.GridUnitType.Star));
				return true;
			}
			if (panel2 == null || !panel2.Visible)
			{
				SetLength(0, new sw.GridLength(1, sw.GridUnitType.Star));
				SetLength(1, new sw.GridLength(0, sw.GridUnitType.Pixel));
				SetLength(2, new sw.GridLength(0, sw.GridUnitType.Pixel));
				return true;
			}
			return false;
		}

		void SetPosition(int newPosition)
		{
			if (SetHiddenPanels())
				return;
			SetLength(1, sw.GridLength.Auto);
			if (!Control.IsLoaded)
			{
				position = newPosition;
				relative = double.NaN;
				SetLength(0, new sw.GridLength(Math.Max(0, newPosition), sw.GridUnitType.Pixel));
				SetLength(2, new sw.GridLength(1, sw.GridUnitType.Star));
				return;
			}

			position = null;
			var size = GetAvailableSize(false);
			relative = fixedPanel == SplitterFixedPanel.Panel1 ? Math.Max(0, newPosition)
				: fixedPanel == SplitterFixedPanel.Panel2 ? Math.Max(0, size - newPosition)
				: size <= 0 ? 0.5 : Math.Max(0.0, Math.Min(1.0, newPosition / (double)size));
			if (fixedPanel == SplitterFixedPanel.Panel1)
			{
				SetLength(0, new sw.GridLength(Math.Max(0, newPosition), sw.GridUnitType.Pixel));
				SetLength(2, new sw.GridLength(1, sw.GridUnitType.Star));
			}
			else if (fixedPanel == SplitterFixedPanel.Panel2)
			{
				SetLength(0, new sw.GridLength(1, sw.GridUnitType.Star));
				SetLength(2, new sw.GridLength(Math.Max(0, size - newPosition), sw.GridUnitType.Pixel));
			}
			else
			{
				SetLength(0, new sw.GridLength(Math.Max(0, newPosition), sw.GridUnitType.Star));
				SetLength(2, new sw.GridLength(Math.Max(0, size - newPosition), sw.GridUnitType.Star));
			}
		}

		void SetRelative(double newRelative)
		{
			if (SetHiddenPanels())
				return;
			position = null;
			relative = newRelative;
			PositionChangedEnabled++;
			SetLength(1, sw.GridLength.Auto);
			if (fixedPanel == SplitterFixedPanel.Panel1)
			{
				SetLength(0, new sw.GridLength(Math.Max(0, relative), sw.GridUnitType.Pixel));
				SetLength(2, new sw.GridLength(1, sw.GridUnitType.Star));
			}
			else if (fixedPanel == SplitterFixedPanel.Panel2)
			{
				SetLength(0, new sw.GridLength(1, sw.GridUnitType.Star));
				SetLength(2, new sw.GridLength(Math.Max(0, relative), sw.GridUnitType.Pixel));
			}
			else
			{
				SetLength(0, new sw.GridLength(Math.Max(0, relative), sw.GridUnitType.Star));
				SetLength(2, new sw.GridLength(Math.Max(0, 1 - relative), sw.GridUnitType.Star));
			}
			PositionChangedEnabled--;
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			var control = panel1.GetWpfFrameworkElement();
			if (control != null)
				control.SetScale(true, true);
			control = panel2.GetWpfFrameworkElement();
			if (control != null)
				control.SetScale(true, true);
		}

		public Control Panel1
		{
			get { return panel1; }
			set
			{
				panel1VisibilityNotifier.PropertySource = null;

				panel1 = value;
				pane1.Children.Clear();
				if (panel1 != null)
				{
					var control = panel1.GetWpfFrameworkElement();
					SetStretch(panel1);
					if (Widget.Loaded)
						control.SetScale(true, true);

					pane1.Children.Add(control.ContainerControl);
					panel1VisibilityNotifier.PropertySource = control.ContainerControl;
					HandlePanelVisibleChanged(ref panel1Visible, panel1);
				}
				else
				{
					SetHiddenPanels();
				}
			}
		}
		
		public Control Panel2
		{
			get { return panel2; }
			set
			{
				panel2VisibilityNotifier.PropertySource = null;

				panel2 = value;
				pane2.Children.Clear();
				if (panel2 != null)
				{
					var control = panel2.GetWpfFrameworkElement();
					SetStretch(panel2);
					if (Widget.Loaded)
						control.SetScale(true, true);
					pane2.Children.Add(control.ContainerControl);

					panel2VisibilityNotifier.PropertySource = control.ContainerControl;
					HandlePanelVisibleChanged(ref panel2Visible, panel2);
				}
				else
				{
					SetHiddenPanels();
				}
			}
		}

		void HandlePanel2IsVisibleChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
		{
			HandlePanelVisibleChanged(ref panel2Visible, panel2);
		}

		void HandlePanel1IsVisibleChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
		{
			HandlePanelVisibleChanged(ref panel1Visible, panel1);
		}

		void HandlePanelVisibleChanged(ref bool isVisible, Control panel)
		{
			if ((Control.IsLoaded || WasLoaded) && isVisible != panel.Visible)
			{
				isVisible = panel.Visible;
				if (!panel.Visible)
				{
					position = null;
					if ((panel1 != null && panel1.Visible) || (panel2 != null && panel2.Visible))
						UpdateRelative();
					SetHiddenPanels();
				}
				else if (!double.IsNaN(relative))
				{
					SetRelative(relative);
				}
			}
		}

		public override void Remove(sw.FrameworkElement child)
		{
			if (pane1.Children.Contains(child))
			{
				panel1VisibilityNotifier.PropertySource = null;
				panel1 = null;
				pane1.Children.Remove(child);
			}
			else if (pane2.Children.Contains(child))
			{
				panel2VisibilityNotifier.PropertySource = null;
				panel2 = null;
				pane2.Children.Remove(child);
			}
		}

		static readonly object WasLoaded_Key = new object();

		bool WasLoaded
		{
			get { return Widget.Properties.Get<bool>(WasLoaded_Key); }
			set { Widget.Properties.Set(WasLoaded_Key, value); }
		}
		
		private void ResetMinMax()
		{
			if (Orientation == Orientation.Horizontal)
			{
				Control.ColumnDefinitions[0].MinWidth = panel1MinimumSize;
				Control.ColumnDefinitions[2].MinWidth = panel2MinimumSize;
				Control.RowDefinitions[0].MinHeight = 0;
				Control.RowDefinitions[2].MinHeight = 0;
			}
			else
			{
				Control.ColumnDefinitions[0].MinWidth = 0;
				Control.ColumnDefinitions[2].MinWidth = 0;
				Control.RowDefinitions[0].MinHeight = Panel1MinimumSize;
				Control.RowDefinitions[2].MinHeight = panel2MinimumSize;
			}

			if (Control.IsLoaded)
			{
				var available = this.GetAvailableSize();
				// need to set the max height of resizing panel otherwise it can grow outside of window
				if (Orientation == Orientation.Horizontal)
				{
					Control.ColumnDefinitions[0].MaxWidth = FixedPanel == SplitterFixedPanel.Panel1 ? Math.Max(0, available - Panel2MinimumSize) : double.PositiveInfinity;
					Control.ColumnDefinitions[2].MaxWidth = FixedPanel == SplitterFixedPanel.Panel2 ? Math.Max(0, available - Panel1MinimumSize) : double.PositiveInfinity;
					Control.RowDefinitions[0].MaxHeight = double.PositiveInfinity;
					Control.RowDefinitions[2].MaxHeight = double.PositiveInfinity;
				}
				else
				{
					Control.ColumnDefinitions[0].MaxWidth = double.PositiveInfinity;
					Control.ColumnDefinitions[2].MaxWidth = double.PositiveInfinity;
					Control.RowDefinitions[0].MaxHeight = FixedPanel == SplitterFixedPanel.Panel1 ? Math.Max(0, available - Panel2MinimumSize) : double.PositiveInfinity;
					Control.RowDefinitions[2].MaxHeight = FixedPanel == SplitterFixedPanel.Panel2 ? Math.Max(0, available - Panel1MinimumSize) : double.PositiveInfinity;
				}
			}
			else
			{
				// need to set the max height of panel 2 otherwise it will grow beyond the bounds
				Control.ColumnDefinitions[0].MaxWidth = double.PositiveInfinity;
				Control.ColumnDefinitions[2].MaxWidth = double.PositiveInfinity;
				Control.RowDefinitions[0].MaxHeight = double.PositiveInfinity;
				Control.RowDefinitions[2].MaxHeight = double.PositiveInfinity;
			}
		}

		public int Panel1MinimumSize
		{
			get { return panel1MinimumSize; }
			set
			{
				panel1MinimumSize = value;
				ResetMinMax();
			}
		}

		public int Panel2MinimumSize
		{
			get { return panel2MinimumSize; }
			set
			{
				panel2MinimumSize = value;
				ResetMinMax();
			}
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			WasLoaded = false;
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			WasLoaded = true;
			if (double.IsNaN(relative) || !HasHiddenPanels)
				UpdateRelative();
			position = null;
		}
	}
}