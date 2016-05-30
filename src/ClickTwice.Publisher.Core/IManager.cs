using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickTwice.Publisher.Core
{
    public abstract class Manager
    {
        protected Manager(string projectFilePath)
        {
            this.ProjectFilePath = projectFilePath;
        }

        public string ProjectFilePath { get; protected set; }

        public bool CleanOutputOnCompletion { protected get; set; } = true;
    }
}
