
// originally from : https://blogs.msdn.microsoft.com/adamroot/2008/02/19/shell-style-drag-and-drop-in-net-part-3/
// downloads link from: https://stackoverflow.com/questions/36239516/real-implementation-of-the-shell-style-drag-and-drop-in-net-wpf-and-winforms
// modified to properly set standard cursors without hard coding copy/link/move/etc labels.

#region WpfDragDropLib\WpfDragDropLibExtensions.cs

namespace DragDropLib
{
	using System;
	using System.Windows;

	static class WpfDragDropLibExtensions
	{
		/// <summary>
		/// Converts a System.Windows.Point value to a DragDropLib.Win32Point value.
		/// </summary>
		/// <param name="pt">Input value.</param>
		/// <returns>Converted value.</returns>
		public static Win32Point ToWin32Point(this Point pt)
		{
			Win32Point wpt = new Win32Point();
			wpt.x = (int)pt.X;
			wpt.y = (int)pt.Y;
			return wpt;
		}
	}
}

#endregion // WpfDragDropLib\WpfDragDropLibExtensions.cs

#region WpfDragDropLib\WpfDragSourceHelper.cs

namespace System.Windows
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Windows.Input;
	using System.Windows.Media.Imaging;
	using DragDropLib;
	using ComTypes = System.Runtime.InteropServices.ComTypes;

	/// <summary>
	/// Provides helper methods for working with the Shell drag image manager.
	/// </summary>
	public static class DragSourceHelper
	{
		#region DLL imports

		[DllImport("user32.dll")]
		private static extern void PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		#endregion // DLL imports

		#region Native constants

		// This is WM_USER + 3. The window controlled by the drag image manager
		// looks for this message (WM_USER + 2 seems to work, too) to invalidate
		// the drag image.
		private const uint WM_INVALIDATEDRAGIMAGE = 0x403;
		private const uint WM_SETDRAGCURSOR = 0x402;

		// CFSTR_DROPDESCRIPTION
		private const string DropDescriptionFormat = "DropDescription";

		// The drag image manager sets this flag to indicate if the current
		// drop target supports drag images.
		private const string IsShowingLayeredFormat = "IsShowingLayered";

		#endregion // Native constants

		/// <summary>
		/// Internally used to track information about the current drop description.
		/// </summary>
		[Flags]
		private enum DropDescriptionFlags
		{
			None = 0,
			IsDefault = 1,
			InvalidateRequired = 2
		}

		/// <summary>
		/// Keeps a cached drag source context, keyed on the drag source control.
		/// </summary>
		private static IDictionary<UIElement, DragSourceEntry> s_dataContext = new Dictionary<UIElement, DragSourceEntry>();

		/// <summary>
		/// Represents a drag source context entry.
		/// </summary>
		private class DragSourceEntry
		{
			public IDataObject data;
			public int adviseConnection = 0;

			public DragSourceEntry(IDataObject data)
			{
				this.data = data;
			}
		}

		/// <summary>
		/// Keeps drop description info for a data object.
		/// </summary>
		private static IDictionary<IDataObject, DropDescriptionFlags> s_dropDescriptions = new Dictionary<IDataObject, DropDescriptionFlags>();

		/// <summary>
		/// Creates a default DataObject with an internal COM callable implemetation of IDataObject.
		/// </summary>
		/// <returns>A new instance of System.Windows.Forms.IDataObject.</returns>
		public static IDataObject CreateDataObject()
		{
			return new System.Windows.DataObject(new DragDropLib.DataObject());
		}

		/// <summary>
		/// Creates a DataObject with an internal COM callable implementation of IDataObject.
		/// This override also sets the drag image to the specified Bitmap and sets a flag
		/// on the system IDragSourceHelper2 to allow drop descriptions.
		/// </summary>
		/// <param name="dragImage">A Bitmap from which to create the drag image.</param>
		/// <param name="cursorOffset">The drag image cursor offset.</param>
		/// <returns>A new instance of System.Windows.Forms.IDataObject.</returns>
		public static IDataObject CreateDataObject(BitmapSource dragImage, Point cursorOffset)
		{
			IDataObject data = CreateDataObject();
			AllowDropDescription(true);
			data.SetDragImage(dragImage, cursorOffset);
			return data;
		}

		/// <summary>
		/// Creates a DataObject with an internal COM callable implementation of IDataObject.
		/// This override also sets the drag image to a bitmap created from the specified
		/// Control instance's UI. It also sets a flag on the system IDragSourceHelper2 to
		/// allow drop descriptions.
		/// </summary>
		/// <param name="element">A UIElement to initialize the drag image from.</param>
		/// <param name="cursorOffset">The drag image cursor offset.</param>
		/// <returns>A new instance of System.Windows.Forms.IDataObject.</returns>
		public static IDataObject CreateDataObject(UIElement element, Point cursorOffset)
		{
			IDataObject data = CreateDataObject();
			AllowDropDescription(true);
			data.SetDragImage(element, cursorOffset);
			return data;
		}

		/// <summary>
		/// Registers a Control as a drag source and provides default implementations of
		/// GiveFeedback and QueryContinueDrag.
		/// </summary>
		/// <param name="dragSource">The drag source UIElement instance.</param>
		/// <param name="data">The DataObject associated to the drag source.</param>
		/// <remarks>Callers must call UnregisterDefaultDragSource when the drag and drop
		/// operation is complete to avoid memory leaks.</remarks>
		public static void RegisterDefaultDragSource(UIElement dragSource, IDataObject data)
		{
			// Cache the drag source and the associated data object
			DragSourceEntry entry = new DragSourceEntry(data);
			if (!s_dataContext.ContainsKey(dragSource))
				s_dataContext.Add(dragSource, entry);
			else
				s_dataContext[dragSource] = entry;

			// We need to listen for drop description changes. If a drop target
			// changes the drop description, we shouldn't provide a default one.
			entry.adviseConnection = ComTypes.ComDataObjectExtensions.Advise(((ComTypes.IDataObject)data), new AdviseSink(data), DropDescriptionFormat, 0);

			// Hook up the default drag source event handlers
			dragSource.GiveFeedback += new GiveFeedbackEventHandler(DefaultGiveFeedbackHandler);
			dragSource.QueryContinueDrag += new QueryContinueDragEventHandler(DefaultQueryContinueDragHandler);
		}

		/// <summary>
		/// Registers a Control as a drag source and provides default implementations of
		/// GiveFeedback and QueryContinueDrag. This override also handles the data object
		/// creation, including initialization of the drag image from the Control.
		/// </summary>
		/// <param name="dragSource">The drag source UIElement instance.</param>
		/// <param name="cursorOffset">The drag image cursor offset.</param>
		/// <returns>The created data object.</returns>
		/// <remarks>Callers must call UnregisterDefaultDragSource when the drag and drop
		/// operation is complete to avoid memory leaks.</remarks>
		public static IDataObject RegisterDefaultDragSource(UIElement dragSource, Point cursorOffset)
		{
			IDataObject data = CreateDataObject(dragSource, cursorOffset);
			RegisterDefaultDragSource(dragSource, data);
			return data;
		}

		/// <summary>
		/// Registers a Control as a drag source and provides default implementations of
		/// GiveFeedback and QueryContinueDrag. This override also handles the data object
		/// creation, including initialization of the drag image from the speicified Bitmap.
		/// </summary>
		/// <param name="dragSource">The drag source UIElement instance.</param>
		/// <param name="dragImage">A Bitmap to initialize the drag image from.</param>
		/// <param name="cursorOffset">The drag image cursor offset.</param>
		/// <returns>The created data object.</returns>
		/// <remarks>Callers must call UnregisterDefaultDragSource when the drag and drop
		/// operation is complete to avoid memory leaks.</remarks>
		public static IDataObject RegisterDefaultDragSource(UIElement dragSource, BitmapSource dragImage, Point cursorOffset)
		{
			IDataObject data = CreateDataObject(dragImage, cursorOffset);
			RegisterDefaultDragSource(dragSource, data);
			return data;
		}

		/// <summary>
		/// Unregisters a drag source from the internal cache.
		/// </summary>
		/// <param name="dragSource">The drag source UIElement.</param>
		public static void UnregisterDefaultDragSource(UIElement dragSource)
		{
			if (s_dataContext.ContainsKey(dragSource))
			{
				DragSourceEntry entry = s_dataContext[dragSource];
				ComTypes.IDataObject dataObjectCOM = (ComTypes.IDataObject)entry.data;

				// Stop listening to drop description changes
				dataObjectCOM.DUnadvise(entry.adviseConnection);

				// Unhook the default drag source event handlers
				dragSource.GiveFeedback -= new GiveFeedbackEventHandler(DefaultGiveFeedbackHandler);
				dragSource.QueryContinueDrag -= new QueryContinueDragEventHandler(DefaultQueryContinueDragHandler);

				// Remove the entries from our context caches
				s_dataContext.Remove(dragSource);
				s_dropDescriptions.Remove(entry.data);
			}
		}

		/// <summary>
		/// Performs a default drag and drop operation for the specified drag source.
		/// </summary>
		/// <param name="dragSource">The drag source UIElement.</param>
		/// <param name="cursorOffset">The drag image cursor offset.</param>
		/// <param name="allowedEffects">The allowed drop effects.</param>
		/// <param name="data">The associated data.</param>
		/// <returns>The accepted drop effects from the completed operation.</returns>
		public static DragDropEffects DoDragDrop(UIElement dragSource, Point cursorOffset, DragDropEffects allowedEffects, params KeyValuePair<string, object>[] data)
		{
			IDataObject dataObject = RegisterDefaultDragSource(dragSource, cursorOffset);
			return DoDragDropInternal(dragSource, dataObject, allowedEffects, data);
		}

		/// <summary>
		/// Performs a default drag and drop operation for the specified drag source.
		/// </summary>
		/// <param name="dragSource">The drag source UIElement.</param>
		/// <param name="dragImage">The Bitmap to initialize the drag image from.</param>
		/// <param name="cursorOffset">The drag image cursor offset.</param>
		/// <param name="allowedEffects">The allowed drop effects.</param>
		/// <param name="data">The associated data.</param>
		/// <returns>The accepted drop effects from the completed operation.</returns>
		public static DragDropEffects DoDragDrop(UIElement dragSource, BitmapSource dragImage, Point cursorOffset, DragDropEffects allowedEffects, params KeyValuePair<string, object>[] data)
		{
			IDataObject dataObject = RegisterDefaultDragSource(dragSource, dragImage, cursorOffset);
			return DoDragDropInternal(dragSource, dataObject, allowedEffects, data);
		}

		/// <summary>
		/// Performs a default drag and drop operation for the specified drag source.
		/// </summary>
		/// <param name="dragSource">The drag source UIElement.</param>
		/// <param name="dataObject">The data object associated to the drag and drop operation.</param>
		/// <param name="allowedEffects">The allowed drop effects.</param>
		/// <param name="data">The associated data.</param>
		/// <returns>The accepted drop effects from the completed operation.</returns>
		private static DragDropEffects DoDragDropInternal(UIElement dragSource, IDataObject dataObject, DragDropEffects allowedEffects, KeyValuePair<string, object>[] data)
		{
			// Set the data onto the data object.
			if (data != null)
			{
				foreach (KeyValuePair<string, object> dataPair in data)
					dataObject.SetDataEx(dataPair.Key, dataPair.Value);
			}

			try
			{
				return DragDrop.DoDragDrop(dragSource, dataObject, allowedEffects);
			}
			finally
			{
				UnregisterDefaultDragSource(dragSource);
			}
		}

		/// <summary>
		/// Provides a default GiveFeedback event handler for drag sources.
		/// </summary>
		/// <param name="sender">The object that raised the event. Should be set to the drag source.</param>
		/// <param name="e">The event arguments.</param>
		public static void DefaultGiveFeedbackHandler(object sender, GiveFeedbackEventArgs e)
		{
			UIElement dragSource = sender as UIElement;
			if (dragSource != null)
			{
				if (s_dataContext.ContainsKey(dragSource))
				{
					DefaultGiveFeedback(s_dataContext[dragSource].data, e);
				}
			}
		}

		/// <summary>Some text
		/// Provides a default GiveFeedback event handler for drag sources.
		/// </summary>
		/// <param name="data">The associated data object for the event.</param>
		/// <param name="e">The event arguments.</param>
		public static void DefaultGiveFeedback(IDataObject data, GiveFeedbackEventArgs e)
		{
			if (IsShowingLayered(data))
			{
				// The default drag source implementation uses drop descriptions,
				// so we won't use default cursors.
				e.UseDefaultCursors = false;

				if (IsDropDescriptionDefault(data))
				{
					Mouse.SetCursor(Cursors.Arrow);
					SetDropDescriptionIsDefault(data, false);
				}
				//System.Threading.Thread.Sleep(200);
				//if (!Equals(data.GetDataEx(Eto.Wpf.Forms.WpfFrameworkElement.CustomCursor_DataKey), true))
				//Mouse.SetCursor(Cursors.Arrow);

				SetDragCursor(data, e.Effects);
				InvalidateDragImage(data);
			}
			else
			{
				e.UseDefaultCursors = true;
				SetDropDescriptionIsDefault(data, true);
			}

			e.Handled = true;
		}

		/// <summary>
		/// Provides a default handler for the QueryContinueDrag drag source event.
		/// </summary>
		/// <param name="sender">The object that raised the event. Not used internally.</param>
		/// <param name="e">The event arguments.</param>
		public static void DefaultQueryContinueDragHandler(object sender, QueryContinueDragEventArgs e)
		{
			DefaultQueryContinueDrag(e);
		}

		/// <summary>
		/// Provides a default handler for the QueryContinueDrag drag source event.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		public static void DefaultQueryContinueDrag(QueryContinueDragEventArgs e)
		{
			if (e.EscapePressed)
			{
				e.Action = DragAction.Cancel;
				e.Handled = true;
			}
		}

		/// <summary>
		/// Sets a flag on the system IDragSourceHelper2 object to allow drop descriptions
		/// on the drag image.
		/// </summary>
		/// <param name="allow">True to allow drop descriptions, otherwise False.</param>
		/// <remarks>Must be called before IDragSourceHelper.InitializeFromBitmap or
		/// IDragSourceHelper.InitializeFromControl is called.</remarks>
		public static void AllowDropDescription(bool allow)
		{
			IDragSourceHelper2 sourceHelper = (IDragSourceHelper2)new DragDropHelper();
			sourceHelper.SetFlags(allow ? 1 : 0);
		}

		/// <summary>
		/// Invalidates the drag image.
		/// </summary>
		/// <param name="dataObject">The data object for which to invalidate the drag image.</param>
		/// <remarks>This call tells the drag image manager to reformat the internal
		/// cached drag image, based on the already set drag image bitmap and current drop
		/// description.</remarks>
		public static void InvalidateDragImage(IDataObject dataObject)
		{
			if (dataObject.GetDataPresent("DragWindow"))
			{
				IntPtr hwnd = GetIntPtrFromData(dataObject.GetData("DragWindow"));
				PostMessage(hwnd, WM_INVALIDATEDRAGIMAGE, IntPtr.Zero, IntPtr.Zero);
			}
		}

		public static void SetDragCursor(IDataObject dataObject, DragDropEffects effects)
		{
			if (dataObject.GetDataPresent("DragWindow"))
			{
				IntPtr hwnd = GetIntPtrFromData(dataObject.GetData("DragWindow"));
				IntPtr wparam = (IntPtr)0;
				switch (effects)
				{
					default:
					case DragDropEffects.None:
						wparam = (IntPtr)1;
						break;
					case DragDropEffects.Copy:
						wparam = (IntPtr)3;
						break;
					case DragDropEffects.Move:
						wparam = (IntPtr)2;
						break;
					case DragDropEffects.Link:
						wparam = (IntPtr)4;
						break;
				}
				PostMessage(hwnd, WM_SETDRAGCURSOR, wparam, IntPtr.Zero);
			}
		}

		public static void InvalidateDragImage(IntPtr hwnd)
		{
			PostMessage(hwnd, WM_INVALIDATEDRAGIMAGE, IntPtr.Zero, IntPtr.Zero);
		}

		#region Helper methods

		/// <summary>
		/// Gets an IntPtr from data acquired from a data object.
		/// </summary>
		/// <param name="data">The data that contains the IntPtr.</param>
		/// <returns>An IntPtr.</returns>
		private static IntPtr GetIntPtrFromData(object data)
		{
			byte[] buf = null;

			if (data is MemoryStream)
			{
				buf = new byte[4];
				if (4 != ((MemoryStream)data).Read(buf, 0, 4))
					throw new ArgumentException("Could not read an IntPtr from the MemoryStream");
			}
			if (data is byte[])
			{
				buf = (byte[])data;
				if (buf.Length < 4)
					throw new ArgumentException("Could not read an IntPtr from the byte array");
			}

			if (buf == null)
				throw new ArgumentException("Could not read an IntPtr from the " + data.GetType().ToString());

			int p = (buf[3] << 24) | (buf[2] << 16) | (buf[1] << 8) | buf[0];
			return new IntPtr(p);
		}

		/// <summary>
		/// Determines if the IsShowingLayered flag is set on the data object.
		/// </summary>
		/// <param name="dataObject">The data object.</param>
		/// <returns>True if the flag is set, otherwise false.</returns>
		private static bool IsShowingLayered(IDataObject dataObject)
		{
			if (dataObject.GetDataPresent(IsShowingLayeredFormat))
			{
				object data = dataObject.GetData(IsShowingLayeredFormat);
				if (data != null)
					return GetBooleanFromData(data);
			}

			return false;
		}

		/// <summary>
		/// Converts compatible clipboard data to a boolean value.
		/// </summary>
		/// <param name="data">The clipboard data.</param>
		/// <returns>True if the data can be converted to a boolean and is set, otherwise False.</returns>
		private static bool GetBooleanFromData(object data)
		{
			if (data is Stream)
			{
				Stream stream = data as Stream;
				BinaryReader reader = new BinaryReader(stream);
				return reader.ReadBoolean();
			}

			// Anything else isn't supported for now
			return false;
		}

		/// <summary>
		/// Checks if the current drop description, if any, is valid.
		/// </summary>
		/// <param name="dataObject">The DataObject from which to get the drop description.</param>
		/// <returns>True if the drop description is set, and the 
		/// DropImageType is not DropImageType.Invalid.</returns>
		private static bool IsDropDescriptionValid(IDataObject dataObject)
		{
			object data = ComTypes.ComDataObjectExtensions.GetDropDescription((ComTypes.IDataObject)dataObject);
			if (data is DropDescription)
				return (DropImageType)((DropDescription)data).type != DropImageType.Invalid;
			return false;
		}

		/// <summary>
		/// Checks if the IsDefault drop description flag is set for the associated DataObject.
		/// </summary>
		/// <param name="dataObject">The associated DataObject.</param>
		/// <returns>True if the IsDefault flag is set, otherwise False.</returns>
		private static bool IsDropDescriptionDefault(IDataObject dataObject)
		{
			if (s_dropDescriptions.ContainsKey(dataObject))
				return (s_dropDescriptions[dataObject] & DropDescriptionFlags.IsDefault) == DropDescriptionFlags.IsDefault;
			return false;
		}

		/// <summary>
		/// Checks if the InvalidateRequired drop description flag is set for the associated DataObject.
		/// </summary>
		/// <param name="dataObject">The associated DataObject.</param>
		/// <returns>True if the InvalidateRequired flag is set, otherwise False.</returns>
		private static bool InvalidateRequired(IDataObject dataObject)
		{
			if (s_dropDescriptions.ContainsKey(dataObject))
				return (s_dropDescriptions[dataObject] & DropDescriptionFlags.InvalidateRequired) == DropDescriptionFlags.InvalidateRequired;
			return false;
		}

		/// <summary>
		/// Sets the IsDefault drop description flag for the associated DataObject.
		/// </summary>
		/// <param name="dataObject">The associdated DataObject.</param>
		/// <param name="isDefault">True to set the flag, False to unset it.</param>
		public static void SetDropDescriptionIsDefault(IDataObject dataObject, bool isDefault)
		{
			if (isDefault)
				SetDropDescriptionFlag(dataObject, DropDescriptionFlags.IsDefault);
			else
				UnsetDropDescriptionFlag(dataObject, DropDescriptionFlags.IsDefault);
		}

		/// <summary>
		/// Sets the InvalidatedRequired drop description flag for the associated DataObject.
		/// </summary>
		/// <param name="dataObject">The associdated DataObject.</param>
		/// <param name="isDefault">True to set the flag, False to unset it.</param>
		private static void SetInvalidateRequired(IDataObject dataObject, bool required)
		{
			if (required)
				SetDropDescriptionFlag(dataObject, DropDescriptionFlags.InvalidateRequired);
			else
				UnsetDropDescriptionFlag(dataObject, DropDescriptionFlags.InvalidateRequired);
		}

		/// <summary>
		/// Sets a drop description flag.
		/// </summary>
		/// <param name="dataObject">The associated DataObject.</param>
		/// <param name="flag">The drop description flag to set.</param>
		private static void SetDropDescriptionFlag(IDataObject dataObject, DropDescriptionFlags flag)
		{
			if (s_dropDescriptions.ContainsKey(dataObject))
				s_dropDescriptions[dataObject] |= flag;
			else
				s_dropDescriptions.Add(dataObject, flag);
		}

		/// <summary>
		/// Unsets a drop description flag.
		/// </summary>
		/// <param name="dataObject">The associated DataObject.</param>
		/// <param name="flag">The drop description flag to unset.</param>
		private static void UnsetDropDescriptionFlag(IDataObject dataObject, DropDescriptionFlags flag)
		{
			if (s_dropDescriptions.ContainsKey(dataObject))
			{
				DropDescriptionFlags current = s_dropDescriptions[dataObject];
				s_dropDescriptions[dataObject] = (current | flag) ^ flag;
			}
		}

		/// <summary>
		/// Gets the current DropDescription's drop image type.
		/// </summary>
		/// <param name="dataObject">The DataObject.</param>
		/// <returns>The current drop image type.</returns>
		private static DropImageType GetDropImageType(IDataObject dataObject)
		{
			object data = ComTypes.ComDataObjectExtensions.GetDropDescription((ComTypes.IDataObject)dataObject);
			if (data is DropDescription)
				return (DropImageType)((DropDescription)data).type;
			return DropImageType.Invalid;
		}

		#endregion // Helper methods

		#region AdviseSink class

		/// <summary>
		/// Provides an advisory sink for the COM IDataObject implementation.
		/// </summary>
		private class AdviseSink : ComTypes.IAdviseSink
		{
			// The associated data object
			private IDataObject data;

			/// <summary>
			/// Creates an AdviseSink associated to the specified data object.
			/// </summary>
			/// <param name="data">The data object.</param>
			public AdviseSink(IDataObject data)
			{
				this.data = data;
			}

			/// <summary>
			/// Handles DataChanged events from a COM IDataObject.
			/// </summary>
			/// <param name="format">The data format that had a change.</param>
			/// <param name="stgmedium">The data value.</param>
			public void OnDataChange(ref ComTypes.FORMATETC format, ref ComTypes.STGMEDIUM stgmedium)
			{
				// We listen to DropDescription changes, so that we can unset the IsDefault
				// drop description flag.
				object odd = ComTypes.ComDataObjectExtensions.GetDropDescription((ComTypes.IDataObject)data);
				//if (odd != null)
					//DragSourceHelper.SetDropDescriptionIsDefault(data, false);
			}

			#region Unsupported callbacks

			public void OnClose()
			{
				throw new NotImplementedException();
			}

			public void OnRename(System.Runtime.InteropServices.ComTypes.IMoniker moniker)
			{
				throw new NotImplementedException();
			}

			public void OnSave()
			{
				throw new NotImplementedException();
			}

			public void OnViewChange(int aspect, int index)
			{
				throw new NotImplementedException();
			}

			#endregion // Unsupported callbacks
		}

		#endregion // AdviseSink class
	}
}

#endregion // WpfDragDropLib\WpfDragSourceHelper.cs

#region WpfDragDropLib\WpfDropTargetHelper.cs

namespace System.Windows
{
	using System;
	using System.Windows;
	using System.Windows.Interop;
	using ComIDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
	using DragDropLib;

	public static class DropTargetHelper
	{
		/// <summary>
		/// Gets a value indicating that the DropTargetHelper is supported with the current drag operation
		/// </summary>
		/// <remarks>
		/// Make sure not to call any of the other DropTargetHelper methods unless this returns true.
		/// </remarks>
		/// <param name="data">The DataObject containing a drag image.</param>
		/// <returns>True if it is supported, false otherwise</returns>
		public static bool IsSupported(IDataObject data)
		{
			return data.GetDataPresent("DragSourceHelperFlags");
		}

		/// <summary>
		/// Internal instance of the DragDropHelper.
		/// </summary>
		private static IDropTargetHelper s_instance = (IDropTargetHelper)new DragDropHelper();

		static DropTargetHelper()
		{
		}

		/// <summary>
		/// Notifies the DragDropHelper that the specified Window received
		/// a DragEnter event.
		/// </summary>
		/// <param name="window">The Window the received the DragEnter event.</param>
		/// <param name="data">The DataObject containing a drag image.</param>
		/// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
		/// <param name="effect">The accepted drag drop effect.</param>
		public static void DragEnter(Window window, IDataObject data, Point cursorOffset, DragDropEffects effect)
		{
			IntPtr windowHandle = IntPtr.Zero;
			if (window != null)
				windowHandle = (new WindowInteropHelper(window)).Handle;
			Win32Point pt = WpfDragDropLibExtensions.ToWin32Point(cursorOffset);
			s_instance.DragEnter(windowHandle, (ComIDataObject)data, ref pt, (int)effect);
		}

		/// <summary>
		/// Notifies the DragDropHelper that the specified Window received
		/// a DragEnter event.
		/// </summary>
		/// <param name="window">The Window the received the DragEnter event.</param>
		/// <param name="data">The DataObject containing a drag image.</param>
		/// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
		/// <param name="effect">The accepted drag drop effect.</param>
		/// <param name="descriptionMessage">The drop description message.</param>
		/// <param name="descriptionInsert">The drop description insert.</param>
		/// <remarks>Callers of this DragEnter override should make sure to call
		/// the DragLeave override taking an IDataObject parameter in order to clear
		/// the drop description.</remarks>
		public static void DragEnter(Window window, IDataObject data, Point cursorOffset, DragDropEffects effect, string descriptionMessage, string descriptionInsert)
		{
			data.SetDropDescription((DropImageType)effect, descriptionMessage, descriptionInsert);
			DragEnter(window, data, cursorOffset, effect);
		}

		/// <summary>
		/// Notifies the DragDropHelper that the current Window received
		/// a DragOver event.
		/// </summary>
		/// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
		/// <param name="effect">The accepted drag drop effect.</param>
		public static void DragOver(Point cursorOffset, DragDropEffects effect)
		{
			Win32Point pt = WpfDragDropLibExtensions.ToWin32Point(cursorOffset);
			s_instance.DragOver(ref pt, (int)effect);
		}

		/// <summary>
		/// Notifies the DragDropHelper that the current Window received
		/// a DragLeave event.
		/// </summary>
		public static void DragLeave()
		{
			s_instance.DragLeave();
		}

		/// <summary>
		/// Notifies the DragDropHelper that the current Window received
		/// a DragLeave event.
		/// </summary>
		/// <param name="data">The data object associated to the event.</param>
		public static void DragLeave(IDataObject data)
		{
			data.SetDropDescription(DropImageType.Invalid, null, null);
			DragLeave();
		}

		/// <summary>
		/// Notifies the DragDropHelper that the current Window received
		/// a DragOver event.
		/// </summary>
		/// <param name="data">The DataObject containing a drag image.</param>
		/// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
		/// <param name="effect">The accepted drag drop effect.</param>
		public static void Drop(IDataObject data, Point cursorOffset, DragDropEffects effect)
		{
			Win32Point pt = WpfDragDropLibExtensions.ToWin32Point(cursorOffset);
			s_instance.Drop((ComIDataObject)data, ref pt, (int)effect);
		}

		/// <summary>
		/// Tells the DragDropHelper to show or hide the drag image.
		/// </summary>
		/// <param name="show">True to show the image. False to hide it.</param>
		public static void Show(bool show)
		{
			s_instance.Show(show);
		}
	}
}

#endregion // WpfDragDropLib\WpfDropTargetHelper.cs


#region WpfDragDropLib\WpfDataObjectExtensions.cs

namespace System.Windows
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing.Imaging;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;
	using DragDropLib;
	using Bitmap = System.Drawing.Bitmap;
	using Color = System.Windows.Media.Color;
	using ComIDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
	using ComTypes = System.Runtime.InteropServices.ComTypes;
	using DrawingColor = System.Drawing.Color;
	using DrawingColorPalette = System.Drawing.Imaging.ColorPalette;
	using DrawingPixelFormat = System.Drawing.Imaging.PixelFormat;
	using DrawingRectangle = System.Drawing.Rectangle;
	using PixelFormat = System.Windows.Media.PixelFormat;

	public enum DropImageType
	{
		Invalid = -1,
		None = 0,
		Copy = (int)DragDropEffects.Copy,
		Move = (int)DragDropEffects.Move,
		Link = (int)DragDropEffects.Link,
		Label = 6,
		Warning = 7
	}

	/// <summary>
	/// Provides extended functionality to the System.Windows.IDataObject interface.
	/// </summary>
	public static class WpfDataObjectExtensions
	{
		#region DLL imports

		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hgdi);

		[DllImport("ole32.dll")]
		private static extern void ReleaseStgMedium(ref ComTypes.STGMEDIUM pmedium);

		#endregion // DLL imports

		/// <summary>
		/// Sets the drag image by rendering the specified UIElement.
		/// </summary>
		/// <param name="dataObject">The DataObject to set the drag image for.</param>
		/// <param name="element">The element to render as the drag image.</param>
		/// <param name="cursorOffset">The offset of the cursor relative to the UIElement.</param>
		public static void SetDragImage(this IDataObject dataObject, UIElement element, Point cursorOffset)
		{
			Size size = element.RenderSize;

			// Get the device's DPI so we render at full size
			// HACK
			int dpix = 96;
			int dpiy = 96;
			//GetDeviceDpi(element, out dpix, out dpiy);

			// Create our renderer at full size
			RenderTargetBitmap renderSource = new RenderTargetBitmap(
				(int)size.Width, (int)size.Height, dpix, dpiy, PixelFormats.Pbgra32);

			// Render the element
			renderSource.Render(element);

			// Set the drag image by the bitmap source
			SetDragImage(dataObject, renderSource, cursorOffset);
		}

		/// <summary>
		/// Sets the drag image from a BitmapSource.
		/// </summary>
		/// <param name="dataObject">The DataObject on which to set the drag image.</param>
		/// <param name="image">The image source.</param>
		/// <param name="cursorOffset">The offset relative to the bitmap image.</param>
		public static void SetDragImage(this IDataObject dataObject, BitmapSource image, Point cursorOffset)
		{
			// Our internal routine requires an HBITMAP, so we'll convert the
			// BitmapSource to a System.Drawing.Bitmap.
			Bitmap bmp = GetBitmapFromBitmapSource(image, Colors.Magenta);

			// Sets the drag image from a Bitmap
			SetDragImage(dataObject, bmp, cursorOffset);
		}

		/// <summary>
		/// Sets the drag image.
		/// </summary>
		/// <param name="dataObject">The DataObject to set the drag image on.</param>
		/// <param name="image">The drag image.</param>
		/// <param name="cursorOffset">The location of the cursor relative to the image.</param>
		private static void SetDragImage(this IDataObject dataObject, Bitmap bitmap, Point cursorOffset)
		{
			ShDragImage shdi = new ShDragImage();

			Win32Size size;
			size.cx = bitmap.Width;
			size.cy = bitmap.Height;
			shdi.sizeDragImage = size;

			Win32Point wpt;
			wpt.x = (int)cursorOffset.X;
			wpt.y = (int)cursorOffset.Y;
			shdi.ptOffset = wpt;

			shdi.crColorKey = DrawingColor.Magenta.ToArgb();

			// This HBITMAP will be managed by the DragDropHelper
			// as soon as we pass it to InitializeFromBitmap. If we fail
			// to make the hand off, we'll delete it to prevent a mem leak.
			IntPtr hbmp = bitmap.GetHbitmap();
			shdi.hbmpDragImage = hbmp;

			try
			{
				IDragSourceHelper sourceHelper = (IDragSourceHelper)new DragDropHelper();

				try
				{
					sourceHelper.InitializeFromBitmap(ref shdi, (ComIDataObject)dataObject);
				}
				catch (NotImplementedException ex)
				{
					throw new Exception("A NotImplementedException was caught. This could be because you forgot to construct your DataObject using a DragDropLib.DataObject", ex);
				}
			}
			catch
			{
				// We failed to initialize the drag image, so the DragDropHelper
				// won't be managing our memory. Release the HBITMAP we allocated.
				DeleteObject(hbmp);
			}
		}

		public static unsafe void InitializeFromWindow(this IDataObject dataObject, IntPtr hwnd, Point? offset)
		{
			try
			{
				IDragSourceHelper sourceHelper = (IDragSourceHelper)new DragDropHelper();

				try
				{
					Win32Point* ppt = null;
					if (offset != null)
					{
						Win32Point pt = offset.Value.ToWin32Point();
						ppt = &pt;
					}
					sourceHelper.InitializeFromWindow(hwnd, ppt, (ComIDataObject)dataObject);
				}
				catch (NotImplementedException ex)
				{
					throw new Exception("A NotImplementedException was caught. This could be because you forgot to construct your DataObject using a DragDropLib.DataObject", ex);
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Sets the drop description for the drag image manager.
		/// </summary>
		/// <param name="dataObject">The DataObject to set.</param>
		/// <param name="type">The type of the drop image.</param>
		/// <param name="format">The format string for the description.</param>
		/// <param name="insert">The parameter for the drop description.</param>
		/// <remarks>
		/// When setting the drop description, the text can be set in two part,
		/// which will be rendered slightly differently to distinguish the description
		/// from the subject. For example, the format can be set as "Move to %1" and
		/// the insert as "Temp". When rendered, the "%1" in format will be replaced
		/// with "Temp", but "Temp" will be rendered slightly different from "Move to ".
		/// </remarks>
		public static void SetDropDescription(this IDataObject dataObject, DropImageType type, string format, string insert)
		{
			if (format != null && format.Length > 259)
				throw new ArgumentException("Format string exceeds the maximum allowed length of 259.", "format");
			if (insert != null && insert.Length > 259)
				throw new ArgumentException("Insert string exceeds the maximum allowed length of 259.", "insert");

			// Fill the structure
			DropDescription dd;
			dd.type = (int)type;
			dd.szMessage = format;
			dd.szInsert = insert;

			ComTypes.ComDataObjectExtensions.SetDropDescription((ComTypes.IDataObject)dataObject, dd);
		}

		private static void SetBoolFormat(this IDataObject dataObject, string format, bool val)
		{
			DataFormat dataFormat = DataFormats.GetDataFormat(format);

			ComTypes.FORMATETC formatETC = new ComTypes.FORMATETC();
			formatETC.cfFormat = (short)dataFormat.Id;
			formatETC.dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT;
			formatETC.lindex = -1;
			formatETC.ptd = IntPtr.Zero;
			formatETC.tymed = ComTypes.TYMED.TYMED_HGLOBAL;
			IntPtr num = Marshal.AllocHGlobal(4);
			try
			{
				Marshal.Copy(BitConverter.GetBytes(val ? 1 : 0), 0, num, 4);
				ComTypes.STGMEDIUM medium;
				medium.pUnkForRelease = (object)null;
				medium.tymed = ComTypes.TYMED.TYMED_HGLOBAL;
				medium.unionmember = num;
				((System.Runtime.InteropServices.ComTypes.IDataObject)dataObject).SetData(ref formatETC, ref medium, true);
			}
			catch
			{
				Marshal.FreeHGlobal(num);
				throw;
			}
		}

		/// <summary>
		/// Sets managed data to a clipboard DataObject.
		/// </summary>
		/// <param name="dataObject">The DataObject to set the data on.</param>
		/// <param name="format">The clipboard format.</param>
		/// <param name="data">The data object.</param>
		/// <remarks>
		/// Because the underlying data store is not storing managed objects, but
		/// unmanaged ones, this function provides intelligent conversion, allowing
		/// you to set unmanaged data into the COM implemented IDataObject.</remarks>
		public static void SetDataEx(this IDataObject dataObject, string format, object data)
		{
			DataFormat dataFormat = DataFormats.GetDataFormat(format);

			// Initialize the format structure
			ComTypes.FORMATETC formatETC = new ComTypes.FORMATETC();
			formatETC.cfFormat = (short)dataFormat.Id;
			formatETC.dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT;
			formatETC.lindex = -1;
			formatETC.ptd = IntPtr.Zero;

			// Try to discover the TYMED from the format and data
			ComTypes.TYMED tymed = GetCompatibleTymed(format, data);
			// If a TYMED was found, we can use the system DataObject
			// to convert our value for us.
			if (tymed != ComTypes.TYMED.TYMED_NULL)
			{
				formatETC.tymed = tymed;

				if (data is byte[] bytes)
				{
					// don't convert byte array as it adds extra data
					ComTypes.ComDataObjectExtensions.SetByteData((ComTypes.IDataObject)dataObject, format, bytes);
				}
				else
				{
					// Set data on an empty DataObject instance
					DataObject conv = new DataObject();
					conv.SetData(format, data, true);

					// Now retrieve the data, using the COM interface.
					// This will perform a managed to unmanaged conversion for us.
					ComTypes.STGMEDIUM medium;
					((ComTypes.IDataObject)conv).GetData(ref formatETC, out medium);
					try
					{
						// Now set the data on our data object
						((ComTypes.IDataObject)dataObject).SetData(ref formatETC, ref medium, true);
					}
					catch
					{
						// On exceptions, release the medium
						ReleaseStgMedium(ref medium);
						throw;
					}
				}
			}
			else
			{
				// Since we couldn't determine a TYMED, this data
				// is likely custom managed data, and won't be used
				// by unmanaged code, so we'll use our custom marshaling
				// implemented by our COM IDataObject extensions.

				ComTypes.ComDataObjectExtensions.SetManagedData((ComTypes.IDataObject)dataObject, format, data);
			}
		}

		/// <summary>
		/// Gets a system compatible TYMED for the given format.
		/// </summary>
		/// <param name="format">The data format.</param>
		/// <param name="data">The data.</param>
		/// <returns>A TYMED value, indicating a system compatible TYMED that can
		/// be used for data marshaling.</returns>
		private static ComTypes.TYMED GetCompatibleTymed(string format, object data)
		{
			if (IsFormatEqual(format, DataFormats.Bitmap) && (data is System.Drawing.Bitmap || data is System.Windows.Media.Imaging.BitmapSource))
				return ComTypes.TYMED.TYMED_GDI;
			if (IsFormatEqual(format, DataFormats.EnhancedMetafile))
				return ComTypes.TYMED.TYMED_ENHMF;
			if (IsFormatEqual(format, System.Windows.Ink.StrokeCollection.InkSerializedFormat))
				return ComTypes.TYMED.TYMED_ISTREAM;
			if (data is Stream
				|| IsFormatEqual(format, DataFormats.Html) || IsFormatEqual(format, DataFormats.Xaml)
				|| IsFormatEqual(format, DataFormats.Text) || IsFormatEqual(format, DataFormats.Rtf)
				|| IsFormatEqual(format, DataFormats.OemText)
				|| IsFormatEqual(format, DataFormats.UnicodeText) || IsFormatEqual(format, "ApplicationTrust")
				|| IsFormatEqual(format, DataFormats.FileDrop)
				|| IsFormatEqual(format, "FileName")
				|| IsFormatEqual(format, "FileNameW"))
				return ComTypes.TYMED.TYMED_HGLOBAL;
			if (IsFormatEqual(format, DataFormats.Dib) && data is System.Drawing.Image)
				return System.Runtime.InteropServices.ComTypes.TYMED.TYMED_NULL;
			if (IsFormatEqual(format, typeof(System.Windows.Media.Imaging.BitmapSource).FullName)
				|| IsFormatEqual(format, typeof(System.Drawing.Bitmap).FullName))
				return ComTypes.TYMED.TYMED_HGLOBAL;
			if (IsFormatEqual(format, DataFormats.EnhancedMetafile) || data is System.Drawing.Imaging.Metafile)
				return System.Runtime.InteropServices.ComTypes.TYMED.TYMED_NULL;
			if (IsFormatEqual(format, DataFormats.Serializable) || (data is System.Runtime.Serialization.ISerializable)
				|| ((data != null) && data.GetType().IsSerializable))
				return ComTypes.TYMED.TYMED_HGLOBAL;

			return ComTypes.TYMED.TYMED_NULL;
		}

		/// <summary>
		/// Compares the equality of two clipboard formats.
		/// </summary>
		/// <param name="formatA">First format.</param>
		/// <param name="formatB">Second format.</param>
		/// <returns>True if the formats are equal. False otherwise.</returns>
		private static bool IsFormatEqual(string formatA, string formatB)
		{
			return string.CompareOrdinal(formatA, formatB) == 0;
		}

		/// <summary>
		/// Gets managed data from a clipboard DataObject.
		/// </summary>
		/// <param name="dataObject">The DataObject to obtain the data from.</param>
		/// <param name="format">The format for which to get the data in.</param>
		/// <returns>The data object instance.</returns>
		public static object GetDataEx(this IDataObject dataObject, string format)
		{
			// Get the data
			object data = dataObject.GetData(format, true);

			// If the data is a stream, we'll check to see if it
			// is stamped by us for custom marshaling
			if (data is Stream)
			{
				object data2 = ComTypes.ComDataObjectExtensions.GetManagedData((ComTypes.IDataObject)dataObject, format);
				if (data2 != null)
					return data2;
			}

			return data;
		}

		#region Helper methods

		/// <summary>
		/// Gets the device capabilities.
		/// </summary>
		/// <param name="reference">A reference UIElement for getting the relevant device caps.</param>
		/// <param name="dpix">The horizontal DPI.</param>
		/// <param name="dpiy">The vertical DPI.</param>
		private static void GetDeviceDpi(Visual reference, out int dpix, out int dpiy)
		{
			Matrix m = PresentationSource.FromVisual(reference).CompositionTarget.TransformToDevice;
			dpix = (int)(96 * m.M11);
			dpiy = (int)(96 * m.M22);
		}

		/// <summary>
		/// Gets a System.Drawing.Bitmap from a BitmapSource.
		/// </summary>
		/// <param name="source">The source image from which to create our Bitmap.</param>
		/// <param name="transparencyKey">The transparency key. This is used by the DragDropHelper
		/// in rendering transparent pixels.</param>
		/// <returns>An instance of Bitmap which is a copy of the BitmapSource's image.</returns>
		private static Bitmap GetBitmapFromBitmapSource(BitmapSource source, Color transparencyKey)
		{
			// Copy at full size
			Int32Rect sourceRect = new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight);

			// Convert to our destination pixel format
			DrawingPixelFormat pxFormat = ConvertPixelFormat(source.Format);

			// Create the Bitmap, full size, full rez
			Bitmap bmp = new Bitmap(sourceRect.Width, sourceRect.Height, pxFormat);
			// If the format is an indexed format, copy the color palette
			if ((pxFormat & DrawingPixelFormat.Indexed) == DrawingPixelFormat.Indexed)
				ConvertColorPalette(bmp.Palette, source.Palette);

			// Get the transparency key as a System.Drawing.Color
			DrawingColor transKey = transparencyKey.ToDrawingColor();

			// Lock our Bitmap bits, we need to write to it
			BitmapData bmpData = bmp.LockBits(
				sourceRect.ToDrawingRectangle(),
				ImageLockMode.ReadWrite,
				pxFormat);
			{
				// Copy the source bitmap data to our new Bitmap
				source.CopyPixels(sourceRect, bmpData.Scan0, bmpData.Stride * sourceRect.Height, bmpData.Stride);

				// The drag image seems to work in full 32-bit color, except when
				// alpha equals zero. Then it renders those pixels at black. So
				// we make a pass and set all those pixels to the transparency key
				// color. This is only implemented for 32-bit pixel colors for now.
				if ((pxFormat & DrawingPixelFormat.Alpha) == DrawingPixelFormat.Alpha)
					ReplaceTransparentPixelsWithTransparentKey(bmpData, transKey);
			}
			// Done, unlock the bits
			bmp.UnlockBits(bmpData);

			return bmp;
		}

		/// <summary>
		/// Replaces any pixel with a zero alpha value with the specified transparency key.
		/// </summary>
		/// <param name="bmpData">The bitmap data in which to perform the operation.</param>
		/// <param name="transKey">The transparency color. This color is rendered transparent
		/// by the DragDropHelper.</param>
		/// <remarks>
		/// This function only supports 32-bit pixel formats for now.
		/// </remarks>
		private static void ReplaceTransparentPixelsWithTransparentKey(BitmapData bmpData, DrawingColor transKey)
		{
			DrawingPixelFormat pxFormat = bmpData.PixelFormat;

			if (DrawingPixelFormat.Format32bppArgb == pxFormat
				|| DrawingPixelFormat.Format32bppPArgb == pxFormat)
			{
				int transKeyArgb = transKey.ToArgb();

				// We will just iterate over the data... we don't care about pixel location,
				// just that every pixel is checked.
				unsafe
				{
					byte* pscan = (byte*)bmpData.Scan0.ToPointer();
					{
						for (int y = 0; y < bmpData.Height; ++y, pscan += bmpData.Stride)
						{
							int* prgb = (int*)pscan;
							for (int x = 0; x < bmpData.Width; ++x, ++prgb)
							{
								// If the alpha value is zero, replace this pixel's color
								// with the transparency key.
								if ((*prgb & 0xFF000000L) == 0L)
									*prgb = transKeyArgb;
							}
						}
					}
				}
			}
			else
			{
				// If it is anything else, we aren't supporting it, but we
				// won't throw, cause it isn't an error
				System.Diagnostics.Trace.TraceWarning("Not converting transparent colors to transparency key.");
				return;
			}
		}

		/// <summary>
		/// Converts a System.Windows.Media.Color to System.Drawing.Color.
		/// </summary>
		/// <param name="color">System.Windows.Media.Color value to convert.</param>
		/// <returns>System.Drawing.Color value.</returns>
		private static DrawingColor ToDrawingColor(this Color color)
		{
			return DrawingColor.FromArgb(
				color.A, color.R, color.G, color.B);
		}

		/// <summary>
		/// Converts a System.Windows.Int32Rect to a System.Drawing.Rectangle value.
		/// </summary>
		/// <param name="rect">The System.Windows.Int32Rect to convert.</param>
		/// <returns>The System.Drawing.Rectangle converted value.</returns>
		private static DrawingRectangle ToDrawingRectangle(this Int32Rect rect)
		{
			return new DrawingRectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>
		/// Converts the entries in a BitmapPalette to ColorPalette entries.
		/// </summary>
		/// <param name="destPalette">ColorPalette destination palette.</param>
		/// <param name="bitmapPalette">BitmapPalette source palette.</param>
		private static void ConvertColorPalette(DrawingColorPalette destPalette, BitmapPalette bitmapPalette)
		{
			DrawingColor[] destEntries = destPalette.Entries;
			IList<Color> sourceEntries = bitmapPalette.Colors;

			if (destEntries.Length < sourceEntries.Count)
				throw new ArgumentException("Destination palette has less entries than the source palette");

			for (int i = 0, count = sourceEntries.Count; i < count; ++i)
				destEntries[i] = sourceEntries[i].ToDrawingColor();
		}

		/// <summary>
		/// Converts a System.Windows.Media.PixelFormat instance to a
		/// System.Drawing.Imaging.PixelFormat value.
		/// </summary>
		/// <param name="pixelFormat">The input PixelFormat.</param>
		/// <returns>The converted value.</returns>
		private static DrawingPixelFormat ConvertPixelFormat(PixelFormat pixelFormat)
		{
			if (PixelFormats.Bgr24 == pixelFormat)
				return DrawingPixelFormat.Format24bppRgb;
			if (PixelFormats.Bgr32 == pixelFormat)
				return DrawingPixelFormat.Format32bppRgb;
			if (PixelFormats.Bgr555 == pixelFormat)
				return DrawingPixelFormat.Format16bppRgb555;
			if (PixelFormats.Bgr565 == pixelFormat)
				return DrawingPixelFormat.Format16bppRgb565;
			if (PixelFormats.Bgra32 == pixelFormat)
				return DrawingPixelFormat.Format32bppArgb;
			if (PixelFormats.BlackWhite == pixelFormat)
				return DrawingPixelFormat.Format1bppIndexed;
			if (PixelFormats.Gray16 == pixelFormat)
				return DrawingPixelFormat.Format16bppGrayScale;
			if (PixelFormats.Indexed1 == pixelFormat)
				return DrawingPixelFormat.Format1bppIndexed;
			if (PixelFormats.Indexed4 == pixelFormat)
				return DrawingPixelFormat.Format4bppIndexed;
			if (PixelFormats.Indexed8 == pixelFormat)
				return DrawingPixelFormat.Format8bppIndexed;
			if (PixelFormats.Pbgra32 == pixelFormat)
				return DrawingPixelFormat.Format32bppPArgb;
			if (PixelFormats.Prgba64 == pixelFormat)
				return DrawingPixelFormat.Format64bppPArgb;
			if (PixelFormats.Rgb24 == pixelFormat)
				return DrawingPixelFormat.Format24bppRgb;
			if (PixelFormats.Rgb48 == pixelFormat)
				return DrawingPixelFormat.Format48bppRgb;
			if (PixelFormats.Rgba64 == pixelFormat)
				return DrawingPixelFormat.Format64bppArgb;

			throw new NotSupportedException("The pixel format of the source bitmap is not supported.");
		}

		#endregion // Helper methods
	}
}

#endregion // WpfDragDropLib\WpfDataObjectExtensions.cs

