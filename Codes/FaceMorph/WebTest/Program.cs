using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FaceAPIWeb;

namespace WebTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string url1 = @"http://static.7192.com/uploadfile/2012/0322/115052152.jpg";
            string url2 = @"http://photocdn.sohu.com/20130204/Img365500363.jpg";
            FaceAPIWeb.FaceAPIWeb.Morph(url1, url2, 0.5);
        }
    }
}
