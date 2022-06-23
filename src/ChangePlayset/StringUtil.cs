// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System;

namespace NekoBoiNick.change_playset {

  /// <summary>
  /// Contains utility functions for formatting localizable strings.
  /// </summary>
  internal static class StringUtil {
    internal const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
    internal const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
    internal const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
    internal const uint FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000;
    internal const uint FORMAT_MESSAGE_FROM_HMODULE = 0x00000800;
    internal const uint FORMAT_MESSAGE_FROM_STRING = 0x00000400;

    [DllImport("api-ms-win-core-localization-l1-2-1", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern uint FormatMessage(uint dwFlags,
                                              IntPtr lpSource,
                                              uint dwMessageId,
                                              uint dwLanguageId,
                                              [Out] StringBuilder lpBuffer,
                                              uint nSize,
                                              string[] Arguments);

    internal static string Format(string str) {
      return string.Format(CultureInfo.CurrentCulture, str);
    }

    internal static string Format(string fmt, string p0) {
      return string.Format(CultureInfo.CurrentCulture, fmt, p0);
    }

    internal static string Format(string fmt, string p0, string p1) {
      return string.Format(CultureInfo.CurrentCulture, fmt, p0, p1);
    }

    internal static string Format(string fmt, uint p0) {
      return string.Format(CultureInfo.CurrentCulture, fmt, p0);
    }

    internal static string Format(string fmt, int p0) {
      return string.Format(CultureInfo.CurrentCulture, fmt, p0);
    }

    internal static string FormatMessage(uint messageId, string[] args) {
      var message = new System.Text.StringBuilder(256);
      UInt32 flags = FORMAT_MESSAGE_FROM_SYSTEM;

      if (args == null)
        flags |= FORMAT_MESSAGE_IGNORE_INSERTS;
      else
        flags |= FORMAT_MESSAGE_ARGUMENT_ARRAY;

      var length = FormatMessage(flags, IntPtr.Zero, messageId, 0, message, 256, args);

      if (length > 0)
        return message.ToString();

      return null;
    }

    internal static string GetSystemMessage(uint messageId) {
      return FormatMessage(messageId, null);
    }
  }
}
