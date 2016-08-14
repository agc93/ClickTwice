using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickTwice.Publisher.MSBuild
{
    class ArgBuilder
    {
        private StringBuilder _sb;

        public ArgBuilder(string argBase = "")
        {
            _sb = new StringBuilder(argBase);
        }

        public ArgBuilder Append(string arg)
        {
            _sb.Append($" {arg}");
            return this;
        }

        public ArgBuilder AppendQuoted(string arg)
        {
            _sb.Append($@" ""{arg}""");
            return this;
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}
