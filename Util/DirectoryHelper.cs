using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public static class DirectoryHelper
    {
        private static IEnumerable<String> ListAllDirectories(String root)
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

        public static IEnumerable<String> ListAllDirectories(String root, bool recurse = true)
        {
            yield return root;

            if (recurse)
            {
                foreach(var dir in ListAllDirectories(root))
                {
                    yield return dir;
                }
            }
            else
            {
                foreach (var dir in Directory.EnumerateDirectories(root))
                {
                    yield return dir;
                }
            }
        }
    }
}
