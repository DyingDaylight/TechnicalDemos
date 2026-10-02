namespace Core
{
    // Dungeon coordinates use [x, y]:
    // x = horizontal (width), y = vertical (height).
    public class DungeonMap : IReadOnlyDungeonMap
    {
        private readonly TileType[,] _map;

        public int Width { get; }
        public int Height { get; }

        public DungeonMap(int width, int height)
        {
            _map = new TileType[width, height];
            Width = width;
            Height = height;
        }
        
        public TileType this[int x, int y]
        {
            get => _map[x, y];
            set => _map[x, y] = value;
        }

        public bool IsInside(int x, int y)
        {
            return x >= 0 && x < Width &&
                   y >= 0 && y < Height;
        }
    }
}