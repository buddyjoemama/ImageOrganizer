using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Util.Configuration;

namespace Util
{
    public static class AppExtensions
    {
        public static bool IsValidFile(this String file, ServiceConfigurationModel model)
        {
            return model.SearchTypes.Any(s => file.EndsWith(s.Extension, true, CultureInfo.InvariantCulture)) &&
                model.IgnoreTypes.Select(s => s.Pattern).None(file);
        }

        public static bool None(this IEnumerable<String> ignoreTypes, String fileFullPath)
        {
            String matcher = Path.GetFileName(fileFullPath);

            return !ignoreTypes.Any(s => Regex.IsMatch(matcher, s));
        }

        public static String GetFolderName(this DateTime dt)
        {
            String season = null;

            if (dt.Month >= 1 && dt.Month <= 3)
            {
                season = "Winter";
            }
            else if (dt.Month > 3 && dt.Month <= 6)
            {
                season = "Spring";
            }
            else if (dt.Month > 6 && dt.Month <= 9)
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
}
