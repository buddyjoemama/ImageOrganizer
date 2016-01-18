using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Util
{
    public partial class ServiceConfiguration
    {
        private static object locker = new object();

        public String RootDirectory
        {
            get
            {
                return Path.Combine("\\\\" + this.host, this.rootPath);
            }
        }

        public String DestinationRoot
        {
            get
            {
                return Path.Combine("\\\\" + this.archive.destination, this.archive.path);
            }
        }

        public int MaxThreadsAsInt
        {
            get
            {
                return int.Parse(maxThreads);
            }
        }

        public static ServiceConfiguration Config
        {
            get
            {
                XmlSerializer ser = new XmlSerializer(typeof(ServiceConfiguration));
                ServiceConfiguration config = ser.Deserialize(File.Open("ServiceConfig.xml", FileMode.Open)) as ServiceConfiguration;

                return config;
            }
        }

        public bool IsValidFile(string file)
        {
            file = file.ToLower();

            return SearchTypes.Any(s => file.EndsWith(s.key)) &&
                IgnoreTypes.Select(s => s.key).None(file);
        }

        public void ArchiveFile(DateTime fileCreateTime, String fileFullPath)
        {
            String ext = Path.GetExtension(fileFullPath);
            String folder = GetFolderName(fileCreateTime);
            String targetDir = Path.Combine(DestinationRoot, folder);
            String targetFile = Path.Combine(targetDir,
                $"{fileCreateTime.ToString("MM-dd-yyyy")}_{DateTime.UtcNow.Ticks.ToString()}{ext}");

            if (Directory.Exists(targetDir))
            {
                CopyOrMove(fileFullPath, targetFile);
            }
            else
            {
                lock (locker)
                {
                    Directory.CreateDirectory(targetDir);
                }

                CopyOrMove(fileFullPath, targetFile);
            }
        }

        private void CopyOrMove(String fileFullPath, String targetFile)
        {
            try
            {
                File.Copy(fileFullPath, targetFile, true);
            }
            catch { }
        }

        private String GetFolderName(DateTime dt)
        {
            String season = null;

            if(dt.Month >= 1 && dt.Month <= 3)
            {
                season = "Winter";
            }
            else if(dt.Month > 3 && dt.Month <= 6)
            {
                season = "Spring";
            }
            else if(dt.Month > 6 && dt.Month <= 9)
            {
                season = "Summer";
            }
            else
            {
                season = "Fall";
            }

            return Path.Combine(dt.Year.ToString(), season);
        }
    }

    public static class Extensions
    {
        public static bool None(this IEnumerable<String> list, String fileFullPath)
        {
            String matcher = Path.GetFileName(fileFullPath);

            return !list.Any(s => Regex.IsMatch(matcher, s));
        }
    }
}
