using System;
using System.Collections.Generic;
using System.Text;

namespace SICAR
{
    public class Crop
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
