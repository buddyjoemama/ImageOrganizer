using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExifTagManager;
using System.Drawing;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Image p = Bitmap.FromFile(@"C:\IMG_0001.jpg");

            var d = TagParser.Parse<MyObj>(p.PropertyItems.ToList());
     
        }
    }

    public class MyObj
    {

        public DateTime DT { get; set; }
    }
}
