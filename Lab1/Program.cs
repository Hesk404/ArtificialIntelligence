using Lab1.Model;

namespace Lab1
{
    public class Program
    {
        static void Main(string[] args)
        {
            string mapPath = "Resources/Map/map1.txt";

            Map map = new Map(mapPath);

            State state = new State { coordinate = map.GetStartPose(), direction = Direction.Left};
            State finishState = new State { coordinate = map.GetFinishPose(), direction = Direction.Down };
            Cube cube = new Cube(state);
            
            HashSet<State> closedStates = new HashSet<State>();
            Stack<State> openedStates = new Stack<State>();
            
            Random rnd = new Random();

            var neighbors = map.GetNeighbors(cube.state.coordinate);

            Console.Clear();

            openedStates.Push(new State { coordinate = cube.state.coordinate, direction = cube.state.direction});
            //closedStates.Add(new State { coordinate = cube.state.coordinate, direction = cube.state.direction });

            int count = 0;
            State tmpState = new State { coordinate = new Coordinate { x = -1, y = -1}, direction = Direction.Forward };

            while (true)
            {
                Print(map, cube, neighbors);


                if (tmpState == finishState)
                {
                    break;
                }
                    
                if (cube.state.ToString() == "[19.4]; Right")
                {
                   
                }


                if (count == 0)
                {
                    tmpState = openedStates.Pop();
                    closedStates.Add(tmpState);
                }

                foreach (var neighbor in neighbors)
                {
                    var neighborState = new State { coordinate = neighbor, direction = cube.DirectionAfterMove(neighbor)};
                    var tmp1 = openedStates.Contains(neighborState);
                    var tmp2 = closedStates.Contains(neighborState);
                    if (!openedStates.Contains(neighborState) && !closedStates.Contains(neighborState))
                    {
                        openedStates.Push(neighborState);
                    }
                }

                if (openedStates.Count == 0)
                    break;


                tmpState = openedStates.Pop();
                closedStates.Add(tmpState);

                cube.Step(tmpState.coordinate);
                neighbors = map.GetNeighbors(cube.state.coordinate);
                //Thread.Sleep(100);
                count++;
            }

        }

        static void Print(Map map, Cube cube, List<Coordinate?> neighbors)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Cube state: {cube.state.ToString()}         ");
            Console.WriteLine(new string(' ', 7*4));
            Console.SetCursorPosition(0, 1);
            foreach(Coordinate? coord in neighbors)
            {
                Console.Write($"{coord.ToString()}; ");
            }
            Console.WriteLine();

            map.PrintMap(cube.state);
        }
    }
}