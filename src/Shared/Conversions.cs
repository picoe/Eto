namespace Eto
{

	/// <summary>
	/// Conversions code which are platform independent
	/// </summary>
	public static partial class Conversions
	{
		/// <summary>
		/// Translates degrees to radians.
		/// </summary>
		/// <param name="angle">The angle for conversion.</param>
		public static float DegreesToRadians(float angle)
		{
			return (float)Math.PI * angle / 180.0f;
		}

		/// <summary>
		/// Translates mnemonics in a string to a platform.
		/// </summary>
		/// <param name="value">The value to check for mnemonics.</param>
		public static string ToPlatformMnemonic(this string value)
		{
			if (value == null)
				return string.Empty;

			value = value.Replace("_", "__");

			var index = FindMnemonic(value, '&');
			if (index >= 0)
			{
				var sb = new StringBuilder(value);
				sb[index] = '_';
				sb.Replace("&&", "&");
				return sb.ToString();
			}

			return value.Replace("&&", "&");
		}

		/// <summary>
		/// Translates mnemonics in a string to eto.
		/// </summary>
		/// <param name="value">The value to check for mnemonics.</param>
		public static string ToEtoMnemonic(this string value)
		{
			if (value == null)
				return null;

			var index = FindMnemonic(value, '_');
			if (index >= 0)
			{
				var sb = new StringBuilder(value);
				sb[index] = '&';
				sb.Replace("__", "_");
				return sb.ToString();
			}

			return value.Replace("__", "_");
		}

		internal static int FindMnemonic(string value, char marker)
		{
			for (int i = value.IndexOf(marker); i >= 0; i = value.IndexOf(marker, i + 1))
			{
				var start = i;
				while (i + 1 < value.Length && value[i + 1] == marker)
					i++;
				// A leading run containing escaped pairs remains literal for compatibility.
				if ((i - start) % 2 == 0 && (start > 0 || i == 0))
					return i;
			}
			return -1;
		}

	}

}
