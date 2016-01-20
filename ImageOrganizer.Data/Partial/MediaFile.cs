using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganizer.Data.Entites
{
    public partial class MediaFile
    {
        public static bool Exists(String hash)
        {
            using (OrganizerDatabaseContext context = new OrganizerDatabaseContext())
            {
                return context.MediaFiles.Any(s => s.ContentHash == hash);
            }
        }

        public static MediaFile GetByHash(String hash)
        {
            using (OrganizerDatabaseContext context = new OrganizerDatabaseContext())
            {
                return context.MediaFiles.SingleOrDefault(s => s.ContentHash == hash); 
            }
        }

        public static void DeleteByHash(String hash)
        {
            using (OrganizerDatabaseContext context = new OrganizerDatabaseContext())
            {
                var file = context.MediaFiles.SingleOrDefault(s => s.ContentHash == hash);

                if(file != null)
                {
                    context.MediaFiles.Remove(file);
                    context.SaveChanges();
                }
            }
        }
    }
}
