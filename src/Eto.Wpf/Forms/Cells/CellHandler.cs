namespace Eto.Wpf.Forms.Cells
{
	public interface ICellContainerHandler
	{
		Grid Grid { get; }
		sw.FrameworkElement SetupCell(ICellHandler handler, sw.FrameworkElement defaultContent, swc.DataGridCell cell);
		void FormatCell(ICellHandler cell, sw.FrameworkElement element, swc.DataGridCell datacell, object dataItem);
		void CellEdited(ICellHandler cell, sw.FrameworkElement element);
	}

	public interface ICellHandler : Cell.IHandler
	{
		ICellContainerHandler ContainerHandler { get; set; }
		swc.DataGridColumn Control { get; }
		void OnMouseDown(GridCellMouseEventArgs args, sw.DependencyObject hitTestResult, swc.DataGridCell cell);
		void OnMouseUp(GridCellMouseEventArgs args, sw.DependencyObject hitTestResult, swc.DataGridCell cell);
	}

	static class CellProperties
	{
		public static sw.DependencyProperty ControlInitializedProperty = sw.DependencyProperty.Register("EtoControlInitialized", typeof(bool), typeof(sw.FrameworkElement), new sw.PropertyMetadata(false));
		public static sw.DependencyProperty ControlEditInitializedProperty = sw.DependencyProperty.Register("EtoControlEditInitialized", typeof(bool), typeof(sw.FrameworkElement), new sw.PropertyMetadata(false));
	}

	public abstract class CellHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, ICellHandler
		where TControl : swc.DataGridColumn
		where TWidget : Cell
	{
		public ICellContainerHandler ContainerHandler { get; set; }

		public Controls.IGridHandler GridHandler => ContainerHandler?.Grid?.Handler as Controls.IGridHandler;

		public static bool IsControlInitialized(sw.DependencyObject obj) => (bool)obj.GetValue(CellProperties.ControlInitializedProperty);
		public static void SetControlInitialized(sw.DependencyObject obj, bool value) => obj.SetValue(CellProperties.ControlInitializedProperty, value);
		public static bool IsControlEditInitialized(sw.DependencyObject obj) => (bool)obj.GetValue(CellProperties.ControlEditInitializedProperty);
		public static void SetControlEditInitialized(sw.DependencyObject obj, bool value) => obj.SetValue(CellProperties.ControlEditInitializedProperty, value);

		swc.DataGridColumn ICellHandler.Control => Control;

		public void FormatCell(sw.FrameworkElement element, swc.DataGridCell cell, object dataItem)
		{
			ContainerHandler.FormatCell(this, element, cell, dataItem);
		}

		public sw.FrameworkElement SetupCell(sw.FrameworkElement defaultContent, swc.DataGridCell cell)
		{
			return ContainerHandler != null ? ContainerHandler.SetupCell(this, defaultContent, cell) : defaultContent;
		}

		protected static T GetControl<T>(swc.DataGridCell cell)
			where T: sw.DependencyObject
		{
			if (cell == null)
				return null;

			var content = cell.Content;
			if (content is T ctl)
				return ctl;

			if (content is sw.DependencyObject dp)
				return dp.FindChild<T>();

			return null;
		}

		public virtual void OnMouseDown(GridCellMouseEventArgs args, sw.DependencyObject hitTestResult, swc.DataGridCell cell)
		{
		}

		public virtual void OnMouseUp(GridCellMouseEventArgs args, sw.DependencyObject hitTestResult, swc.DataGridCell cell)
		{
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			Widget.Properties.Set(swc.DataGridColumn.ActualWidthProperty, PropertyChangeNotifier.Register(swc.DataGridColumn.ActualWidthProperty, HandleWidthChanged, Control));
		}

		private void HandleWidthChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
		{
			GridHandler?.OnColumnWidthChanged(ContainerHandler as Controls.GridColumnHandler);
		}
	}
}