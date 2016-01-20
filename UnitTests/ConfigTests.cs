using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using Util;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using Util.Configuration;

namespace UnitTests
{
    [TestClass]
    public class ConfigTests
    {
        [TestMethod]
        public void TestOpenConfigFile()
        {
            XmlSerializer ser = new XmlSerializer(typeof(ServiceConfiguration));
            ser.Deserialize(File.Open("ServiceConfig.xml", FileMode.Open));
        }

        [TestMethod]
        public void TestConfigFile()
        {
            XmlSerializer ser = new XmlSerializer(typeof(ServiceConfiguration));
            ServiceConfiguration config = ser.Deserialize(File.Open("ServiceConfig.xml", FileMode.Open)) as ServiceConfiguration;

            Assert.IsNotNull(config);

            Assert.IsTrue(config.IgnoreTypes.Count() >= 1);
            Assert.IsTrue(config.SearchTypes.Count() > 1);
        }

        [TestMethod]
        public void TestEnumeration()
        {
            XmlSerializer ser = new XmlSerializer(typeof(ServiceConfiguration));
            ServiceConfiguration config = ser.Deserialize(File.Open("ServiceConfig.xml", FileMode.Open)) as ServiceConfiguration;

            var dirs = Recurse(config.RootDirectory).OrderBy(s => s).ToList();
        }

        public IEnumerable<String> Recurse(String root)
        {
            foreach(String dir in Directory.EnumerateDirectories(root))
            {
                if (Directory.GetFiles(dir).Count() > 0)
                    yield return dir;

                foreach (String subDir in Recurse(dir))
                {
                    yield return subDir;
                }
            }
        }

        [TestMethod]
        public void TestJson()
        {
            var obj = ServiceConfigurationModel.Deserialize();

            Assert.IsNotNull(obj);
        }
    }
}
