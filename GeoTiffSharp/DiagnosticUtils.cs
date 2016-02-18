using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTiffSharp
{
    static class DiagnosticUtils
    {
        //public static void WriteBitmap(byte[] byteArray, int width, int height, int bytesPervalue, string outputFilename)
        //{
        //    Bitmap bitmap = new Bitmap(width, height);
        //    int value;
        //    for (int y = 0; y < height; y++)
        //    {
        //        for (int x = 0; x < width; x++)
        //        {
        //            var rowSize = width * bytesPervalue;
        //            var offset = rowSize * y + x * bytesPervalue;
        //            switch (bytesPervalue)
        //            {
        //                case 4:
        //                    value = BitConverter.ToInt32(byteArray, offset);
        //                    break;
        //                case 2:
        //                    value = BitConverter.ToInt16(byteArray, offset);
        //                    break;
        //            }
                    
        //            Color c = Color.FromArgb()
        //            bitmap.SetPixel()
        //        }
        //    }
        //}
    }
}
