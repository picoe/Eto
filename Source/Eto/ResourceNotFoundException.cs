using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;

namespace Eto
{
	[Serializable]
	public class ResourceNotFoundException : EtoException
	{
		public ResourceNotFoundException () { }
		public ResourceNotFoundException (string message) : base (message) { }
		public ResourceNotFoundException (string message, Exception inner) : base (message, inner) { }
		public ResourceNotFoundException (Assembly assembly, string resourceName)
			: this (string.Format ("Resource '{0}' not found in assembly '{1}'", resourceName, assembly.FullName))
		{ }
		protected ResourceNotFoundException (SerializationInfo info, StreamingContext context) : base (info, context) { }
	}
}
