
namespace Lab1.Model
{
    public enum Floor { Empty = 0, Solid = 1, Start = 2, Finish = 3}

    public class Cell
    {
        public Coordinate cordinate { get; set; }
        public Floor floor { get; set; }

        public override string ToString()
        {
            return $"{cordinate.ToString()}; {floor.ToString()}";
        }
    }


    public class Map
    {
        public List<Cell> cells;

        private int _mapRowsCount;
        private int _mapColumnsCount;

        public Map(string mapPath)
        {
                var lines = File.ReadAllLines(mapPath);

                cells = new List<Cell>();

                _mapRowsCount = Int32.Parse(lines[0]);
                _mapColumnsCount = Int32.Parse(lines[1]);

                lines = lines.Skip(2).ToArray();

                for (int i = 0; i < _mapRowsCount; i++)
                {
                    for (int j = 0; j < _mapColumnsCount; j++)
                    {
                        cells.Add(new Cell { cordinate = new Coordinate { x = j, y = i }, floor = lines[i][j] == '0' ? Floor.Empty : (lines[i][j] == '1' ? Floor.Solid : (lines[i][j] == '2' ? Floor.Start : Floor.Finish)) });
                    }
                }

                CheckMap();   
        }

        private void CheckMap()
        {
            int startPoses = 0;
            int finishPoses = 0;

            if (cells.Count() != (_mapRowsCount * _mapColumnsCount))
                throw new Exception("The size of the map does not match the values from the file!");

            foreach(var cell in cells)
            {
                if (cell.floor == Floor.Start)
                    startPoses++;
                if (cell.floor == Floor.Finish)
                    finishPoses++;
            }

            if (startPoses != 1)
                throw new Exception("The number of starting positions is not equal to 1!");
            if (finishPoses != 1)
                throw new Exception("The number of finishing positions is not equal to 1!");

        }

        public void PrintMap(State state)
        {
            int counter = 0;
            foreach(var cell in cells)
            {
                if(state.Coordinate == cell.cordinate)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{state.Direction.ToString()[0]}  ");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                    Console.Write($"{(int)cell.floor}  ");
                counter++;
                if (counter == _mapColumnsCount)
                {
                    Console.WriteLine();
                    counter = 0;
                }
            }
            Console.WriteLine();
        }

        public Coordinate GetStartPose()
        {
           return cells.Find(x => x.floor == Floor.Start).cordinate;
        }

        public Coordinate GetFinishPose()
        {
            return cells.Find(x => x.floor == Floor.Finish).cordinate;
        }

        private bool CanPass(Coordinate coordinate)
        {
            bool result = false;

            if (coordinate.Equals(default(Coordinate)))
                return result;

            var tmpCell = cells.Find(x => (x.cordinate == coordinate));
            if(tmpCell != null)
            {
                if (tmpCell.floor != Floor.Empty)
                    result = true;
            }

            return result;
        }

        public List<Coordinate> GetNeighbors(Coordinate coordinate)
        {
            List<Coordinate> neighbors = new List<Coordinate>();

            foreach (var cell in cells)
            {
                if (cell.cordinate.x == coordinate.x + 1 && cell.cordinate.y == coordinate.y)
                {
                    var tmp = new Coordinate { x = cell.cordinate.x, y = cell.cordinate.y };
                    if (CanPass(tmp))
                        neighbors.Add(tmp);
                }
                if (cell.cordinate.x == coordinate.x - 1 && cell.cordinate.y == coordinate.y)
                {
                    var tmp = new Coordinate { x = cell.cordinate.x, y = cell.cordinate.y };
                    if (CanPass(tmp))
                        neighbors.Add(tmp);
                }
                if (cell.cordinate.x == coordinate.x && cell.cordinate.y == coordinate.y + 1)
                {
                    var tmp = new Coordinate { x = cell.cordinate.x, y = cell.cordinate.y };
                    if (CanPass(tmp))
                        neighbors.Add(tmp);
                }
                if (cell.cordinate.x == coordinate.x && cell.cordinate.y == coordinate.y - 1)
                {
                    var tmp = new Coordinate { x = cell.cordinate.x, y = cell.cordinate.y };
                    if (CanPass(tmp))
                        neighbors.Add(tmp);
                }
            }

            return neighbors;
        }

        public int GetMapSize() => _mapColumnsCount * _mapRowsCount;

    }
}
