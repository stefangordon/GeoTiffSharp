using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjParser
{
    class GeoObj
    {
        public static object ParseMetadata(string filename)
        {
            Obj obj = new Obj();
            obj.LoadObj(filename);

            return null;
        }
    }
}
