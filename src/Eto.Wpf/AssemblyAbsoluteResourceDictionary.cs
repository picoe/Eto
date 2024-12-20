using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eto.Wpf
{
	public class AssemblyAbsoluteResourceDictionary : sw.ResourceDictionary, ISupportInitialize
	{
		public string AssemblyName { get; set; }

		public string Path { get; set; }

		internal static Uri GetAbsolutePackUri(string path, string assemblyName = null)
		{
			// Support having multiple copies of Eto running, potentially with different version and/or public key
			var assembly = assemblyName != null ? Assembly.Load(assemblyName) : typeof(AssemblyAbsoluteResourceDictionary).Assembly;
			var name = assembly.GetName();

			var version = "v" + name.Version.ToString() + ";";

			var publicKey = name.GetPublicKey();
			var publicKeyString = publicKey?.Length > 0 ? BitConverter.ToString(publicKey).Replace("-", "") + ";" : null;

			return new Uri($"pack://application:,,,/{name.Name};{version}{publicKeyString}component/{path}", UriKind.Absolute);
		}

		void ISupportInitialize.EndInit()
		{
			SetupSource();

			base.EndInit();
		}

		private void SetupSource()
		{
			if (Source != null)
				return;
			if (string.IsNullOrEmpty(Path))
				throw new InvalidOperationException("No Path was specified");

			Source = GetAbsolutePackUri(Path, AssemblyName);
		}
	}
}