using ObjParser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTiffSharp
{
    public static class GeoObj
    {
        static readonly int BITS_PER_SAMPLE = 16;

        static readonly int TARGET_MAP_HEIGHT = 100;

        public static FileMetadata ParseMetadata(string filename)
        {
            FileMetadata metadata = new FileMetadata();

            Obj obj = new Obj();
            obj.LoadObj(filename);

            metadata.BitsPerSample = BITS_PER_SAMPLE;
            double pointsPerPixel = obj.Size.YSize / TARGET_MAP_HEIGHT;
            metadata.Height = (int)(obj.Size.YSize / pointsPerPixel) + 1;
            metadata.Width = (int)(obj.Size.XSize / pointsPerPixel) + 1;
            double[,] heights = new double[metadata.Height, metadata.Width];
            bool[,] dataPresent = new bool[metadata.Height, metadata.Width];
            Bitmap bm = new Bitmap(metadata.Width, metadata.Height);
            
            metadata.OriginLatitude = obj.Size.XMin;
            metadata.OriginLongitude = obj.Size.YMax;       

            metadata.WorldUnits = "meters";

            for(int y = 0; y < metadata.Height;y++)
            {
                Console.Write($"{y}: ");
                var row = obj.VertexList.Where(v => (int)((v.Y - obj.Size.YMin)/pointsPerPixel) == y);
                //Console.WriteLine("RowSize: " + row.Count());
                var rowCols = row.GroupBy(r => (int)((r.X - obj.Size.XMin) / pointsPerPixel)).OrderBy(rc => rc.Key);

                int x = 0;
                foreach(var rowcol in rowCols)
                {
                    while(rowcol.Key > x)
                    {
                        heights[y, x] = -1;
                        //Console.Write($"{heights[y, x]} ");
                        bm.SetPixel(x, y, Color.Red);
                        x++;
                    }
                    heights[y, x] = rowcol.Average(v => v.Z);
                    dataPresent[y, x] = true;
                    int colorValue = (int)(255 * (heights[y, x] - obj.Size.ZMin) / obj.Size.ZSize);
                    bm.SetPixel(x, y, Color.FromArgb(colorValue, colorValue,colorValue));
                    //Console.Write($"{heights[y, x]} ");
                    x++;

                }
                while(x < metadata.Width)
                {
                    heights[y, x] = -1;
                    //Console.Write($"{heights[y, x]} ");
                    bm.SetPixel(x, y, Color.Red);
                    x++;
                }
                Console.WriteLine("");
            }

            //bool dataMissing = true;
            //while (dataMissing)
            //{
            //    for (int i = 0; i < dataPresent.GetLength(0); i++)
            //    {
            //        for (int j = 0; j < dataPresent.GetLength(1); j++)
            //        {
            //            if (!dataPresent[i, j])
            //            {
            //                var sum = 0.0;
                            
            //            }
            //        }
            //    }
            //}

            bm.Save("testobj.bmp");
                
            return metadata;

        }
    }
}
