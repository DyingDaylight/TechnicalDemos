namespace Core
{
    // Immutable copy of a dungeon map at a specific point in time.
    public class DungeonSnapshot : IReadOnlyDungeonMap
    {
        private readonly TileType[,] map;
        
        public int Width { get; }
        public int Height { get; }

        public TileType this[int x, int y] => map[x, y];
        
        public DungeonSnapshot(DungeonMap map)
        {
            Width = map.Width;
            Height = map.Height;

            this.map = new TileType[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    this.map[x, y] = map[x, y];
                }
            }
        }
    }
}