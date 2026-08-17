# Eto.Forms — Agent Guide

Eto.Forms cross-platform UI toolkit. Keep this file compact — it loads every
session. Record only durable, non-obvious Eto facts; skip anything obvious from code/git.

**AGENTS.md admission rule:** add a fact only when it is broadly useful across future tasks,
stable, genuinely difficult or costly to rediscover, and not better captured by code, tests,
comments, or focused documentation. Do not record machine-specific state, one-off failures,
routine implementation details, or facts already evident from the repository. When in doubt,
do not add it. Curate existing entries and remove stale or low-value notes to justify the token
cost this file imposes on every request.

## Running unit tests

```bash
dotnet test --project test/Eto.Test.UnitTests/Eto.Test.UnitTests.csproj -f net10.0 --filter "BrushTests"
```

- `--filter` uses NUnit / Microsoft.Testing.Platform syntax; a bare class name (`"BrushTests"`)
  or `FullyQualifiedName~Brush` both work. Omit `--filter` to run everything.
- **Always exclude the `ManualTest` category** when running broader/unscoped test sets — those
  tests require user interaction and will otherwise stall waiting for input. Append
  `TestCategory!=ManualTest` (combine with `&`), e.g.
  `--filter "FullyQualifiedName~Grid&TestCategory!=ManualTest"` or, to run everything else,
  `--filter "TestCategory!=ManualTest"`.
- **Always pass `-f`** — the project multi-targets `net48;net10.0;net10.0-windows`.
  On Linux/macOS use `-f net10.0`; the Windows-only TFMs won't build there.
- Test runner is Microsoft.Testing.Platform (set in `global.json`), NUnit 4.
- **The modern Mac-specific tests are in `test/Eto.Test.Mac/Eto.Test.macOS.csproj` and require
  the matching .NET macOS workload.** The presence of `Microsoft.macOS.Ref` packs alone is not
  sufficient. If the workload rejects a newer Xcode patch version, set
  `<ValidateXcodeVersion>false</ValidateXcodeVersion>` (already set in `build/Common.Build.props`).
- **Reflection gotcha (net48 vs net):** `Type.GetType("Ns.Type, PresentationCore")` (partial assembly
  name) resolves on .NET but returns **null** on .NET Framework, so tests that reflect over WPF types
  (e.g. finding the native `ScrollViewer`) silently no-op on net48. Search loaded assemblies instead:
  `AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(fullName)).FirstOrDefault(t => t != null)`.
- **Windowed/GUI tests run without special setup on all platforms** (Linux, Mac, Windows) as
  long as a display is available — on Linux that's a real X display (e.g. `DISPLAY=:0`; no
  `xvfb-run` needed). Tests that `Show()`/`Close()` windows (e.g.
  `PreferredSizeShouldAccountForAllRowsWhenLargerThanWindow`, anything via
  `ShownAsync`/`Form`/`Paint`) actually execute.
- **Wpf gotcha (fixed in `Eto.Test.UnitTests/Program.cs`) — showing a themed control like
  `TreeGridView` under the test host used to crash with a bogus `FileLoadException: Could not load
  file or assembly 'Eto.Wpf.Aero2'` (`E_POINTER`) → `NullReferenceException` in
  `NUnit.Engine.Internal.RuntimeLibrariesStrategy.TryToResolve`.** Laying out a themed Eto.Wpf
  control makes WPF probe an *external* per-OS-theme satellite assembly (`Eto.Wpf.Aero2`, etc.)
  before falling back to the embedded `themes/generic.xaml`; that satellite never exists and in the
  real app the probe just returns "not found", but the NUnit test host's assembly `Resolving`
  handler throws an NRE instead of returning null, surfacing as a fatal load error during layout.
  Fix: `Program.Main` registers an `AssemblyLoadContext.Default.Resolving` handler FIRST (before
  NUnit's) that throws `FileNotFoundException` for `Eto.*` theme-satellite names (`.Aero2`, `.Aero`,
  `.AeroLite`, `.Classic`, `.Luna`, `.Royale`, `.Generic`) so WPF falls back cleanly and NUnit's
  broken handler never runs (`#if NET` — .NET-Core test host only). Was deterministic per grid type
  (GridView fine, TreeGridView crashed), observed on arm64. (Surfaced writing the b46b9827
  scrollable-fill regression tests.)

## Screenshotting a window to diagnose visual/layout bugs

When a test "passes" but the rendered output looks wrong (clipping, wrong size, misplaced
widgets), capture the actual pixels and look at them. There are usually **no OS screenshot tools**
(`scrot`, `import`, `grim`, …) available, so grab the window from the backend's own native API
inside a tiny standalone Eto app.

**Cross-platform strategy (backend-agnostic):**

- Write a small console `.csproj` (`net10.0`, `OutputType=Exe`) that **`<Reference>`s the built
  DLLs** for the backend under test (see per-backend note below). Build the platform lib you're
  editing first, e.g. `dotnet build src/Eto.Gtk/Eto.Gtk.csproj -f net10.0`.
- Build the form exactly as the test does, `Show()` it, then in a timer/dispatch callback (after
  it has actually mapped/rendered) take the screenshot and `app.Quit()`.
- Run it, then `Read` the PNG (the Read tool renders images) to actually *see* the output.
- **Gotcha:** `dotnet bin/.../app.dll` loads its dependencies from that **same `bin` output dir**,
  not from wherever your `<HintPath>` points — after rebuilding a platform DLL you must copy it
  into the app's `bin/Debug/net10.0/` for the change to take effect.
- Toggle deep diagnostics from native code with an env var (e.g.
  `Environment.GetEnvironmentVariable("ETO_X") == "1"` → `Console.Error.WriteLine(...)`) so you can
  dump internal measurements without editing call sites. Remove them before finishing.

**The actual pixel-grab is backend-specific** — reach `form.ControlObject`/`.NativeHandle` for the
native window and use that toolkit's capture API:

- **Gtk** (verified — Linux/XWayland here; run with `DISPLAY=:0 GDK_BACKEND=x11`):

  ```csharp
  var native = form.ControlObject as Gtk.Widget;
  var gdkWin = native?.Window ?? (native?.Toplevel as Gtk.Window)?.Window;
  var pb = new Gdk.Pixbuf(gdkWin, 0, 0, gdkWin.Width, gdkWin.Height); // Gdk.Pixbuf.FromWindow
  pb.Save("/path/out.png", "png");
  ```

- **Mac** (not yet tried here): render the `NSView`/`NSWindow` — `NSView.BitmapImageRepForCachingDisplay`
  with `CacheDisplay`, or `CGWindowListCreateImage` for the whole window — then write via
  `NSBitmapImageRep.AsTiff()`/PNG representation.
- **Wpf** (not yet tried): `RenderTargetBitmap.Render(visual)` → `PngBitmapEncoder` to a file.
- **WinForms** (not yet tried): `Control.DrawToBitmap(bmp, rect)`, or `Graphics.CopyFromScreen` for
  on-screen pixels → `Bitmap.Save(..., ImageFormat.Png)`.

Only the Gtk path above has been exercised; treat the others as starting points to verify.

## Injecting real keystrokes to test input (Gtk)

To exercise the *real* keypress→widget path (e.g. verifying a TextBox fires input events exactly
once), drive a shown window with **xdotool over the real X display** — `gtk_test_widget_send_key`
P/Invoke is a no-op here (needs a focused toplevel under a WM). Window management can't be relied
on (see the mouse section below — the session is mutter/XWayland), so:

- Launch the app (`DISPLAY=:0 GDK_BACKEND=x11`), give the target widget focus, print a `READY`
  marker, and add a `UITimer` auto-quit so the process never hangs (no `kill` — it's forbidden).
- `xdotool` isn't on the login-shell PATH here; call it as `/usr/bin/xdotool`.
- `search --name <title>` returns *two* window ids (client + GTK helper) — use the **last** one.
- `windowactivate` fails `BadWindow` without a WM; use `windowfocus --sync <id>` then inject with
  **XTEST** (`xdotool key a` / `xdotool type "ab"` — *no* `--window`, which uses XSendEvent that GTK
  ignores). XTEST goes to whatever holds X input focus, which `windowfocus` just set.

## Injecting mouse events to test drag/capture behaviour (Gtk)

**xdotool pointer injection does not work here** (only keyboard does): under a mutter/XWayland
session a full-screen `mutter guard window` sits above all X clients, so XTEST `mousemove`/
`mousedown`/`click` never reach the app (`xdotool getmouselocation` reports the root window even
when the pointer is over your window). Also `xdotool search --name` returns both the mutter frame
*and* the client window — the frame's geometry is offset from the client's, so clicking `frame + n`
lands on the decoration; check `xwininfo -id <id>` (`Width`/`Height`) to pick the client.

Instead synthesize GDK events and push them through GTK's real dispatch, which honours grabs
(`gtk_grab_add` from `gtk_dialog_run`, etc.), so capture/grab behaviour is exercised faithfully:

```csharp
var ev = Gdk.EventHelper.New(Gdk.EventType.ButtonPress); // or MotionNotify/ButtonRelease
var bev = new Gdk.EventButton(ev.Handle) { Window = eventBoxGdkWindow, SendEvent = true,
    Time = time += 100, X = 60, Y = 60, Button = 1, State = 0,
    Device = Gdk.Display.Default.DefaultSeat.Pointer }; // XRoot/YRoot from Window.GetOrigin
Gtk.Main.DoEvent(ev);
```

- Get the widget to target from the handler's `EventControl` (an `EtoEventBox` for `Panel`) and use
  its `.Window` as the event window, otherwise GTK routes the event elsewhere.
- A modal `Dialog` blocks in a nested main loop inside your handler, so schedule each later step on
  its **own** `GLib.Timeout` source — a single source won't re-enter while its callback is blocked.
- Anything reading the *real* pointer (`Mouse.Position`, `Mouse.Buttons`, and so the location and
  buttons of events Eto synthesizes itself) still reports the physical mouse, not your fake events.

## Test project layout (non-obvious)

- Unit-test **source `.cs` files live in `test/Eto.Test/UnitTests/`** (compiled as part of
  the shared `Eto.Test` project) — that's where you edit tests.
- You **run** them via `test/Eto.Test.UnitTests/` — a thin runner project (only `Program.cs`)
  that references `Eto.Test.csproj`. Don't look for test code there.
- **Backend-specific tests go in `test/Eto.Test.<Platform>/UnitTests/`** (e.g.
  `test/Eto.Test.Mac/UnitTests/`, namespace `Eto.Test.Mac.UnitTests`, deriving from the shared
  `Eto.Test.UnitTests.TestBase`). Put a test there rather than P/Invoking or reflecting your way to
  native APIs from the shared project — those projects reference the backend and its bindings
  directly (`Eto.Mac.Messaging`, `ObjCExtensions`, MonoMac types via global usings; note `Messaging`
  is ambiguous with `MonoMac.ObjCRuntime.Messaging`, so qualify it as `Eto.Mac.Messaging`).
- They run **both** ways: `Eto.Test.UnitTests` references the platform test apps and its
  `Program.GetTestAssemblies()` yields each one (gated on `Platform.Instance.IsMac`/`IsGtk`/…), so
  `dotnet test` picks them up; and the Eto.Test GUI app's **Unit Tests section** runs them via
  `app.TestAssemblies.Add(typeof(Startup).Assembly)` in each platform's `Startup`. Discovery is
  per-assembly, so a new fixture in an existing `Eto.Test.<Platform>/UnitTests/` folder needs no
  registration. `dotnet test ... -- --platform=mac|gtk|wpf|winforms` overrides platform detection.

## Mac: `AddMethod`/`ClassAddProtocol` are per-CLASS, not per-instance

`MacBase.AddMethod` and `ObjCExtensions.ClassAddProtocol` both resolve `Class.GetHandle(view.GetType())`,
so anything they add applies to **every instance of that native class, app-wide, forever**. The delegate
bodies compensate by looking the handler up per instance (`MacBase.GetHandler(obj)`), but *conformance*
does not — e.g. hooking `TextInput` on one `Drawable` used to make every `EtoDrawableView` conform to
`NSTextInputClient`, so the OS treated them all as text input (AutoFill in their context menus).
`MacViewTextInput` handles this by also overriding `inputContext`/`conformsToProtocol:` to answer per
instance from `IMacViewHandler.HandlesTextInput`. Apply the same pattern for any new class-level state.

**`e.Handled = true` in `MouseDown` silently kills the control's `ContextMenu` on Mac.**
`MacView.TriggerMouseDown` only forwards to `objc_msgSendSuper` when `!args.Handled`, and it's that super
call to `rightMouseDown:` that makes AppKit show the view's menu. A handler that blanket-sets `Handled`
must exclude `MouseButtons.Alternate` (see `DrawableSection.InputMethodDrawable`).

**Verifying such overrides needs raw `objc_msgSend`.** MonoMac's managed members (`NSView.InputContext`,
`NSObject.ConformsToProtocol`, …) dispatch via `objc_msgSendSuper` for *managed subclasses*
(`IsDirectBinding == false`), which skips the very override you added — so the managed API reports the
un-overridden answer and the fix looks broken. Use `Eto.Mac.Messaging.*_objc_msgSend*` on `view.Handle`
instead (see `Eto.Test.Mac/UnitTests/DrawableTests.IsTextInputClient`). AppKit itself calls through
normal dispatch, so it does see the override.

## Adding a member to a widget's `IHandler`

Handler interfaces have no default implementations (core targets `netstandard2.0`), so a new member
must be implemented in **every** registered backend or the build breaks. For a given widget, find
them via `grep -rn "<Widget>.IHandler" src/ --include="*.cs"` — note `Eto.iOS`/`Eto.WinUI` often have
theirs commented out in `Platform.cs` and so need nothing. On macOS/Linux you can only compile
`Eto`, `Eto.Gtk` and `Eto.Mac`; `Eto.Wpf`/`Eto.WinForms` need Windows (net48 also needs the targeting
pack) and `Eto.Android` needs the android workload — so those edits go in unverified by compile.

Note this is a binary-breaking change for any third-party handler implementation.

## `CheckBoxList` / `EnumCheckBoxList` gotcha

**`SelectedValues`/`SelectedKeys` silently no-op until the control is loaded.** The setters iterate
an internal `buttons` list that isn't populated until `CheckBoxList.OnLoad` assigns
`DataStore = CreateDefaultItems()` — and that happens *after* `base.OnLoad` raises the `Load` event,
so a `Load` handler is still too early. Set an initial selection from `LoadComplete` (or after
touching `.Items`, which materializes the data store), never from the constructor.

Also, `EnumCheckBoxList`'s `AddValue` filter matches on the enum **value**, not the name, so it can't
tell apart two members that share a value — filtering out an aggregate like `All` also drops any
single flag that happens to equal it, and the list can come up empty. Same reason `Enum.GetNames`
reports both names for that value.

## `[Flags]` enums that mean "everything, including future values"

Enum members are inlined into consumers as **compile-time constants**, so redefining an aggregate
(`All = A` → `All = A | B`) silently leaves already-compiled callers passing the old bits. Where a
value means "all of it, whatever gets added later", give it every bit: `All = ~0` (see
`ContextMenuSystemItems`; the BCL does the same with `EventKeywords.All = -1`). Handlers must then
test `value != None` rather than `value.HasFlag(All)`, which only matches when *every* bit is set.
`ToString()` still prints `All`, since exact member matches win over bitfield decomposition.
Note `MenuBarSystemItems.All = Common | Quit` predates this and has the older shape.

## Platform / TFM mapping

The test + platform projects pick a backend by TargetFramework:
- `net10.0` → Gtk (Linux) and Mac
- `net10.0-windows` / `net48` → Wpf (Windows only)

Core `Eto` project targets `netstandard2.0;net6.0;net8.0;net10.0`.
Platform backends live in `src/Eto.<Platform>/` (Gtk, Mac, Wpf, WinForms, WinUI, iOS,
Android, Direct2D). Solution: `src/Eto.slnx`.

## Gtk preferred-size gotchas (non-obvious)

- **GTK returns a *stale* preferred size on the first size request after `ShowAll` on an
  unrealized/unmapped widget** (esp. `GtkTreeView`); the settled value only comes back on a
  *subsequent* request. `GtkControl.GetPreferredSizeForControl` primes this with a throwaway
  `GetPreferredSize` call (GTK3 only) so measuring a control before it's shown is correct — and
  so a control's standalone `GetPreferredSize()` matches its size when measured inside a
  container (the invariant `ControlsShouldHavePreferredSize` enforces).
- **`GtkTreeView` cell renderers report size `0` until the tree is mapped on a *real* on-screen
  window** — realizing, pumping the event loop, and even `Gtk.OffscreenWindow` do *not* make
  them measure. So grid height can't be measured natively before first show; `GridHandler`
  computes it explicitly from row count × row height (falling back to the Pango font line height
  when the renderers report 0) and feeds it into `EtoScrolledWindow.OnGetPreferredHeight` so both
  direct and in-container measurements are correct.

## Code style

- **C# (`*.cs`) uses tabs, indent size 4** (per `.editorconfig`). Project/props files
  (`*.csproj`, `*.props`, `*.targets`, `*.slnx`) use 2-space.
- Braces on their own line (`csharp_new_line_before_open_brace = all`).
