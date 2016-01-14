using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public partial class ServiceConfiguration
    {
        public String RootDirectory
        {
            get
            {
                return Path.Combine("\\\\" + this.host, this.rootPath);
            }
        }

        public int MaxThreadsAsInt
        {
            get
            {
                return int.Parse(maxThreads);
            }
        }
    }
}
