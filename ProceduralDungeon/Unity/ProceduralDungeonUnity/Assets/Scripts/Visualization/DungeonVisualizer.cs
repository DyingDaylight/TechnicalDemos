using Core;
using UnityEngine;

namespace Visualization
{
    public class DungeonVisualizer : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        
        public virtual void Draw(IReadOnlyDungeonMap map)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    GameObject tile = Instantiate(
                        tilePrefab,
                        new Vector3(x, y, 0),
                        Quaternion.identity,
                        transform);

                    tile.layer = gameObject.layer;

                    SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();

                    renderer.color = map[x, y] == TileType.Wall
                        ? Color.black
                        : Color.white;
                }
            }
        }
        
        public Vector2 GetSize(IReadOnlyDungeonMap map)
        {
            return new Vector2(map.Width, map.Height);
        }
        
        public Vector2 GetCenter(IReadOnlyDungeonMap map)
        {
            return new Vector2(
                (map.Width - 1) / 2f,
                (map.Height - 1) / 2f
            );
        }
    }
}