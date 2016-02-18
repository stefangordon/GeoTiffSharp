using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTiffSharp
{
    public class FileMetadata
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public double PixelScaleX { get; set; }
        public double PixelScaleY { get; set; }
        public double OriginLatitude { get; set; }
        public double OriginLongitude { get; set; }
        public int BitsPerSample { get; set; }
        public string WorldUnits { get; set; }
        public string SampleFormat { get; set; }
        public string NoDataValue { get; set; }
    }
}
