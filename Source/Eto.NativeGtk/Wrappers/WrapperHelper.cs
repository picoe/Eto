using System;
using System.Runtime.InteropServices;
using System.Text;

static class WrapperHelper
{
    public static string GetString(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
            return "";

        int len = 0;
        while (Marshal.ReadByte(ptr, len) != 0)
            len++;

        var bytes = new byte[len];
        Marshal.Copy(ptr, bytes, 0, bytes.Length);

        return Encoding.UTF8.GetString(bytes);
    }
}
