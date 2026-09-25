using Core;
using UnityEngine;

namespace Visualization
{
    public class DungeonVisualizer : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] [Range(0, 1)] private float tileScale = 0.5f;
        
        public void Draw(DungeonMap map)
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
                        new Vector3(x * tileScale, y * tileScale, 0),
                        Quaternion.identity,
                        transform);

                    tile.transform.localScale = new Vector3(tileScale, tileScale, 1);
                    tile.layer = gameObject.layer;

                    SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();

                    renderer.color = map[x, y] == TileType.Wall
                        ? Color.black
                        : Color.white;
                }
            }
        }
        
        public Vector2 GetSize(DungeonMap map)
        {
            return new Vector2(
                map.Width * tileScale,
                map.Height * tileScale
            );
        }
        
        public Vector2 GetCenter(DungeonMap map)
        {
            return new Vector2(
                (map.Width - 1) * tileScale / 2f,
                (map.Height - 1) * tileScale / 2f
            );
        }
    }
}