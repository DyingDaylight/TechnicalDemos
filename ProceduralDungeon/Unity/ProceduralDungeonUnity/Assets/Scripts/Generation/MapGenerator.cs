using Core;

namespace Generation
{
    public class MapGenerator
    {
        private readonly MazeGenerator mazeGenerator;

        public MapGenerator(MazeGenerator mazeGenerator)
        {
            this.mazeGenerator = mazeGenerator;
        }

        public DungeonMap Generate(int width, int height)
        {
            DungeonMap map = new DungeonMap(width, height);
            
            mazeGenerator.Generate(map);
            
            return map;
        }
    }
}