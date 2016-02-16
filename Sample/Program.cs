using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoTiffSharp;
using Newtonsoft.Json;

namespace Sample
{
    class Program
    {
        public const string tiffPath = @"..\..\..\SampleData\sample.tif";

        static void Main(string[] args)
        {
            // Write out metadata to console
            var result = GeoTiff.ParseMetadata(tiffPath);
            Console.Write(JsonConvert.SerializeObject(result));

            // Save image              
            var outputPath = Path.Combine(Path.GetDirectoryName(tiffPath), "binary.dat");
            if (File.Exists(outputPath)) File.Delete(outputPath);
            GeoTiff.WriteBinary(tiffPath, outputPath, result);

            // Wait...         
            Console.ReadKey();
        }
    }
}
