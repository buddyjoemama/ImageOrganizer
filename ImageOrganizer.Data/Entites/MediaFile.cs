using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganizer.Data.Entites
{
    public partial class MediaFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MediaFileId { get; set; }

        [MaxLength(250)]
        [Required]
        [Index]
        public String OriginalFileName { get; set; }

        [MaxLength(250)]
        [Required]
        [Index]
        public String TargetFileName { get; set; }

        [MaxLength(50)]
        [Index(IsUnique =true)]
        public String ContentHash { get; set; }

        public bool MarkForDelete { get; set; }

        public DateTime? OriginalDeleteDateTime { get; set; }

        public DateTime ArchiveDateTime { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
