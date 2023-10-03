using Lab1.Model;
using System.Diagnostics.Metrics;

namespace Lab1
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to start");
            Console.ReadLine();
            string mapPath = "Resources/Map/map1.txt";

            Map map = new Map(mapPath);

            State startState = new State { coordinate = map.GetStartPose(), direction = Direction.Left};
            State finishState = new State { coordinate = map.GetFinishPose(), direction = Direction.Down };
            Cube cube = new Cube(startState);
            
            HashSet<State> closedStates = new HashSet<State>();
            Stack<State> openedStates = new Stack<State>();
            
            Random rnd = new Random();

            var neighbors = map.GetNeighbors(cube.state.coordinate);

            Console.Clear();

            openedStates.Push(new State { coordinate = cube.state.coordinate, direction = cube.state.direction});
            //closedStates.Add(new State { coordinate = cube.state.coordinate, direction = cube.state.direction });

            int count = 0;
            State tmpState = new State { coordinate = new Coordinate { x = -1, y = -1}, direction = Direction.Forward };
            State nullState = new State { coordinate = new Coordinate { x = -1, y = -1 }, direction = Direction.Forward };

            int maxO = 0;
            int maxOandC = 0;

            while (true)
            {
                Print(map, cube, neighbors);

                if (count == 0)
                    cube.state.parentState = startState;

                if (tmpState == finishState)
                {
                    break;
                }
                    


                if (count == 0)
                {
                    tmpState = openedStates.Pop();
                    closedStates.Add(tmpState);
                }

                foreach (var neighbor in neighbors)
                {
                    var neighborState = new State { coordinate = neighbor, direction = cube.DirectionAfterMove(neighbor), parentState = cube.state};
                    if (!openedStates.Contains(neighborState) && !closedStates.Contains(neighborState))
                    {
                        openedStates.Push(neighborState);
                    }
                }

                if (openedStates.Count == 0)
                    break;

                

                tmpState = openedStates.Pop();
                closedStates.Add(tmpState);

                if(maxO < openedStates.Count() + 1)
                    maxO = openedStates.Count() + 1;
                if (maxOandC < openedStates.Count() + 1 + closedStates.Count())
                    maxOandC = openedStates.Count() + 1 + closedStates.Count();

                cube.state = tmpState;


                //cube.Step(tmpState.coordinate);
                neighbors = map.GetNeighbors(cube.state.coordinate);
                //Thread.Sleep(100);
                count++;
            }

            Console.WriteLine("Press enter to replay");
            Console.ReadLine();
            Console.Clear();
            Thread.Sleep(100);


            State finishCubeState = cube.state;

            State test = null;

            Stack<State> finishWay = new Stack<State>();
            finishWay.Push(cube.state);

            while(cube.state.parentState.ToString() != startState.ToString())
            {
                finishWay.Push(cube.state.parentState);
                cube.state = cube.state.parentState;
            }
            finishWay.Push(startState);

            cube.state = finishWay.Pop();
            while(finishWay.Count > 0)
            {
                Print(map, cube, null);
                PrintStatistic(maxO, maxOandC, count, openedStates.Count());
                cube.state = finishWay.Pop();
                Thread.Sleep(500);
            }
            Print(map, cube, null);
            PrintStatistic(maxO, maxOandC, count, openedStates.Count());



        }

        static void Print(Map map, Cube cube, List<Coordinate?> neighbors)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Cube state: {cube.state.ToString()}         ");
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, 1);
            if(neighbors != null)
            {
                foreach (Coordinate? coord in neighbors)
                {
                    Console.Write($"{coord.ToString()}; ");
                }
            }

            Console.WriteLine();

            map.PrintMap(cube.state);
        }

        static void PrintStatistic(int maxO, int maxOandC, int count, int finalO)
        {
            Console.WriteLine($"max O: {maxO}; max O and C: {maxOandC}; count of iterations: {count}; final count O: {finalO}");
        }
    }
}