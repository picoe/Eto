using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Eto.Designer;

namespace Eto.Designer
{
	public static class PlatformInitializerExtensions
	{
		public static void LoadAssembly(this Platform platform, string assemblyName)
		{
			var asm = Assembly.Load(assemblyName);
			if (asm != null)
				LoadAssembly(platform, asm);
		}

		public static void LoadAssembly(this Platform platform, Assembly assembly)
		{
			var attributes = assembly.GetCustomAttributes<PlatformInitializerAttribute>();
			foreach (var initInfo in attributes)
			{
				var init = Activator.CreateInstance(initInfo.InitializerType) as IPlatformInitializer;
				if (init == null)
					throw new ArgumentNullException(string.Format(CultureInfo.CurrentCulture, "Wrong type specified for platform initializer (must implement IPlatformInitializer)"));
				init.Initialize(platform);
			}
		}
	}

	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class PlatformInitializerAttribute : Attribute
	{
		public Type InitializerType { get; private set; }

		public PlatformInitializerAttribute(Type initializerType)
		{
			if (!typeof(IPlatformInitializer).IsAssignableFrom(initializerType))
				throw new ArgumentOutOfRangeException("initializerType", "Type specified for PlatformInitializerAttribute must implement Eto.IPlatformInitializer");
			this.InitializerType = initializerType;
		}
	}

	public interface IPlatformInitializer
	{
		void Initialize(Platform platform);
	}
}
