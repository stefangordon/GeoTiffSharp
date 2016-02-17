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
        public const string objPath = @"..\..\..\SampleData\ObjSample\GC.obj";

        static void Main(string[] args)
        {
            // OBJ Testing
            var objMetadataOutput = Path.Combine(Path.GetDirectoryName(objPath), "objMetadata.json");
            if (File.Exists(objMetadataOutput)) File.Delete(objMetadataOutput);
            var objBinaryOutput = Path.Combine(Path.GetDirectoryName(objPath), "objBinary.dat");
            if (File.Exists(objBinaryOutput)) File.Delete(objBinaryOutput);

            var objResult = GeoObj.ParseMetadata(objPath, objBinaryOutput);
            File.WriteAllText(objMetadataOutput, JsonConvert.SerializeObject(objResult, Formatting.Indented));
            
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
