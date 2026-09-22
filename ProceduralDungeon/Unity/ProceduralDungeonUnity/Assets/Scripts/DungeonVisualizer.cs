using Core;
using Generation;
using UnityEngine;

namespace DefaultNamespace
{
    public class DungeonVisualizer : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] [Range(0, 1)] private float tileScale = 0.5f;
        
        public void Draw(DungeonMap map)
        {
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

                    SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();

                    renderer.color = map[x, y] == TileType.Wall
                        ? Color.black
                        : Color.white;
                }
            }
        }
        
    }
}