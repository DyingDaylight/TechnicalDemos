namespace Core
{
    // Dungeon coordinates use [x, y]:
    // x = horizontal (width), y = vertical (height).
    public class DungeonMap : IReadOnlyDungeonMap
    {
        private readonly TileType[,] map;

        public int Width { get; }
        public int Height { get; }

        public DungeonMap(int width, int height)
        {
            map = new TileType[width, height];
            Width = width;
            Height = height;
        }

        public DungeonMap(IReadOnlyDungeonMap sourceMap)
        {
            Width = sourceMap.Width;
            Height = sourceMap.Height;

            map = new TileType[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    map[x, y] = sourceMap[x, y];
                }
            }
        }

        public TileType this[int x, int y]
        {
            get => map[x, y];
            set => map[x, y] = value;
        }

        public bool IsInside(int x, int y)
        {
            return x >= 0 && x < Width &&
                   y >= 0 && y < Height;
        }
    }
}