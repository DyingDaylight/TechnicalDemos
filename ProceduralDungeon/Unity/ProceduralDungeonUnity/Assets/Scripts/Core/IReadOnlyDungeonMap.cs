namespace Core
{
    public interface IReadOnlyDungeonMap
    {
        int Width { get; }
        int Height { get; }

        TileType this[int x, int y] { get; }

        bool IsInside(int x, int y)
        {
            return x >= 0 && x < Width &&
                   y >= 0 && y < Height;
        }
    }
}