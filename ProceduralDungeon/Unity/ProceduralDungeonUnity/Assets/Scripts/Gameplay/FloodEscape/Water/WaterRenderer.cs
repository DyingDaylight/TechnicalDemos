using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.FloodEscape.Water
{
    public class WaterRenderer : MonoBehaviour
    {
        private class FallObjects
        {
            public GameObject Left { get; }
            public GameObject Right { get; }

            public FallObjects(GameObject left, GameObject right)
            {
                Left = left;
                Right = right;
            }
        }
        
        [SerializeField] private Sprite waterSprite;
        [SerializeField] private Sprite waterFallSprite;
        [SerializeField] private float fallWidth = 0.15f;

        
        private GameObject[,] waterObjects;
        private WaterSimulation simulation;
        
        private readonly Dictionary<WaterFront, FallObjects> fallObjects = new();

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
                    
                    waterObject.SetActive(level > 0f && HasWaterSupport(x, y));
                    waterObject.transform.localScale = new Vector3(1f, level, 1f);
                }
            }
            
            RefreshFalls();
        }
        
        public void Clear()
        {
            ClearWaterObjects();
            ClearFallObjects();
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
        
        private GameObject CreateFallObject(WaterFront front, string side)
        {
            GameObject fallObject = new GameObject($"WaterFall_{front.EntryPoint.x}_{front.EntryPoint.y}_{side}");
            fallObject.transform.SetParent(transform, false);

            SpriteRenderer spriteRenderer = fallObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = waterFallSprite;
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            spriteRenderer.sortingLayerName = "Water";

            fallObject.SetActive(false);

            return fallObject;
        }

        private void RefreshFalls()
        {
            foreach (WaterFront front in simulation.Fronts)
            {
                if (!front.IncomingFall.HasValue)
                    continue;

                WaterFall fall = front.IncomingFall.Value;

                if (!fallObjects.TryGetValue(front, out FallObjects fallObject))
                {
                    fallObject = new FallObjects(
                        CreateFallObject(front, "Left"),
                        CreateFallObject(front, "Right"));
                    fallObjects.Add(front, fallObject);
                }
                
                var height = GetFallHeight(fall, out var bottom);

                if (height <= 0f)
                {
                    fallObject.Left.SetActive(false);
                    fallObject.Right.SetActive(false);
                    continue;
                }
                
                WaterFallDirection directions = simulation.GetFallDirection(
                    fall.Opening.x,
                    fall.Opening.y);
                
                bool fromLeft = (directions & WaterFallDirection.FromLeft) != 0;
                bool fromRight = (directions & WaterFallDirection.FromRight) != 0;
                
                fallObject.Left.SetActive(fromLeft);
                fallObject.Right.SetActive(fromRight);

                if (fromLeft)
                {
                    float leftX = fall.Opening.x + fallWidth * 0.5f;
                    fallObject.Left.transform.localPosition = new Vector3(
                        leftX, fall.Opening.y, 0f);
                    
                    fallObject.Left.transform.localScale = new Vector3(
                        fallWidth, height, 1f);
                }

                if (fromRight)
                {
                    float rightX = fall.Opening.x + 1f - fallWidth * 0.5f;
                    fallObject.Right.transform.localPosition = new Vector3(
                        rightX, fall.Opening.y, 0f);
                    
                    fallObject.Right.transform.localScale = new Vector3(
                        fallWidth, height, 1f);
                }
            }
        }

        private float GetFallHeight(WaterFall fall, out float bottom)
        {
            bottom = fall.Destination.y;
            float top = fall.Opening.y;
            int x = fall.Opening.x;

            for (int y = fall.Destination.y; y < fall.Opening.y; y++)
            {
                float level = simulation[x, y];

                if (level <= 0f || !HasWaterSupport(x, y))
                    break;

                bottom = y + level;

                if (level < 1f - 0.0001f)
                    break;
            }
            
            float height = top - bottom;
            return height;
        }
        
        private int GetFallDirection(Vector2Int opening)
        {
            int x = opening.x;
            int y = opening.y;

            float level = simulation[x, y];

            bool fromLeft = x > 0 &&
                            simulation.CanContainWater(x - 1, y) &&
                            simulation[x - 1, y] > level;

            bool fromRight = x < simulation.Width - 1 &&
                             simulation.CanContainWater(x + 1, y) &&
                             simulation[x + 1, y] > level;

            if (fromLeft && !fromRight)
                return -1;

            if (fromRight && !fromLeft)
                return 1;

            return 0;
        }
        
        private bool HasWaterSupport(int x, int y)
        {
            if (y == 0 || !simulation.CanContainWater(x, y - 1))
                return true;

            return simulation[x, y - 1] >= 1f - 0.0001f;
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

        private void ClearFallObjects()
        {
            foreach (FallObjects fallObject in fallObjects.Values)
            {
                if (fallObject != null)
                {
                    Destroy(fallObject.Left);
                    Destroy(fallObject.Right);
                }
            }

            fallObjects.Clear();
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

            PrintWaterLevels();

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
                    float height = front.IncomingFall.HasValue
                        ? GetFallHeight(front.IncomingFall.Value, out var bottom) 
                        : 0.0f;
                    
                    string opening = front.IncomingFall.HasValue
                        ? front.IncomingFall.Value.Opening.ToString()
                        : "none";

                    UnityEditor.Handles.Label(
                        position + Vector3.up * 0.4f,
                        $"F{i} E:{front.EntryPoint} Opening:{opening} H: {height}"
                    );
#endif
                }
            }
        }

        private void PrintWaterLevels()
        {
            for (int x = 0; x < simulation.Width; x++)
            {
                for (int y = 0; y < simulation.Height; y++)
                {
                    if (simulation[x, y] > 0f)
                    {
                        Vector3 position = new Vector3(x + 0.5f, y + 0.5f, 0f);
                        
                        UnityEditor.Handles.Label(
                            position,
                            $"{simulation[x, y]:F2}"
                        );
                    }
                }
            }
        }
    }
}