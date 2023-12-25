using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Model
{
    public enum Direction { Forward = 1, Backward = 2, Left = 3, Right = 4, Up = 5, Down = 6 }
    public class State : ICloneable
    {
        public Coordinate Coordinate { get; set; }
        public Direction Direction { get; set; }

        public State? ParentState { get; set; }

        public int Depth { get; set; }


        public State() { }

        public State(State state)
        {
            Coordinate = new Coordinate { x = state.Coordinate.x, y = state.Coordinate.y };
            Direction = state.Direction;
            ParentState = (state.ParentState is null || state.ParentState == state) ? null : new(state.ParentState);
            Depth = state.Depth;
        }

        public override string ToString()
        {
            if (this.IsNull())
                return null;
            string result = this.Direction switch
            {
                Direction.Forward => $"{this.Coordinate.ToString()}; Forward",
                Direction.Backward => $"{this.Coordinate.ToString()}; Backward",
                Direction.Left => $"{this.Coordinate.ToString()}; Left",
                Direction.Right => $"{this.Coordinate.ToString()}; Right",
                Direction.Up => $"{this.Coordinate.ToString()}; Up",
                Direction.Down => $"{this.Coordinate.ToString()}; Down",
            };
            return result;
        }

        public static bool operator ==(State state1, State state2) => (state1.Coordinate == state2.Coordinate) && (state1.Direction == state2.Direction);

        public static bool operator !=(State state1, State state2) => (state1.Coordinate != state2.Coordinate) || (state1.Direction == state2.Direction);

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

        public bool IsNull()
        {
            return (this.Coordinate.IsNull() && Direction == null && ParentState.IsNull());
        }

        public object Clone()
        {
            return new State(this);
        }
    }
}
