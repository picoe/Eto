#if NET
namespace Eto.GtkSharp
{
	static class DllImportResolverManager
	{
		static readonly object gate = new object();
		static readonly List<DllImportResolver> resolvers = new List<DllImportResolver>();

		static DllImportResolverManager()
		{
			NativeLibrary.SetDllImportResolver(typeof(DllImportResolverManager).Assembly, Resolve);
		}

		public static void Add(DllImportResolver resolver)
		{
			if (resolver == null)
				throw new ArgumentNullException(nameof(resolver));

			lock (gate)
			{
				resolvers.Insert(0, resolver); // Insert at beginning so more recent entries get priority over older ones 
			}
		}

		public static void Remove(DllImportResolver resolver)
		{
			if (resolver == null)
				throw new ArgumentNullException(nameof(resolver));

			lock (gate)
			{
				resolvers.Remove(resolver);
			}
		}

		static IntPtr Resolve(string name, Assembly assembly, DllImportSearchPath? path)
		{
			lock (gate)
			{
				foreach (var resolver in resolvers)
				{
					var handle = resolver(name, assembly, path);
					if (handle != IntPtr.Zero)
						return handle;
				}
			}

			return IntPtr.Zero;
		}
	}
}
#endif
