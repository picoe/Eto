// Native Wayland clipboard backend using the wlr/ext data-control protocol.
//
// GTK's clipboard talks the standard wl_data_device protocol, which is focus-gated:
// the compositor only delivers selection offers to the surface holding keyboard focus,
// and clipboard data is served lazily by the owning client. That makes background /
// unfocused / headless clipboard access unreliable.
//
// The data-control protocol (zwlr_data_control_manager_v1, or the newer
// ext_data_control_manager_v1) is the focus-independent path used by wl-clipboard and
// every Wayland clipboard manager. KWin/KDE implements zwlr. This class binds it over a
// dedicated libwayland-client connection on its own dispatch thread, independent of GDK.
//
// When the protocol is unavailable (X11/XWayland, or a compositor without data-control)
// IsAvailable returns false and the GTK ClipboardHandler falls back to its normal path.

#if NET6_0_OR_GREATER

namespace Eto.GtkSharp.Forms
{
	static class WaylandClipboard
	{
		/// <summary>Raised on the dispatch thread whenever the system selection changes.</summary>
		public static event Action SelectionChanged;

		/// <summary>True when a data-control connection is up and usable.</summary>
		public static bool IsAvailable
		{
			get
			{
				EnsureInit();
				return instance != null;
			}
		}

		/// <summary>
		/// Inactivity budget for a blocking clipboard read, in milliseconds. A read gives up only after this
		/// long with no further data, so large but steady transfers complete while a stalled or dead source
		/// bails promptly. Reads run synchronously on the calling (often UI) thread, so this also bounds how
		/// long a paste can stall the UI when the owning application is unresponsive.
		/// </summary>
		public static int ReadTimeoutMs { get; set; } = 1000;

		public static string GetText()
		{
			var mime = PickTextMime();
			if (mime == null)
				return null;
			var bytes = GetData(mime);
			return bytes == null ? null : Encoding.UTF8.GetString(bytes);
		}

		public static byte[] GetData(string mime) => mime != null ? Instance?.Receive(mime, ReadTimeoutMs) : null;

		public static string GetString(string mime)
		{
			var bytes = GetData(mime);
			return bytes == null ? null : Encoding.UTF8.GetString(bytes);
		}

		public static string[] GetMimeTypes() => Instance?.CurrentMimeTypes() ?? System.Array.Empty<string>();

		public static bool Contains(string mime)
		{
			if (mime == null)
				return false;
			var types = GetMimeTypes();
			foreach (var t in types)
				if (string.Equals(t, mime, StringComparison.OrdinalIgnoreCase))
					return true;
			return false;
		}

		public static bool ContainsText => PickTextMime() != null;

		/// <summary>Replace the text representation of the current selection (keeps other mimes, mirroring the GTK handler).</summary>
		public static void SetText(string text)
		{
			var current = Instance;
			if (current == null)
				return;
			var bytes = Encoding.UTF8.GetBytes(text ?? string.Empty);
			lock (current.entriesLock)
			{
				foreach (var m in TextMimes)
					current.entries[m] = bytes;
			}
			current.PublishSelection();
		}

		/// <summary>Set/replace a single mime entry of the current selection.</summary>
		public static void SetData(string mime, byte[] data)
		{
			var current = Instance;
			if (current == null || mime == null)
				return;
			lock (current.entriesLock)
				current.entries[mime] = data != null ? (byte[])data.Clone() : System.Array.Empty<byte>();
			current.PublishSelection();
		}

		public static void Clear()
		{
			var current = Instance;
			if (current == null)
				return;
			lock (current.entriesLock)
				current.entries.Clear();
			current.PublishSelection();
		}

		// Text mime types we publish (and recognise when reading), best first.
		static readonly string[] TextMimes =
		{
			"text/plain;charset=utf-8",
			"text/plain",
			"UTF8_STRING",
			"STRING",
			"TEXT"
		};

		static string PickTextMime()
		{
			var types = GetMimeTypes();
			foreach (var pref in TextMimes)
				foreach (var t in types)
					if (string.Equals(t, pref, StringComparison.OrdinalIgnoreCase))
						return t;
			return null;
		}

		// ----- lazy init / detection -----

		static readonly object initLock = new object();
		static bool initTried;
		static Impl instance;

		static Impl Instance
		{
			get
			{
				EnsureInit();
				return instance;
			}
		}

		static void EnsureInit()
		{
			if (initTried)
				return;
			lock (initLock)
			{
				if (initTried)
					return;
				// One-shot by design: the platform (X11/Wayland, compositor, data-control support) is fixed for
				// the process, and the choice between this backend and the GTK handler is made once at handler
				// registration. We latch before attempting so a failure can't make every clipboard call retry
				// (and re-spawn the connection/dispatch thread). A transient first-attempt failure therefore
				// disables this backend for the process; in practice the failure modes here are all permanent.
				initTried = true;
				try
				{
					if (!EtoEnvironment.Platform.IsLinux)
						return;
					if (string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")))
						return;
					var impl = new Impl();
					try
					{
						if (impl.TryConnect(() => SelectionChanged?.Invoke()))
						{
							instance = impl;
							impl = null;
						}
					}
					finally
					{
						impl?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"WaylandClipboard init failed: {ex}");
				}
			}
		}

		// =====================================================================
		// libwayland-client interop
		// =====================================================================

		const string Lib = "libwayland-client.so.0";
		const uint WL_MARSHAL_FLAG_DESTROY = 1;

		[DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr wl_display_connect(string name);
		[DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
		static extern void wl_display_disconnect(IntPtr display);
		[DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
		static extern int wl_display_roundtrip(IntPtr display);
		[DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
		static extern int wl_display_flush(IntPtr display);
		[DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
		static extern int wl_display_get_fd(IntPtr display);
		[DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
		static extern int wl_display_prepare_read(IntPtr display);
		[DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
		static extern int wl_display_read_events(IntPtr display);
		[DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
		static extern void wl_display_cancel_read(IntPtr display);
		[DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
		static extern int wl_display_dispatch_pending(IntPtr display);
		[DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
		static extern int wl_proxy_add_listener(IntPtr proxy, IntPtr implementation, IntPtr data);
		[DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr wl_proxy_marshal_array_flags(IntPtr proxy, uint opcode, IntPtr iface, uint version, uint flags, [In] WlArgument[] args);

		// libc helpers (pipes for receiving selection data)
		[DllImport("libc", SetLastError = true)]
		static extern int pipe2([In, Out] int[] fds, int flags);
		[DllImport("libc", SetLastError = true)]
		static extern nint read(int fd, byte[] buf, nuint count);
		[DllImport("libc", SetLastError = true)]
		static extern nint write(int fd, byte[] buf, nuint count);
		[DllImport("libc", SetLastError = true)]
		static extern nint write(int fd, IntPtr buf, nuint count);
		[DllImport("libc", SetLastError = true)]
		static extern int close(int fd);
		[DllImport("libc", SetLastError = true)]
		static extern int poll([In, Out] Pollfd[] fds, nuint nfds, int timeout);
		[DllImport("libc", SetLastError = true)]
		static extern int fcntl(int fd, int cmd, int arg);

		const int O_CLOEXEC = 0x80000;
		const int O_NONBLOCK = 0x800;
		const short POLLIN = 0x001;
		const short POLLOUT = 0x004;
		const int F_GETFL = 3;
		const int F_SETFL = 4;
		const int EAGAIN = 11;     // == EWOULDBLOCK on Linux
		const int EINTR = 4;

		[StructLayout(LayoutKind.Explicit, Size = 8)]
		struct WlArgument
		{
			[FieldOffset(0)] public int i;
			[FieldOffset(0)] public uint u;
			[FieldOffset(0)] public IntPtr o; // object / string / new_id / array
		}

		static WlArgument ArgU(uint v) => new WlArgument { u = v };
		static WlArgument ArgI(int v) => new WlArgument { i = v };
		static WlArgument ArgP(IntPtr p) => new WlArgument { o = p };
		static readonly WlArgument ArgNew = new WlArgument { o = IntPtr.Zero };

		[StructLayout(LayoutKind.Sequential)]
		struct Pollfd
		{
			public int fd;
			public short events;
			public short revents;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct WlMessage
		{
			public IntPtr name;
			public IntPtr signature;
			public IntPtr types; // const wl_interface**
		}

		[StructLayout(LayoutKind.Sequential)]
		struct WlInterface
		{
			public IntPtr name;
			public int version;
			public int method_count;
			public IntPtr methods;
			public int event_count;
			public IntPtr events;
		}

		// Listener vtables are arrays of cdecl function pointers, one per event in
		// protocol order. Delegates must stay rooted for the connection's lifetime.
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void RegistryGlobal(IntPtr data, IntPtr registry, uint name, IntPtr iface, uint version);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void RegistryGlobalRemove(IntPtr data, IntPtr registry, uint name);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void DeviceDataOffer(IntPtr data, IntPtr device, IntPtr offer);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void DeviceSelection(IntPtr data, IntPtr device, IntPtr offer);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void DeviceFinished(IntPtr data, IntPtr device);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void OfferOffer(IntPtr data, IntPtr offer, IntPtr mime);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void SourceSend(IntPtr data, IntPtr source, IntPtr mime, int fd);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void SourceCancelled(IntPtr data, IntPtr source);

		// =====================================================================
		// Implementation instance
		// =====================================================================

		sealed class Impl : IDisposable
		{
			IntPtr display;
			IntPtr libwaylandHandle;
			IntPtr registry;
			IntPtr seat;
			IntPtr manager;
			IntPtr device;
			// 'source' (our currently-owned data source proxy) is touched by PublishSelection on the caller
			// thread and OnSourceCancelled on the dispatch thread; all reads/writes/destroys go through sourceLock
			// so a republish can never race a cancellation into a double-destroy. weOwn is also written under it.
			readonly object sourceLock = new object();
			IntPtr source;                 // our currently-owned source (or zero); guarded by sourceLock
			Thread dispatchThread;
			volatile bool running;
			int[] wakePipe;
			Action onSelectionChanged;

			// hand-built interface descriptor tables (kept rooted)
			IntPtr ifManager, ifDevice, ifSource, ifOffer;
			IntPtr seatIface, registryIface;

			// rooted native allocations (strings, message/type arrays, vtables, delegates)
			readonly List<IntPtr> roots = new List<IntPtr>();
			readonly List<Delegate> delegateRoots = new List<Delegate>();

			// listener vtable pointers
			IntPtr registryVtable, deviceVtable, offerVtable, sourceVtable;

			// detection: registry globals
			uint managerName, seatName;
			bool managerIsExt;
			string managerInterfaceName;

			// current incoming selection state (guarded by stateLock)
			readonly object stateLock = new object();
			IntPtr currentOffer;
			List<string> currentMimes = new List<string>();
			readonly Dictionary<IntPtr, List<string>> pendingOffers = new Dictionary<IntPtr, List<string>>();

			// our outgoing entries (guarded by entriesLock)
			internal readonly object entriesLock = new object();
			internal readonly Dictionary<string, byte[]> entries = new Dictionary<string, byte[]>(StringComparer.Ordinal);

			// True while we own the selection: serve reads directly from entries (instant, no compositor
			// echo round-trip). Cleared when another client takes ownership (source cancelled) or on clear.
			volatile bool weOwn;

			public bool TryConnect(Action selectionChanged)
			{
				onSelectionChanged = selectionChanged;
				display = wl_display_connect(null);
				if (display == IntPtr.Zero)
					return false;

				BuildListenerDelegates();

				// stock interfaces exported by libwayland
				libwaylandHandle = NativeLibrary.Load(Lib);
				registryIface = NativeLibrary.GetExport(libwaylandHandle, "wl_registry_interface");
				seatIface = NativeLibrary.GetExport(libwaylandHandle, "wl_seat_interface");

				// wl_display.get_registry (opcode 1) -> wl_registry
				registry = wl_proxy_marshal_array_flags(display, 1, registryIface, 1, 0, new[] { ArgNew });
				if (registry == IntPtr.Zero)
					return false;
				wl_proxy_add_listener(registry, registryVtable, IntPtr.Zero);

				// first roundtrip: enumerate globals
				if (wl_display_roundtrip(display) < 0)
					return false;
				if (managerInterfaceName == null || seatName == 0)
					return false;

				BuildProtocolInterfaces(managerIsExt);

				// bind seat (v1) and the data-control manager (v1)
				seat = RegistryBind(seatName, seatIface, "wl_seat", 1);
				manager = RegistryBind(managerName, ifManager, managerInterfaceName, 1);
				if (seat == IntPtr.Zero || manager == IntPtr.Zero)
					return false;

				// manager.get_data_device(seat) -> device   (opcode 1, sig "no")
				device = wl_proxy_marshal_array_flags(manager, 1, ifDevice, 1, 0, new[] { ArgNew, ArgP(seat) });
				if (device == IntPtr.Zero)
					return false;
				wl_proxy_add_listener(device, deviceVtable, IntPtr.Zero);

				// second roundtrip: receive the initial selection offer
				wl_display_roundtrip(display);

				// dedicated dispatch thread + wakeup pipe for clean shutdown
				wakePipe = new int[2];
				if (pipe2(wakePipe, O_CLOEXEC | O_NONBLOCK) != 0)
					return false;
				running = true;
				dispatchThread = new Thread(DispatchLoop) { IsBackground = true, Name = "WaylandClipboard" };
				dispatchThread.Start();
				return true;
			}

			IntPtr RegistryBind(uint name, IntPtr iface, string ifaceName, uint version)
			{
				var namePtr = Utf8(ifaceName);
				try
				{
					// wl_registry.bind (opcode 0), args: name(u), interface(s), version(u), id(n)
					return wl_proxy_marshal_array_flags(registry, 0, iface, version, 0,
						new[] { ArgU(name), ArgP(namePtr), ArgU(version), ArgNew });
				}
				finally
				{
					Marshal.FreeHGlobal(namePtr);
				}
			}

			// ---- dispatch loop (owns the read side of the connection) ----

			void DispatchLoop()
			{
				int fd = wl_display_get_fd(display);
				var pfds = new Pollfd[2];
				while (running)
				{
					while (wl_display_prepare_read(display) != 0)
						wl_display_dispatch_pending(display);
					wl_display_flush(display);

					pfds[0].fd = fd; pfds[0].events = POLLIN; pfds[0].revents = 0;
					pfds[1].fd = wakePipe[0]; pfds[1].events = POLLIN; pfds[1].revents = 0;

					int r = poll(pfds, 2, -1);
					if (r < 0)
					{
						wl_display_cancel_read(display);
						continue;
					}

					if ((pfds[0].revents & POLLIN) != 0)
					{
						wl_display_read_events(display);
						wl_display_dispatch_pending(display);
					}
					else
					{
						wl_display_cancel_read(display);
					}

					if ((pfds[1].revents & POLLIN) != 0)
					{
						var drain = new byte[16];
						_ = read(wakePipe[0], drain, (nuint)drain.Length);
					}
				}
			}

				void Wake()
				{
					if (wakePipe != null)
						_ = write(wakePipe[1], new byte[] { 1 }, 1);
			}

			// ---- reading the current selection ----

			public string[] CurrentMimeTypes()
			{
				if (weOwn)
					lock (entriesLock)
						if (entries.Count > 0)
						{
							var keys = new string[entries.Count];
							entries.Keys.CopyTo(keys, 0);
							return keys;
						}
				lock (stateLock)
					return currentMimes.ToArray();
			}

				// Absolute bounds for a single read so a hostile/buggy source cannot wedge the (often UI) thread
				// or exhaust memory with a steady trickle: MaxReadMs caps total wall-clock regardless of progress,
				// MaxReadBytes caps accumulated size. timeoutMs remains the idle (no-progress) bound on top of these.
				const int MaxReadMs = 5000;
				const long MaxReadBytes = 256L * 1024 * 1024;

				public byte[] Receive(string mime, int timeoutMs)
				{
					// If we own the selection, serve our own data directly (no compositor round-trip). weOwn is a
					// best-effort fast path: if ownership is lost concurrently we may serve the data we last
					// published, which is the inherent "value just before the change" clipboard race and harmless.
					if (weOwn)
						lock (entriesLock)
						{
							if (entries.TryGetValue(mime, out var own))
								return (byte[])own.Clone();
							foreach (var entry in entries)
								if (string.Equals(entry.Key, mime, StringComparison.OrdinalIgnoreCase))
									return (byte[])entry.Value.Clone();
						}

					var fds = new int[2];
					if (pipe2(fds, O_CLOEXEC) != 0)
						return null;
					int rfd = fds[0], wfd = fds[1];

					try
					{
						// Hold stateLock across the receive request: OnDeviceSelection (dispatch thread) destroys the
						// current offer under the same lock, so this prevents it from freeing the proxy between us
						// reading currentOffer and marshalling the request on it (use-after-free).
						lock (stateLock)
						{
							var offer = currentOffer;
							var offerMime = currentMimes.FirstOrDefault(m => string.Equals(m, mime, StringComparison.OrdinalIgnoreCase));
							if (offer == IntPtr.Zero || offerMime == null)
								return null;
							var mimePtr = Utf8(offerMime);
							try
							{
								// offer.receive(mime, wfd)  (opcode 0, sig "sh")
								wl_proxy_marshal_array_flags(offer, 0, IntPtr.Zero, 0, 0, new[] { ArgP(mimePtr), ArgI(wfd) });
								wl_display_flush(display);
							}
							finally
							{
								Marshal.FreeHGlobal(mimePtr);
							}
						}

						close(wfd); // we only read; the sending client owns the write end now
						wfd = -1;

						using var ms = new MemoryStream();
						var buf = new byte[8192];
						var pfd = new Pollfd[1];
						// idleDeadline: bail after timeoutMs with NO progress, so steady transfers complete but a
						// stalled/dead source bails promptly. hardDeadline: absolute ceiling so a steady trickle
						// can't wedge the thread indefinitely. MaxReadBytes bounds memory either way.
						var idleDeadline = System.Environment.TickCount64 + timeoutMs;
						var hardDeadline = System.Environment.TickCount64 + System.Math.Max(timeoutMs, MaxReadMs);
						while (true)
						{
							long now = System.Environment.TickCount64;
							int remaining = (int)(System.Math.Min(idleDeadline, hardDeadline) - now);
							if (remaining <= 0)
								return null; // no progress for timeoutMs, or absolute MaxReadMs ceiling hit
							pfd[0].fd = rfd; pfd[0].events = POLLIN; pfd[0].revents = 0;
							int pr = poll(pfd, 1, remaining);
							if (pr <= 0)
								return null; // timeout or poll error
							nint n = read(rfd, buf, (nuint)buf.Length);
							if (n < 0)
							{
								int err = Marshal.GetLastWin32Error();
								if (err == EAGAIN || err == EINTR)
									continue;
								return null; // read error
							}
							if (n == 0)
								break; // EOF: transfer complete
							ms.Write(buf, 0, (int)n);
							if (ms.Length > MaxReadBytes)
								return null; // refuse a pathologically large payload
							idleDeadline = System.Environment.TickCount64 + timeoutMs; // progress: reset idle timer only
						}
						return ms.ToArray();
					}
					finally
					{
						if (wfd >= 0)
							close(wfd);
						close(rfd);
					}
				}

			// ---- publishing our own selection ----

			public void PublishSelection()
			{
				// Snapshot the mime set under entriesLock, then do all proxy/source work under sourceLock.
				// The two locks are never held at once (here or anywhere), so there is no ordering hazard.
				// Wire marshalling is internally serialised by libwayland; sourceLock only protects the
				// lifetime of the 'source' proxy against a concurrent OnSourceCancelled on the dispatch thread.
				string[] mimes;
				lock (entriesLock)
				{
					if (entries.Count == 0)
					{
						mimes = null;
					}
					else
					{
						mimes = new string[entries.Count];
						entries.Keys.CopyTo(mimes, 0);
					}
				}

				lock (sourceLock)
				{
					// destroy the previous source (if any) exactly once
					if (source != IntPtr.Zero)
					{
						var old = source;
						source = IntPtr.Zero;
						wl_proxy_marshal_array_flags(old, 1, IntPtr.Zero, 0, WL_MARSHAL_FLAG_DESTROY, null);
					}

					if (mimes == null)
					{
						// nothing to offer: clear ownership
						weOwn = false;
						// device.set_selection(null)  (opcode 0, sig "?o")
						wl_proxy_marshal_array_flags(device, 0, IntPtr.Zero, 0, 0, new[] { ArgP(IntPtr.Zero) });
						wl_display_flush(display);
						Wake();
						return;
					}

					// manager.create_data_source (opcode 0) -> source
					source = wl_proxy_marshal_array_flags(manager, 0, ifSource, 1, 0, new[] { ArgNew });
					if (source == IntPtr.Zero)
					{
						weOwn = false;
						return;
					}
					wl_proxy_add_listener(source, sourceVtable, IntPtr.Zero);

					foreach (var mime in mimes)
					{
						var p = Utf8(mime);
						// source.offer(mime)  (opcode 0, sig "s")
						wl_proxy_marshal_array_flags(source, 0, IntPtr.Zero, 0, 0, new[] { ArgP(p) });
						Marshal.FreeHGlobal(p);
					}

					// device.set_selection(source)  (opcode 0)
					wl_proxy_marshal_array_flags(device, 0, IntPtr.Zero, 0, 0, new[] { ArgP(source) });
					wl_display_flush(display);
					weOwn = true;
					Wake();
				}
			}

			// ---- listener callbacks (fire on dispatch thread) ----

			void OnRegistryGlobal(IntPtr data, IntPtr reg, uint name, IntPtr ifacePtr, uint version)
			{
				var iface = Marshal.PtrToStringUTF8(ifacePtr);
				if (iface == "wl_seat")
				{
					if (seatName == 0)
						seatName = name;
				}
				else if (iface == "zwlr_data_control_manager_v1")
				{
					// prefer zwlr (broad KDE/wlroots support)
					managerName = name;
					managerInterfaceName = iface;
					managerIsExt = false;
				}
				else if (iface == "ext_data_control_manager_v1" && managerInterfaceName == null)
				{
					managerName = name;
					managerInterfaceName = iface;
					managerIsExt = true;
				}
			}

			void OnRegistryGlobalRemove(IntPtr data, IntPtr reg, uint name) { }

			void OnDeviceDataOffer(IntPtr data, IntPtr dev, IntPtr offer)
			{
				lock (stateLock)
					pendingOffers[offer] = new List<string>();
				wl_proxy_add_listener(offer, offerVtable, IntPtr.Zero);
			}

			void OnOfferOffer(IntPtr data, IntPtr offer, IntPtr mimePtr)
			{
				var mime = Marshal.PtrToStringUTF8(mimePtr);
				if (mime == null)
					return;
				lock (stateLock)
				{
					if (pendingOffers.TryGetValue(offer, out var list))
						list.Add(mime);
				}
			}

			void OnDeviceSelection(IntPtr data, IntPtr dev, IntPtr offer)
			{
				lock (stateLock)
				{
					// destroy the previously-held offer
					if (currentOffer != IntPtr.Zero && currentOffer != offer)
						wl_proxy_marshal_array_flags(currentOffer, 1, IntPtr.Zero, 0, WL_MARSHAL_FLAG_DESTROY, null);

					currentOffer = offer;
					if (offer != IntPtr.Zero && pendingOffers.TryGetValue(offer, out var list))
						currentMimes = list;
					else
						currentMimes = new List<string>();

					// Destroy and drop every other announced-but-unselected offer. The compositor mints a
					// fresh wl_data_offer per selection change, so anything still pending here is an orphan;
					// without this they (and their proxies) accumulate for the life of the connection.
					foreach (var kv in pendingOffers)
					{
						if (kv.Key != offer)
							wl_proxy_marshal_array_flags(kv.Key, 1, IntPtr.Zero, 0, WL_MARSHAL_FLAG_DESTROY, null);
					}
					pendingOffers.Clear();
				}
				try { onSelectionChanged?.Invoke(); } catch { }
			}

			void OnDeviceFinished(IntPtr data, IntPtr dev) { }

			// Maximum time a single selection transfer may take before we give up. A consumer that stops
			// reading must never wedge us; this only bounds a stuck transfer, normal ones finish far sooner.
			const int SendTimeoutMs = 5000;

			void OnSourceSend(IntPtr data, IntPtr src, IntPtr mimePtr, int fd)
			{
				var mime = Marshal.PtrToStringUTF8(mimePtr);
				byte[] payload = null;
				if (mime != null)
					lock (entriesLock)
					{
						if (entries.TryGetValue(mime, out var entry))
							payload = (byte[])entry.Clone();
					}

				// Serve on a background thread: writing to the consumer's fd can block on a large payload
				// or a slow/non-reading consumer, and this runs on the single dispatch thread — blocking it
				// would freeze all clipboard event processing. The fd ownership transfers to the writer; if the
				// enqueue itself fails (e.g. OOM/teardown) we close it here so it can't leak.
				try
				{
					System.Threading.Tasks.Task.Run(() => WritePayload(fd, payload));
				}
				catch
				{
					close(fd);
				}
			}

			static void WritePayload(int fd, byte[] payload)
			{
				try
				{
					if (payload == null || payload.Length == 0)
						return;
					// The poll(POLLOUT)+deadline bound below only works on a non-blocking fd; a blocking write()
					// never returns EAGAIN and could wedge this thread forever on a stuck consumer. If we can't
					// make it non-blocking, abandon the transfer (the finally closes fd -> consumer sees EOF).
					if (!SetNonBlocking(fd))
						return;
					var handle = GCHandle.Alloc(payload, GCHandleType.Pinned);
					try
					{
						var ptr = handle.AddrOfPinnedObject();
						int off = 0;
						var pfd = new Pollfd[1];
						var deadline = System.Environment.TickCount64 + SendTimeoutMs;
						while (off < payload.Length)
						{
							nint n = write(fd, ptr + off, (nuint)(payload.Length - off));
							if (n > 0)
							{
								off += (int)n;
								continue;
							}
							if (n < 0)
							{
								int err = Marshal.GetLastWin32Error();
								if (err != EAGAIN && err != EINTR)
									break; // real error (e.g. EPIPE: consumer closed its end)
							}
							int remaining = (int)(deadline - System.Environment.TickCount64);
							if (remaining <= 0)
								break; // stuck consumer: give up rather than leak this thread/fd forever
							pfd[0].fd = fd; pfd[0].events = POLLOUT; pfd[0].revents = 0;
							if (poll(pfd, 1, remaining) <= 0)
								break; // timeout or error
						}
					}
					finally
					{
						handle.Free();
					}
				}
				finally
				{
					close(fd);
				}
			}

			static bool SetNonBlocking(int fd)
			{
				int flags = fcntl(fd, F_GETFL, 0);
				if (flags < 0)
					return false;
				return fcntl(fd, F_SETFL, flags | O_NONBLOCK) >= 0;
			}

			void OnSourceCancelled(IntPtr data, IntPtr src)
			{
				// Destroy the cancelled source exactly once. If it is no longer the current source, PublishSelection
				// has already replaced and destroyed it, so destroying again here would be a double-free. Both
				// paths take sourceLock, which serialises this decision against a concurrent republish.
				lock (sourceLock)
				{
					if (src != source)
						return;
					source = IntPtr.Zero;
					weOwn = false; // another client took ownership of the selection
					wl_proxy_marshal_array_flags(src, 1, IntPtr.Zero, 0, WL_MARSHAL_FLAG_DESTROY, null);
				}
			}

			// ---- building delegate vtables and interface tables ----

			void BuildListenerDelegates()
			{
				RegistryGlobal g = OnRegistryGlobal;
				RegistryGlobalRemove gr = OnRegistryGlobalRemove;
				registryVtable = Vtable(g, gr);

				DeviceDataOffer ddo = OnDeviceDataOffer;
				DeviceSelection ds = OnDeviceSelection;
				DeviceFinished df = OnDeviceFinished;
				deviceVtable = Vtable(ddo, ds, df);

				OfferOffer oo = OnOfferOffer;
				offerVtable = Vtable(oo);

				SourceSend ss = OnSourceSend;
				SourceCancelled sc = OnSourceCancelled;
				sourceVtable = Vtable(ss, sc);
			}

			IntPtr Vtable(params Delegate[] handlers)
			{
				var arr = Marshal.AllocHGlobal(IntPtr.Size * handlers.Length);
				roots.Add(arr);
				for (int i = 0; i < handlers.Length; i++)
				{
					delegateRoots.Add(handlers[i]);
					Marshal.WriteIntPtr(arr, i * IntPtr.Size, Marshal.GetFunctionPointerForDelegate(handlers[i]));
				}
				return arr;
			}

			// Construct the four data-control wl_interface descriptor tables in unmanaged
			// memory (normally emitted by wayland-scanner). Names use the zwlr or ext prefix.
			void BuildProtocolInterfaces(bool ext)
			{
				string p = ext ? "ext_data_control_" : "zwlr_data_control_";

				// allocate the four interface structs first so messages can reference them
				ifManager = Marshal.AllocHGlobal(Marshal.SizeOf<WlInterface>()); roots.Add(ifManager);
				ifDevice = Marshal.AllocHGlobal(Marshal.SizeOf<WlInterface>()); roots.Add(ifDevice);
				ifSource = Marshal.AllocHGlobal(Marshal.SizeOf<WlInterface>()); roots.Add(ifSource);
				ifOffer = Marshal.AllocHGlobal(Marshal.SizeOf<WlInterface>()); roots.Add(ifOffer);

				// manager: requests create_data_source(n), get_data_device(no), destroy()
				var mgrMethods = new[]
				{
					Msg("create_data_source", "n", ifSource),
					Msg("get_data_device", "no", ifDevice, seatIface),
					Msg("destroy", "", null)
				};
				FillInterface(ifManager, p + "manager_v1", 1, mgrMethods, null);

				// device: requests set_selection(?o), destroy(); events data_offer(n), selection(?o), finished()
				var devMethods = new[]
				{
					Msg("set_selection", "?o", ifSource),
					Msg("destroy", "", null)
				};
				var devEvents = new[]
				{
					Msg("data_offer", "n", ifOffer),
					Msg("selection", "?o", ifOffer),
					Msg("finished", "", null)
				};
				FillInterface(ifDevice, p + "device_v1", 1, devMethods, devEvents);

				// source: requests offer(s), destroy(); events send(sh), cancelled()
				var srcMethods = new[]
				{
					Msg("offer", "s", (IntPtr?)null),
					Msg("destroy", "", null)
				};
				var srcEvents = new[]
				{
					Msg("send", "sh", null, null),
					Msg("cancelled", "", null)
				};
				FillInterface(ifSource, p + "source_v1", 1, srcMethods, srcEvents);

				// offer: requests receive(sh), destroy(); events offer(s)
				var offMethods = new[]
				{
					Msg("receive", "sh", null, null),
					Msg("destroy", "", null)
				};
				var offEvents = new[]
				{
					Msg("offer", "s", (IntPtr?)null)
				};
				FillInterface(ifOffer, p + "offer_v1", 1, offMethods, offEvents);
			}

			// Build one wl_message in unmanaged memory. typeIfaces gives the wl_interface*
			// for each argument slot (IntPtr.Zero for non-object args).
			WlMessage Msg(string name, string signature, params IntPtr?[] typeIfaces)
			{
				int argc = typeIfaces?.Length ?? 0;
				var typesArr = Marshal.AllocHGlobal(IntPtr.Size * System.Math.Max(argc, 1));
				roots.Add(typesArr);
				for (int i = 0; i < argc; i++)
					Marshal.WriteIntPtr(typesArr, i * IntPtr.Size, typeIfaces[i] ?? IntPtr.Zero);
				return new WlMessage
				{
					name = Utf8Root(name),
					signature = Utf8Root(signature),
					types = typesArr
				};
			}

			void FillInterface(IntPtr ifacePtr, string name, int version, WlMessage[] methods, WlMessage[] events)
			{
				IntPtr methodsPtr = WriteMessages(methods);
				IntPtr eventsPtr = WriteMessages(events);
				var iface = new WlInterface
				{
					name = Utf8Root(name),
					version = version,
					method_count = methods?.Length ?? 0,
					methods = methodsPtr,
					event_count = events?.Length ?? 0,
					events = eventsPtr
				};
				Marshal.StructureToPtr(iface, ifacePtr, false);
			}

			IntPtr WriteMessages(WlMessage[] msgs)
			{
				if (msgs == null || msgs.Length == 0)
					return IntPtr.Zero;
				int size = Marshal.SizeOf<WlMessage>();
				var block = Marshal.AllocHGlobal(size * msgs.Length);
				roots.Add(block);
				for (int i = 0; i < msgs.Length; i++)
					Marshal.StructureToPtr(msgs[i], block + i * size, false);
				return block;
			}

			IntPtr Utf8Root(string s)
			{
				var p = Utf8(s);
				roots.Add(p);
				return p;
			}

			public void Dispose()
			{
				running = false;
				Wake();
				try { dispatchThread?.Join(500); } catch { }
				if (display != IntPtr.Zero)
				{
					try { wl_display_disconnect(display); } catch { }
					display = IntPtr.Zero;
				}
				if (wakePipe != null)
				{
					close(wakePipe[0]);
					close(wakePipe[1]);
					wakePipe = null;
				}
				if (libwaylandHandle != IntPtr.Zero)
				{
					try { NativeLibrary.Free(libwaylandHandle); } catch { }
					libwaylandHandle = IntPtr.Zero;
				}
				foreach (var r in roots)
					Marshal.FreeHGlobal(r);
				roots.Clear();
				delegateRoots.Clear();
			}
		}

		static IntPtr Utf8(string s)
		{
			var bytes = Encoding.UTF8.GetBytes(s ?? string.Empty);
			var p = Marshal.AllocHGlobal(bytes.Length + 1);
			Marshal.Copy(bytes, 0, p, bytes.Length);
			Marshal.WriteByte(p, bytes.Length, 0);
			return p;
		}
	}
}

#endif
