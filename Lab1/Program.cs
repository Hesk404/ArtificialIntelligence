using Lab1.Model;
using System.Diagnostics.Metrics;

namespace Lab1
{
    public class Program
    {
        private static Map map;
        private static State startState;
        private static State finishState;
        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to start");
            Console.ReadLine();
            string mapPath = "Resources/Map/map1.txt";

            map = new Map(mapPath);

            startState = new State { Coordinate = map.GetStartPose(), Direction = Direction.Left};
            finishState = new State { Coordinate = map.GetFinishPose(), Direction = Direction.Down };

            Cube cube = new Cube(startState);

            Statistic finder = new Statistic();

            finder = FindWay(cube, int.MaxValue);

            if (finder.IsHaveWay)
            {
                Console.WriteLine("Press enter to replay");
                Console.ReadLine();
                Console.Clear();
                Thread.Sleep(100);


                State finishCubeState = cube.State;

                State test = null;

                Stack<State> finishWay = new Stack<State>();
                finishWay.Push(cube.State);

                while (cube.State.ParentState.ToString() != startState.ToString())
                {
                    finishWay.Push(cube.State.ParentState);
                    cube.State = cube.State.ParentState;
                }
                finishWay.Push(startState);

                cube.State = finishWay.Pop();
                while (finishWay.Count > 0)
                {
                    Print(map, cube, null);
                    PrintStatistic(finder.MaxO, finder.MaxOAndC, finder.Count, finder.LastO);
                    cube.State = finishWay.Pop();
                    Thread.Sleep(500);
                }
                Print(map, cube, null);
                PrintStatistic(finder.MaxO, finder.MaxOAndC, finder.Count, finder.LastO);

            }
            else
            {
                Console.WriteLine($"There no way to {finishState.ToString()}");
                PrintStatistic(finder.MaxO, finder.MaxOAndC, finder.Count, finder.LastO);
            }

        }

        static Statistic FindWay(Cube cube, int maxDepth)
        {
            cube.State.Depth = 0;

            HashSet<State> closedStates = new HashSet<State>();
            Stack<State> openedStates = new Stack<State>();

            Statistic stat = new Statistic();

            var neighbors = map.GetNeighbors(cube.State.Coordinate);

            openedStates.Push(new State { Coordinate = cube.State.Coordinate, Direction = cube.State.Direction });
            //closedStates.Add(new State { coordinate = cube.state.coordinate, direction = cube.state.direction });

            State tmpState = new State { Coordinate = new Coordinate { x = -1, y = -1 }, Direction = Direction.Forward };
            //State nullState = new State { coordinate = new Coordinate { x = -1, y = -1 }, direction = Direction.Forward };

            List<State> finishStates = new List<State>();

            while (openedStates.Count >= 0)
            {
                Print(map, cube, neighbors);

                if (stat.Count == 0)
                    cube.State.ParentState = startState;

                if (tmpState == finishState)
                {
                    //finishStates.Add(tmpState);
                    stat.IsHaveWay = true;
                    break;
                }

                if (stat.Count == 0)
                {
                    tmpState = openedStates.Pop();
                    closedStates.Add(tmpState);
                }

                foreach (var neighbor in neighbors)
                {
                    var neighborState = new State { Coordinate = neighbor, Direction = cube.DirectionAfterMove(neighbor), ParentState = cube.State, Depth = cube.State.Depth + 1 };
                    if (cube.State.Depth <= maxDepth)
                    {
                        if (!openedStates.Contains(neighborState) && !closedStates.Contains(neighborState))
                        {
                            openedStates.Push(neighborState);
                        }
                    }
                }

                if (openedStates.Count == 0)
                    break;

                if (openedStates.Count == 0 && cube.State.Depth >= maxDepth)
                    break;


                tmpState = openedStates.Pop();
                closedStates.Add(tmpState);

                if (stat.MaxO < openedStates.Count() + 1)
                    stat.MaxO = openedStates.Count() + 1;
                if (stat.MaxOAndC < openedStates.Count() + 1 + closedStates.Count())
                    stat.MaxOAndC = openedStates.Count() + 1 + closedStates.Count();

                cube.State = tmpState;

                //cube.Step(tmpState.coordinate);
                neighbors = map.GetNeighbors(cube.State.Coordinate);
                //Thread.Sleep(300);
                stat.Count++;
            }

            stat.LastO = openedStates.Count();

            return stat;
        }

        static void Print(Map map, Cube cube, List<Coordinate?> neighbors)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Cube state: {cube.State.ToString()}         ");
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

            map.PrintMap(cube.State);
        }

        static void PrintStatistic(int maxO, int maxOandC, int count, int finalO)
        {
            Console.WriteLine($"max O: {maxO}; max O and C: {maxOandC}; count of iterations: {count}; final count O: {finalO}");
        }
    }
}