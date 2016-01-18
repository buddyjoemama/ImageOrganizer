using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class DirectoryHelper
    {
        public static IEnumerable<String> ListAllDirectories(String root)
        {
            foreach (String dir in Directory.EnumerateDirectories(root))
            {
                if (Directory.GetFiles(dir).Count() > 0)
                    yield return dir;

                foreach (String subDir in ListAllDirectories(dir))
                {
                    yield return subDir;
                }
            }
        }
    }
}
