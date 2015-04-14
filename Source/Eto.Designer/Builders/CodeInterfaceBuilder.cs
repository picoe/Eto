using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Eto.Forms;
using Eto.Drawing;
using Eto.Designer;

namespace Eto.Designer.Builders
{
	public abstract class CodeInterfaceBuilder : IInterfaceBuilder, IDisposable
	{
		readonly object generate_lock = new object();
		AppDomain newDomain;
		string output;
		const string assemblyName = "Generated";


		public string InitializeAssembly { get; set; }
		protected string BaseDir { get; private set; }

		public abstract string GetSample();

		protected CodeInterfaceBuilder(string baseDir = null)
		{
			BaseDir = baseDir ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		}

		~CodeInterfaceBuilder()
		{
			Dispose(false);
		}

		private void RemoveOutput()
		{
			if (!string.IsNullOrEmpty(output) && File.Exists(output))
			{
				File.Delete(output);
				output = null;
			}
		}

		void UnloadDomain()
		{
			if (newDomain != null && !newDomain.IsFinalizingForUnload())
			{
				AppDomain.Unload(newDomain);
				newDomain = null;
			}
		}

		public static Control InstantiateControl(Type type)
		{
			var control = Activator.CreateInstance(type) as Control;
			if (control != null)
			{
				var initializeMethod = control.GetType().GetMethod("InitializeComponent", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly, Type.DefaultBinder, Type.EmptyTypes, null);
				if (initializeMethod != null)
					initializeMethod.Invoke(control, null);
			}
			var window = control as Window;
			if (window != null)
			{
				control = window.Content;
				window.Content = null;
			}
			return control;
		}

		public static Type FindControlType(Assembly asm)
		{
			return asm.GetTypes().FirstOrDefault(t => typeof(Control).IsAssignableFrom(t));
		}

		public void Create(string text, Action<Control> controlCreated, Action<string> error)
		{
			UnloadDomain();
			RemoveOutput();

			ThreadPool.QueueUserWorkItem(state =>
			{
				try
				{
					bool useAppDomain = Platform.Instance.Supports<IEtoAdapterHandler>();

					output = useAppDomain ? Path.GetTempFileName() + ".dll" : null;

					Assembly asm;

					var result = Compile(output, text, out asm);
					if (result.Success)
					{
						Application.Instance.Invoke(() =>
						{
							try
							{
								Control control = null;
								if (useAppDomain)
								{
									// doesn't work without for some reason
									if (!AppDomain.CurrentDomain.ShadowCopyFiles)
										AppDomain.CurrentDomain.SetShadowCopyFiles();

									var setup = new AppDomainSetup
									{
										ApplicationBase = BaseDir,
										PrivateBinPath = BaseDir,
										//PrivateBinPath = Path.GetDirectoryName(output) + ";" + BaseDir,
										ShadowCopyFiles = "true",
										ShadowCopyDirectories = BaseDir,
										//LoaderOptimization = LoaderOptimization.MultiDomainHost,
										LoaderOptimization = LoaderOptimization.NotSpecified
									};

									newDomain = AppDomain.CreateDomain("newDomain", null, setup);
									var module = newDomain.CreateInstanceFromAndUnwrap(typeof(ControlLoader).Assembly.Location, typeof(ControlLoader).FullName) as ControlLoader;
									if (module == null)
										throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Could not create ControlLoader instance in new domain"));
									//var executeMethod = module.GetType().GetMethod("Execute");
									//var contract = executeMethod.Invoke(module, new object[] { Platform.Instance.GetType().FullName + ", " + Platform.Instance.GetType().Assembly.FullName, assemblyName, InitializeAssembly });
									var contract = module.Execute(Platform.Instance.GetType().FullName + ", " + Platform.Instance.GetType().Assembly.FullName, assemblyName, InitializeAssembly);
									if (contract != null)
										control = EtoAdapter.ToControl(contract);
								}
								else
								{
									using (AssemblyResolver.Register(BaseDir))
									{
										var type = FindControlType(asm);
										if (type != null)
											control = InstantiateControl(type);
									}
								}
								if (control != null)
									controlCreated(control);
								else
									error("Could not find control. Make sure you have a single class derived from Control.");
							}
							catch (Exception ex)
							{
								error(string.Format("Error creating control: {0}", ex));
							}
						});
					}
					else
					{
						var errorText = string.Join("\n", result.Errors.Select(r => r.ToString()));
						Application.Instance.Invoke(() => error(string.Format("Compile error:\n{0}", errorText)));
					}
				}
				catch (Exception ex)
				{
					Application.Instance.Invoke(() => error(string.Format("Compile error: {0}", ex)));
				}
			});
		}

		protected abstract CodeDomProvider CreateCodeProvider();

		protected virtual void SetParameters(CompilerParameters parameters)
		{
			parameters.ReferencedAssemblies.AddRange(new[]
			{ 
				"mscorlib.dll",
				"System.dll",
				"System.Core.dll",
				typeof(Control).Assembly.Location
			});
		}

		public class CompileResult
		{
			public bool Success { get; set; }

			public IEnumerable<CompilerError> Errors { get; set; }
		}

		CompileResult Compile(string output, string code, out Assembly generatedAssembly)
		{
			var inMemory = string.IsNullOrEmpty(output);
			var parameters = new CompilerParameters
			{
				GenerateInMemory = inMemory,
				TreatWarningsAsErrors = false,
				GenerateExecutable = false,
				OutputAssembly = inMemory ? null : output
			};

			SetParameters(parameters);

			var codeProvider = CreateCodeProvider();
			var results = codeProvider.CompileAssemblyFromSource(parameters, code);

			var errors = results.Errors.Cast<CompilerError>().ToList();
			var result = new CompileResult { Errors = errors };
			if (errors.Count == 0 || errors.All(r => r.IsWarning))
			{
				if (inMemory)
					generatedAssembly = results.CompiledAssembly;
				else
					generatedAssembly = null;
				result.Success = true;
			}
			else
			{
				generatedAssembly = null;
				errors.ForEach(msg => Debug.WriteLine(msg.ToString()));
			}
			return result;
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				UnloadDomain();
				GC.SuppressFinalize(this);
			}
			RemoveOutput();
		}
	}
	
}
