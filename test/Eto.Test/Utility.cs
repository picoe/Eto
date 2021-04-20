using System.Text;

namespace Eto.Test
{
	public static class Utility
	{
		public static string LoremText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

		public static string GenerateLoremText(int length)
		{
			var words = LoremText.Split(' ');
			var sb = new StringBuilder();
			var loremIndex = 0;
			for (int i = 0; i < length; i++)
			{
				if (loremIndex >= words.Length)
				{
					loremIndex = 0;
					sb.AppendLine();
				}
				if (i > 0)
					sb.Append(" ");
				sb.Append(words[loremIndex++]);
			}
			return sb.ToString();
		}
		
	}
}
