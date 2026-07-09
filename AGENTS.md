# Eto.Forms — Agent Guide

Eto.Forms cross-platform UI toolkit. Keep this file compact — it loads every
session. Record only durable, non-obvious Eto facts; skip anything obvious from code/git.

**Standing instruction:** whenever you discover a non-obvious fact about this repo — a
build/test broke for a surprising reason, an undocumented quirk/flag/path/ordering, or
something behaved unexpectedly — record it here immediately in the right section. Curate:
merge into existing entries and fix stale ones rather than appending near-duplicates.

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

## Test project layout (non-obvious)

- Unit-test **source `.cs` files live in `test/Eto.Test/UnitTests/`** (compiled as part of
  the shared `Eto.Test` project) — that's where you edit tests.
- You **run** them via `test/Eto.Test.UnitTests/` — a thin runner project (only `Program.cs`)
  that references `Eto.Test.csproj`. Don't look for test code there.

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
