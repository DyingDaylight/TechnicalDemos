using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests.Gameplay.FloodEscape
{
    public class DoorControllerTests
    {
        private GameObject door;
        private Texture2D texture;
        private Sprite openSprite;
        private Sprite closedSprite;

        private SpriteRenderer spriteRenderer;
        private DoorController doorController;
        
        [SetUp]
        public void Setup()
        {
            door = new GameObject("Door");
            doorController = door.AddComponent<DoorController>();
            spriteRenderer = door.AddComponent<SpriteRenderer>();
            
            SerializedObject serializedObject = new SerializedObject(doorController);
            SerializedProperty spriteRendererProperty = serializedObject.FindProperty("spriteRenderer");
            spriteRendererProperty.objectReferenceValue = spriteRenderer;
            
            texture = new Texture2D(2, 2);

            openSprite = Sprite.Create(
                texture,
                new Rect(0, 0, 2, 2),
                new Vector2(0.5f, 0.5f));

            closedSprite = Sprite.Create(
                texture,
                new Rect(0, 0, 2, 2),
                new Vector2(0.5f, 0.5f));
            
            SerializedProperty openSpriteProperty = serializedObject.FindProperty("openSprite");
            openSpriteProperty.objectReferenceValue = openSprite;
            
            SerializedProperty closedSpriteProperty = serializedObject.FindProperty("closedSprite");
            closedSpriteProperty.objectReferenceValue = closedSprite;
            
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(door);
            Object.DestroyImmediate(closedSprite);
            Object.DestroyImmediate(openSprite);
            Object.DestroyImmediate(texture);
        }

        [Test]
        public void SetOpen_True_SetsOpenSprite()
        {
            doorController.SetOpen(true);
            
            Assert.AreSame(openSprite, spriteRenderer.sprite);
        }

        [Test]
        public void SetOpen_False_SetsClosedSprite()
        {
            doorController.SetOpen(false);
            
            Assert.AreSame(closedSprite, spriteRenderer.sprite);
        }
    }
}