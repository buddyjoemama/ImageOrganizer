using ImageOrganizer.Data.Entites;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganizer.Data
{
    public class OrganizerDatabaseContext : DbContext
    {
        public OrganizerDatabaseContext() 
            : base("name=DefaultConnection")
        { }

        public DbSet<MediaFile> MediaFiles { get; set; }
    }
}
