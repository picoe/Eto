using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Collections;

namespace Eto
{
	/// <summary>
	/// Obsolete resource helper class
	/// </summary>
	public static class Resources
	{
		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("Use Assembly.GetExecutingAssembly().GetManifestResourceStream")]
		public static Stream GetResource(string filename)
		{
			return GetResource(filename, Assembly.GetCallingAssembly());
		}

		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("Use Assembly.GetManifestResourceStream")]
		public static Stream GetResource (string resourceName, Assembly asm)
		{
			if (asm == null) asm = Assembly.GetCallingAssembly();
			return asm.GetManifestResourceStream(resourceName);
		}
        public static Stream GetEmbeddedResource(
            string resourceFilename,
            string resourceName, 
            Assembly asm)
        {
            if (asm == null) 
                asm = Assembly.GetCallingAssembly();

            // 1. Load embedded .resources file
            using (Stream stream =
                     asm.GetManifestResourceStream(
                       resourceFilename))
            {
                if (stream != null)
                {
                    // 2. Find resource in .resources file
                    //using (
                    var reader =
                    new ResourceReader(stream);
                    //)
                    {
                        foreach (
                            DictionaryEntry item 
                            in reader)
                        {
                            if ((string)item.Key == resourceName)
                            {
                            }                            
                        }
                        byte[] data = null;
                        string resourceType = null;

                        reader.GetResourceData(
                            resourceName.Trim(),
                            out resourceType,
                            out data);

                        if (data != null)
                            return new MemoryStream(
                                data);
                    }
                }
            }

            return null;
        }
	}
}
