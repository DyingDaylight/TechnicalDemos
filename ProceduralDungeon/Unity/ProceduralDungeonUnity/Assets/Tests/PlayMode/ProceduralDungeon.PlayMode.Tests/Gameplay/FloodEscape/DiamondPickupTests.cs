using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Gameplay.FloodEscape
{
    public class DiamondPickupTests
    {
        private DiamondPickup pickup;
        private List<GameObject> createdObjects = new List<GameObject>();

        [SetUp]
        public void SetUp()
        {
            GameObject diamond = new GameObject("Diamond");
            CircleCollider2D collider = diamond.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            pickup = diamond.AddComponent<DiamondPickup>();
            
            createdObjects.Add(diamond);
        }
        
        [UnityTest]
        public IEnumerator OnTriggerEnter2D_CollisionWithPlayer_RaisesCollectedEvent()
        {
            GameObject playerObject = new GameObject("Player");
            playerObject.transform.position = new Vector3(0.5f, 0.5f, 0);
            playerObject.AddComponent<Rigidbody2D>().gravityScale = 0f;
            playerObject.AddComponent<BoxCollider2D>();
            playerObject.tag = "Player";
            createdObjects.Add(playerObject);
            
            bool wasCollected = false;
            pickup.Collected += () => wasCollected = true;
            
            yield return new WaitForFixedUpdate();
            
            Assert.IsTrue(wasCollected);
        }
        
        [UnityTest]
        public IEnumerator OnTriggerEnter2D_CollisionWithNonPlayer_DosntRaiseCollectedEvent()
        {
            GameObject playerObject = new GameObject("Enemy");
            playerObject.transform.position = new Vector3(0.5f, 0.5f, 0);
            playerObject.AddComponent<Rigidbody2D>().gravityScale = 0f;
            playerObject.AddComponent<BoxCollider2D>();
            playerObject.tag = "Untagged";
            createdObjects.Add(playerObject);
            
            bool wasCollected = false;
            pickup.Collected += () => wasCollected = true;
            
            yield return new WaitForFixedUpdate();
            
            Assert.IsFalse(wasCollected);
        }

        [UnityTest]
        public IEnumerator OnTriggerEnter2D_CollisionWithPlayer_DestroysDiamond()
        {
            GameObject playerObject = new GameObject("Player");
            playerObject.transform.position = new Vector3(0.5f, 0.5f, 0);
            playerObject.AddComponent<Rigidbody2D>().gravityScale = 0f;
            playerObject.AddComponent<BoxCollider2D>();
            playerObject.tag = "Player";
            createdObjects.Add(playerObject);

            int collectionCount = 0;
            pickup.Collected += () => collectionCount++;
            
            yield return new WaitForFixedUpdate();
            
            Assert.AreEqual(1, collectionCount);
            
            yield return null;
            
            Assert.IsTrue(pickup == null);
        }
        
        [TearDown]
        public void TearDown()
        {
            foreach (GameObject gameObject in createdObjects)
                Object.DestroyImmediate(gameObject);
            
            createdObjects.Clear();
        }
    }
}