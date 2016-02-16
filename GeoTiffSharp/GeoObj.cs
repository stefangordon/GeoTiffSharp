using ObjParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTiffSharp
{
    class GeoObj
    {
        static readonly int BITS_PER_SAMPLE = 16;

        public static FileMetadata ParseMetadata(string filename)
        {
            FileMetadata metadata = new FileMetadata();

            Obj obj = new Obj();
            obj.LoadObj(filename);

            metadata.BitsPerSample = BITS_PER_SAMPLE;

            metadata.Height = (int)Math.Ceiling(obj.Size.ZMax - obj.Size.ZMin);
            metadata.Width = (int)Math.Ceiling(obj.Size.XMax - obj.Size.XMin);

            metadata.OriginLatitude = obj.Size.XMin;
            metadata.OriginLongitude = obj.Size.ZMax;

            metadata.WorldUnits = "meters";


            return metadata;

        }
    }
}
