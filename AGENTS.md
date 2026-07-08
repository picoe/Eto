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
- **Windowed/GUI tests run without special setup on all platforms** (Linux, Mac, Windows) as
  long as a display is available — on Linux that's a real X display (e.g. `DISPLAY=:0`; no
  `xvfb-run` needed). Tests that `Show()`/`Close()` windows (e.g.
  `PreferredSizeShouldAccountForAllRowsWhenLargerThanWindow`, anything via
  `ShownAsync`/`Form`/`Paint`) actually execute.

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

## Code style

- **C# (`*.cs`) uses tabs, indent size 4** (per `.editorconfig`). Project/props files
  (`*.csproj`, `*.props`, `*.targets`, `*.slnx`) use 2-space.
- Braces on their own line (`csharp_new_line_before_open_brace = all`).
