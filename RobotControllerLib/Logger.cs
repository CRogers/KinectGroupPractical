using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RobotControllerLib
{
    public static class Logger
    {
        public static string File = "log.log";
        public static bool ToStdout = false;

        public static object locker = new object();

        static Logger()
        {
            lock(locker)
                System.IO.File.WriteAllText(File, "");
        }

        public static void Log(string message)
        {
            if (ToStdout)
                Console.WriteLine(message);
            else
                lock(locker)
                    System.IO.File.AppendAllText(File, message + "\n");
        }

        public static void Log(string fmt, params object[] fmts)
        {
            Log(String.Format(fmt, fmts));
        }
    }
}
