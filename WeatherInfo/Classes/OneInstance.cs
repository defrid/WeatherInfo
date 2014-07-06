﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WeatherInfo.Classes
{
    class OneInstance
    {

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        private const int SW_SHOWMAXIMIZED = 1;

        public static bool EnsureSingleInstance()
        {
            Process currentProcess = Process.GetCurrentProcess();

            var runningProcess = (from process in Process.GetProcesses()
                                  where
                                    process.Id != currentProcess.Id &&
                                    process.ProcessName.Equals(
                                      currentProcess.ProcessName,
                                      StringComparison.Ordinal)
                                  select process).FirstOrDefault();

            if (runningProcess != null)
            {
                ShowWindow(runningProcess.MainWindowHandle, SW_SHOWMAXIMIZED);
                SetForegroundWindow(runningProcess.MainWindowHandle);

                return false;
            }

            return true;
        }
    }
}
