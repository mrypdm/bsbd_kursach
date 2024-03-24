using System;
using System.Runtime.InteropServices;
using System.Security;

namespace DatabaseClient.Extensions;

public static class StringExtensions
{
    public static string Unsecure(this SecureString value)
    {
        if (value == null || value.Length == 0)
        {
            return string.Empty;
        }

        var ptr = IntPtr.Zero;
        var result = string.Empty;
        try
        {
            ptr = Marshal.SecureStringToGlobalAllocUnicode(value);
            result = Marshal.PtrToStringUni(ptr)!;
        }
        finally
        {
            if (ptr != IntPtr.Zero)
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
        }

        return result;
    }
}