#if NETSTANDARD1_0
using System;
using System.IO;
using System.Reflection;

namespace System.Runtime.Serialization.Formatters.Binary
{
	// so we don't need #if's everywhere
}

namespace Eto
{
	/// <summary>
	/// BinaryFormatter via reflection used for .net standard 1.0 compatibility
	/// </summary>
	class BinaryFormatter
	{

		static Type s_BinaryFormatterType = Type.GetType("System.Runtime.Serialization.Formatters.Binary.BinaryFormatter")
			?? Type.GetType("System.Runtime.Serialization.Formatters.Binary.BinaryFormatter, System.Runtime.Serialization.Formatters")
			?? Type.GetType("System.Runtime.Serialization.Formatters.Binary.BinaryFormatter, netstandard");
		static MethodInfo s_SerializeMethod = s_BinaryFormatterType?.GetRuntimeMethod("Serialize", new[] { typeof(Stream), typeof(object) });
		static MethodInfo s_DeserializeMethod = s_BinaryFormatterType?.GetRuntimeMethod("Deserialize", new[] { typeof(Stream) });

		object binaryFormatter;

		public BinaryFormatter()
		{
			if (s_BinaryFormatterType == null || s_SerializeMethod == null || s_DeserializeMethod == null)
				throw new InvalidOperationException("Could not create an instance of BinaryFormatter");

			binaryFormatter = Activator.CreateInstance(s_BinaryFormatterType);
		}

		public void Serialize(Stream stream, object value)
		{
			s_SerializeMethod.Invoke(binaryFormatter, new object[] { stream, value });
		}

		public object Deserialize(Stream stream)
		{
			return s_DeserializeMethod.Invoke(binaryFormatter, new object[] { stream });
		}
	}
}
#endif
