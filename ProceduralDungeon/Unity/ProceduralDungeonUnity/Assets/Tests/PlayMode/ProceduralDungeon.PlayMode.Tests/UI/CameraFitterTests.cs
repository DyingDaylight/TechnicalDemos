using System.Collections;
using NUnit.Framework;
using UI;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.UI
{
    public class CameraFitterTests
    {
        private GameObject gameObject;
        private Camera camera;
        private CameraFitter cameraFitter;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject();

            camera = gameObject.AddComponent<Camera>();
            cameraFitter = gameObject.AddComponent<CameraFitter>();
        }

        [UnityTest]
        public IEnumerator Fit_MovesCameraToCenter()
        {
            camera.transform.position = new Vector3(0, 0, -10);

            // Wait one frame so Unity can run Awake() and initialize CameraFitter.
            yield return null;
            
            Vector2 center = new Vector2(4.5f, 2.5f);
            Vector2 size = new Vector2(10, 6);

            cameraFitter.Fit(center, size);
            
            Assert.AreEqual(4.5f, camera.transform.position.x);
            Assert.AreEqual(2.5f, camera.transform.position.y);
            Assert.AreEqual(-10, camera.transform.position.z);
        }
        
        // Orthographic size is half of the camera's visible height.
        // Size 4 gives a visible height of 8.
        // With aspect = 2, the visible width is 16.
        // A 10x8 map fits inside a 16x8 view, so the expected size is 4.
        [UnityTest]
        public IEnumerator Fit_WhenHeightIsLimiting_FitsByHeight()
        {
            camera.aspect = 2f;

            // Wait one frame so Unity can run Awake() and initialize CameraFitter.
            yield return null;

            cameraFitter.Fit(Vector2.zero, new Vector2(10, 8));

            Assert.AreEqual(4f, camera.orthographicSize);
        }

        [UnityTest]
        public IEnumerator Fit_WhenWidthIsLimiting_FitsByWidth()
        {
            camera.aspect = 2f;
            
            // Wait one frame so Unity can run Awake() and initialize CameraFitter.
            yield return null;
            
            cameraFitter.Fit(Vector2.one, new Vector2(20, 8));
            
            Assert.AreEqual(5f, camera.orthographicSize);
        }
        
        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameObject);
        }
    }
}