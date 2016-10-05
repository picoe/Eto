using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eto.Designer.Builders
{
	public abstract class BaseCompiledInterfaceBuilder : IInterfaceBuilder, IDisposable
	{
		string output;

		public static string InitializeAssembly { get; set; }

		~BaseCompiledInterfaceBuilder()
		{
			Dispose(false);
		}

		void RemoveOutput()
		{
			if (!string.IsNullOrEmpty(output) && File.Exists(output))
			{
				File.Delete(output);
				output = null;
			}
		}

		public static Control InstantiateControl(Type type)
		{
			if (type == null)
				return null;
			var control = Activator.CreateInstance(type) as Control;
			if (control != null)
			{
				var initializeMethod = control.GetType().GetMethod("InitializeComponent", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly, Type.DefaultBinder, Type.EmptyTypes, null);
				if (initializeMethod != null)
					initializeMethod.Invoke(control, null);
			}
			return control;
		}

		public class CompileResult
		{
			public bool Success { get; set; }

			public IEnumerable<string> Errors { get; set; }
		}

		protected abstract CompileResult Compile(string outputFile, string mainAssembly, IEnumerable<string> references, string code, out Assembly generatedAssembly);

		public void Create(string text, string mainAssembly, IEnumerable<string> references, Action<Control> controlCreated, Action<Exception> error)
		{
			RemoveOutput();

			references = references?.ToList();
			ThreadPool.QueueUserWorkItem(state =>
			{
				try
				{
					Assembly generatedAssembly;
					//output = Path.Combine(Path.GetTempPath(), "EtoDesigner", Path.GetRandomFileName() + ".dll");

					var result = Compile(null, mainAssembly, references, text, out generatedAssembly);
					if (result.Success)
					{
						Application.Instance.Invoke(() =>
						{
							try
							{
								var type = FindControlType(generatedAssembly);
								var control = InstantiateControl(type);

								if (control != null)
									controlCreated(control);
								else
									error(new FormatException("Could not find control. Make sure you have a single class derived from Control."));
							}
							catch (Exception ex)
							{
								error(ex);
							}
						});
					}
					else
					{
						var errorText = string.Join("\n", result.Errors);
						Application.Instance.Invoke(() => error(new FormatException(string.Format("Compile error: {0}", errorText))));
					}
				}
				catch (Exception ex)
				{
					Application.Instance.Invoke(() => error(ex));
				}
			});
		}


		protected const string ReferenceAssembliesFolder = @"Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5";
		protected const string MacMonoInstallationFolder = @"/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5";

		protected string GetReferenceAssembliesPath(string basePath)
		{
			if (string.IsNullOrEmpty(basePath))
				return null;
			var path = Path.Combine(basePath, ReferenceAssembliesFolder);
			return Directory.Exists(path) ? path : null;
		}

		protected string GetReferenceAssembliesFolder()
		{
			string referenceDir = null;
			if (EtoEnvironment.Platform.IsWindows)
			{
				referenceDir = GetReferenceAssembliesPath(Environment.GetEnvironmentVariable("ProgramFiles(x86)"));
				if (referenceDir == null)
					referenceDir = GetReferenceAssembliesPath(Environment.GetEnvironmentVariable("ProgramFiles"));
			}
			else if (EtoEnvironment.Platform.IsMac && Directory.Exists(MacMonoInstallationFolder))
			{
				referenceDir = MacMonoInstallationFolder;
			}

			if (string.IsNullOrEmpty(referenceDir))
			{
				// Linux
				// use an arbitrary type that lives in mscorlib to find the location of
				// mscorlib. We assume that this path is the directory that contains the othere
				// reference assemblies as well.
				referenceDir = Path.GetDirectoryName(typeof(Array).Assembly.Location);
			}
			return referenceDir;
		}

		public static Type FindControlType(Assembly assembly)
		{
			if (assembly == null)
				return null;
			return assembly.GetTypes().FirstOrDefault(t => typeof(Control).IsAssignableFrom(t));
		}


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			RemoveOutput();
		}
	}
}
