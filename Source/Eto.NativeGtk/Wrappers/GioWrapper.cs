﻿using System;
using System.Runtime.InteropServices;

static partial class GioWrapper
{
    public const string NativeLib = "libgtk-3.so.0";

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "g_app_info_get_executable")]
private extern static IntPtr wg_app_info_get_executable(IntPtr appinfo);

public static string g_app_info_get_executable(IntPtr appinfo)
{
    var ret = WrapperHelper.GetString(wg_app_info_get_executable(appinfo));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "g_file_new_for_path")]
public extern static IntPtr g_file_new_for_path(string path);

}
