using System.Collections;
using Gameplay.FloodEscape.Gameplay;
using NUnit.Framework;
using Tests.Shared.Gameplay.FloodEscape;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace Tests.Gameplay.FloodEscape 
{
    public class DoorControllerTests : InputTestFixture
    {
        private FloodEscapeTestHelper helper;
        private Keyboard keyboard;
        
        public override void Setup()
        {
            base.Setup();
            
            helper = new FloodEscapeTestHelper();
            keyboard = InputSystem.AddDevice<Keyboard>();
        }
        
        public override void TearDown()
        {
            helper?.Dispose();
            base.TearDown();
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
        
        [UnityTest]
        public IEnumerator ClosedDoor_BlocksPlayer()
        {
            DoorController door = helper.CreateDoor();
            door.transform.position = Vector3.zero;
            door.SetOpen(false);

            PlayerController player = helper.CreatePlayer(7f);
            player.transform.position = new Vector3(-2f, 0f);

            Press(keyboard.dKey);

            yield return null;

            for (int i = 0; i < 30; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.Greater(player.transform.position.x, -2f);
            Assert.Less(player.transform.position.x, 0f);
        }
    }
}