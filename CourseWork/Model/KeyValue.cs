using Lab1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Model
{
    public class KeyValue
    {
        public float Key { get; set; }
        public State Value { get; set; }

        public override string ToString()
        {
            return $"{Key}; [{Value.ToString()}]";
        }
    }
}
