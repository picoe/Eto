using System;
using System.Windows;
using sw = System.Windows;

namespace Eto.Wpf.Forms
{
	public class WpfShellDropBehavior
	{
		sw.UIElement _element;
		bool IsDragEntered;
		bool IsDragLeaving;

		public WpfShellDropBehavior(sw.UIElement element)
		{
			_element = element;
			_element.PreviewDragEnter += Element_PreviewDragEnter;
			_element.DragEnter += Element_DragEnter;
			_element.PreviewDragOver += Element_PreviewDragOver;
			_element.DragOver += Element_DragOver;
			_element.PreviewDragLeave += Element_PreviewDragLeave;
			_element.PreviewDrop += Element_PreviewDrop;
			_element.Drop += Element_Drop;
		}
		public void Detatch()
		{
			_element.PreviewDragEnter -= Element_PreviewDragEnter;
			_element.DragEnter -= Element_DragEnter;
			_element.PreviewDragOver -= Element_PreviewDragOver;
			_element.DragOver -= Element_DragOver;
			_element.PreviewDragLeave -= Element_PreviewDragLeave;
			_element.PreviewDrop -= Element_PreviewDrop;
			_element.Drop -= Element_Drop;
		}

		private void Element_Drop(object sender, sw.DragEventArgs e)
		{
			var element = (UIElement)sender;
			e.Effects = sw.DragDropEffects.None;
			e.Handled = true;
		}

		private void Element_PreviewDrop(object sender, sw.DragEventArgs e)
		{
			var element = (UIElement)sender;
			if (IsDragEntered)
			{
				IsDragEntered = false;
				IsDragLeaving = false;
				try
				{
					sw.DropTargetHelper.Drop(e.Data, e.GetPosition(element), e.Effects);
				}
				catch
				{
					// ignore errors
				}
			}
		}

		private void Element_PreviewDragLeave(object sender, sw.DragEventArgs e)
		{
			var element = (UIElement)sender;
			if (IsDragEntered)
			{
				IsDragLeaving = true;
				element.Dispatcher.BeginInvoke(new Action(() =>
				{
					if (IsDragLeaving)
					{
						IsDragEntered = false;
						IsDragLeaving = false;
						try
						{
							sw.DropTargetHelper.DragLeave(e.Data);
						}
						catch
						{
							// ignore errors
						}
					}
				}));
			}
		}

		private void Element_DragOver(object sender, sw.DragEventArgs e)
		{
			e.Effects = sw.DragDropEffects.None;
			e.Handled = true;
		}

		private void Element_PreviewDragOver(object sender, sw.DragEventArgs e)
		{
			var element = (UIElement)sender;
			if (IsDragEntered)
			{
				try
				{
					sw.DropTargetHelper.DragOver(e.GetPosition(element), e.Effects);
				}
				catch
				{
					// ignore errors
				}
			}
		}

		private void Element_DragEnter(object sender, sw.DragEventArgs e)
		{
			e.Effects = sw.DragDropEffects.None;
			e.Handled = true;
		}

		private void Element_PreviewDragEnter(object sender, sw.DragEventArgs e)
		{
			var element = (UIElement)sender;
			IsDragLeaving = false;

			try
			{
				if (!IsDragEntered)
				{
					if (sw.DropTargetHelper.IsSupported(e.Data))
					{
						sw.DropTargetHelper.DragEnter(element.GetVisualParent<sw.Window>(), e.Data, e.GetPosition(element), e.Effects);
						IsDragEntered = true;
					}
				}
				else
				{
					sw.DropTargetHelper.DragOver(e.GetPosition(element), e.Effects);
				}
			}
			catch
			{
				// ignore errors
			}
		}
	}
}