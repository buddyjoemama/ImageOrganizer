using ExifTagManager;
using ExifTagManager.Parsers;
using ImageOrganizer.Data;
using ImageOrganizer.Data.Entites;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Util;
using Util.Configuration;

namespace ImageOrganizerService
{
    static class Program
    {
        private static object locker = new object();
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

        static List<Thread> threads = new List<Thread>();

        static void Backup()
        {
            List<Task> runners = new List<Task>();
            var config = ServiceConfigurationModel.Deserialize();

            // Iterate over all of the specified locations...create threads
            foreach(SearchLocation location in config.SearchLocations.Where(s=>s.IsLocal))
            {
                var allDirectories = DirectoryHelper.ListAllDirectories(location.RootPath, location.Recurse).ToList();

                // Create a blocking collection consisting of the enumerated directories.
                BlockingCollection<String> collection = new BlockingCollection<String>();
                allDirectories.ForEach(s => collection.Add(s));

                // The runners will operate on this blocking collection.
                for(int i = 0; i < 1; i++)
                {
                    Task runner = Task.Run(() =>
                    {
                        String dir = null;

                        while ((collection.TryTake(out dir, TimeSpan.FromSeconds(1))) && dir != null)
                        {
                            var validFiles = Directory.EnumerateFiles(dir)
                                .Where(s => s.IsValidFile(config));

                            foreach(var validFile in validFiles)
                            {
                                SearchType type = config.GetSearchTypeForFile(validFile);

                                Delegate del = Delegate.CreateDelegate(typeof(Action<String, SearchLocation, Archive>), typeof(Program), 
                                    type.Handler);
                                if(del != null)
                                {
                                    del.DynamicInvoke(validFile, location, config.Archive);
                                }
                            }
                        }
                    });

                    runners.Add(runner);
                }
            }

            Task.WaitAll(runners.ToArray());
        }

        private static void CreateDirectory(String targetDir)
        {
            if (Directory.Exists(targetDir))
                return;

            lock(locker)
            {
                if (Directory.Exists(targetDir))
                    return;

                Directory.CreateDirectory(targetDir);
            }
        }

        static void MovHandler(String file, SearchLocation config, Archive archive)
        {
            MD5 hasher = MD5.Create();
            DateTime createTime = File.GetCreationTime(file);

            String ext = Path.GetExtension(file);
            String folder = createTime.GetFolderName();
            String targetDir = Path.Combine(archive.DestinationFullPath, folder);
            String targetFile = Path.Combine(targetDir,
                $"{createTime.ToString("MM-dd-yyyy")}_{DateTime.UtcNow.Ticks.ToString()}{ext}");

            // Copy the file if it doesnt already exist.
            try
            {
                // We might need to create the directory.
                CreateDirectory(targetDir);

                String hash = null;
                using (FileStream stream = File.OpenRead(file))
                {
                    hash = Convert.ToBase64String(hasher.ComputeHash(stream));
                }

                // Does this file already exist in the databse?
                if(!MediaFile.Exists(hash))
                {
                    using (OrganizerDatabaseContext context = new OrganizerDatabaseContext())
                    {
                        MediaFile mediaFile = new MediaFile();
                        mediaFile.ArchiveDateTime = mediaFile.CreatedDateTime = DateTime.UtcNow;
                        mediaFile.ContentHash = hash;
                        mediaFile.OriginalFileName = file;
                        mediaFile.TargetFileName = targetFile;

                        context.MediaFiles.Add(mediaFile);

                        context.SaveChanges();
                    }
                }
                else
                {
                    // Overwrite it if we can.
                    MediaFile existingFile = MediaFile.GetByHash(hash);
                    
                    if(existingFile != null) // Should never be the case.
                    {
                        if(!File.Exists(existingFile.TargetFileName))
                        {
                            targetFile = existingFile.TargetFileName;
                        }
                    }
                }

                // Copy it over.
                try
                {
                    File.Copy(file, targetFile);
                }
                catch
                {
                    // Delete the media file if we couldnt copy it over.
                    MediaFile.DeleteByHash(hash);
                }
            }
            catch (Exception e)
            {
            }
        }

        static void JpegHandler(String file, SearchLocation config, Archive archive)
        {

        }

        static void Run(Object objEvent)
        {
            ManualResetEvent @event = (ManualResetEvent)objEvent;
            MD5 hasher = MD5.Create();

            String dir = null;

            while((directories.TryTake(out dir, TimeSpan.FromSeconds(1))) && dir != null)
            {
                //var validFiles = Directory.EnumerateFiles(dir)
                //    .Where(s => config.IsValidFile(s));

                //if(validFiles.Count() > 0)
                //{
                //    Console.WriteLine($"Processing directory: {dir}. Found {validFiles.Count()} files.");

                //    foreach(var file in validFiles)
                //    {
                //        String hash = null;

                //        try
                //        {
                //            DateTime? dt = null;
                //            var extension = Path.GetExtension(file).ToLower();

                //            using (FileStream stream = File.Open(file, FileMode.Open))
                //            {
                //                switch (extension)
                //                {
                //                    case ".jpg":
                //                        using (Bitmap image = (Bitmap)Bitmap.FromStream(stream))
                //                        {
                //                            var data = TagParser.Parse<ExifTags>(image.PropertyItems.ToList());
                //                            dt = data.FileChangeDateTime;
                //                        }
                //                        break;
                //                }

                //                stream.Position = 0;
                //                hash = Convert.ToBase64String(hasher.ComputeHash(stream));
                //            }

                //            if (dt == null)
                //            {
                //                dt = File.GetCreationTimeUtc(file);
                //            }

                //            if (dt != null)
                //                config.ArchiveFile(dt.Value, file, hash);
                //        }
                //        catch(Exception e)
                //        {
                //            Console.WriteLine(e.ToString());
                //        }
                //    }
                //}
            }

            @event.Set();
        }
    }

    public class RunOperation
    {
        public ManualResetEvent ResetEvent { get; set; }
    }

    public class ExifTags
    {
        [TagId(306)]
        [DateTime]
        public DateTime? FileChangeDateTime { get; set; }
    }
}
