using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTiffSharp
{
    interface IHeightMapConverter
    {
        void ConvertToHeightMap(string inputFile, string outputBinary, string outputMetadata, string outputDiagnosticBitmap);
    }
}
