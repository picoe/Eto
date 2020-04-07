
// originally from : https://blogs.msdn.microsoft.com/adamroot/2008/02/19/shell-style-drag-and-drop-in-net-part-3/
// downloads link from: https://stackoverflow.com/questions/36239516/real-implementation-of-the-shell-style-drag-and-drop-in-net-wpf-and-winforms
// modified to properly set standard cursors without hard coding copy/link/move/etc labels.


#region SwfDragDropLib\SwfDataObjectExtensions.cs

namespace System.Windows.Forms
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;
	using DragDropLib;
	using ComIDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
	using ComTypes = System.Runtime.InteropServices.ComTypes;

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
	/// Provides extended functionality to the System.Windows.Forms.IDataObject interface.
	/// </summary>
	public static class SwfDataObjectExtensions
	{
		#region DLL imports

		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hgdi);

		[DllImport("ole32.dll")]
		private static extern void ReleaseStgMedium(ref ComTypes.STGMEDIUM pmedium);

		#endregion // DLL imports

		/// <summary>
		/// Sets the drag image as the rendering of a control.
		/// </summary>
		/// <param name="dataObject">The DataObject to set the drag image on.</param>
		/// <param name="control">The Control to render as the drag image.</param>
		/// <param name="cursorOffset">The location of the cursor relative to the control.</param>
		public static void SetDragImage(this IDataObject dataObject, Control control, System.Drawing.Point cursorOffset)
		{
			int width = control.Width;
			int height = control.Height;

			Bitmap bmp = new Bitmap(width, height);
			control.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));

			SetDragImage(dataObject, bmp, cursorOffset);
		}

		/// <summary>
		/// Sets the drag image.
		/// </summary>
		/// <param name="dataObject">The DataObject to set the drag image on.</param>
		/// <param name="image">The drag image.</param>
		/// <param name="cursorOffset">The location of the cursor relative to the image.</param>
		public static void SetDragImage(this IDataObject dataObject, Image image, System.Drawing.Point cursorOffset)
		{
			ShDragImage shdi = new ShDragImage();

			Win32Size size;
			size.cx = image.Width;
			size.cy = image.Height;
			shdi.sizeDragImage = size;

			Win32Point wpt;
			wpt.x = cursorOffset.X;
			wpt.y = cursorOffset.Y;
			shdi.ptOffset = wpt;

			shdi.crColorKey = Color.Magenta.ToArgb();

			// This HBITMAP will be managed by the DragDropHelper
			// as soon as we pass it to InitializeFromBitmap. If we fail
			// to make the hand off, we'll delete it to prevent a mem leak.
			IntPtr hbmp = GetHbitmapFromImage(image);
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
				DeleteObject(hbmp);
			}
		}

		/// <summary>
		/// Gets an HBITMAP from any image.
		/// </summary>
		/// <param name="image">The image to get an HBITMAP from.</param>
		/// <returns>An HBITMAP pointer.</returns>
		/// <remarks>
		/// The caller is responsible to call DeleteObject on the HBITMAP.
		/// </remarks>
		private static IntPtr GetHbitmapFromImage(Image image)
		{
			if (image is Bitmap)
			{
				return ((Bitmap)image).GetHbitmap(Color.Magenta);
			}
			else
			{
				Bitmap bmp = new Bitmap(image);
				return bmp.GetHbitmap(Color.Magenta);
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
			DataFormats.Format dataFormat = DataFormats.GetFormat(format);

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
			DataFormats.Format dataFormat = DataFormats.GetFormat(format);

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
					conv.SetData(format, true, data);

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
			if (IsFormatEqual(format, DataFormats.Bitmap) && data is System.Drawing.Bitmap)
				return ComTypes.TYMED.TYMED_GDI;
			if (IsFormatEqual(format, DataFormats.EnhancedMetafile))
				return ComTypes.TYMED.TYMED_ENHMF;
			if (data is Stream
				|| IsFormatEqual(format, DataFormats.Html)
				|| IsFormatEqual(format, DataFormats.Text) || IsFormatEqual(format, DataFormats.Rtf)
				|| IsFormatEqual(format, DataFormats.OemText)
				|| IsFormatEqual(format, DataFormats.UnicodeText) || IsFormatEqual(format, "ApplicationTrust")
				|| IsFormatEqual(format, DataFormats.FileDrop)
				|| IsFormatEqual(format, "FileName")
				|| IsFormatEqual(format, "FileNameW"))
				return ComTypes.TYMED.TYMED_HGLOBAL;
			if (IsFormatEqual(format, DataFormats.Dib) && data is System.Drawing.Image)
				return System.Runtime.InteropServices.ComTypes.TYMED.TYMED_NULL;
			if (IsFormatEqual(format, typeof(System.Drawing.Bitmap).FullName))
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
	}
}

#endregion // SwfDragDropLib\SwfDataObjectExtensions.cs

#region SwfDragDropLib\SwfDragDropLibExtensions.cs

namespace DragDropLib
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	static class SwfDragDropLibExtensions
	{
		/// <summary>
		/// Converts a System.Windows.Point value to a DragDropLib.Win32Point value.
		/// </summary>
		/// <param name="pt">Input value.</param>
		/// <returns>Converted value.</returns>
		public static Win32Point ToWin32Point(this Point pt)
		{
			Win32Point wpt = new Win32Point();
			wpt.x = pt.X;
			wpt.y = pt.Y;
			return wpt;
		}
	}
}

#endregion // SwfDragDropLib\SwfDragDropLibExtensions.cs

#region SwfDragDropLib\SwfDropTargetHelper.cs

namespace System.Windows.Forms
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;
	using DragDropLib;
	using ComIDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

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

		/// <summary>
		/// Internal cache of IDataObjects related to drop targets.
		/// </summary>
		private static IDictionary<Control, IDataObject> s_dataContext = new Dictionary<Control, IDataObject>();

		/// <summary>
		/// Notifies the DragDropHelper that the specified Control received
		/// a DragEnter event.
		/// </summary>
		/// <param name="control">The Control the received the DragEnter event.</param>
		/// <param name="data">The DataObject containing a drag image.</param>
		/// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
		/// <param name="effect">The accepted drag drop effect.</param>
		public static void DragEnter(Control control, IDataObject data, System.Drawing.Point cursorOffset, DragDropEffects effect)
		{
			IntPtr controlHandle = IntPtr.Zero;
			if (control != null)
				controlHandle = control.Handle;
			Win32Point pt = SwfDragDropLibExtensions.ToWin32Point(cursorOffset);
			s_instance.DragEnter(controlHandle, (ComIDataObject)data, ref pt, (int)effect);
			s_dataContext[control] = data;
		}

		/// <summary>
		/// Sets the drop description of the IDataObject and then noTextDataFormattifies the
		/// DragDropHelper that the specified Control received a DragEnter event.
		/// </summary>
		/// <param name="control">The Control the received the DragEnter event.</param>
		/// <param name="data">The DataObject containing a drag image.</param>
		/// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
		/// <param name="effect">The accepted drag drop effect.</param>
		/// <param name="descriptionMessage">The drop description message.</param>
		/// <param name="descriptionInsert">The drop description insert.</param>
		/// <remarks>
		/// Because the DragLeave event in SWF does not provide the IDataObject in the
		/// event args, this DragEnter override of the DropTargetHelper will cache a
		/// copy of the IDataObject based on the provided Control so that it may be
		/// cleared using the DragLeave override that takes a Control parameter. Note that
		/// if you use this override of DragEnter, you must call the DragLeave override
		/// that takes a Control parameter to avoid a possible memory leak. However, calling
		/// this method multiple times with the same Control parameter while not calling the
		/// DragLeave method will not leak memory.
		/// </remarks>
		public static void DragEnter(Control control, IDataObject data, System.Drawing.Point cursorOffset, DragDropEffects effect, string descriptionMessage, string descriptionInsert)
		{
			data.SetDropDescription((DropImageType)effect, descriptionMessage, descriptionInsert);
			DragEnter(control, data, cursorOffset, effect);

			if (!s_dataContext.ContainsKey(control))
				s_dataContext.Add(control, data);
			else
				s_dataContext[control] = data;
		}

		/// <summary>
		/// Notifies the DragDropHelper that the current Control received
		/// a DragOver event.
		/// </summary>
		/// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
		/// <param name="effect">The accepted drag drop effect.</param>
		public static void DragOver(System.Drawing.Point cursorOffset, DragDropEffects effect)
		{
			Win32Point pt = SwfDragDropLibExtensions.ToWin32Point(cursorOffset);
			s_instance.DragOver(ref pt, (int)effect);
		}

		/// <summary>
		/// Notifies the DragDropHelper that the current Control received
		/// a DragLeave event.
		/// </summary>
		public static void DragLeave()
		{
			s_instance.DragLeave();
		}

		/// <summary>
		/// Clears the drop description of the IDataObject previously associated to the
		/// provided control, then notifies the DragDropHelper that the current control
		/// received a DragLeave event.
		/// </summary>
		/// <remarks>
		/// Because the DragLeave event in SWF does not provide the IDataObject in the
		/// event args, this DragLeave override of the DropTargetHelper will lookup a
		/// cached copy of the IDataObject based on the provided Control and clear
		/// the drop description. Note that the underlying DragLeave call of the
		/// Shell IDropTargetHelper object keeps the current control cached, so the
		/// control passed to this method is only relevant to looking up the IDataObject
		/// on which to clear the drop description.
		/// </remarks>
		public static void DragLeave(Control control)
		{
			if (s_dataContext.ContainsKey(control))
			{
				s_dataContext[control].SetDropDescription(DropImageType.Invalid, null, null);
				s_dataContext.Remove(control);
			}

			DragLeave();
		}

		/// <summary>
		/// Notifies the DragDropHelper that the current Control received
		/// a DragOver event.
		/// </summary>
		/// <param name="data">The DataObject containing a drag image.</param>
		/// <param name="cursorOffset">The current cursor's offset relative to the window.</param>
		/// <param name="effect">The accepted drag drop effect.</param>
		public static void Drop(IDataObject data, System.Drawing.Point cursorOffset, DragDropEffects effect)
		{
			// No need to clear the drop description, but don't keep it stored to avoid memory leaks
			foreach (KeyValuePair<Control, IDataObject> pair in s_dataContext)
			{
				if (object.ReferenceEquals(pair.Value, data))
				{
					s_dataContext.Remove(pair);
					break;
				}
			}

			Win32Point pt = SwfDragDropLibExtensions.ToWin32Point(cursorOffset);
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

#endregion // SwfDragDropLib\SwfDropTargetHelper.cs

#region SwfDragDropLib\SwfDragSourceHelper.cs

namespace System.Windows.Forms
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.IO;
	using System.Runtime.InteropServices;
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
		private static IDictionary<Control, DragSourceEntry> s_dataContext = new Dictionary<Control, DragSourceEntry>();

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
			return new System.Windows.Forms.DataObject(new DragDropLib.DataObject());
		}

		/// <summary>
		/// Creates a DataObject with an internal COM callable implementation of IDataObject.
		/// This override also sets the drag image to the specified Bitmap and sets a flag
		/// on the system IDragSourceHelper2 to allow drop descriptions.
		/// </summary>
		/// <param name="dragImage">A Bitmap from which to create the drag image.</param>
		/// <param name="cursorOffset">The drag image cursor offset.</param>
		/// <returns>A new instance of System.Windows.Forms.IDataObject.</returns>
		public static IDataObject CreateDataObject(Bitmap dragImage, System.Drawing.Point cursorOffset)
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
		/// <param name="control">A Control to initialize the drag image from.</param>
		/// <param name="cursorOffset">The drag image cursor offset.</param>
		/// <returns>A new instance of System.Windows.Forms.IDataObject.</returns>
		public static IDataObject CreateDataObject(Control control, System.Drawing.Point cursorOffset)
		{
			IDataObject data = CreateDataObject();
			AllowDropDescription(true);
			data.SetDragImage(control, cursorOffset);
			return data;
		}

		/// <summary>
		/// Registers a Control as a drag source and provides default implementations of
		/// GiveFeedback and QueryContinueDrag.
		/// </summary>
		/// <param name="control">The drag source Control instance.</param>
		/// <param name="data">The DataObject associated to the drag source.</param>
		/// <remarks>Callers must call UnregisterDefaultDragSource when the drag and drop
		/// operation is complete to avoid memory leaks.</remarks>
		public static void RegisterDefaultDragSource(Control control, IDataObject data)
		{
			// Cache the drag source and the associated data object
			DragSourceEntry entry = new DragSourceEntry(data);
			if (!s_dataContext.ContainsKey(control))
				s_dataContext.Add(control, entry);
			else
				s_dataContext[control] = entry;

			// We need to listen for drop description changes. If a drop target
			// changes the drop description, we shouldn't provide a default one.
			entry.adviseConnection = ComTypes.ComDataObjectExtensions.Advise(((ComTypes.IDataObject)data), new AdviseSink(data), DropDescriptionFormat, 0);

			// Hook up the default drag source event handlers
			control.GiveFeedback += new GiveFeedbackEventHandler(DefaultGiveFeedbackHandler);
			control.QueryContinueDrag += new QueryContinueDragEventHandler(DefaultQueryContinueDragHandler);
		}

		/// <summary>
		/// Registers a Control as a drag source and provides default implementations of
		/// GiveFeedback and QueryContinueDrag. This override also handles the data object
		/// creation, including initialization of the drag image from the Control.
		/// </summary>
		/// <param name="control">The drag source Control instance.</param>
		/// <param name="cursorOffset">The drag image cursor offset.</param>
		/// <returns>The created data object.</returns>
		/// <remarks>Callers must call UnregisterDefaultDragSource when the drag and drop
		/// operation is complete to avoid memory leaks.</remarks>
		public static IDataObject RegisterDefaultDragSource(Control control, System.Drawing.Point cursorOffset)
		{
			IDataObject data = CreateDataObject(control, cursorOffset);
			RegisterDefaultDragSource(control, data);
			return data;
		}

		/// <summary>
		/// Registers a Control as a drag source and provides default implementations of
		/// GiveFeedback and QueryContinueDrag. This override also handles the data object
		/// creation, including initialization of the drag image from the speicified Bitmap.
		/// </summary>
		/// <param name="control">The drag source Control instance.</param>
		/// <param name="dragImage">A Bitmap to initialize the drag image from.</param>
		/// <param name="cursorOffset">The drag image cursor offset.</param>
		/// <returns>The created data object.</returns>
		/// <remarks>Callers must call UnregisterDefaultDragSource when the drag and drop
		/// operation is complete to avoid memory leaks.</remarks>
		public static IDataObject RegisterDefaultDragSource(Control control, Bitmap dragImage, System.Drawing.Point cursorOffset)
		{
			IDataObject data = CreateDataObject(dragImage, cursorOffset);
			RegisterDefaultDragSource(control, data);
			return data;
		}

		/// <summary>
		/// Unregisters a drag source from the internal cache.
		/// </summary>
		/// <param name="control">The drag source Control.</param>
		public static void UnregisterDefaultDragSource(Control control)
		{
			if (s_dataContext.ContainsKey(control))
			{
				DragSourceEntry entry = s_dataContext[control];
				ComTypes.IDataObject dataObjectCOM = (ComTypes.IDataObject)entry.data;

				// Stop listening to drop description changes
				dataObjectCOM.DUnadvise(entry.adviseConnection);

				// Unhook the default drag source event handlers
				control.GiveFeedback -= new GiveFeedbackEventHandler(DefaultGiveFeedbackHandler);
				control.QueryContinueDrag -= new QueryContinueDragEventHandler(DefaultQueryContinueDragHandler);

				// Remove the entries from our context caches
				s_dataContext.Remove(control);
				s_dropDescriptions.Remove(entry.data);
			}
		}

		/// <summary>
		/// Performs a default drag and drop operation for the specified drag source.
		/// </summary>
		/// <param name="control">The drag source Control.</param>
		/// <param name="cursorOffset">The drag image cursor offset.</param>
		/// <param name="allowedEffects">The allowed drop effects.</param>
		/// <param name="data">The associated data.</param>
		/// <returns>The accepted drop effects from the completed operation.</returns>
		public static DragDropEffects DoDragDrop(Control control, System.Drawing.Point cursorOffset, DragDropEffects allowedEffects, params KeyValuePair<string, object>[] data)
		{
			IDataObject dataObject = RegisterDefaultDragSource(control, cursorOffset);
			return DoDragDropInternal(control, dataObject, allowedEffects, data);
		}

		/// <summary>
		/// Performs a default drag and drop operation for the specified drag source.
		/// </summary>
		/// <param name="control">The drag source Control.</param>
		/// <param name="dragImage">The Bitmap to initialize the drag image from.</param>
		/// <param name="cursorOffset">The drag image cursor offset.</param>
		/// <param name="allowedEffects">The allowed drop effects.</param>
		/// <param name="data">The associated data.</param>
		/// <returns>The accepted drop effects from the completed operation.</returns>
		public static DragDropEffects DoDragDrop(Control control, Bitmap dragImage, System.Drawing.Point cursorOffset, DragDropEffects allowedEffects, params KeyValuePair<string, object>[] data)
		{
			IDataObject dataObject = RegisterDefaultDragSource(control, dragImage, cursorOffset);
			return DoDragDropInternal(control, dataObject, allowedEffects, data);
		}

		/// <summary>
		/// Performs a default drag and drop operation for the specified drag source.
		/// </summary>
		/// <param name="control">The drag source Control.</param>
		/// <param name="dataObject">The data object associated to the drag and drop operation.</param>
		/// <param name="allowedEffects">The allowed drop effects.</param>
		/// <param name="data">The associated data.</param>
		/// <returns>The accepted drop effects from the completed operation.</returns>
		private static DragDropEffects DoDragDropInternal(Control control, IDataObject dataObject, DragDropEffects allowedEffects, KeyValuePair<string, object>[] data)
		{
			// Set the data onto the data object.
			if (data != null)
			{
				foreach (KeyValuePair<string, object> dataPair in data)
					dataObject.SetDataEx(dataPair.Key, dataPair.Value);
			}

			try
			{
				return control.DoDragDrop(dataObject, allowedEffects);
			}
			finally
			{
				UnregisterDefaultDragSource(control);
			}
		}

		/// <summary>
		/// Provides a default GiveFeedback event handler for drag sources.
		/// </summary>
		/// <param name="sender">The object that raised the event. Should be set to the drag source.</param>
		/// <param name="e">The event arguments.</param>
		public static void DefaultGiveFeedbackHandler(object sender, GiveFeedbackEventArgs e)
		{
			Control control = sender as Control;
			if (control != null)
			{
				if (s_dataContext.ContainsKey(control))
				{
					DefaultGiveFeedback(s_dataContext[control].data, e);
				}
			}
		}

		/// <summary>
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
					Cursor.Current = Cursors.Arrow;
					SetDropDescriptionIsDefault(data, false);
				}
				//System.Threading.Thread.Sleep(200);
				//if (!Equals(data.GetDataEx(Eto.Wpf.Forms.WpfFrameworkElement.CustomCursor_DataKey), true))
				//Mouse.SetCursor(Cursors.Arrow);

				SetDragCursor(data, e.Effect);
				InvalidateDragImage(data);
			}
			else
			{
				e.UseDefaultCursors = true;
				SetDropDescriptionIsDefault(data, true);
			}
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
		private static void SetDropDescriptionIsDefault(IDataObject dataObject, bool isDefault)
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

#endregion // SwfDragDropLib\SwfDragSourceHelper.cs


