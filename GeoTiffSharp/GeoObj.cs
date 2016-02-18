using ObjParser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTiffSharp
{
    public static class GeoObj
    {
        static readonly int BITS_PER_SAMPLE = 32;

        static readonly int TARGET_MAP_HEIGHT = 100;

        public static FileMetadata ParseMetadata(string filename, string outputFile, string outputBitmap)
        {
            FileMetadata metadata = new FileMetadata();

            Obj obj = new Obj();
            obj.LoadObj(filename);

            metadata.BitsPerSample = BITS_PER_SAMPLE;
            metadata.SampleFormat = "Single";

            double pointsPerPixel = obj.Size.YSize / TARGET_MAP_HEIGHT;
            metadata.Height = (int)(obj.Size.YSize / pointsPerPixel) + 1;
            metadata.Width = (int)(obj.Size.XSize / pointsPerPixel) + 1;
            metadata.PixelScaleX = pointsPerPixel;
            metadata.PixelScaleY = pointsPerPixel;

            float[,] heights = new float[metadata.Height, metadata.Width];
            bool[,] dataPresent = new bool[metadata.Height, metadata.Width];
            Bitmap bm = new Bitmap(metadata.Width, metadata.Height);
            
            metadata.OriginLatitude = obj.Size.XMin;
            metadata.OriginLongitude = obj.Size.YMax;       

            metadata.WorldUnits = "meters";

            float minHeight = float.MaxValue;
            float maxHeight = float.MinValue;

            for(int y = 0; y < metadata.Height;y++)
            {
                var row = obj.VertexList.Where(v => (int)((v.Y - obj.Size.YMin)/pointsPerPixel) == y);
                var rowCols = row.GroupBy(r => (int)((r.X - obj.Size.XMin) / pointsPerPixel)).OrderBy(rc => rc.Key);

                int x = 0;
                foreach(var rowcol in rowCols)
                {
                    while(rowcol.Key > x)
                    {
                        heights[y, x] = -1;
                        bm.SetPixel(x, y, Color.Red);
                        x++;
                    }

                    heights[y, x] = (float)rowcol.Average(v => v.Z);

                    if(heights[y,x] > maxHeight)
                    {
                        maxHeight = heights[y, x];
                    }
                    if(heights[y,x] < minHeight)
                    {
                        minHeight = heights[y, x];
                    }

                    dataPresent[y, x] = true;
                    int colorValue = (int)(255 * (heights[y, x] - obj.Size.ZMin) / obj.Size.ZSize);
                    bm.SetPixel(x, y, Color.FromArgb(colorValue, colorValue,colorValue));
                    x++;

                }
                while(x < metadata.Width)
                {
                    heights[y, x] = -1;
                    bm.SetPixel(x, y, Color.Red);
                    x++;
                }                    
            }

            bool dataMissing = true;
            int maxCount = 8;
            while (dataMissing)
            {
                dataMissing = false;
                bool dataFound = false;
                for (int i = 0; i < dataPresent.GetLength(0); i++)
                {
                    for (int j = 0; j < dataPresent.GetLength(1); j++)
                    {
                        if (!dataPresent[i, j])
                        {
                            var sum = 0.0f;
                            var count = 0;

                            bool firstRow = i == 0;
                            bool lastRow = i == dataPresent.GetLength(0) - 1;
                            bool firstCol = j == 0;
                            bool lastCol = j == dataPresent.GetLength(1) - 1;

                            // up left
                            if(!firstRow && !firstCol && dataPresent[i-1,j-1])
                            {
                                sum += heights[i - 1, j - 1];
                                count++;
                            }
                            // up center
                            if(!firstRow  && dataPresent[i-1,j])
                            {
                                sum += heights[i - 1, j];
                                count++;
                            }

                            // up right
                            if(!firstRow && !lastCol && dataPresent[i-1,j+1])
                            {
                                sum += heights[i - 1, j + 1];
                                count++;
                            }

                            // center left
                            if(!firstCol && dataPresent[i,j-1])
                            {
                                sum += heights[i, j - 1];
                                count++;
                            }

                            // center right
                            if(!lastCol && dataPresent[i,j+1])
                            {
                                sum += heights[i, j + 1];
                                count++;
                            }

                            // down left
                            if(!lastRow && !firstCol && dataPresent[i+1,j-1])
                            {
                                sum += heights[i + 1, j - 1];
                                count++;
                            }

                            // down center
                            if(!lastRow && dataPresent[i+1, j])
                            {
                                sum += heights[i + 1, j];
                                count++;
                            }

                            // down right
                            if(!lastRow && !lastCol && dataPresent[i+1, j+1])
                            {
                                sum += heights[i + 1, j + 1];
                                count++;
                            }

                            if(count >= maxCount)
                            {
                                dataPresent[i, j] = true;
                                heights[i, j] = sum / count;
                                dataFound = true;
                                int colorValue = (int)(255 * (heights[i, j] - obj.Size.ZMin) / obj.Size.ZSize);
                                bm.SetPixel(j, i, Color.FromArgb(colorValue, colorValue, colorValue));
                            } else
                            {
                                dataMissing = true;
                            }

                        }
                    }
                }
                if(dataMissing && !dataFound)
                {
                    maxCount--;
                }
            }

            bm.Save(outputBitmap);

            if (!string.IsNullOrEmpty(outputFile))
            {
                using (var stream = File.OpenWrite(outputFile))
                {
                    foreach(var height in heights)
                    {
                        byte[] bytesToWrite = BitConverter.GetBytes(height);
                        stream.Write(bytesToWrite, 0, bytesToWrite.Length);
                    }
                }
            }
                
            return metadata;

        }
    }
}
