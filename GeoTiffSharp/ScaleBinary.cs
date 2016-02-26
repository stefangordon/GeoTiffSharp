using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTiffSharp
{
    public static class ScaleBinary
    {
        public static void Reduce(FileMetadata metadata, Stream input, Stream output, int desiredPoints)
        {
            // s = scaler to solve for
            // d = desired points
            // (x / s) * (y / s) == d
            // so...
            // s = (sqrt(x) * sqrt(y))/sqrt(d)

            float scaler = (float)((Math.Sqrt(metadata.Height) * Math.Sqrt(metadata.Width)) / Math.Sqrt(desiredPoints));

            int newHeight = (int)Math.Floor(metadata.Height / scaler);
            int newWidth = (int)Math.Floor(metadata.Width / scaler);

            float[,] originalMatrix = new float[metadata.Width, metadata.Height];
            float[,] newMatrix = new float[newWidth, newHeight];

            using (BinaryReader reader = new BinaryReader(input))
            {
                for (int y = 0; y < metadata.Height; y++)
                {
                    for (int x = 0; x < metadata.Width; x++)
                    {
                        switch (metadata.BitsPerSample)
                        {
                            case 32:
                                originalMatrix[x, y] = reader.ReadSingle();
                                break;
                            case 16:
                                originalMatrix[x, y] = reader.ReadUInt16();
                                break;
                        }
                    }
                }
            }

            float noDataValue = float.Parse(metadata.NoDataValue);

            using (BinaryWriter writer = new BinaryWriter(output))
            {
                for (int y = 0; y < newHeight; y++)
                {
                    for (int x = 0; x < newWidth; x++)
                    {
                        float value = originalMatrix[(int)(x * scaler), (int)(y * scaler)];

                        if (value != noDataValue)
                            value /= scaler;

                        writer.Write(value);
                    }
                }
            }

            metadata.Width = newWidth;
            metadata.Height = newHeight;
            metadata.PixelScaleX *= scaler;
            metadata.PixelScaleY *= scaler;
            
        }


    }
}
