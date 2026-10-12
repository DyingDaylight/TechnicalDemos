using Core;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonTilemapRenderer : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    
    [SerializeField] private TileBase invisibleBarrierTile;
    
    public void Draw(IReadOnlyDungeonMap map)
    {
        tilemap.ClearAllTiles();
        
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                switch (map[x, y])
                {
                    case TileType.Floor:
                        tilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                        break;
                    case TileType.Wall:
                        tilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                        break;
                }
            }
        }
    }
    
    public void PlaceBarrier(Vector2Int position)
    {
        tilemap.SetTile(new Vector3Int(position.x, position.y, 0),
            invisibleBarrierTile);
    }
}
