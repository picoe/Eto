using System;
using System.Reflection;
using System.IO;

namespace Eto.Drawing
{
	public interface IIcon : IImage
	{
		void Create (Stream stream);

		void Create (string fileName);
	}
	
	public class Icon : Image
	{
		IIcon inner;
		
		public Icon (IIcon inner) : base(Generator.Current, inner)
		{
			this.inner = inner;
		}
	
		public Icon (Stream stream) : base(Generator.Current, typeof(IIcon))
		{
			inner = (IIcon)Handler;
			inner.Create (stream);
		}

		public Icon (string fileName) : base(Generator.Current, typeof(IIcon))
		{
			inner = (IIcon)Handler;
			inner.Create (fileName);
		}
		
		public static Icon FromResource (Assembly asm, string resourceName)
		{
			if (asm == null)
				asm = Assembly.GetCallingAssembly ();
			using (var stream = Resources.GetResource (resourceName, asm)) {
				return new Icon (stream);
			}
		}

		public static Icon FromResource (string resourceName)
		{
			var asm = Assembly.GetCallingAssembly ();
			return FromResource (asm, resourceName);
		}
		
		[Obsolete("Use Icon.FromResource instead")]
		public Icon (Assembly asm, string resourceName) : base(Generator.Current, typeof(IIcon))
		{
			inner = (IIcon)Handler;
			if (asm == null)
				asm = Assembly.GetCallingAssembly ();
			Stream stream = Resources.GetResource (resourceName, asm);
			if (stream == null)
				Console.WriteLine ("Resource not found: {0} - {1}", asm.FullName, resourceName);
			else {
				inner.Create (stream);
				stream.Close ();
			}
		}
	}
}
