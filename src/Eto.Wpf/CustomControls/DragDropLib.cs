
// originally from : https://blogs.msdn.microsoft.com/adamroot/2008/02/19/shell-style-drag-and-drop-in-net-part-3/
// downloads link from: https://stackoverflow.com/questions/36239516/real-implementation-of-the-shell-style-drag-and-drop-in-net-wpf-and-winforms
// modified to properly set standard cursors without hard coding copy/link/move/etc labels.

#region DragDropLibCore\DragDropHelper.cs

namespace DragDropLib
{
	using System;
	using System.Runtime.InteropServices;

	[ComImport]
	[Guid("4657278A-411B-11d2-839A-00C04FD918D0")]
	class DragDropHelper { }
}

#endregion // DragDropLibCore\DragDropHelper.cs

#region DragDropLibCore\IDragSourceHelper.cs

namespace DragDropLib
{
	using System;
	using System.Runtime.InteropServices;
	using System.Runtime.InteropServices.ComTypes;

	[ComVisible(true)]
	[ComImport]
	[Guid("DE5BF786-477A-11D2-839D-00C04FD918D0")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	interface IDragSourceHelper
	{
		void InitializeFromBitmap(
			[In, MarshalAs(UnmanagedType.Struct)] ref ShDragImage dragImage,
			[In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject);

		unsafe void InitializeFromWindow(
			[In] IntPtr hwnd,
			[In] Win32Point* pt,
			[In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject);
	}

	[ComVisible(true)]
	[ComImport]
	[Guid("83E07D0D-0C5F-4163-BF1A-60B274051E40")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	interface IDragSourceHelper2
	{
		void InitializeFromBitmap(
			[In, MarshalAs(UnmanagedType.Struct)] ref ShDragImage dragImage,
			[In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject);

		void InitializeFromWindow(
			[In] IntPtr hwnd,
			[In] ref Win32Point pt,
			[In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject);

		void SetFlags(
			[In] int dwFlags);
	}
}

#endregion // DragDropLibCore\IDragSourceHelper.cs

#region DragDropLibCore\IDropTargetHelper.cs

namespace DragDropLib
{
	using System;
	using System.Runtime.InteropServices;
	using System.Runtime.InteropServices.ComTypes;

	[ComVisible(true)]
	[ComImport]
	[Guid("4657278B-411B-11D2-839A-00C04FD918D0")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	interface IDropTargetHelper
	{
		void DragEnter(
			[In] IntPtr hwndTarget,
			[In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject,
			[In] ref Win32Point pt,
			[In] int effect);

		void DragLeave();

		void DragOver(
			[In] ref Win32Point pt,
			[In] int effect);

		void Drop(
			[In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject,
			[In] ref Win32Point pt,
			[In] int effect);

		void Show(
			[In] bool show);
	}
}

#endregion // DragDropLibCore\IDropTargetHelper.cs

#region DragDropLibCore\NativeStructures.cs

namespace DragDropLib
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	struct Win32Point
	{
		public int x;
		public int y;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct Win32Size
	{
		public int cx;
		public int cy;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct ShDragImage
	{
		public Win32Size sizeDragImage;
		public Win32Point ptOffset;
		public IntPtr hbmpDragImage;
		public int crColorKey;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 1044)]
	struct DropDescription
	{
		public int type;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szMessage;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szInsert;
	}
}

#endregion // DragDropLibCore\NativeStructures.cs

#region DragDropLibCore\DataObjectExtensions.cs

namespace System.Runtime.InteropServices.ComTypes
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Runtime.InteropServices.ComTypes;
	using System.Text;
	using DragDropLib;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;

	/// <summary>
	/// Provides extended functionality for the COM IDataObject interface.
	/// </summary>
	static class ComDataObjectExtensions
	{
		#region DLL imports

		[DllImport("user32.dll")]
		private static extern uint RegisterClipboardFormat(string lpszFormatName);

		[DllImport("ole32.dll")]
		private static extern void ReleaseStgMedium(ref ComTypes.STGMEDIUM pmedium);

		[DllImport("ole32.dll")]
		private static extern int CreateStreamOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease, out IStream ppstm);

		#endregion // DLL imports

		#region Native constants

		// CFSTR_DROPDESCRIPTION
		private const string DropDescriptionFormat = "DropDescription";

		#endregion // Native constants

		/// <summary>
		/// Sets the drop description for the drag image manager.
		/// </summary>
		/// <param name="dataObject">TheSome text DataObject to set.</param>
		/// <param name="dropDescription">The drop description.</param>
		public static void SetDropDescription(this IDataObject dataObject, DropDescription dropDescription)
		{
			ComTypes.FORMATETC formatETC;
			FillFormatETC(DropDescriptionFormat, TYMED.TYMED_HGLOBAL, out formatETC);

			// We need to set the drop description as an HSome textGLOBAL.
			// Allocate space ...
			IntPtr pDD = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DropDescription)));
			try
			{
				// ... and marshal the data
				Marshal.StructureToPtr(dropDescription, pDD, false);

				// The medium wraps the HGLOBAL
				System.Runtime.InteropServices.ComTypes.STGMEDIUM medium;
				medium.pUnkForRelease = null;
				medium.tymed = ComTypes.TYMED.TYMED_HGLOBAL;
				medium.unionmember = pDD;

				// Set the data
				ComTypes.IDataObject dataObjectCOM = (ComTypes.IDataObject)dataObject;
				dataObjectCOM.SetData(ref formatETC, ref medium, true);
			}
			catch
			{
				// If we failed, we need to free the HGLOBAL memory
				Marshal.FreeHGlobal(pDD);
				throw;
			}
		}

		/// <summary>
		/// Gets the DropDescription format data.
		/// </summary>
		/// <param name="dataObject">The DataObject.</param>
		/// <returns>The DropDescription, if set.</returns>
		public static object GetDropDescription(this IDataObject dataObject)
		{
			ComTypes.FORMATETC formatETC;
			FillFormatETC(DropDescriptionFormat, TYMED.TYMED_HGLOBAL, out formatETC);

			if (0 == dataObject.QueryGetData(ref formatETC))
			{
				ComTypes.STGMEDIUM medium;
				dataObject.GetData(ref formatETC, out medium);
				try
				{
					return (DropDescription)Marshal.PtrToStructure(medium.unionmember, typeof(DropDescription));
				}
				finally
				{
					ReleaseStgMedium(ref medium);
				}
			}

			return null;
		}

		// Combination of all non-null TYMEDs
		private const TYMED TYMED_ANY =
			TYMED.TYMED_ENHMF
			| TYMED.TYMED_FILE
			| TYMED.TYMED_GDI
			| TYMED.TYMED_HGLOBAL
			| TYMED.TYMED_ISTORAGE
			| TYMED.TYMED_ISTREAM
			| TYMED.TYMED_MFPICT;

		/// <summary>
		/// Sets up an advisory connection to the data object.
		/// </summary>
		/// <param name="dataObject">The data object on which to set the advisory connection.</param>
		/// <param name="sink">The advisory sink.</param>
		/// <param name="format">The format on which to callback on.</param>
		/// <param name="advf">Advisory flags. Can be 0.</param>
		/// <returns>The ID of the newly created advisory connection.</returns>
		public static int Advise(this IDataObject dataObject, IAdviseSink sink, string format, ADVF advf)
		{
			// Internally, we'll listen for any TYMED
			FORMATETC formatETC;
			FillFormatETC(format, TYMED_ANY, out formatETC);

			int connection;
			int hr = dataObject.DAdvise(ref formatETC, advf, sink, out connection);
			if (hr != 0)
				Marshal.ThrowExceptionForHR(hr);
			return connection;
		}

		/// <summary>
		/// Fills a FORMATETC structure.
		/// </summary>
		/// <param name="format">The format name.</param>
		/// <param name="tymed">The accepted TYMED.</param>
		/// <param name="formatETC">The structure to fill.</param>
		private static void FillFormatETC(string format, TYMED tymed, out FORMATETC formatETC)
		{
			formatETC.cfFormat = (short)RegisterClipboardFormat(format);
			formatETC.dwAspect = DVASPECT.DVASPECT_CONTENT;
			formatETC.lindex = -1;
			formatETC.ptd = IntPtr.Zero;
			formatETC.tymed = tymed;
		}

		// Identifies data that we need to do custom marshaling on
		private static readonly Guid ManagedDataStamp = new Guid("D98D9FD6-FA46-4716-A769-F3451DFBE4B4");

		public static void SetByteData(this IDataObject dataObject, string format, byte[] data)
		{
			// Initialize the format structure
			ComTypes.FORMATETC formatETC;
			FillFormatETC(format, TYMED.TYMED_HGLOBAL, out formatETC);

			ComTypes.STGMEDIUM medium;
			GetMediumFromByteArray(data, out medium);
			try
			{
				// Set the data on our data object
				dataObject.SetData(ref formatETC, ref medium, true);
			}
			catch
			{
				// On exceptions, release the medium
				ReleaseStgMedium(ref medium);
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
		public static void SetManagedData(this IDataObject dataObject, string format, object data)
		{
			// Initialize the format structure
			ComTypes.FORMATETC formatETC;
			FillFormatETC(format, TYMED.TYMED_HGLOBAL, out formatETC);

			// Serialize/marshal our data into an unmanaged medium
			ComTypes.STGMEDIUM medium;
			GetMediumFromObject(data, out medium);
			try
			{
				// Set the data on our data object
				dataObject.SetData(ref formatETC, ref medium, true);
			}
			catch
			{
				// On exceptions, release the medium
				ReleaseStgMedium(ref medium);
				throw;
			}
		}

		/// <summary>
		/// Gets managed data from a clipboard DataObject.
		/// </summary>
		/// <param name="dataObject">The DataObject to obtain the data from.</param>
		/// <param name="format">The format for which to get the data in.</param>
		/// <returns>The data object instance.</returns>
		public static object GetManagedData(this IDataObject dataObject, string format)
		{
			FORMATETC formatETC;
			FillFormatETC(format, TYMED.TYMED_HGLOBAL, out formatETC);

			// Get the data as a stream
			STGMEDIUM medium;
			dataObject.GetData(ref formatETC, out medium);

			IStream nativeStream;
			try
			{
				int hr = CreateStreamOnHGlobal(medium.unionmember, true, out nativeStream);
				if (hr != 0)
				{
					return null;
				}
			}
			finally
			{
				ReleaseStgMedium(ref medium);
			}


			// Convert the native stream to a managed stream            
			STATSTG statstg;
			nativeStream.Stat(out statstg, 0);
			if (statstg.cbSize > int.MaxValue)
				throw new NotSupportedException();
			byte[] buf = new byte[statstg.cbSize];
			nativeStream.Read(buf, (int)statstg.cbSize, IntPtr.Zero);
			MemoryStream dataStream = new MemoryStream(buf);

			// Check for our stamp
			int sizeOfGuid = Marshal.SizeOf(typeof(Guid));
			byte[] guidBytes = new byte[sizeOfGuid];
			if (dataStream.Length >= sizeOfGuid)
			{
				if (sizeOfGuid == dataStream.Read(guidBytes, 0, sizeOfGuid))
				{
					Guid guid = new Guid(guidBytes);
					if (ManagedDataStamp.Equals(guid))
					{
						// Stamp matched, so deserialize
						BinaryFormatter formatter = new BinaryFormatter();
						Type dataType = (Type)formatter.Deserialize(dataStream);
						object data2 = formatter.Deserialize(dataStream);
						if (data2.GetType() == dataType)
							return data2;
						else if (data2 is string)
							return ConvertDataFromString((string)data2, dataType);
						else
							return null;
					}
				}
			}

			// Stamp didn't match... attempt to reset the seek pointer
			if (dataStream.CanSeek)
				dataStream.Position = 0;
			return null;
		}

		#region Helper methods

		private static void GetMediumFromByteArray(byte[] bytes, out STGMEDIUM medium)
		{
			// Now copy to an HGLOBAL
			IntPtr p = Marshal.AllocHGlobal(bytes.Length);
			try
			{
				Marshal.Copy(bytes, 0, p, bytes.Length);
			}
			catch
			{
				// Make sure to free the memory on exceptions
				Marshal.FreeHGlobal(p);
				throw;
			}

			// Now allocate an STGMEDIUM to wrap the HGLOBAL
			medium.unionmember = p;
			medium.tymed = ComTypes.TYMED.TYMED_HGLOBAL;
			medium.pUnkForRelease = null;
		}

		/// <summary>
		/// Serializes managed data to an HGLOBAL.
		/// </summary>
		/// <param name="data">The managed data object.</param>
		/// <returns>An STGMEDIUM pointing to the allocated HGLOBAL.</returns>
		private static void GetMediumFromObject(object data, out STGMEDIUM medium)
		{
			// We'll serialize to a managed stream temporarily
			MemoryStream stream = new MemoryStream();

			// Write an indentifying stamp, so we can recognize this as custom
			// marshaled data.
			stream.Write(ManagedDataStamp.ToByteArray(), 0, Marshal.SizeOf(typeof(Guid)));

			// Now serialize the data. Note, if the data is not directly serializable,
			// we'll try type conversion. Also, we serialize the type. That way,
			// during deserialization, we know which type to convert back to, if
			// appropriate.
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, data.GetType());
			formatter.Serialize(stream, GetAsSerializable(data));

			// Now copy to an HGLOBAL
			byte[] bytes = stream.GetBuffer();
			IntPtr p = Marshal.AllocHGlobal(bytes.Length);
			try
			{
				Marshal.Copy(bytes, 0, p, bytes.Length);
			}
			catch
			{
				// Make sure to free the memory on exceptions
				Marshal.FreeHGlobal(p);
				throw;
			}

			// Now allocate an STGMEDIUM to wrap the HGLOBAL
			medium.unionmember = p;
			medium.tymed = ComTypes.TYMED.TYMED_HGLOBAL;
			medium.pUnkForRelease = null;
		}

		/// <summary>
		/// Gets a serializable object representing the data.
		/// </summary>
		/// <param name="obj">The data.</param>
		/// <returns>If the data is serializable, then it is returned. Otherwise,
		/// type conversion is attempted. If successful, a string value will be
		/// returned.</returns>
		private static object GetAsSerializable(object obj)
		{
			// If the data is directly serializable, run with it
			if (obj.GetType().IsSerializable)
				return obj;

			// Attempt type conversion to a string, but only if we know it can be converted back
			TypeConverter conv = GetTypeConverterForType(obj.GetType());
			if (conv != null && conv.CanConvertTo(typeof(string)) && conv.CanConvertFrom(typeof(string)))
				return conv.ConvertToInvariantString(obj);

			throw new NotSupportedException("Cannot serialize the object");
		}

		/// <summary>
		/// Converts data from a string to the specified format.
		/// </summary>
		/// <param name="data">The data to convert.</param>
		/// <param name="dataType">The target data type.</param>
		/// <returns>Returns the converted data instance.</returns>
		private static object ConvertDataFromString(string data, Type dataType)
		{
			TypeConverter conv = GetTypeConverterForType(dataType);
			if (conv != null && conv.CanConvertFrom(typeof(string)))
				return conv.ConvertFromInvariantString(data);

			throw new NotSupportedException("Cannot convert data");
		}

		/// <summary>
		/// Gets a TypeConverter instance for the specified type.
		/// </summary>
		/// <param name="dataType">The type.</param>
		/// <returns>An instance of a TypeConverter for the type, if one exists.</returns>
		private static TypeConverter GetTypeConverterForType(Type dataType)
		{
			TypeConverterAttribute[] typeConverterAttrs = (TypeConverterAttribute[])dataType.GetCustomAttributes(typeof(TypeConverterAttribute), true);
			if (typeConverterAttrs.Length > 0)
			{
				Type convType = Type.GetType(typeConverterAttrs[0].ConverterTypeName);
				return (TypeConverter)Activator.CreateInstance(convType);
			}

			return null;
		}

		#endregion // Helper methods
	}
}

#endregion // DragDropLibCore\DataObjectExtensions.cs

#region DragDropLibCore\DataObject.cs

namespace DragDropLib
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Runtime.InteropServices.ComTypes;

	/// <summary>
	/// Implements the COM version of IDataObject including SetData.
	/// </summary>
	/// <remarks>
	/// <para>Use this object when using shell (or other unmanged) features
	/// that utilize the clipboard and/or drag and drop.</para>
	/// <para>The System.Windows.DataObject (.NET 3.0) and
	/// System.Windows.Forms.DataObject do not support SetData from their COM
	/// IDataObject interface implementation.</para>
	/// <para>To use this object with .NET drag and drop, create an instance
	/// of System.Windows.DataObject (.NET 3.0) or System.Window.Forms.DataObject
	/// passing an instance of DataObject as the only constructor parameter. For
	/// example:</para>
	/// <code>
	/// System.Windows.DataObject data = new System.Windows.DataObject(new DragDropLib.DataObject());
	/// </code>
	/// </remarks>
	[ComVisible(true)]
	class DataObject : IDataObject, IDisposable
	{
		#region Unmanaged functions

		// These are helper functions for managing STGMEDIUM structures

		[DllImport("urlmon.dll")]
		private static extern int CopyStgMedium(ref STGMEDIUM pcstgmedSrc, ref STGMEDIUM pstgmedDest);
		[DllImport("ole32.dll")]
		private static extern void ReleaseStgMedium(ref STGMEDIUM pmedium);

		#endregion // Unmanaged functions

		// Our internal storage is a simple list
		private IList<KeyValuePair<FORMATETC, STGMEDIUM>> storage;

		// Keeps a progressive unique connection id
		private int nextConnectionId = 1;

		// List of advisory connections
		private IDictionary<int, AdviseEntry> connections;

		// Represents an advisory connection entry.
		private class AdviseEntry
		{
			public FORMATETC format;
			public ADVF advf;
			public IAdviseSink sink;

			public AdviseEntry(ref FORMATETC format, ADVF advf, IAdviseSink sink)
			{
				this.format = format;
				this.advf = advf;
				this.sink = sink;
			}
		}

		/// <summary>
		/// Creates an empty instance of DataObject.
		/// </summary>
		public DataObject()
		{
			storage = new List<KeyValuePair<FORMATETC, STGMEDIUM>>();
			connections = new Dictionary<int, AdviseEntry>();
		}

		/// <summary>
		/// Releases unmanaged resources.
		/// </summary>
		~DataObject()
		{
			Dispose(false);
		}

		/// <summary>
		/// Clears the internal storage array.
		/// </summary>
		/// <remarks>
		/// ClearStorage is called by the IDisposable.Dispose method implementation
		/// to make sure all unmanaged references are released properly.
		/// </remarks>
		private void ClearStorage()
		{
			lock (storage)
			{
				foreach (KeyValuePair<FORMATETC, STGMEDIUM> pair in storage)
				{
					STGMEDIUM medium = pair.Value;
					ReleaseStgMedium(ref medium);
				}
				storage.Clear();
			}
		}

		/// <summary>
		/// Releases resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Releases resources.
		/// </summary>
		/// <param name="disposing">Indicates if the call was made by a managed caller, or the garbage collector.
		/// True indicates that someone called the Dispose method directly. False indicates that the garbage collector
		/// is finalizing the release of the object instance.</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				// No managed objects to release
			}

			// Always release unmanaged objects
			ClearStorage();
		}

		#region COM IDataObject Members

		#region COM constants

		private const int OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003);

		private const int DV_E_FORMATETC = unchecked((int)0x80040064);
		private const int DV_E_TYMED = unchecked((int)0x80040069);
		private const int DV_E_CLIPFORMAT = unchecked((int)0x8004006A);
		private const int DV_E_DVASPECT = unchecked((int)0x8004006B);

		#endregion // COM constants

		#region Unsupported functions

		public int EnumDAdvise(out IEnumSTATDATA enumAdvise)
		{
			throw Marshal.GetExceptionForHR(OLE_E_ADVISENOTSUPPORTED);
		}

		public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
		{
			formatOut = formatIn;
			return DV_E_FORMATETC;
		}

		#endregion // Unsupported functions

		/// <summary>
		/// Adds an advisory connection for the specified format.
		/// </summary>
		/// <param name="pFormatetc">The format for which this sink is called for changes.</param>
		/// <param name="advf">Advisory flags to specify callback behavior.</param>
		/// <param name="adviseSink">The IAdviseSink to call for this connection.</param>
		/// <param name="connection">Returns the new connection's ID.</param>
		/// <returns>An HRESULT.</returns>
		public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
		{
			// Check that the specified advisory flags are supported.
			const ADVF ADVF_ALLOWED = ADVF.ADVF_NODATA | ADVF.ADVF_ONLYONCE | ADVF.ADVF_PRIMEFIRST;
			if ((int)((advf | ADVF_ALLOWED) ^ ADVF_ALLOWED) != 0)
			{
				connection = 0;
				return OLE_E_ADVISENOTSUPPORTED;
			}

			// Create and insert an entry for the connection list
			AdviseEntry entry = new AdviseEntry(ref pFormatetc, advf, adviseSink);
			connections.Add(nextConnectionId, entry);
			connection = nextConnectionId;
			nextConnectionId++;

			// If the ADVF_PRIMEFIRST flag is specified and the data exists,
			// raise the DataChanged event now.
			if ((advf & ADVF.ADVF_PRIMEFIRST) == ADVF.ADVF_PRIMEFIRST)
			{
				KeyValuePair<FORMATETC, STGMEDIUM> dataEntry;
				if (GetDataEntry(ref pFormatetc, out dataEntry))
					RaiseDataChanged(connection, ref dataEntry);
			}

			// S_OK
			return 0;
		}

		/// <summary>
		/// Removes an advisory connection.
		/// </summary>
		/// <param name="connection">The connection id to remove.</param>
		public void DUnadvise(int connection)
		{
			connections.Remove(connection);
		}

		/// <summary>
		/// Gets an enumerator for the formats contained in this DataObject.
		/// </summary>
		/// <param name="direction">The direction of the data.</param>
		/// <returns>An instance of the IEnumFORMATETC interface.</returns>
		public IEnumFORMATETC EnumFormatEtc(DATADIR direction)
		{
			// We only support GET
			if (DATADIR.DATADIR_GET == direction)
				return new EnumFORMATETC(storage);

			throw new NotImplementedException("OLE_S_USEREG");
		}

		/// <summary>
		/// Gets the specified data.
		/// </summary>
		/// <param name="format">The requested data format.</param>
		/// <param name="medium">When the function returns, contains the requested data.</param>
		public void GetData(ref FORMATETC format, out STGMEDIUM medium)
		{
			medium = new STGMEDIUM();
			GetDataHere(ref format, ref medium);
		}

		/// <summary>
		/// Gets the specified data.
		/// </summary>
		/// <param name="format">The requested data format.</param>
		/// <param name="medium">When the function returns, contains the requested data.</param>
		/// <remarks>Differs from GetData only in that the STGMEDIUM storage is
		/// allocated and owned by the caller.</remarks>
		public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
		{
			// Locate the data
			KeyValuePair<FORMATETC, STGMEDIUM> dataEntry;
			if (GetDataEntry(ref format, out dataEntry))
			{
				STGMEDIUM source = dataEntry.Value;
				medium = CopyMedium(ref source);
				return;
			}

			// Didn't find it. Return an empty data medium.
			medium = default(STGMEDIUM);
		}

		/// <summary>
		/// Determines if data of the requested format is present.
		/// </summary>
		/// <param name="format">The request data format.</param>
		/// <returns>Returns the status of the request. If the data is present, S_OK is returned.
		/// If the data is not present, an error code with the best guess as to the reason is returned.</returns>
		public int QueryGetData(ref FORMATETC format)
		{
			// We only support CONTENT aspect
			if ((DVASPECT.DVASPECT_CONTENT & format.dwAspect) == 0)
				return DV_E_DVASPECT;

			int ret = DV_E_TYMED;

			lock (storage)
			{
				// Try to locate the data
				// TODO: The ret, if not S_OK, is only relevant to the last item
				foreach (KeyValuePair<FORMATETC, STGMEDIUM> pair in storage)
				{
					if ((pair.Key.tymed & format.tymed) > 0)
					{
						if (pair.Key.cfFormat == format.cfFormat)
						{
							// Found it, return S_OK;
							return 0;
						}
						else
						{
							// Found the medium type, but wrong format
							ret = DV_E_CLIPFORMAT;
						}
					}
					else
					{
						// Mismatch on medium type
						ret = DV_E_TYMED;
					}
				}
			}

			return ret;
		}

		/// <summary>
		/// Sets data in the specified format into storage.
		/// </summary>
		/// <param name="formatIn">The format of the data.</param>
		/// <param name="medium">The data.</param>
		/// <param name="release">If true, ownership of the medium's memory will be transferred
		/// to this object. If false, a copy of the medium will be created and maintained, and
		/// the caller is responsible for the memory of the medium it provided.</param>
		public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
		{
			lock (storage)
			{
				// If the format exists in our storage, remove it prior to resetting it
				foreach (KeyValuePair<FORMATETC, STGMEDIUM> pair in storage)
				{
					if ((pair.Key.tymed & formatIn.tymed) > 0
						&& pair.Key.dwAspect == formatIn.dwAspect
						&& pair.Key.cfFormat == formatIn.cfFormat)
					{
						STGMEDIUM releaseMedium = pair.Value;
						ReleaseStgMedium(ref releaseMedium);
						storage.Remove(pair);
						break;
					}
				}

				// If release is true, we'll take ownership of the medium.
				// If not, we'll make a copy of it.
				STGMEDIUM sm = medium;
				if (!release)
					sm = CopyMedium(ref medium);

				// Add it to the internal storage
				KeyValuePair<FORMATETC, STGMEDIUM> addPair = new KeyValuePair<FORMATETC, STGMEDIUM>(formatIn, sm);
				storage.Add(addPair);
				RaiseDataChanged(ref addPair);
			}
		}

		/// <summary>
		/// Creates a copy of the STGMEDIUM structure.
		/// </summary>
		/// <param name="medium">The data to copy.</param>
		/// <returns>The copied data.</returns>
		private STGMEDIUM CopyMedium(ref STGMEDIUM medium)
		{
			STGMEDIUM sm = new STGMEDIUM();
			int hr = CopyStgMedium(ref medium, ref sm);
			if (hr != 0)
				throw Marshal.GetExceptionForHR(hr);

			return sm;
		}

		#endregion

		#region Helper methods

		/// <summary>
		/// Gets a data entry by the specified format.
		/// </summary>
		/// <param name="pFormatetc">The format to locate the data entry for.</param>
		/// <param name="dataEntry">The located data entry.</param>
		/// <returns>True if the data entry was found, otherwise False.</returns>
		private bool GetDataEntry(ref FORMATETC pFormatetc, out KeyValuePair<FORMATETC, STGMEDIUM> dataEntry)
		{
			lock (storage)
			{
				foreach (KeyValuePair<FORMATETC, STGMEDIUM> entry in storage)
				{
					FORMATETC format = entry.Key;
					if (IsFormatCompatible(ref pFormatetc, ref format))
					{
						dataEntry = entry;
						return true;
					}
				}
			}

			// Not found... default allocate the out param
			dataEntry = default(KeyValuePair<FORMATETC, STGMEDIUM>);
			return false;
		}

		/// <summary>
		/// Raises the DataChanged event for the specified connection.
		/// </summary>
		/// <param name="connection">The connection id.</param>
		/// <param name="dataEntry">The data entry for which to raise the event.</param>
		private void RaiseDataChanged(int connection, ref KeyValuePair<FORMATETC, STGMEDIUM> dataEntry)
		{
			AdviseEntry adviseEntry = connections[connection];
			FORMATETC format = dataEntry.Key;
			STGMEDIUM medium;
			if ((adviseEntry.advf & ADVF.ADVF_NODATA) != ADVF.ADVF_NODATA)
				medium = dataEntry.Value;
			else
				medium = default(STGMEDIUM);

			adviseEntry.sink.OnDataChange(ref format, ref medium);

			if ((adviseEntry.advf & ADVF.ADVF_ONLYONCE) == ADVF.ADVF_ONLYONCE)
				connections.Remove(connection);
		}

		/// <summary>
		/// Raises the DataChanged event for any advisory connections that
		/// are listening for it.
		/// </summary>
		/// <param name="dataEntry">The relevant data entry.</param>
		private void RaiseDataChanged(ref KeyValuePair<FORMATETC, STGMEDIUM> dataEntry)
		{
			foreach (KeyValuePair<int, AdviseEntry> connection in connections)
			{
				if (IsFormatCompatible(connection.Value.format, dataEntry.Key))
					RaiseDataChanged(connection.Key, ref dataEntry);
			}
		}

		/// <summary>
		/// Determines if the formats are compatible.
		/// </summary>
		/// <param name="format1">A FORMATETC.</param>
		/// <param name="format2">A FORMATETC.</param>
		/// <returns>True if the formats are compatible, otherwise False.</returns>
		/// <remarks>Compatible formats have the same DVASPECT, same format ID, and share
		/// at least one TYMED.</remarks>
		private bool IsFormatCompatible(FORMATETC format1, FORMATETC format2)
		{
			return IsFormatCompatible(ref format1, ref format2);
		}

		/// <summary>
		/// Determines if the formats are compatible.
		/// </summary>
		/// <param name="format1">A FORMATETC.</param>
		/// <param name="format2">A FORMATETC.</param>
		/// <returns>True if the formats are compatible, otherwise False.</returns>
		/// <remarks>Compatible formats have the same DVASPECT, same format ID, and share
		/// at least one TYMED.</remarks>
		private bool IsFormatCompatible(ref FORMATETC format1, ref FORMATETC format2)
		{
			return ((format1.tymed & format2.tymed) > 0
					&& format1.dwAspect == format2.dwAspect
					&& format1.cfFormat == format2.cfFormat);
		}

		#endregion // Helper methods

		#region EnumFORMATETC class

		/// <summary>
		/// Helps enumerate the formats available in our DataObject class.
		/// </summary>
		[ComVisible(true)]
		private class EnumFORMATETC : IEnumFORMATETC
		{
			// Keep an array of the formats for enumeration
			private FORMATETC[] formats;
			// The index of the next item
			private int currentIndex = 0;

			/// <summary>
			/// Creates an instance from a list of key value pairs.
			/// </summary>
			/// <param name="storage">List of FORMATETC/STGMEDIUM key value pairs</param>
			internal EnumFORMATETC(IList<KeyValuePair<FORMATETC, STGMEDIUM>> storage)
			{
				lock (storage)
				{
					// Get the formats from the list
					formats = new FORMATETC[storage.Count];
					for (int i = 0; i < formats.Length; i++)
						formats[i] = storage[i].Key;
				}
			}

			/// <summary>
			/// Creates an instance from an array of FORMATETC's.
			/// </summary>
			/// <param name="formats">Array of formats to enumerate.</param>
			private EnumFORMATETC(FORMATETC[] formats)
			{
				// Get the formats as a copy of the array
				this.formats = new FORMATETC[formats.Length];
				formats.CopyTo(this.formats, 0);
			}

			#region IEnumFORMATETC Members

			/// <summary>
			/// Creates a clone of this enumerator.
			/// </summary>
			/// <param name="newEnum">When this function returns, contains a new instance of IEnumFORMATETC.</param>
			public void Clone(out IEnumFORMATETC newEnum)
			{
				EnumFORMATETC ret = new EnumFORMATETC(formats);
				ret.currentIndex = currentIndex;
				newEnum = ret;
			}

			/// <summary>
			/// Retrieves the next elements from the enumeration.
			/// </summary>
			/// <param name="celt">The number of elements to retrieve.</param>
			/// <param name="rgelt">An array to receive the formats requested.</param>
			/// <param name="pceltFetched">An array to receive the number of element fetched.</param>
			/// <returns>If the fetched number of formats is the same as the requested number, S_OK is returned.
			/// There are several reasons S_FALSE may be returned: (1) The requested number of elements is less than
			/// or equal to zero. (2) The rgelt parameter equals null. (3) There are no more elements to enumerate.
			/// (4) The requested number of elements is greater than one and pceltFetched equals null or does not
			/// have at least one element in it. (5) The number of fetched elements is less than the number of
			/// requested elements.</returns>
			public int Next(int celt, FORMATETC[] rgelt, int[] pceltFetched)
			{
				// Start with zero fetched, in case we return early
				if (pceltFetched != null && pceltFetched.Length > 0)
					pceltFetched[0] = 0;

				// This will count down as we fetch elements
				int cReturn = celt;

				// Short circuit if they didn't request any elements, or didn't
				// provide room in the return array, or there are not more elements
				// to enumerate.
				if (celt <= 0 || rgelt == null || currentIndex >= formats.Length)
					return 1; // S_FALSE

				// If the number of requested elements is not one, then we must
				// be able to tell the caller how many elements were fetched.
				if ((pceltFetched == null || pceltFetched.Length < 1) && celt != 1)
					return 1; // S_FALSE

				// If the number of elements in the return array is too small, we
				// throw. This is not a likely scenario, hence the exception.
				if (rgelt.Length < celt)
					throw new ArgumentException("The number of elements in the return array is less than the number of elements requested");

				// Fetch the elements.
				for (int i = 0; currentIndex < formats.Length && cReturn > 0; i++, cReturn--, currentIndex++)
					rgelt[i] = formats[currentIndex];

				// Return the number of elements fetched
				if (pceltFetched != null && pceltFetched.Length > 0)
					pceltFetched[0] = celt - cReturn;

				// cReturn has the number of elements requested but not fetched.
				// It will be greater than zero, if multiple elements were requested
				// but we hit the end of the enumeration.
				return (cReturn == 0) ? 0 : 1; // S_OK : S_FALSE
			}

			/// <summary>
			/// Resets the state of enumeration.
			/// </summary>
			/// <returns>S_OK</returns>
			public int Reset()
			{
				currentIndex = 0;
				return 0; // S_OK
			}

			/// <summary>
			/// Skips the number of elements requested.
			/// </summary>
			/// <param name="celt">The number of elements to skip.</param>
			/// <returns>If there are not enough remaining elements to skip, returns S_FALSE. Otherwise, S_OK is returned.</returns>
			public int Skip(int celt)
			{
				if (currentIndex + celt > formats.Length)
					return 1; // S_FALSE

				currentIndex += celt;
				return 0; // S_OK
			}

			#endregion
		}

		#endregion // EnumFORMATETC class
	}
}

#endregion // DragDropLibCore\DataObject.cs

