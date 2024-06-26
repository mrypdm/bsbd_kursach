﻿using System;
using System.Runtime.InteropServices;
using System.Security;

namespace DatabaseClient.Extensions;

public static class StringExtensions
{
    public static SecureString AsSecure(this string value)
    {
        var secureString = new SecureString();

        if (!string.IsNullOrWhiteSpace(value))
        {
            foreach (var c in value)
            {
                secureString.AppendChar(c);
            }
        }

        return secureString;
    }

    public static string Unsecure(this SecureString value)
    {
        if (value == null || value.Length == 0)
        {
            return string.Empty;
        }

        var ptr = IntPtr.Zero;

        try
        {
            ptr = Marshal.SecureStringToGlobalAllocUnicode(value);
            return Marshal.PtrToStringUni(ptr);
        }
        finally
        {
            if (ptr != IntPtr.Zero)
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
        }
    }
}