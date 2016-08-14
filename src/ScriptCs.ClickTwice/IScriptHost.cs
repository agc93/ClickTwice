using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptCs.ClickTwice
{
    public interface IScriptHost
    {
        void Log(string message);
        void Error(string message);
        string GetCurrentPath();
        void ExitWithException(Exception ex, int exitCode = 1);
    }
}
