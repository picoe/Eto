using System;

namespace Eto
{
	public class EtoException : Exception
	{
		public EtoException(string message)
			: base(message)
		{
		}
		
		public EtoException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
