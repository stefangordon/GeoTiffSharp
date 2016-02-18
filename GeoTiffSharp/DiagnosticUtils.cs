using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTiffSharp
{
    class DiagnosticUtils
    {
        public static void OutputDebugBitmap(float[,] data, float minValue, float maxValue, string bitmapFilename, float noDataValue)
        {
            OutputDebugBitmapInternal(data, minValue, maxValue, bitmapFilename, noDataValue, true);
        }

        public static void OutputDebugBitmap(float[,] data, float minValue, float maxValue, string bitmapFilename)
        {
            OutputDebugBitmapInternal(data, minValue, maxValue, bitmapFilename, 0.0f, false);
        }

        private static void OutputDebugBitmapInternal(float[,] data, float minValue, float maxValue, string bitmapFilename, float noDataValue, bool filterNodata)
        {
            float range = maxValue - minValue;
            Bitmap bm = new Bitmap(data.GetLength(0), data.GetLength(1));
            for(int i = 0; i < data.GetLength(0); i++)
            {
                for(int j =0; j< data.GetLength(1);j++)
                {
                    var dataValue = data[i, j];

                    if (filterNodata && dataValue == noDataValue)
                    {
                        bm.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                    }
                    else
                    {
                        var normalizedValue = (ushort)((data[i, j] - minValue) / range * 255);
                        bm.SetPixel(i, j, Color.FromArgb(normalizedValue, normalizedValue, normalizedValue));
                    }
                }
            }

            bm.Save(bitmapFilename);
        }
    }
}
