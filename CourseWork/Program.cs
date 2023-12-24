using Lab1.Model;
using Lab3.Model;
using System.Diagnostics.Metrics;

namespace Lab1
{
    public enum Heuristic { BFS = 1, Manhattan, ManhattanExtended, IDS}

    public class Program
    {
        private static Map map;
        private static State startState;
        private static State finishState;

        private static Heuristic heuristic;

        private static int depth = 2;
        private static bool isIds = false;
        static void Main(string[] args)
        {
            string mapPath = "Resources/Map/map2.txt";

            map = new Map(mapPath);

            startState = new State { Coordinate = map.GetStartPose(), Direction = Direction.Left };
            finishState = new State { Coordinate = map.GetFinishPose(), Direction = Direction.Down };

            while (true)
            {
                bool exit = false;
                int swit = -1;

                string emptyStr = new string(' ', 4);

                

                while (true)
                {


                    //heuristic = Heuristic.Default;
                    //var test = GenerateState();
                    //startState = test;



                    Console.WriteLine($"Choose option:" +
                        $"\r\n{emptyStr}0 - exit;" +
                        $"\r\n{emptyStr}1 - Generate new start state" +
                        $"\r\n{emptyStr}2 - BFS" +
                        $"\r\n{emptyStr}3 - IDS" +
                        $"\r\n{emptyStr}4 - Manhattan(g(x) + h1(x))" +
                        $"\r\n{emptyStr}5 - Mahattan Extended(g(x) + h2(x))" +
                        $"\r\n{emptyStr}6 - Tests");
                    try
                    {
                        swit = Int32.Parse(Console.ReadLine());
                    }
                    catch (Exception ex) { Console.WriteLine("Type some number!"); }

                    switch (swit)
                    {
                        case 0: return; break;
                        case 1: GenerateNewStartState(); break;
                        case 2: heuristic = Heuristic.BFS; exit = true; break;
                        case 3: heuristic = Heuristic.IDS; isIds = true; exit = true; break;
                        case 4: heuristic = Heuristic.Manhattan; exit = true; break;
                        case 5: heuristic = Heuristic.ManhattanExtended; exit = true; break;
                        case 6: StartTests(); break;
                        default: Console.WriteLine("Type correct number!"); break;
                    }
                    if (exit)
                        break;
                }




                Console.WriteLine("Press enter to start");
                Console.ReadLine();
                Console.Clear();


                
                
                Cube cube = new Cube(startState);

                Statistic finder = new Statistic();

                if (!isIds)
                    finder = FindWay(cube, int.MaxValue);
                else
                { 
                    for(int i = 0; i < int.MaxValue; i++)
                    {
                        cube = new Cube(startState);
                        Statistic tmpFinder = FindWayWithIterativeDepths(cube, i);
                        finder.MaxO = tmpFinder.MaxO > finder.MaxO ? tmpFinder.MaxO : finder.MaxO;
                        finder.MaxOAndC = tmpFinder.MaxOAndC > finder.MaxOAndC ? tmpFinder.MaxOAndC : finder.MaxOAndC;
                        finder.IsHaveWay = tmpFinder.IsHaveWay;
                        finder.LastO = tmpFinder.LastO;
                        finder.Count += tmpFinder.Count;
                        if (finder.IsHaveWay)
                            break;
                        if (i == map.GetMapSize())
                            break;
                    }
                }
                isIds = false;

                WriteStatisticToFile(finder);

                Console.WriteLine("Replay?");

                if (Console.ReadLine() == "y")
                {
                    if (finder.IsHaveWay)
                    {
                        Console.WriteLine("Press enter to replay");
                        Console.ReadLine();
                        Console.Clear();
                        Thread.Sleep(100);


                        State finishCubeState = cube.State;

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
                }
                Console.WriteLine();
                swit = -1;
                exit = false;
            }
        }

        static void GenerateNewStartState()
        {
            

            Console.Write("Type depth ( >= 2): ");
            int.TryParse(Console.ReadLine(), out depth);

            if (depth < 2)
                depth = 2;

            Console.WriteLine($"Depth will be {depth}");

            heuristic = Heuristic.BFS; 
            startState = GenerateState(depth);
            Console.WriteLine($"Generated start state: {startState.ToString()}");
            map.SetStartPose(startState.Coordinate);
        }

        static Statistic FindWay(Cube cube, int maxDepth, bool isPrint = true)
        {
            cube.State.Depth = 0;

            HashSet<State> closedStates = new HashSet<State>();
            //Stack<State> openedStates = new Stack<State>();

            //PriorityQueue<State, float> openedStates = new PriorityQueue<State, float>();

            List<KeyValue> openedStates = new List<KeyValue>();


            Statistic stat = new Statistic();

            var neighbors = map.GetNeighbors(cube.State.Coordinate);

            //openedStates.Push(new State { Coordinate = cube.State.Coordinate, Direction = cube.State.Direction });
            //openedStates.Enqueue(new State { Coordinate = cube.State.Coordinate, Direction = cube.State.Direction }, HeuristicFunction(cube));
            openedStates.Add(new KeyValue { Key = HeuristicFunction(cube), Value = new State { Coordinate = cube.State.Coordinate, Direction = cube.State.Direction } });
            openedStates = openedStates.OrderBy(x => x.Key).ToList();
            //closedStates.Add(new State { coordinate = cube.state.coordinate, direction = cube.state.direction });

            State tmpState = new State { Coordinate = new Coordinate { x = -1, y = -1 }, Direction = Direction.Forward };
            //State nullState = new State { coordinate = new Coordinate { x = -1, y = -1 }, direction = Direction.Forward };

            List<State> finishStates = new List<State>();

            while (openedStates.Count >= 0)
            {
                if(isPrint)
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
                    //tmpState = openedStates.Dequeue();
                    tmpState = openedStates.First().Value;
                    openedStates.Remove(openedStates.First());
                    openedStates = openedStates.OrderBy(x => x.Key).ToList();
                    closedStates.Add(tmpState);
                }

                

                foreach (var neighbor in neighbors)
                {
                    var neighborState = new State { Coordinate = neighbor, Direction = cube.DirectionAfterMove(neighbor), ParentState = cube.State, Depth = cube.State.Depth + 1 };
                    if (cube.State.Depth <= maxDepth)
                    {
                        //if (!openedStates.Contains(neighborState) && !closedStates.Contains(neighborState))
                        //if (!openedStates.UnorderedItems.Select(x => x.ToString() == neighbor.ToString()).FirstOrDefault() && !closedStates.Contains(neighborState))
                        var tmp = openedStates.Where(x => (x.Key >= 0) && (x.Key <= openedStates.Max(x => x.Key)) && (x.Key != HeuristicFunction(cube)) && (x.Value.ToString() == neighborState.ToString())).FirstOrDefault();
                        if (tmp != null)
                        {
                            if (tmp.Key > HeuristicFunction(cube))
                            {
                                tmp.Key = HeuristicFunction(cube);
                                openedStates.OrderBy(x => x.Key);
                                continue;
                            }
                        }
                        

                        if (!openedStates.Contains(new KeyValue { Key = HeuristicFunction(cube), Value = neighborState }) && !closedStates.Contains(neighborState))
                        {
                            //openedStates.Push(neighborState);
                            //openedStates.Enqueue(neighborState, HeuristicFunction(cube));
                            openedStates.Add(new KeyValue { Key = HeuristicFunction(cube), Value = neighborState });
                            openedStates = openedStates.OrderBy(x => x.Key).ToList();
                        }
                    }
                }

                

                if (openedStates.Count == 0)
                    break;

                if (openedStates.Count == 0 && cube.State.Depth >= maxDepth)
                    break;


                //tmpState = openedStates.Pop();
                //tmpState = openedStates.Dequeue();
                tmpState = openedStates.First().Value;
                openedStates.Remove(openedStates.First());
                openedStates = openedStates.OrderBy(x => x.Key).ToList();
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

        static Statistic FindWayWithIterativeDepths(Cube cube, int maxDepth, bool isPrint = true)
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
                if(isPrint)
                    Print(map, cube, neighbors);

                if (stat.Count == 0)
                    cube.State.ParentState = startState;

                if (tmpState == finishState)
                {
                    //finishStates.Add(tmpState);
                    stat.IsHaveWay = true;
                    break;
                }

                //if(cube.State.Coordinate.x == 20 && cube.State.Coordinate.y == 4)
                //{
                //    GC.Collect();
                //}

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

            GC.Collect();
            return stat;
        }

        static public float HeuristicFunction(Cube cube)
        {
            float result = 0;
            float wayLength = 0;
            switch (heuristic)
            {
                case Heuristic.BFS:
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

        static State GenerateState(int depth = 10)
        {
            State state;
            Cube cube;

            Random rand = new Random();

            while (true)
            {
                state = new State { Coordinate = map.GetRandomCoordinate(), Direction = (Direction)rand.Next(1, 7), Depth = 1 };
                cube = new Cube(state);
                var find = FindWay(cube, depth, false).IsHaveWay;
                if (find)
                {
                    if (cube.State.Depth >= depth)
                    {
                        var initialDepth = cube.State.Depth;
                        while (cube.State.Depth > (initialDepth - depth))
                        {
                            var tmp = cube.State.ParentState;
                            cube.Step(cube.State.ParentState.Coordinate);
                            cube.State.ParentState = tmp.ParentState;
                            cube.State.Depth = tmp.Depth;
                        }
                        return cube.State;
                    }
                }
            }
            

            

            return null;
        }

        static void WriteStatisticToFile(Statistic finder)
        {
            string folder = "App_Data";
            string file = "Stat.txt";

            string str = $"Depth: {depth};  Start state: {startState.ToString()}; Statistic: {finder.ToString(true)}\tAlghorithm: {heuristic.ToString()};\r\n";

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), folder)))
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), folder));

            File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), folder, file), str);
        }

        static Statistic GetTest()
        {
            Cube cube = new Cube(startState);

            Statistic finder = new Statistic();

            if (!isIds)
                finder = FindWay(cube, int.MaxValue, false);
            else
            {
                for (int i = 0; i < int.MaxValue; i++)
                {
                    cube = new Cube(startState);
                    Statistic tmpFinder = FindWayWithIterativeDepths(cube, i, false);
                    finder.MaxO = tmpFinder.MaxO > finder.MaxO ? tmpFinder.MaxO : finder.MaxO;
                    finder.MaxOAndC = tmpFinder.MaxOAndC > finder.MaxOAndC ? tmpFinder.MaxOAndC : finder.MaxOAndC;
                    finder.IsHaveWay = tmpFinder.IsHaveWay;
                    finder.LastO = tmpFinder.LastO;
                    finder.Count += tmpFinder.Count;
                    if (finder.IsHaveWay)
                        break;
                    if (i == map.GetMapSize())
                        break;
                }
            }
            isIds = false;

            return finder;
        }

        static void StartTests()
        {
            //Console.WriteLine("Press enter to start");
            //Console.ReadLine();
            Console.Clear();
            
            List<int> MaxO_BFS = new List<int>();
            List<int> MaxO_IDS = new List<int>();
            List<int> MaxO_Manhattan = new List<int>();
            List<int> MaxO_ManhattanExtended = new List<int>();

            List<int> Count_BFS = new List<int>();
            List<int> Count_IDS = new List<int>();
            List<int> Count_Manhattan = new List<int>();
            List<int> Count_ManhattanExtended = new List<int>();

            List<int> avgMaxO = new List<int>();
            List<int> avgCount = new List<int>();

            Statistic finder;

            Console.Write("Type depth: ");
            int.TryParse(Console.ReadLine(), out depth);

            List<string> states = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                while(true)
                {
                    startState = GenerateState(depth);
                    if (!states.Contains(startState.ToString()))
                    {
                        states.Add(startState.ToString());
                        break;
                    }

                }

                Console.WriteLine($"Generated state: {startState.ToString()}");

                heuristic = Heuristic.BFS;
                finder = GetTest();
                MaxO_BFS.Add(finder.MaxOAndC);
                Count_BFS.Add(finder.Count);
                WriteStatisticToFile(finder);
                finder = null;


                heuristic = Heuristic.IDS;
                isIds = true;
                finder = GetTest();
                MaxO_IDS.Add(finder.MaxOAndC);
                Count_IDS.Add(finder.Count);
                WriteStatisticToFile(finder);
                finder = null;

                heuristic = Heuristic.Manhattan;
                finder = GetTest();
                MaxO_Manhattan.Add(finder.MaxOAndC);
                Count_Manhattan.Add(finder.Count);
                WriteStatisticToFile(finder);
                finder = null;

                heuristic = Heuristic.ManhattanExtended;
                finder = GetTest();
                MaxO_ManhattanExtended.Add(finder.MaxOAndC);
                Count_ManhattanExtended.Add(finder.Count);
                WriteStatisticToFile(finder);
                finder = null;


                File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Stat.txt"), "\r\n");
            }

            avgMaxO.Add(GetAvg(MaxO_BFS));
            avgMaxO.Add(GetAvg(MaxO_IDS));
            avgMaxO.Add(GetAvg(MaxO_Manhattan));
            avgMaxO.Add(GetAvg(MaxO_ManhattanExtended));

            avgCount.Add(GetAvg(Count_BFS));
            avgCount.Add(GetAvg(Count_IDS));
            avgCount.Add(GetAvg(Count_Manhattan));
            avgCount.Add(GetAvg(Count_ManhattanExtended));

            string avgMax = string.Join("\t", avgMaxO);
            string avgCou = string.Join("\t", avgCount);

            File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Stat.txt"), avgMax + "\r\n");
            File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Stat.txt"), avgCou + "\r\n");
            File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Stat.txt"), "\r\n");


        }

        static int GetAvg(List<int> list)
        {
            int result = 0;
            foreach (var item in list)
                result += item;
            result = result / list.Count;
            return result;
        }

       
    }
}