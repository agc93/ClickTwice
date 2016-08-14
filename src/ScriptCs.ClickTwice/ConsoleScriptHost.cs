using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ScriptCs.ClickTwice
{
    class ConsoleScriptHost : IScriptHost
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message)
        {
            var oldColor = ForegroundColor;
            ForegroundColor = ConsoleColor.Red;
            WriteLine(message);
            ForegroundColor = oldColor;
        }

        public string GetCurrentPath()
        {
            return Environment.CurrentDirectory;
        }

        public void ExitWithException(Exception ex, int exitCode = 1)
        {
            Error("=====================");
            Error(ex.Message);
            Error("=====================");
            Environment.Exit(exitCode);
        }
    }
}
