using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GeoTiffSharp;

namespace Sample
{
    class Program
    {
        public const string tiffPath = @"..\..\..\SampleData\sample.tif";

        static void Main(string[] args)
        {
            // Save metadata
            var outputPath = Path.Combine(Path.GetDirectoryName(tiffPath), "metadata.json");
            if (File.Exists(outputPath)) File.Delete(outputPath);

            var result = GeoTiff.ParseMetadata(tiffPath);
            File.WriteAllText(outputPath, JsonConvert.SerializeObject(result, Formatting.Indented));

            // Save image              
            outputPath = Path.Combine(Path.GetDirectoryName(tiffPath), "binary.dat");
            if (File.Exists(outputPath)) File.Delete(outputPath);

            GeoTiff.WriteBinary(tiffPath, outputPath, result);
        }
    }
}
