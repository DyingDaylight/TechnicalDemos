using UnityEngine;

namespace Gameplay.FloodEscape
{
    public class WaterRenderer : MonoBehaviour
    {
        [SerializeField] private Sprite waterSprite;
        
        private GameObject[,] waterObjects;
        private WaterSimulation simulation;

        public void Initialize(WaterSimulation simulation)
        {
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
        
        private GameObject CreateWaterCell(int x, int y)
        {
            GameObject waterObject = new GameObject($"Water_{x}_{y}");
            waterObject.transform.SetParent(transform, false);
            waterObject.transform.localPosition = new Vector3(x + 0.5f, y, 0f);

            SpriteRenderer spriteRenderer = waterObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = waterSprite;
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            spriteRenderer.sortingLayerName = "Water";

            //waterObject.transform.localScale = new Vector3(1f, level, 1f);
            
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
    }
}