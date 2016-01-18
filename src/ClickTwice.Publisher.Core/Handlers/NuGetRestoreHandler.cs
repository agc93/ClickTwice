using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickTwice.Publisher.Core.Handlers
{
    class NuGetRestoreHandler : IInputHandler
    {
        public string Name => "NuGet Package Restore";

        public HandlerResponse Process(string inputPath)
        {
            try
            {
                if (File.Exists(Path.Combine(new FileInfo(inputPath).DirectoryName, "packages.config")))
                {
                    
                }
            }
            catch (Exception ex)
            {
                return new HandlerResponse(this, false, ex.Message);
            }
            return null;
        }
    }
}
