using System.Text;
using Core;

namespace DebugTools
{
    public class DungeonDebug
    {
        public static string ToString(DungeonMap map)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int y = map.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    switch (map[x, y])
                    {
                        case TileType.Floor:
                            stringBuilder.Append('.');
                            break;
                        case TileType.Wall:
                            stringBuilder.Append('#');
                            break;
                    }
                }
                stringBuilder.Append("\n");
            }
            
            return stringBuilder.ToString();
        }
    }
}