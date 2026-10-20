using UnityEngine;

namespace Gameplay.FloodEscape.Water
{
    public class WaterRenderer : MonoBehaviour
    {
        [SerializeField] private Sprite waterSprite;
        
        private GameObject[,] waterObjects;
        private WaterSimulation simulation;

        public void Initialize(WaterSimulation simulation)
        {
            Clear();
            
            this.simulation = simulation;

            waterObjects = new GameObject[simulation.Width, simulation.Height];

            for (int x = 0; x < simulation.Width; x++)
            {
                for (int y = 0; y < simulation.Height; y++)
                {
                    if (!simulation.CanContainWater(x, y))
                        continue;

                    waterObjects[x,y] = CreateWaterCell(x, y);
                }
            }
        }
        
        public void Refresh()
        {
            if (simulation == null || waterObjects == null)
                return;
            
            for (int x = 0; x < simulation.Width; x++)
            {
                for (int y = 0; y < simulation.Height; y++)
                {
                    GameObject waterObject = waterObjects[x, y];

                    if (waterObject == null)
                        continue;

                    float level = simulation[x, y];

                    waterObject.SetActive(level > 0f);
                    waterObject.transform.localScale = new Vector3(1f, level, 1f);
                }
            }
        }
        
        public void Clear()
        {
            ClearWaterObjects();
            waterObjects = null;
            simulation = null;
        }
        
        private GameObject CreateWaterCell(int x, int y)
        {
            GameObject waterObject = new GameObject($"Water_{x}_{y}");
            waterObject.transform.SetParent(transform, false);
            waterObject.transform.localPosition = new Vector3(x + 0.5f, y, 0f);

            SpriteRenderer spriteRenderer = waterObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = waterSprite;
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            spriteRenderer.sortingLayerName = "Water";
            
            waterObject.SetActive(false);
            return waterObject;
        }

        private void ClearWaterObjects()
        {
            if (waterObjects != null)
            {
                foreach (GameObject waterObject in waterObjects)
                {
                    if (waterObject != null)
                    {
                        waterObject.SetActive(false);
                        Destroy(waterObject);
                    }
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            if (simulation == null)
                return;

            Color[] colors =
            {
                Color.magenta,
                Color.yellow,
                Color.green,
                Color.cyan,
                Color.red,
                Color.blue
            };

            for (int i = 0; i < simulation.Fronts.Count; i++)
            {
                WaterFront front = simulation.Fronts[i];

                Gizmos.color = colors[i % colors.Length];

                foreach (Vector2Int cell in front.Cells)
                {
                    Vector3 position = new Vector3(
                        cell.x + 0.5f,
                        cell.y + 0.5f,
                        0f
                    );

                    Gizmos.DrawWireCube(position, Vector3.one * 0.8f);

#if UNITY_EDITOR
                    string opening = front.IncomingFall.HasValue
                        ? front.IncomingFall.Value.Opening.ToString()
                        : "none";

                    UnityEditor.Handles.Label(
                        position + Vector3.up * 0.4f,
                        $"F{i} E:{front.EntryPoint} Opening:{opening}"
                    );
#endif
                }
            }
        }
    }
}