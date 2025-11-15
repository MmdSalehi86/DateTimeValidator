using System.Diagnostics;
using System;

class TimeZoneHelper
{
    public static bool EnsureTimeZoneIsTehran()
    {
        string expectedTz = "Iran Standard Time";

        var getProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "tzutil",
                Arguments = "/g",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        getProcess.Start();
        string currentTz = getProcess.StandardOutput.ReadToEnd().Trim();
        getProcess.WaitForExit();

        if (!string.Equals(currentTz, expectedTz, StringComparison.OrdinalIgnoreCase))
        {
            var setProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "tzutil",
                    Arguments = $"/s \"{expectedTz}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            setProcess.Start();
            setProcess.WaitForExit();

            if (setProcess.ExitCode == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[Time Zone successfully set to Tehran]");
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Failed to set Time Zone. Exit Code] : {setProcess.ExitCode}");
                return false;
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[Time Zone is already set to Tehran]");
            return true;
        }
    }
}