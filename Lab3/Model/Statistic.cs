using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Model
{
    public class Statistic
    {
        public bool IsHaveWay { get; set; } = false;
        public int MaxO { get; set; }
        public int MaxOAndC { get; set; }
        public int LastO { get; set; }
        public int Count { get; set; }

        public void Print()
        {
            Console.WriteLine(ToString());
        }

        public override string ToString()
        {
            return $"max O: {MaxO}; max O and C: {MaxOAndC}; count of iterations: {Count}; final count O: {LastO}";
        }
    }
}