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
            var objMetadataOutput = Path.Combine(Path.GetDirectoryName(objPath), "objSample.json");
            if (File.Exists(objMetadataOutput)) File.Delete(objMetadataOutput);
            var objBinaryOutput = Path.Combine(Path.GetDirectoryName(objPath), "objSample.dat");
            if (File.Exists(objBinaryOutput)) File.Delete(objBinaryOutput);

            var objBitmapOutput = Path.Combine(Path.GetDirectoryName(objPath), "objSample.bmp");
            if (File.Exists(objBitmapOutput)) File.Delete(objBitmapOutput);

            var objConverter = new GeoObj();

            objConverter.ConvertToHeightMap(objPath, objBinaryOutput, objMetadataOutput, objBitmapOutput);
            
            // Save metadata
            var outputManifestPath = Path.Combine(Path.GetDirectoryName(tiffPath), "tiffSample.json");
            if (File.Exists(outputManifestPath)) File.Delete(outputManifestPath);
            
            // Save image              
            var outputPath = Path.Combine(Path.GetDirectoryName(tiffPath), "tiffSample.dat");
            if (File.Exists(outputPath)) File.Delete(outputPath);

            var bitmapPath = Path.Combine(Path.GetDirectoryName(tiffPath), "tiffSample.bmp");
            if (File.Exists(bitmapPath)) File.Delete(bitmapPath);

            using (GeoTiff tiffConverter = new GeoTiff())
            {
                tiffConverter.ConvertToHeightMap(tiffPath, outputPath, outputManifestPath, bitmapPath);
            }
        }
    }
}
