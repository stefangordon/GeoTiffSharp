using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitMiracle.LibTiff.Classic;

namespace GeoTiffSharp
{
    public static class GeoTiff
    {
        public static GeoTiffMetadata ParseMetadata(string filename)
        {
            GeoTiffMetadata metadata = new GeoTiffMetadata();

            using (Tiff tiff = Tiff.Open(filename, "r"))
            {
                metadata.Height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                metadata.Width = tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();

                FieldValue[] modelPixelScaleTag = tiff.GetField((TiffTag)33550);
                FieldValue[] modelTiepointTag = tiff.GetField((TiffTag)33922);

                byte[] modelPixelScale = modelPixelScaleTag[1].GetBytes();
                metadata.PixelSizeX = BitConverter.ToDouble(modelPixelScale, 0);
                metadata.PixelSizeY = BitConverter.ToDouble(modelPixelScale, 8);

                // Ignores first set of model points (3 bytes) and assumes they are 0's...
                byte[] modelTransformation = modelTiepointTag[1].GetBytes();
                metadata.OriginLongitude = BitConverter.ToDouble(modelTransformation, 24);
                metadata.OriginLatitude = BitConverter.ToDouble(modelTransformation, 32);

                // Grab some raster metadata
                metadata.RasterBitsPerSample = tiff.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();
                metadata.RasterSamplesPerPixel = tiff.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToInt();                
            }

            return metadata;
        }

        public static void WriteBinary(string inputFilename, string outputFilename, GeoTiffMetadata metadata)
        {
            using (Tiff tiff = Tiff.Open(inputFilename, "r"))
            {

                using (var outStream = File.OpenWrite(outputFilename))
                {
                    BinaryWriter writer = new BinaryWriter(outStream);

                    for (int i = 0; i < tiff.NumberOfStrips(); i++)
                    {
                        int stripSize = (int)tiff.RawStripSize(i);
                        byte[] buffer = new byte[stripSize];
                        tiff.ReadRawStrip(i, buffer, 0, (int)tiff.RawStripSize(i));

                        outStream.Write(buffer, 0, stripSize);
                    }                                             
                }
            }
        } 

    }
}
