using System.Collections;
using Gameplay.FloodEscape.Gameplay;
using NUnit.Framework;
using Tests.Shared.Gameplay.FloodEscape;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Gameplay.FloodEscape
{
    public class DiamondPickupTests
    {
        private DiamondPickup pickup;
        private FloodEscapeTestHelper helper;
        
        [SetUp]
        public void SetUp()
        {
            helper = new FloodEscapeTestHelper();
            pickup = helper.CreateDiamond();
        }

        [TearDown]
        public void TearDown()
        {
            helper.Dispose();
        }
        
        [UnityTest]
        public IEnumerator OnTriggerEnter2D_CollisionWithPlayer_RaisesCollectedEvent()
        {
            PlayerController player = helper.CreatePlayer();
            player.transform.position = pickup.transform.position;
            
            bool wasCollected = false;
            pickup.Collected += () => wasCollected = true;
            
            yield return new WaitForFixedUpdate();
            
            Assert.IsTrue(wasCollected);
        }
        
        [UnityTest]
        public IEnumerator OnTriggerEnter2D_CollisionWithNonPlayer_DoesntRaiseCollectedEvent()
        {
            PlayerController player = helper.CreatePlayer();
            player.transform.position = pickup.transform.position;
            player.tag = "Untagged";
            
            bool wasCollected = false;
            pickup.Collected += () => wasCollected = true;
            
            yield return new WaitForFixedUpdate();
            
            Assert.IsFalse(wasCollected);
        }

        [UnityTest]
        public IEnumerator OnTriggerEnter2D_CollisionWithPlayer_DestroysDiamond()
        {
            PlayerController player = helper.CreatePlayer();
            player.transform.position = pickup.transform.position;

            int collectionCount = 0;
            pickup.Collected += () => collectionCount++;
            
            yield return new WaitForFixedUpdate();
            
            Assert.AreEqual(1, collectionCount);
            
            yield return null;
            
            Assert.IsTrue(pickup == null);
        }
    }
}