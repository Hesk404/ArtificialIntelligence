using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Model
{
    public class Coordinate
    {
        public int x { get; set; }
        public int y { get; set; }

        public override string ToString()
        {
            return $"{x}.{y}";
        }
    }
}
