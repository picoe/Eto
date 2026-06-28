using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eto.GirCore.Forms;

public class ApplicationHandler : WidgetHandler<Gtk.Application, Application, Application.ICallback>, Application.IHandler
{
	bool attached;
	SynchronizationContext? mainContext;

	public ApplicationHandler()
	{
	}

	public bool QuitIsSupported => true;

	public Keys CommonModifier => Keys.Control;

	public Keys AlternateModifier => Keys.Alt;

	public string BadgeLabel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public bool IsActive => throw new NotImplementedException();

	public int MainThreadID { get; private set; }
	public Theme Theme { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public void AsyncInvoke(Action action)
	{
		mainContext?.Post(arg =>
		{
			action();
		}, null);
	}

	public void Attach(object context)
	{
		attached = true;
		Control = (Gtk.Application)context;
		MainThreadID = Thread.CurrentThread.ManagedThreadId;
		mainContext = SynchronizationContext.Current;
	}
	public void Invoke(Action action)
	{
		if (Thread.CurrentThread.ManagedThreadId == MainThreadID)
			action();
		else
		{
			mainContext?.Send(arg =>
			{
				action();
			}, null);
		}
	}

	public void OnMainFormChanged()
	{
	}

	public void Open(string url)
	{
		try
		{
			Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
		}
		catch
		{
			// Use fallback for recent mono versions that do not support UseShellExecute.
			url = Uri.EscapeUriString(url);
			Process.Start("xdg-open", url);
		}
	}

	public void Quit()
	{
		var args = new CancelEventArgs();
		// var mainForm = Widget.MainForm != null ? Widget.MainForm.Handler as IGirWindow : null;
		// if (mainForm != null)
		// 	args.Cancel = !mainForm.CloseWindow(ce => Callback.OnTerminating(Widget, ce));
		// else
			Callback.OnTerminating(Widget, args);

		if (!args.Cancel)
		{
			Control.Quit();
		}
	}

	private string GetCommandLineArgs()
	{
		var cmdLine = string.Empty;
		var oldArgs = Environment.GetCommandLineArgs();
		if (oldArgs.Length > 1)
		{
			var args = new String[oldArgs.Length - 1];
			Array.Copy(oldArgs, 1, args, 0, args.Length);
			cmdLine = String.Join(" ", args);
		}

		return cmdLine;
	}

	private void RestartInternal()
	{
		var cmdLine = GetCommandLineArgs();
		var entry = Assembly.GetEntryAssembly()?.Location;
		if (entry == null)
			throw new InvalidOperationException("Cannot restart application without an entry assembly");
		if (entry.EndsWith(".exe", StringComparison.InvariantCulture))
		{
			// mono or windows, use Process.Start()
			Process.Start(entry, cmdLine);
		}
		else if (entry.EndsWith(".dll", StringComparison.InvariantCulture))
		{
			// .net core, look for self-contained deployment
			var exeExtension = Environment.OSVersion.Platform == PlatformID.Win32NT ? ".exe" : null;
			var loader = Path.ChangeExtension(entry, exeExtension);

			if (File.Exists(loader))
			{
				// self contained deployment
				Process.Start(loader, cmdLine);
			}
			else
			{
				// use dotnet to run entry dll
				Process.Start("dotnet", $"{entry} {cmdLine}");
			}
		}
		else
		{
			// don't know how to handle this
			throw new NotImplementedException("Entry assembly has unknown extension " + entry);
		}
	}

	public void Restart()
	{
		Control.Quit();

		RestartInternal();
	}

	public void Run()
	{

		if (!attached)
		{
			MainThreadID = Thread.CurrentThread.ManagedThreadId;
			Control = Gtk.Application.New("org.eto.gircore", Gio.ApplicationFlags.FlagsNone);
			Control.OnActivate += (sender, e) =>
			{
				mainContext = SynchronizationContext.Current;
				Callback.OnInitialized(Widget, EventArgs.Empty);
			};
			Control.RunWithSynchronizationContext(null);
		}

	}

	public void RunIteration()
	{
		var start = DateTime.Now;
		// while (Gtk.Application.EventsPending() && (DateTime.Now - start).TotalMilliseconds < 100)
		// 	Gtk.Application.RunIteration();
	}

	public override void AttachEvent(string id)
	{
		switch (id)
		{
			case Eto.Forms.Application.TerminatingEvent:
				// called automatically
				break;
			case Eto.Forms.Application.UnhandledExceptionEvent:
				GLib.UnhandledException.SetHandler(OnUnhandledException);
				break;
			case Eto.Forms.Application.NotificationActivatedEvent:
				// handled by NotificationHandler
				break;
			case Eto.Forms.Application.IsActiveChangedEvent:
				break;
			default:
				base.AttachEvent(id);
				break;
		}
	}

	private void OnUnhandledException(Exception exception)
	{
		var unhandledExceptionArgs = new UnhandledExceptionEventArgs(exception, true);
		Callback.OnUnhandledException(Widget, unhandledExceptionArgs);
	}
}
