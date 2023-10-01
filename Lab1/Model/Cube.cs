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

            if (coord.x >= state.coordinate.x + 2 || coord.x <= state.coordinate.x - 2 || coord.y >= state.coordinate.y + 2 || coord.y <= state.coordinate.y - 2)
                return;

            if(coord.x < state.coordinate.x && coord.y == state.coordinate.y)
            {
                state.direction = state.direction switch
                {
                    Direction.Up => Direction.Left,
                    Direction.Left => Direction.Down,
                    Direction.Down => Direction.Right,
                    Direction.Right => Direction.Up,
                    _ => state.direction
                };
            }
            else if(coord.x > state.coordinate.x && coord.y == state.coordinate.y)
            {
                state.direction = state.direction switch
                {
                    Direction.Up => Direction.Right,
                    Direction.Right => Direction.Down,
                    Direction.Down => Direction.Left,
                    Direction.Left => Direction.Up,
                    _ => state.direction
                };
            }
            else if(coord.x == state.coordinate.x && coord.y < state.coordinate.y)
            {
                state.direction = state.direction switch
                {
                    Direction.Up => Direction.Forward,
                    Direction.Forward => Direction.Down,
                    Direction.Down => Direction.Backward,
                    Direction.Backward => Direction.Up,
                    _ => state.direction
                };
            }
            else if(coord.x == state.coordinate.x && coord.y > state.coordinate.y)
            {
                state.direction = state.direction switch
                {
                    Direction.Up => Direction.Backward,
                    Direction.Backward => Direction.Down,
                    Direction.Down => Direction.Forward,
                    Direction.Forward => Direction.Up,
                    _ => state.direction
                };
            }

            state.coordinate = coord;
        }
    }
}
