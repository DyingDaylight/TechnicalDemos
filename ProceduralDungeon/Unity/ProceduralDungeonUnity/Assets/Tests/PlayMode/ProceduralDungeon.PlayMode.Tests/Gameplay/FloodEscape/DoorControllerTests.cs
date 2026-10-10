using System.Collections;
using NUnit.Framework;
using Tests.Shared.Gameplay.FloodEscape;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Gameplay.FloodEscape
{
    public class DoorControllerTests
    {
        private FloodEscapeTestHelper helper;
        
        [SetUp]
        public void SetUp()
        {
            helper = new FloodEscapeTestHelper();
        }

        [TearDown]
        public void TearDown()
        {
            helper.Dispose();
        }
        
        [UnityTest]
        public IEnumerator OpenDoor_WhenPlayerEnters_RaisesEntered()
        {
            DoorController door = helper.CreateDoor();
            PlayerController player = helper.CreatePlayer();

            player.transform.position = new Vector2(-3f, 0f);

            bool entered = false;
            door.Entered += () => entered = true;

            door.SetOpen(true);

            yield return new WaitForFixedUpdate();

            player.transform.position = door.transform.position;

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            Assert.IsTrue(entered);
        }
    }
}