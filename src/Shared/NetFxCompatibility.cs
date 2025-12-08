namespace Eto;

#if NETFRAMEWORK || !NET7_0_OR_GREATER

static class StreamExtensions
{
	/// <summary>
	/// Reads exactly the specified number of bytes from the stream into the buffer.
	/// </summary>
	/// <param name="stream"></param>
	/// <param name="buffer"></param>
	/// <param name="offset"></param>
	/// <param name="count"></param>
	/// <exception cref="EndOfStreamException"></exception>
	public static void ReadExactly(this Stream stream, byte[] buffer, int offset, int count)
	{
		int totalRead = 0;
		while (totalRead < count)
		{
			int bytesRead = stream.Read(buffer, offset + totalRead, count - totalRead);
			if (bytesRead == 0)
				throw new EndOfStreamException("Unable to read the required number of bytes from the stream.");
			totalRead += bytesRead;
		}
	}
}

#endif