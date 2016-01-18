using ExifTagManager;
using ExifTagManager.Parsers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Util;

namespace ImageOrganizerService
{
    static class Program
    {
        private static BlockingCollection<String> directories = new BlockingCollection<string>();
        private static ServiceConfiguration config = ServiceConfiguration.Config;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                Backup();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new OrganizerService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }

        static void Backup()
        {
            foreach(var directory in DirectoryHelper.ListAllDirectories(config.RootDirectory))
            {
                directories.Add(directory);
            }

            for(int i = 0; i < 10; i++)
            {
                Thread thread = new Thread(new ThreadStart(Run));
                thread.Start();
            }

            Thread.CurrentThread.Join();
        }

        static void Run()
        {
            String dir = null;
            while((dir = directories.Take()) != null)
            {
                var validFiles = Directory.EnumerateFiles(dir)
                    .Where(s => config.IsValidFile(s));

                if(validFiles.Count() > 0)
                {
                    Console.WriteLine($"Processing directory: {dir}. Found {validFiles.Count()} files.");

                    foreach(var file in validFiles)
                    {
                        try
                        {
                            DateTime? dt = null;
                            var extension = Path.GetExtension(file).ToLower();

                            switch (extension)
                            {
                                case ".jpg":
                                    using (FileStream stream = File.Open(file, FileMode.Open))
                                    using (Bitmap image = (Bitmap)Bitmap.FromStream(stream))
                                    {
                                        var data = TagParser.Parse<ExifTags>(image.PropertyItems.ToList());
                                        dt = data.FileChangeDateTime;
                                    }
                                    break;
                            }

                            if (dt == null)
                            {
                                dt = File.GetCreationTimeUtc(file);
                            }

                            if (dt != null)
                                config.ArchiveFile(dt.Value, file);
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                }

                Thread.Sleep(0);
            }
        }
    }

    public class ExifTags
    {
        [TagId(306)]
        [DateTime]
        public DateTime? FileChangeDateTime { get; set; }
    }
}
