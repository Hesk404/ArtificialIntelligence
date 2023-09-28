using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Model
{
    public class Cube
    {
        public State state;

        public Cube(State state) 
        {
            this.state = state;
        }

        public void Step(Coordinate coord)
        {
            if(coord.x == state.coordinate.x && coord.y == state.coordinate.y)
                return;

            if (coord.x + 1 > state.coordinate.x || coord.x + 1 < state.coordinate.x || coord.y + 1 > state.coordinate.y || coord.y + 1 < state.coordinate.y)
                return;

            if(coord.x < state.coordinate.x && coord.y == state.coordinate.y)
            {
                state.direction = state.direction switch
                {
                    Direction.Up => Direction.Left,
                    Direction.Left => Direction.Down,
                    Direction.Down => Direction.Right,
                    Direction.Right => Direction.Up
                };
            }
            else if(coord.x > state.coordinate.x && coord.y == state.coordinate.y)
            {
                state.direction = state.direction switch
                {
                    Direction.Up => Direction.Right,
                    Direction.Right => Direction.Down,
                    Direction.Down => Direction.Left,
                    Direction.Left => Direction.Up
                };
            }
            else if(coord.x == state.coordinate.x && coord.y < state.coordinate.y)
            {
                state.direction = state.direction switch
                {
                    Direction.Up => Direction.Forward,
                    Direction.Forward => Direction.Down,
                    Direction.Down => Direction.Backward,
                    Direction.Backward => Direction.Up
                };
            }
            else if(coord.x == state.coordinate.x && coord.y > state.coordinate.y)
            {
                state.direction = state.direction switch
                {
                    Direction.Up => Direction.Backward,
                    Direction.Backward => Direction.Down,
                    Direction.Down => Direction.Forward,
                    Direction.Forward => Direction.Up
                };
            }

            state.coordinate = coord;
        }
    }
}
