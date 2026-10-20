namespace Core
{
    // Dungeon coordinates use [x, y]:
    // x = horizontal (width), y = vertical (height).
    public class DungeonMap : IReadOnlyDungeonMap
    {
        private readonly TileType[,] map;

        public int Width { get; }
        public int Height { get; }

        public DungeonMap(int width, int height, TileType initialTile = TileType.Wall)
        {
            map = new TileType[width, height];
            Width = width;
            Height = height;
            
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    map[x, y] = initialTile;
                }
            }
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
        
    }
}