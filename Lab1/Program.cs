using Lab1.Model;

namespace Lab1
{
    public class Program
    {
        static void Main(string[] args)
        {
            string mapPath = "Resources/Map/map1.txt";

            Map map = new Map(mapPath);

            State state = new State { coordinate = map.GetStartPose(), direction = Direction.Forward};
            Cube cube = new Cube(state);


            var tmp = map.GetNeighbors(cube.state.coordinate);

            map.PrintMap(cube.state);
            cube.Step(tmp[0]);

            map.PrintMap(cube.state);
            tmp = map.GetNeighbors(cube.state.coordinate);
            cube.Step(tmp[0]);
            map.PrintMap(cube.state);

        }
    }
}