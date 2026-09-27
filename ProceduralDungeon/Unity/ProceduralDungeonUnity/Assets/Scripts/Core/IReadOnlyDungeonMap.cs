namespace Core
{
    public interface IReadOnlyDungeonMap
    {
        int Width { get; }
        int Height { get; }

        TileType this[int x, int y] { get; }
    }
}