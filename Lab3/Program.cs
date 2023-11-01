using Lab1.Model;
using System.Diagnostics.Metrics;

namespace Lab1
{
    public enum Heuristic { Default = 1, Manhattan, ManhattanExtended}

    public class Program
    {
        private static Map map;
        private static State startState;
        private static State finishState;

        private static Heuristic heuristic;
        static void Main(string[] args)
        {

            while (true)
            {
                bool exit = false;
                int swit = -1;

                string emptyStr = new string(' ', 4);

                while (true)
                {
                    Console.WriteLine($"Choose option:\r\n{emptyStr}0 - exit;\r\n{emptyStr}1 - Default(g(x));\r\n{emptyStr}2 - Manhattan(g(x) + h1(x))\r\n{emptyStr}3 - Mahattan Extended(g(x) + h2(x))");
                    try
                    {
                        swit = Int32.Parse(Console.ReadLine());
                    }
                    catch (Exception ex) { Console.WriteLine("Type some number!"); }

                    switch (swit)
                    {
                        case 0: return; break;
                        case 1: heuristic = Heuristic.Default; exit = true; break;
                        case 2: heuristic = Heuristic.Manhattan; exit = true; break;
                        case 3: heuristic = Heuristic.ManhattanExtended; exit = true; break;
                        default: Console.WriteLine("Type correct number!"); break;
                    }
                    if (exit)
                        break;
                }




                Console.WriteLine("Press enter to start");
                Console.ReadLine();
                Console.Clear();

                string mapPath = "Resources/Map/map2.txt";

                map = new Map(mapPath);

                startState = new State { Coordinate = map.GetStartPose(), Direction = Direction.Left };
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
                        finder.Print();
                        //PrintStatistic(finder.MaxO, finder.MaxOAndC, finder.Count, finder.LastO);
                        cube.State = finishWay.Pop();
                        Thread.Sleep(500);
                    }
                    Print(map, cube, null);
                    finder.Print();
                    //PrintStatistic(finder.MaxO, finder.MaxOAndC, finder.Count, finder.LastO);


                }
                else
                {
                    Console.WriteLine($"There no way to {finishState.ToString()}");
                    finder.Print();
                    //PrintStatistic(finder.MaxO, finder.MaxOAndC, finder.Count, finder.LastO);
                }

                Console.WriteLine();
                swit = -1;
                exit = false;
            }
        }

        static Statistic FindWay(Cube cube, int maxDepth)
        {
            cube.State.Depth = 0;

            HashSet<State> closedStates = new HashSet<State>();
            //Stack<State> openedStates = new Stack<State>();

            PriorityQueue<State, float> openedStates = new PriorityQueue<State, float>();

            Statistic stat = new Statistic();

            var neighbors = map.GetNeighbors(cube.State.Coordinate);

            //openedStates.Push(new State { Coordinate = cube.State.Coordinate, Direction = cube.State.Direction });
            openedStates.Enqueue(new State { Coordinate = cube.State.Coordinate, Direction = cube.State.Direction }, HeuristicFunction(cube));
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
                    //tmpState = openedStates.Pop();
                    tmpState = openedStates.Dequeue();
                    closedStates.Add(tmpState);
                }

                foreach (var neighbor in neighbors)
                {
                    var neighborState = new State { Coordinate = neighbor, Direction = cube.DirectionAfterMove(neighbor), ParentState = cube.State, Depth = cube.State.Depth + 1 };
                    if (cube.State.Depth <= maxDepth)
                    {
                        //if (!openedStates.Contains(neighborState) && !closedStates.Contains(neighborState))
                        if (!openedStates.UnorderedItems.Select(x => x.ToString() == neighbor.ToString()).FirstOrDefault() && !closedStates.Contains(neighborState))
                        {
                            //openedStates.Push(neighborState);
                            openedStates.Enqueue(neighborState, HeuristicFunction(cube));
                        }
                    }
                }

                if (openedStates.Count == 0)
                    break;

                if (openedStates.Count == 0 && cube.State.Depth >= maxDepth)
                    break;


                //tmpState = openedStates.Pop();
                tmpState = openedStates.Dequeue();
                closedStates.Add(tmpState);

                if (stat.MaxO < openedStates.Count + 1)
                    stat.MaxO = openedStates.Count + 1;
                if (stat.MaxOAndC < openedStates.Count + 1 + closedStates.Count())
                    stat.MaxOAndC = openedStates.Count + 1 + closedStates.Count();

                cube.State = tmpState;

                //cube.Step(tmpState.coordinate);
                neighbors = map.GetNeighbors(cube.State.Coordinate);
                //Thread.Sleep(300);
                stat.Count++;
            }

            stat.LastO = openedStates.Count;

            return stat;
        }

        static public float HeuristicFunction(Cube cube)
        {
            float result = 0;
            float wayLength = 0;
            switch (heuristic)
            {
                case Heuristic.Default:
                    {
                        result = cube.State.Depth;
                    }
                    break;
                case Heuristic.Manhattan:
                    {
                        wayLength = Math.Abs(cube.State.Coordinate.x - finishState.Coordinate.x) + Math.Abs(cube.State.Coordinate.y - finishState.Coordinate.y);
                        result = cube.State.Depth + wayLength;
                    }
                    break;
                case Heuristic.ManhattanExtended:
                    {
                        var tmpCube = new Cube(new State(cube.State));

                        while (tmpCube.State.Coordinate.x > finishState.Coordinate.x)
                            tmpCube.Step(new Coordinate { x = tmpCube.State.Coordinate.x - 1, y = tmpCube.State.Coordinate.y });
                        while (tmpCube.State.Coordinate.x < finishState.Coordinate.x)
                            tmpCube.Step(new Coordinate { x = tmpCube.State.Coordinate.x + 1, y = tmpCube.State.Coordinate.y });
                        while (tmpCube.State.Coordinate.y > finishState.Coordinate.y)
                            tmpCube.Step(new Coordinate { x = tmpCube.State.Coordinate.x, y = tmpCube.State.Coordinate.y - 1 });
                        while (tmpCube.State.Coordinate.y < finishState.Coordinate.y)
                            tmpCube.Step(new Coordinate { x = tmpCube.State.Coordinate.x, y = tmpCube.State.Coordinate.y + 1 });

                        wayLength = Math.Abs(cube.State.Coordinate.x - finishState.Coordinate.x) + Math.Abs(cube.State.Coordinate.y - finishState.Coordinate.y);
                        wayLength = tmpCube.State.Direction switch
                        {
                            Direction.Right => wayLength + 1,
                            Direction.Left => wayLength + 1,
                            Direction.Up => wayLength + 2,
                            Direction.Down => wayLength + 0,
                            Direction.Forward => wayLength + 1,
                            Direction.Backward => wayLength + 1
                        };
                        result = cube.State.Depth + wayLength;
                    }
                    break;
            }


            return result;
        }

        static void Print(Map map, Cube cube, List<Coordinate?> neighbors)
        {
            string empty = new string(' ', 10);
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Cube state: {cube.State.ToString()}; Depth: {cube.State.Depth}{empty}");
            Console.WriteLine($"Current algorithm: {heuristic.ToString()}");
            //Console.WriteLine(new string(' ', Console.WindowWidth));
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