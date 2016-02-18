using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitMiracle.LibTiff.Classic;

namespace GeoTiffSharp
{
    public static class GeoTiff
    {
        public static FileMetadata ParseMetadata(string filename)
        {
            FileMetadata metadata = new FileMetadata();

            using (Tiff tiff = Tiff.Open(filename, "r"))
            {
                metadata.Height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                metadata.Width = tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();

                FieldValue[] modelPixelScaleTag = tiff.GetField((TiffTag)33550);
                FieldValue[] modelTiepointTag = tiff.GetField((TiffTag)33922);

                byte[] modelPixelScale = modelPixelScaleTag[1].GetBytes();
                metadata.PixelScaleX = BitConverter.ToDouble(modelPixelScale, 0);
                metadata.PixelScaleY = BitConverter.ToDouble(modelPixelScale, 8);

                // Ignores first set of model points (3 bytes) and assumes they are 0's...
                byte[] modelTransformation = modelTiepointTag[1].GetBytes();
                metadata.OriginLongitude = BitConverter.ToDouble(modelTransformation, 24);
                metadata.OriginLatitude = BitConverter.ToDouble(modelTransformation, 32);

                // Grab some raster metadata
                metadata.BitsPerSample = tiff.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();

                foreach (var en in Enum.GetValues(typeof(TiffTag)))
                {
                    PrintTagInfo(tiff, (TiffTag)en);
                }

                PrintTagInfo(tiff, (TiffTag)42113);

                metadata.WorldUnits = "meter";             
            }

            return metadata;
        }

        private static void PrintTagInfo(Tiff tiff, TiffTag tiffTag)
        {
            try
            {
                var field = tiff.GetField(tiffTag);
                if (field != null)
                {
                    Console.WriteLine($"{tiffTag}");
                    for (int i = 0; i < field.Length; i++)
                    {
                        Console.WriteLine($"  [{i}] {field[i].Value}");
                        byte[] bytes = field[i].Value as byte[];
                        if (bytes != null)
                        {
                            Console.WriteLine($"    Length: {bytes.Length}");
                            if (bytes.Length % 8 == 0)
                            {
                                for (int k = 0; k < bytes.Length / 8; k++)
                                {
                                    Console.WriteLine($"      [{k}] {BitConverter.ToDouble(bytes, k * 8)}");
                                }
                            }


                            try
                            {
                                Console.WriteLine($"   > {System.Text.Encoding.ASCII.GetString(bytes).Trim()} < ");
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {tiffTag}");
            }
        }

        public static void WriteBinary(string inputFilename, string outputFilename, FileMetadata metadata)
        {
            Bitmap bm = new Bitmap(metadata.Width, metadata.Height);
            using (Tiff tiff = Tiff.Open(inputFilename, "r"))
            {
                using (var outStream = File.OpenWrite(outputFilename))
                {
                    BinaryWriter writer = new BinaryWriter(outStream);

                    var min = Single.MaxValue;
                    var max = Single.MinValue;
                    max = 971.053345F;
                    min = 316.391174F;
                    var range = max - min;

                    for (int i = 0; i < metadata.Height; i++)
                    {                 
                        byte[] buffer = new byte[metadata.Width * 4];
                        tiff.ReadScanline(buffer, i);
                        for(int p = 0;p<metadata.Width;p++)
                        {
                            var colorVal = BitConverter.ToSingle(buffer, p * 4);
                            var normalizedColor = 0;
                            if (colorVal != -10000)
                            {
                                min = Math.Min(min, colorVal);
                                max = Math.Max(max, colorVal);
                                normalizedColor = (short)((colorVal - min) / range * 255);
                            }
                            bm.SetPixel(p, i, Color.FromArgb(normalizedColor, normalizedColor, normalizedColor));
                        }                                        
                        outStream.Write(buffer, 0, metadata.Width * 4);
                    }                                             
                }

                bm.Save("tiff.bmp");
            }                                                                                               
        }

    }
}
