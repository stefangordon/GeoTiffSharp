using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BitMiracle.LibTiff.Classic;

namespace GeoTiffSharp
{
    public class GeoTiff : IHeightMapConverter, IDisposable
    {
        Tiff _tiff;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tiff?.Dispose();
            }
        }

        ~GeoTiff()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private FileMetadata ParseMetadata(string filename)
        {
            FileMetadata metadata = new FileMetadata();
            _tiff = Tiff.Open(filename, "r");

            metadata.Height = _tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
            metadata.Width = _tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();

            FieldValue[] modelPixelScaleTag = _tiff.GetField((TiffTag)33550);
            FieldValue[] modelTiepointTag = _tiff.GetField((TiffTag)33922);

            byte[] modelPixelScale = modelPixelScaleTag[1].GetBytes();
            metadata.PixelScaleX = BitConverter.ToDouble(modelPixelScale, 0);
            metadata.PixelScaleY = BitConverter.ToDouble(modelPixelScale, 8);

            // Ignores first set of model points (3 bytes) and assumes they are 0's...
            byte[] modelTransformation = modelTiepointTag[1].GetBytes();
            metadata.OriginLongitude = BitConverter.ToDouble(modelTransformation, 24);
            metadata.OriginLatitude = BitConverter.ToDouble(modelTransformation, 32);

            // Grab some raster metadata
            metadata.BitsPerSample = _tiff.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();

            // Add other information about the data
            metadata.SampleFormat = "Single";
            // TODO: Read this from tiff metadata or determine after parsing
            metadata.NoDataValue = "-10000";

            metadata.WorldUnits = "meter";

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

        private void WriteBinary(string inputFilename, Stream output, string bitmapFilename, FileMetadata metadata)
        {
            float min = float.MaxValue;
            float max = float.MinValue;
            float range;

            float[,] data = new float[metadata.Width, metadata.Height];

            for (int i = 0; i < metadata.Height; i++)
            {
                byte[] buffer = new byte[metadata.Width * metadata.BitsPerSample / 8];
                _tiff.ReadScanline(buffer, i);
                for (int p = 0; p < metadata.Width; p++)
                {
                    var heightValue = BitConverter.ToSingle(buffer, p * metadata.BitsPerSample / 8);
                    data[p, i] = heightValue;
                    if (heightValue != -10000)
                    {
                        min = Math.Min(min, heightValue);
                        max = Math.Max(max, heightValue);
                    }
                }
                output.Write(buffer, 0, metadata.Width * metadata.BitsPerSample / 8);
            }

            // compute range of heights so we can normalize the values for the grayscale bmp
            range = max - min;

            if (!string.IsNullOrEmpty(bitmapFilename))
            {
                DiagnosticUtils.OutputDebugBitmap(data, min, max, bitmapFilename, -10000);
            }
        }
       

        public void ConvertToHeightMap(string inputFile, string outputBinary, string outputMetadata, string outputDiagnosticBitmap)
        {
            var metadata = ParseMetadata(inputFile);

            MemoryStream buffer = new MemoryStream();
            WriteBinary(inputFile, buffer, outputDiagnosticBitmap, metadata);

            buffer.Position = 0;

            using (var fileStream = File.OpenWrite(outputBinary))
            {
                ScaleBinary.Reduce(metadata, buffer, fileStream, 64000);
            }

            File.WriteAllText(outputMetadata, JsonConvert.SerializeObject(metadata, Formatting.Indented));
        }
    }
}
