using Gameplay.FloodEscape.Gameplay;
using NUnit.Framework;
using Tests.Shared.Gameplay.FloodEscape;
using UnityEngine;

namespace Tests.Gameplay.FloodEscape
{
    public class DoorControllerTests
    {
        private Texture2D texture;
        private Sprite openSprite;
        private Sprite closedSprite;

        private SpriteRenderer spriteRenderer;
        private DoorController doorController;
        
        private FloodEscapeTestHelper helper;
        
        
        [SetUp]
        public void Setup()
        {
            helper = new FloodEscapeTestHelper();
            
            texture = new Texture2D(2, 2);
            openSprite = Sprite.Create(texture, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
            closedSprite = Sprite.Create(texture, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));

            doorController = helper.CreateDoor(openSprite, closedSprite);
            spriteRenderer = doorController.GetComponent<SpriteRenderer>();
        }

        [TearDown]
        public void TearDown()
        {
            helper.Dispose();
            
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
        
        [Test]
        public void SetOpen_True_EnablesTrigger()
        {
            doorController.SetOpen(true);

            Assert.IsTrue(doorController.GetComponent<BoxCollider2D>().isTrigger);
        }
        
        [Test]
        public void SetOpen_False_DisablesTrigger()
        {
            doorController.SetOpen(false);

            Assert.IsFalse(doorController.GetComponent<BoxCollider2D>().isTrigger);
        }
    }
}