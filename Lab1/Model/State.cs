using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Model
{
    public enum Direction { Forward = 1, Backward = 2, Left = 3, Right = 4, Up = 5, Down = 6 }
    public class State
    {
        public Coordinate coordinate { get; set; }
        public Direction direction { get; set; }

        public override string ToString()
        {
            string result = this.direction switch
            {
                Direction.Forward => $"{this.coordinate.ToString()}; Forward",
                Direction.Backward => $"{this.coordinate.ToString()}; Backward",
                Direction.Left => $"{this.coordinate.ToString()}; Left",
                Direction.Right => $"{this.coordinate.ToString()}; Right",
                Direction.Up => $"{this.coordinate.ToString()}; Up",
                Direction.Down => $"{this.coordinate.ToString()}; Down",
            };
            return result;
        }

        public static bool operator ==(State state1, State state2) => (state1.coordinate == state2.coordinate) && (state1.direction == state2.direction);

        public static bool operator !=(State state1, State state2) => (state1.coordinate != state2.coordinate) || (state1.direction == state2.direction);

        public override bool Equals(object? obj)
        {
            return this == (State)obj;
        }

        public override int GetHashCode()
        {
            int result = 0;
            foreach(var item in this.ToString())
            {
                result += (int)item;
            }
            return result;
        }
    }
}
