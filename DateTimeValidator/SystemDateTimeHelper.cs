using System;
using System.Runtime.InteropServices;

namespace DateTime_Validator
{
    public static class SystemDateTimeHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st); // UTC

        public static bool SetWindowsSystemDateTime(DateTime utc)
        {
            SYSTEMTIME st = new SYSTEMTIME
            {
                wYear = (ushort)utc.Year,
                wMonth = (ushort)utc.Month,
                wDay = (ushort)utc.Day,
                wHour = (ushort)utc.Hour,
                wMinute = (ushort)utc.Minute,
                wSecond = (ushort)utc.Second,
                wMilliseconds = (ushort)utc.Millisecond
            };

            var needUpdating = NeedUpdating(utc);
            if (needUpdating)
            {

                bool success = SetSystemTime(ref st);
                if (success)
                {
                    int err = Marshal.GetLastWin32Error();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[DateTime updated successfully]");
                    return true;
                }
                else
                {
                    int err = Marshal.GetLastWin32Error();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[Set DateTime Error] : {err}");
                    Console.WriteLine($"[Run as admin app]");
                    return false;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[The DateTime has already been updated]");
                return true;
            }
        }

        private static bool NeedUpdating(DateTime newUTC)
        {
            var now = DateTime.UtcNow;
            var difference = Math.Abs((now - newUTC).TotalMinutes);
            if (difference >= 1)
                return true;
            return false;
        }
    }
}
