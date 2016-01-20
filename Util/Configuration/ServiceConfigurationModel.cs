using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Util.Configuration
{
    [DataContract]
    public partial class ServiceConfigurationModel
    {
        public ServiceConfigurationModel() { }

        [DataMember]
        public int MaxThreads { get; set; }

        [DataMember]
        public bool Monitor { get; set; }

        [DataMember]
        public Archive Archive { get; set; }

        [DataMember]
        public List<SearchLocation> SearchLocations { get; set; }

        [DataMember]
        public List<SearchType> SearchTypes { get; set; }

        [DataMember]
        public List<IgnoreType> IgnoreTypes { get; set; }
        
        public static ServiceConfigurationModel Deserialize()
        {
            return JsonConvert.DeserializeObject<ServiceConfigurationModel>(File.ReadAllText("ServiceConfigurationModel.json"));
        }

        public SearchType GetSearchTypeForFile(string validFile)
        {
            String extension = Path.GetExtension(validFile);

            return SearchTypes.SingleOrDefault(s => String.Compare(extension, s.Extension, true) == 0);
        }
    }

    [DataContract]
    public partial class Archive
    {
        public Archive() { }
        
        [DataMember]
        public String Destination { get; set; }

        [DataMember]
        public String Path { get; set; }

        public string DestinationFullPath
        {
            get
            {
                return System.IO.Path.Combine("\\\\" + Destination, Path);
            }
        }
    }

    [DataContract]
    public partial class IgnoreType
    {
        public IgnoreType() { }

        [DataMember]
        public String Pattern { get; set; }
    }

    [DataContract]
    public partial class SearchType
    {
        public SearchType() { }

        [DataMember]
        public String Extension { get; set; }

        [DataMember]
        public String Handler { get; set; }
    }

    [DataContract]
    public partial class SearchLocation
    {
        public SearchLocation() { }

        [DataMember]
        public bool Recurse { get; set; }

        [DataMember]
        public String Host { get; set; }

        [DataMember]
        public String RootPath { get; set; }

        [DataMember]
        public bool IsLocal { get; set; }

        [DataMember]
        public bool MarkForDeleteOnArchive { get; set; }
    }
}
