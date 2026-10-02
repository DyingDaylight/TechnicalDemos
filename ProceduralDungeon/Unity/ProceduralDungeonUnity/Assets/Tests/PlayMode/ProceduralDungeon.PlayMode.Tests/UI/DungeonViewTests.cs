using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UI;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.UI
{
    public class DungeonViewTests
    {
        private GameObject gameObject;
        private GameObject previewObject;
        private RectTransform viewRect;
        private RectTransform previewRect;
        private DungeonView dungeonView;
        
        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject("DungeonView", 
                typeof(RectTransform), typeof(DungeonView));

            viewRect = gameObject.GetComponent<RectTransform>();
            dungeonView = gameObject.GetComponent<DungeonView>();

            previewObject = new GameObject("PreviewImage",
                typeof(RectTransform));

            previewRect = previewObject.GetComponent<RectTransform>();
            previewRect.SetParent(viewRect, false);

            FieldInfo previewImageField = typeof(DungeonView)
                .GetField("previewImage",
                    BindingFlags.NonPublic | BindingFlags.Instance);

            previewImageField.SetValue(dungeonView, previewRect);
        }
        
        [TestCase(20, 10, 400f)]
        [TestCase(10, 20, 300f)]
        public void FitPreview_FitsPreviewToView(int dungeonWidth, int dungeonHeight,
            float expectedSize)
        {
            viewRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, 400);

            viewRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical, 300);

            dungeonView.FitPreview(dungeonWidth, dungeonHeight);

            Assert.AreEqual(expectedSize, previewRect.rect.width);
            Assert.AreEqual(expectedSize, previewRect.rect.height);
        }
        
        [UnityTest]
        public IEnumerator ViewSizeChanged_UpdatesPreviewSize()
        {
            viewRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, 400);

            viewRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical, 300);

            dungeonView.FitPreview(20, 10);

            Assert.AreEqual(400f, previewRect.rect.width);
            Assert.AreEqual(400f, previewRect.rect.height);

            viewRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, 200);

            // Wait one frame so Unity can process the RectTransform size change
            // and call OnRectTransformDimensionsChange().
            yield return null;

            Assert.AreEqual(200f, previewRect.rect.width);
            Assert.AreEqual(200f, previewRect.rect.height);
        }
        
        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameObject);
        }
    }
}