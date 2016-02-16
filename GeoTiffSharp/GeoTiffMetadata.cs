using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTiffSharp
{
    public class PointD
    {
        double X;
        double Y;
    }

    public class GeoTiffMetadata
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public double PixelSizeX { get; set; }
        public double PixelSizeY { get; set; }
        public double OriginLatitude { get; set; }
        public double OriginLongitude { get; set; }
        public int RasterBitsPerSample { get; set; }
        public int RasterSamplesPerPixel { get; set; }
        public PointD UpperLeft { get; set; }
        public PointD LowerRight { get; set; }
        public PointD Center { get; set; }
    }
}
