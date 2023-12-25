using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Model
{
    public class Cube
    {
        public State State;

        public Cube(State state) 
        {
            this.State = state;
        }

        public void Step(Coordinate coord)
        {
            if(coord.x < State.Coordinate.x && coord.y == State.Coordinate.y)
            {
                State.Direction = State.Direction switch
                {
                    Direction.Up => Direction.Left,
                    Direction.Left => Direction.Down,
                    Direction.Down => Direction.Right,
                    Direction.Right => Direction.Up,
                    _ => State.Direction
                };
            }
            else if(coord.x > State.Coordinate.x && coord.y == State.Coordinate.y)
            {
                State.Direction = State.Direction switch
                {
                    Direction.Up => Direction.Right,
                    Direction.Right => Direction.Down,
                    Direction.Down => Direction.Left,
                    Direction.Left => Direction.Up,
                    _ => State.Direction
                };
            }
            else if(coord.x == State.Coordinate.x && coord.y < State.Coordinate.y)
            {
                State.Direction = State.Direction switch
                {
                    Direction.Up => Direction.Forward,
                    Direction.Forward => Direction.Down,
                    Direction.Down => Direction.Backward,
                    Direction.Backward => Direction.Up,
                    _ => State.Direction
                };
            }
            else if(coord.x == State.Coordinate.x && coord.y > State.Coordinate.y)
            {
                State.Direction = State.Direction switch
                {
                    Direction.Up => Direction.Backward,
                    Direction.Backward => Direction.Down,
                    Direction.Down => Direction.Forward,
                    Direction.Forward => Direction.Up,
                    _ => State.Direction
                };
            }

            State.Coordinate = coord;
        }

        public Direction DirectionAfterMove(Coordinate coord)
        {
            if (coord.x < State.Coordinate.x && coord.y == State.Coordinate.y)
            {
                return State.Direction switch
                {
                    Direction.Up => Direction.Left,
                    Direction.Left => Direction.Down,
                    Direction.Down => Direction.Right,
                    Direction.Right => Direction.Up,
                    _ => State.Direction
                };
            }
            else if (coord.x > State.Coordinate.x && coord.y == State.Coordinate.y)
            {
                return State.Direction switch
                {
                    Direction.Up => Direction.Right,
                    Direction.Right => Direction.Down,
                    Direction.Down => Direction.Left,
                    Direction.Left => Direction.Up,
                    _ => State.Direction
                };
            }
            else if (coord.x == State.Coordinate.x && coord.y < State.Coordinate.y)
            {
                return State.Direction switch
                {
                    Direction.Up => Direction.Forward,
                    Direction.Forward => Direction.Down,
                    Direction.Down => Direction.Backward,
                    Direction.Backward => Direction.Up,
                    _ => State.Direction
                };
            }
            else if (coord.x == State.Coordinate.x && coord.y > State.Coordinate.y)
            {
                return State.Direction switch
                {
                    Direction.Up => Direction.Backward,
                    Direction.Backward => Direction.Down,
                    Direction.Down => Direction.Forward,
                    Direction.Forward => Direction.Up,
                    _ => State.Direction
                };
            }

            return State.Direction;
        }
    }
}
