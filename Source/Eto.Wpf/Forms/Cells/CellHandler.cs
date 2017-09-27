using sw = System.Windows;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Wpf.Forms.Cells
{
	public interface ICellContainerHandler
	{
		sw.FrameworkElement SetupCell(ICellHandler cell, sw.FrameworkElement defaultContent);
		void FormatCell(ICellHandler cell, sw.FrameworkElement element, swc.DataGridCell datacell, object dataItem);
		void CellEdited(ICellHandler cell, sw.FrameworkElement element);
	}

	public interface ICellHandler : Cell.IHandler
	{
		ICellContainerHandler ContainerHandler { get; set; }
		swc.DataGridColumn Control { get; }
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

		public static bool IsControlInitialized(sw.DependencyObject obj) => (bool)obj.GetValue(CellProperties.ControlInitializedProperty);
		public static void SetControlInitialized(sw.DependencyObject obj, bool value) => obj.SetValue(CellProperties.ControlInitializedProperty, value);
		public static bool IsControlEditInitialized(sw.DependencyObject obj) => (bool)obj.GetValue(CellProperties.ControlEditInitializedProperty);
		public static void SetControlEditInitialized(sw.DependencyObject obj, bool value) => obj.SetValue(CellProperties.ControlEditInitializedProperty, value);

		swc.DataGridColumn ICellHandler.Control => Control;

		public void FormatCell(sw.FrameworkElement element, swc.DataGridCell cell, object dataItem)
		{
			ContainerHandler.FormatCell(this, element, cell, dataItem);
		}

		public sw.FrameworkElement SetupCell(sw.FrameworkElement defaultContent)
		{
			return ContainerHandler != null ? ContainerHandler.SetupCell(this, defaultContent) : defaultContent;
		}
	}
}