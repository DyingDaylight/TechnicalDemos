using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tests.Gameplay.FloodEscape
{
    public class DungeonTilemapRendererTests
    {
        private GameObject gridObject;

        private Tilemap tilemap;
        private Tile barrierTile;
        private DungeonTilemapRenderer renderer;

        [SetUp]
        public void SetUp()
        {
            gridObject = new GameObject("Grid", typeof(Grid));

            GameObject tilemapObject = new GameObject("Tilemap", typeof(Tilemap));
            tilemapObject.transform.SetParent(gridObject.transform);

            tilemap = tilemapObject.GetComponent<Tilemap>();
            barrierTile = ScriptableObject.CreateInstance<Tile>();
            barrierTile.colliderType = Tile.ColliderType.Grid;

            renderer = gridObject.AddComponent<DungeonTilemapRenderer>();

            SerializedObject serializedRenderer = new SerializedObject(renderer);
            serializedRenderer.FindProperty("tilemap").objectReferenceValue = tilemap;
            serializedRenderer.FindProperty("invisibleBarrierTile").objectReferenceValue = barrierTile;
            serializedRenderer.ApplyModifiedPropertiesWithoutUndo();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gridObject);
            Object.DestroyImmediate(barrierTile);
        }
        
        [Test]
        public void PlaceBarrier_SetsInvisibleBarrierTile()
        {
            Vector2Int position = new Vector2Int(2, -1);

            renderer.PlaceBarrier(position);

            Assert.AreSame(barrierTile, tilemap.GetTile(new Vector3Int(2, -1, 0)));
        }
    }
}