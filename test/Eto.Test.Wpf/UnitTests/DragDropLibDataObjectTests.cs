using NUnit.Framework;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using ComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using DragDropDataObject = DragDropLib.DataObject;
using IStream = System.Runtime.InteropServices.ComTypes.IStream;
using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

namespace Eto.Test.Wpf.UnitTests
{
	/// <summary>
	/// Unit tests for DragDropLib.DataObject, the COM IDataObject implementation that WPF
	/// drag/drop is built on.
	/// </summary>
	/// <remarks>
	/// These cover the memory ownership invariants behind RH-95450 / RH-97411, where dragging a
	/// texture thumbnail a few times would take the process down with an access violation
	/// (surfaced as System.ExecutionEngineException) inside ReleaseStgMedium while the data object
	/// was being released. Every failure mode there was "something outside the data object ended
	/// the lifetime of a handle the data object still had in its storage list", so that is what
	/// these assert against.
	/// </remarks>
	[TestFixture]
	public class DragDropLibDataObjectTests
	{
		#region Win32

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		static extern uint RegisterClipboardFormat(string lpszFormatName);

		[DllImport("ole32.dll")]
		static extern void ReleaseStgMedium(ref ComTypes.STGMEDIUM pmedium);

		[DllImport("kernel32.dll")]
		static extern IntPtr GlobalLock(IntPtr hMem);

		[DllImport("kernel32.dll")]
		static extern bool GlobalUnlock(IntPtr hMem);

		#endregion

		#region Helpers

		static ComTypes.FORMATETC MakeFormat(string name) => new ComTypes.FORMATETC
		{
			cfFormat = (short)RegisterClipboardFormat(name),
			dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT,
			lindex = -1,
			ptd = IntPtr.Zero,
			tymed = ComTypes.TYMED.TYMED_HGLOBAL
		};

		static ComTypes.STGMEDIUM MakeMedium(byte[] bytes)
		{
			var handle = Marshal.AllocHGlobal(bytes.Length);
			Marshal.Copy(bytes, 0, handle, bytes.Length);
			return new ComTypes.STGMEDIUM
			{
				tymed = ComTypes.TYMED.TYMED_HGLOBAL,
				unionmember = handle,
				pUnkForRelease = null
			};
		}

		static ComTypes.STGMEDIUM MakeMedium(int value) => MakeMedium(BitConverter.GetBytes(value));

		/// <summary>
		/// Reads an HGLOBAL through GlobalLock, so it works whether the handle came from
		/// AllocHGlobal or from the duplicate CopyStgMedium/OleDuplicateData produces.
		/// </summary>
		static byte[] ReadMedium(IntPtr handle, int length)
		{
			var ptr = GlobalLock(handle);
			Assert.That(ptr, Is.Not.EqualTo(IntPtr.Zero), "Could not lock the HGLOBAL -- it is not a valid handle");
			try
			{
				var bytes = new byte[length];
				Marshal.Copy(ptr, bytes, 0, length);
				return bytes;
			}
			finally
			{
				GlobalUnlock(handle);
			}
		}

		static int ReadMediumInt(IntPtr handle) => BitConverter.ToInt32(ReadMedium(handle, sizeof(int)), 0);

		/// <summary>
		/// The handle held by the single entry in the data object's storage.
		/// </summary>
		static IntPtr OnlyStoredHandle(DragDropDataObject dataObject)
		{
			Assert.That(dataObject.storage, Has.Count.EqualTo(1), "expected exactly one entry in storage");
			return dataObject.storage[0].Value.unionmember;
		}

		/// <summary>
		/// An advise sink that deliberately misbehaves: IAdviseSink says the medium it is handed
		/// stays the caller's to release, so a sink that releases it anyway must not be able to
		/// reach the entry the data object still owns.
		/// </summary>
		class ReleasingAdviseSink : ComTypes.IAdviseSink
		{
			public int CallCount;
			public IntPtr LastHandle;

			public void OnDataChange(ref ComTypes.FORMATETC format, ref ComTypes.STGMEDIUM stgmedium)
			{
				CallCount++;
				LastHandle = stgmedium.unionmember;
				if (stgmedium.unionmember != IntPtr.Zero)
					ReleaseStgMedium(ref stgmedium);
			}

			public void OnViewChange(int aspect, int index) { }
			public void OnRename(ComTypes.IMoniker moniker) { }
			public void OnSave() { }
			public void OnClose() { }
		}

		/// <summary>
		/// An advise sink that fails, to make SetData throw after it has already stored the data.
		/// </summary>
		class ThrowingAdviseSink : ComTypes.IAdviseSink
		{
			public void OnDataChange(ref ComTypes.FORMATETC format, ref ComTypes.STGMEDIUM stgmedium)
				=> throw new InvalidOperationException("sink failed");

			public void OnViewChange(int aspect, int index) { }
			public void OnRename(ComTypes.IMoniker moniker) { }
			public void OnSave() { }
			public void OnClose() { }
		}

		#endregion

		[Test]
		public void SetDataTakingOwnershipShouldStoreItsOwnCopy()
		{
			var dataObject = new DragDropDataObject();
			var format = MakeFormat("eto-test-owned");
			var medium = MakeMedium(0x1234abcd);
			var callerHandle = medium.unionmember;

			((ComDataObject)dataObject).SetData(ref format, ref medium, true);

			Assert.That(OnlyStoredHandle(dataObject), Is.Not.EqualTo(callerHandle),
				"storage kept the caller's handle rather than a duplicate, so anything that frees the caller's handle leaves the entry dangling");
			Assert.That(ReadMediumInt(OnlyStoredHandle(dataObject)), Is.EqualTo(0x1234abcd),
				"the duplicate did not carry the data over");

			dataObject.Dispose();
		}

		[Test]
		public void SetDataWithoutOwnershipShouldStoreItsOwnCopy()
		{
			var dataObject = new DragDropDataObject();
			var format = MakeFormat("eto-test-unowned");
			var medium = MakeMedium(0x0badf00d);
			var callerHandle = medium.unionmember;

			((ComDataObject)dataObject).SetData(ref format, ref medium, false);

			Assert.That(OnlyStoredHandle(dataObject), Is.Not.EqualTo(callerHandle));
			Assert.That(ReadMediumInt(OnlyStoredHandle(dataObject)), Is.EqualTo(0x0badf00d));

			dataObject.Dispose();

			// release == false left the handle ours to free, and disposing must not have touched it
			Assert.That(ReadMediumInt(callerHandle), Is.EqualTo(0x0badf00d),
				"disposing the data object released the caller's handle even though ownership was not transferred");
			Marshal.FreeHGlobal(callerHandle);
		}

		[Test]
		public void SharingOneHandleAcrossFormatsShouldNotShareOwnership()
		{
			var dataObject = new DragDropDataObject();
			var data = (ComDataObject)dataObject;
			var medium = MakeMedium(0x0f0f0f0f);
			var callerHandle = medium.unionmember;

			// the same handle under two formats: the entries must own separate resources, or
			// ClearStorage releases one handle twice
			var formatA = MakeFormat("eto-test-shared-a");
			var formatB = MakeFormat("eto-test-shared-b");
			data.SetData(ref formatA, ref medium, false);
			data.SetData(ref formatB, ref medium, false);

			Assert.That(dataObject.storage, Has.Count.EqualTo(2));
			var handleA = dataObject.storage[0].Value.unionmember;
			var handleB = dataObject.storage[1].Value.unionmember;

			Assert.That(handleA, Is.Not.EqualTo(callerHandle));
			Assert.That(handleB, Is.Not.EqualTo(callerHandle));
			Assert.That(handleA, Is.Not.EqualTo(handleB),
				"both entries point at one handle, so releasing storage would free it twice");
			Assert.That(ReadMediumInt(handleA), Is.EqualTo(0x0f0f0f0f));
			Assert.That(ReadMediumInt(handleB), Is.EqualTo(0x0f0f0f0f));

			dataObject.Dispose();
			Marshal.FreeHGlobal(callerHandle);
		}

		[Test]
		public void GetDataForMissingFormatShouldReturnAnEmptyMedium()
		{
			var dataObject = new DragDropDataObject();
			var data = (ComDataObject)dataObject;
			var format = MakeFormat("eto-test-missing");
			var medium = default(ComTypes.STGMEDIUM);

			// Must NOT raise DV_E_FORMATETC. WPF and Eto read this object through in-process managed
			// calls without checking QueryGetData first, so an HRESULT here becomes a COMException in
			// the middle of a drag. An empty medium (TYMED_NULL) is the contract callers rely on.
			Assert.DoesNotThrow(() => data.GetData(ref format, out medium));
			Assert.That(medium.tymed, Is.EqualTo(ComTypes.TYMED.TYMED_NULL));
			Assert.That(medium.unionmember, Is.EqualTo(IntPtr.Zero));
			Assert.That(medium.pUnkForRelease, Is.Null);
			Assert.That(data.QueryGetData(ref format), Is.Not.EqualTo(0), "QueryGetData claimed the format is present");

			dataObject.Dispose();
		}

		[Test]
		public void ConsumerReleasingItsCopyShouldNotAffectStoredData()
		{
			// this is the shape of RH-95450: a drop target asks for the data, releases the medium
			// it was given, and the data object is then disposed
			var dataObject = new DragDropDataObject();
			var data = (ComDataObject)dataObject;
			var format = MakeFormat("eto-test-consumer");
			var medium = MakeMedium(0x11223344);
			data.SetData(ref format, ref medium, true);

			var storedHandle = OnlyStoredHandle(dataObject);

			for (int i = 0; i < 10; i++)
			{
				ComTypes.STGMEDIUM copy;
				data.GetData(ref format, out copy);

				Assert.That(copy.unionmember, Is.Not.EqualTo(IntPtr.Zero));
				Assert.That(copy.unionmember, Is.Not.EqualTo(storedHandle),
					"GetData handed out the stored handle itself, so the consumer releasing it frees data we still own");
				Assert.That(ReadMediumInt(copy.unionmember), Is.EqualTo(0x11223344));

				ReleaseStgMedium(ref copy);

				// the entry we still own has to survive the consumer releasing its copy
				Assert.That(OnlyStoredHandle(dataObject), Is.EqualTo(storedHandle));
				Assert.That(ReadMediumInt(storedHandle), Is.EqualTo(0x11223344), $"stored data was released by the consumer on pass {i}");
			}

			dataObject.Dispose();
		}

		[Test]
		public void AdviseSinkShouldNotBeAbleToReleaseStoredData()
		{
			var dataObject = new DragDropDataObject();
			var data = (ComDataObject)dataObject;
			var format = MakeFormat("eto-test-advise");
			var sink = new ReleasingAdviseSink();

			int connection;
			Assert.That(data.DAdvise(ref format, default(ComTypes.ADVF), sink, out connection), Is.EqualTo(0), "DAdvise failed");

			var medium = MakeMedium(0x5a5a5a5a);
			data.SetData(ref format, ref medium, true);

			Assert.That(sink.CallCount, Is.EqualTo(1), "the sink was not notified");
			Assert.That(sink.LastHandle, Is.Not.EqualTo(IntPtr.Zero), "the sink was handed an empty medium");
			Assert.That(sink.LastHandle, Is.Not.EqualTo(OnlyStoredHandle(dataObject)),
				"the sink was handed the live storage entry, so releasing it frees data the object still owns");
			Assert.That(ReadMediumInt(OnlyStoredHandle(dataObject)), Is.EqualTo(0x5a5a5a5a),
				"the sink releasing its medium freed the stored entry");

			data.DUnadvise(connection);
			dataObject.Dispose();
		}

		[Test]
		public void SetDataThatFailsShouldNotHaveReleasedTheCallersHandle()
		{
			// Callers that pass release: true free the medium themselves when SetData throws --
			// ComDataObjectExtensions.SetDropDescription and SetByteData both do. So a failure must
			// leave the caller's handle untouched, or their own cleanup double frees it.
			var dataObject = new DragDropDataObject();
			var data = (ComDataObject)dataObject;
			var format = MakeFormat("eto-test-throwing-sink");

			int connection;
			data.DAdvise(ref format, default(ComTypes.ADVF), new ThrowingAdviseSink(), out connection);

			var medium = MakeMedium(0x0abcdef0);
			var callerHandle = medium.unionmember;

			Assert.Throws<InvalidOperationException>(() => data.SetData(ref format, ref medium, true));

			Assert.That(ReadMediumInt(callerHandle), Is.EqualTo(0x0abcdef0),
				"SetData released the caller's medium before failing, so the caller's own cleanup would free it twice");

			Marshal.FreeHGlobal(callerHandle);
			dataObject.Dispose();
		}

		[Test]
		public void CallerCleanupAfterSetDataFailsShouldNotLeaveStorageDangling()
		{
			// The shape of RH-95450 / RH-97411. Every Eto WPF drag registers an advise sink on the
			// DropDescription format (DragSourceHelper.RegisterDefaultDragSource) and then calls
			// SetDropDescription with release: true on every DragOver. If SetData fails after it has
			// stored the entry, SetDropDescription's finally frees the HGLOBAL it passed in -- so if
			// storage kept that very handle instead of a duplicate, the entry is left dangling and
			// the ReleaseStgMedium in ClearStorage faults when the data object is finally released,
			// which is where the reported crash lands.
			var dataObject = new DragDropDataObject();
			var data = (ComDataObject)dataObject;
			var format = MakeFormat("eto-test-dangling");

			int connection;
			data.DAdvise(ref format, default(ComTypes.ADVF), new ThrowingAdviseSink(), out connection);

			var medium = MakeMedium(0x0d0a0d0a);
			var callerHandle = medium.unionmember;

			Assert.Throws<InvalidOperationException>(() => data.SetData(ref format, ref medium, true));

			// the entry was stored before the failure, and it must not be the caller's handle
			Assert.That(dataObject.storage, Has.Count.EqualTo(1));
			var storedHandle = dataObject.storage[0].Value.unionmember;
			Assert.That(storedHandle, Is.Not.EqualTo(callerHandle),
				"storage kept the caller's handle, so the caller's own cleanup leaves this entry dangling");

			// exactly what SetDropDescription's finally does when SetData throws
			Marshal.FreeHGlobal(callerHandle);

			// so releasing storage is still safe: this is the ClearStorage call that crashed
			Assert.That(ReadMediumInt(storedHandle), Is.EqualTo(0x0d0a0d0a),
				"the caller's cleanup freed the handle that storage is still holding");
			Assert.DoesNotThrow(() => dataObject.Dispose());
		}

		[Test]
		public void MultipleOnlyOnceAdviseSinksShouldEachBeNotifiedOnce()
		{
			var dataObject = new DragDropDataObject();
			var data = (ComDataObject)dataObject;
			var format = MakeFormat("eto-test-onlyonce");
			var first = new ReleasingAdviseSink();
			var second = new ReleasingAdviseSink();

			int connection;
			data.DAdvise(ref format, ComTypes.ADVF.ADVF_ONLYONCE, first, out connection);
			data.DAdvise(ref format, ComTypes.ADVF.ADVF_ONLYONCE, second, out connection);

			// ADVF_ONLYONCE drops each connection as it fires, which must not disturb the walk
			var medium = MakeMedium(1);
			Assert.DoesNotThrow(() => data.SetData(ref format, ref medium, true));
			Assert.That(first.CallCount, Is.EqualTo(1));
			Assert.That(second.CallCount, Is.EqualTo(1));

			// and both connections are spent now
			var secondMedium = MakeMedium(2);
			data.SetData(ref format, ref secondMedium, true);
			Assert.That(first.CallCount, Is.EqualTo(1), "an ADVF_ONLYONCE sink fired twice");
			Assert.That(second.CallCount, Is.EqualTo(1), "an ADVF_ONLYONCE sink fired twice");

			dataObject.Dispose();
		}

		[Test]
		public void DisposingTwiceShouldNotReleaseTwice()
		{
			var dataObject = new DragDropDataObject();
			var format = MakeFormat("eto-test-dispose");
			var medium = MakeMedium(7);
			((ComDataObject)dataObject).SetData(ref format, ref medium, true);

			dataObject.Dispose();
			Assert.That(dataObject.storage, Is.Empty);

			// a second pass must not release the handles the first pass already released
			Assert.DoesNotThrow(() => dataObject.Dispose());
			Assert.That(dataObject.storage, Is.Empty);
		}

		[Test]
		public void CollectingDataObjectsShouldNotCorruptTheHeap()
		{
			// the finalizer used to run ClearStorage a second time after Dispose had already
			// released everything. Mix disposed and abandoned instances, then force the finalizer.
			var format = MakeFormat("eto-test-collect");
			for (int i = 0; i < 50; i++)
			{
				var dataObject = new DragDropDataObject();
				var medium = MakeMedium(i);
				((ComDataObject)dataObject).SetData(ref format, ref medium, true);
				if (i % 2 == 0)
					dataObject.Dispose();
			}

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			GC.WaitForPendingFinalizers();

			// if the heap survived that, a fresh round trip still works
			var survivor = new DragDropDataObject();
			var lastMedium = MakeMedium(0x600d600d);
			((ComDataObject)survivor).SetData(ref format, ref lastMedium, true);
			Assert.That(ReadMediumInt(OnlyStoredHandle(survivor)), Is.EqualTo(0x600d600d));
			survivor.Dispose();
		}

		[Test]
		public void GetManagedDataShouldNotFreeTheMediumTwice()
		{
			// GetManagedData used to wrap the medium's HGLOBAL in an IStream with
			// fDeleteOnRelease: true and also call ReleaseStgMedium on the medium, freeing the
			// handle while the stream still owned it -- a use after free while reading, then a
			// second free when the stream's RCW was collected.
			const string formatName = "eto-test-managed";
			var format = MakeFormat(formatName);

			// deliberately not stamped as custom marshaled managed data, so GetManagedData runs
			// the whole HGLOBAL -> IStream -> read path and then returns null on the stamp mismatch
			var payload = Enumerable.Range(0, 32).Select(r => (byte)(r + 1)).ToArray();

			var dataObject = new DragDropDataObject();
			var data = (ComDataObject)dataObject;

			for (int i = 0; i < 50; i++)
			{
				var medium = MakeMedium(payload);
				data.SetData(ref format, ref medium, true);

				Assert.That(ComTypes.ComDataObjectExtensions.GetManagedData(data, formatName), Is.Null,
					"unstamped data should not deserialize to anything");

				// the entry GetManagedData just read from has to still be intact
				Assert.That(ReadMedium(OnlyStoredHandle(dataObject), payload.Length), Is.EqualTo(payload),
					$"reading the managed data released the stored medium on pass {i}");
			}

			// force the stream RCWs to finalize, which is when the second free used to land
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			Assert.That(ReadMedium(OnlyStoredHandle(dataObject), payload.Length), Is.EqualTo(payload));
			dataObject.Dispose();
		}

		[Test]
		public void GetManagedDataForMissingFormatShouldReturnNull()
		{
			var dataObject = new DragDropDataObject();
			var data = (ComDataObject)dataObject;

			Assert.That(ComTypes.ComDataObjectExtensions.GetManagedData(data, "eto-test-managed-missing"), Is.Null);

			dataObject.Dispose();
		}

		#region COM pointer / thread affinity

		// Not everything in storage is plain memory. The shell's drag image manager parks its drag
		// context in the data object as a TYMED_ISTREAM, so ReleaseStgMedium ends up calling Release
		// straight through a raw interface pointer -- no proxy, no apartment switch. Running that on
		// the finalizer thread (which is MTA) tears an STA object down on the wrong thread, and takes
		// the process out with an access violation inside ReleaseStgMedium in ClearStorage, with
		// ~DataObject on the stack. That is RH-95450 / RH-97411.

		/// <summary>
		/// A no-op IStream, only there to be handed out as a real COM pointer whose reference count
		/// the tests can watch.
		/// </summary>
		class CountedStream : IStream
		{
			public void Clone(out IStream ppstm) => ppstm = null;
			public void Commit(int grfCommitFlags) { }
			public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten) { }
			public void LockRegion(long libOffset, long cb, int dwLockType) { }
			public void Read(byte[] pv, int cb, IntPtr pcbRead) { }
			public void Revert() { }
			public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition) { }
			public void SetSize(long libNewSize) { }
			public void Stat(out STATSTG pstatstg, int grfStatFlag) => pstatstg = new STATSTG();
			public void UnlockRegion(long libOffset, long cb, int dwLockType) { }
			public void Write(byte[] pv, int cb, IntPtr pcbWritten) { }
		}

		/// <summary>
		/// Reads a COM object's reference count without changing it. Only safe while the caller holds
		/// a reference of its own.
		/// </summary>
		static int RefCount(IntPtr unknown)
		{
			int count = Marshal.AddRef(unknown);
			Marshal.Release(unknown);
			return count - 1;
		}

		/// <summary>
		/// A synchronization context that just queues, so a test can decide when the owning thread
		/// gets to run the work posted back to it.
		/// </summary>
		class QueueingSynchronizationContext : SynchronizationContext
		{
			readonly Queue<Action> queue = new Queue<Action>();

			public int Count { get { lock (queue) return queue.Count; } }

			public override void Post(SendOrPostCallback d, object state)
			{
				lock (queue)
					queue.Enqueue(() => d(state));
			}

			public override void Send(SendOrPostCallback d, object state) => d(state);

			public int Drain()
			{
				int ran = 0;
				while (true)
				{
					Action action;
					lock (queue)
					{
						if (queue.Count == 0)
							break;
						action = queue.Dequeue();
					}
					action();
					ran++;
				}
				return ran;
			}
		}

		/// <summary>
		/// Creates a data object on a dedicated thread, so the tests below are running somewhere
		/// other than the thread that owns it -- the position the finalizer thread is in.
		/// </summary>
		static DragDropDataObject CreateOnOtherThread(SynchronizationContext context)
		{
			DragDropDataObject dataObject = null;
			var thread = new Thread(() =>
			{
				SynchronizationContext.SetSynchronizationContext(context);
				dataObject = new DragDropDataObject();
			});
			thread.IsBackground = true;
			thread.Start();
			Assert.That(thread.Join(TimeSpan.FromSeconds(10)), Is.True, "the owning thread did not finish");
			return dataObject;
		}

		/// <summary>
		/// Adds a TYMED_ISTREAM entry, as the shell's drag context does, and returns the raw pointer.
		/// The caller keeps one reference of its own so the pointer stays valid for the assertions.
		/// </summary>
		static IntPtr AddStreamEntry(DragDropDataObject dataObject, string format)
		{
			var unknown = Marshal.GetComInterfaceForObject(new CountedStream(), typeof(IStream));

			var formatetc = MakeFormat(format);
			formatetc.tymed = ComTypes.TYMED.TYMED_ISTREAM;
			var medium = new ComTypes.STGMEDIUM
			{
				tymed = ComTypes.TYMED.TYMED_ISTREAM,
				unionmember = unknown,
				pUnkForRelease = null
			};

			// release: false -- we keep our reference, the data object takes its own.
			((ComDataObject)dataObject).SetData(ref formatetc, ref medium, false);
			return unknown;
		}

		static void DisposeAsFinalizer(DragDropDataObject dataObject)
		{
			var dispose = typeof(DragDropDataObject).GetMethod("Dispose", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(bool) }, null);
			Assert.That(dispose, Is.Not.Null, "DragDropLib.DataObject.Dispose(bool) is gone");
			dispose.Invoke(dataObject, new object[] { false });
		}

		[Test]
		public void SetDataShouldAddReferenceToStreamMedium()
		{
			var context = new QueueingSynchronizationContext();
			var dataObject = CreateOnOtherThread(context);
			var unknown = AddStreamEntry(dataObject, "eto-test-stream-ref");
			try
			{
				// ours plus the data object's
				Assert.That(RefCount(unknown), Is.EqualTo(2), "storage did not take its own reference to the stream");
				Assert.That(dataObject.storage, Has.Count.EqualTo(1));
				Assert.That(dataObject.storage[0].Value.tymed, Is.EqualTo(ComTypes.TYMED.TYMED_ISTREAM));
			}
			finally
			{
				dataObject.Dispose();
				Marshal.Release(unknown);
			}
		}

		[Test]
		public void FinalizingOffThreadShouldNotReleaseComPointersInPlace()
		{
			var context = new QueueingSynchronizationContext();
			var dataObject = CreateOnOtherThread(context);
			var unknown = AddStreamEntry(dataObject, "eto-test-stream-finalize");
			try
			{
				Assert.That(RefCount(unknown), Is.EqualTo(2));

				DisposeAsFinalizer(dataObject);

				Assert.That(RefCount(unknown), Is.EqualTo(2),
					"the stream was released from the wrong thread instead of being posted back to its owner");
				Assert.That(dataObject.storage, Has.Count.EqualTo(1), "storage was cleared from the wrong thread");
				Assert.That(context.Count, Is.EqualTo(1), "nothing was posted back to the owning thread");

				// the owning thread gets around to it
				Assert.That(context.Drain(), Is.EqualTo(1));
				Assert.That(RefCount(unknown), Is.EqualTo(1), "the owning thread did not release the stream");
				Assert.That(dataObject.storage, Is.Empty);
			}
			finally
			{
				Marshal.Release(unknown);
			}
		}

		[Test]
		public void FinalizingWithNoWayHomeShouldFreeMemoryAndLeakComPointers()
		{
			// No synchronization context on the creating thread, so there is nowhere to post to.
			var dataObject = CreateOnOtherThread(null);
			var unknown = AddStreamEntry(dataObject, "eto-test-stream-orphan");
			try
			{
				var format = MakeFormat("eto-test-orphan-memory");
				var medium = MakeMedium(0x5150);
				((ComDataObject)dataObject).SetData(ref format, ref medium, true);
				Assert.That(dataObject.storage, Has.Count.EqualTo(2));

				DisposeAsFinalizer(dataObject);

				// The HGLOBAL is safe to free from any thread, the stream is not: leaking it beats
				// releasing an STA object from the finalizer thread.
				Assert.That(dataObject.storage, Has.Count.EqualTo(1), "the plain memory entry should have been released");
				Assert.That(dataObject.storage[0].Value.tymed, Is.EqualTo(ComTypes.TYMED.TYMED_ISTREAM));
				Assert.That(RefCount(unknown), Is.EqualTo(2), "the stream was released off its owning thread");
			}
			finally
			{
				Marshal.Release(unknown);
			}
		}

		[Test]
		public void DisposeOnOwningThreadShouldReleaseEverythingInPlace()
		{
			var context = new QueueingSynchronizationContext();
			DragDropDataObject dataObject = null;
			IntPtr unknown = IntPtr.Zero;

			var thread = new Thread(() =>
			{
				SynchronizationContext.SetSynchronizationContext(context);
				dataObject = new DragDropDataObject();
				unknown = AddStreamEntry(dataObject, "eto-test-stream-owner");
				dataObject.Dispose();
			});
			thread.IsBackground = true;
			thread.Start();
			Assert.That(thread.Join(TimeSpan.FromSeconds(10)), Is.True, "the owning thread did not finish");

			try
			{
				Assert.That(dataObject.storage, Is.Empty);
				Assert.That(context.Count, Is.EqualTo(0), "nothing needed posting -- Dispose ran on the owning thread");
				Assert.That(RefCount(unknown), Is.EqualTo(1), "Dispose did not release the stream");
			}
			finally
			{
				Marshal.Release(unknown);
			}
		}

		#endregion
	}
}
