using System.Collections;
using Gameplay.FloodEscape.Gameplay;
using NUnit.Framework;
using Tests.Shared.Gameplay.FloodEscape;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

namespace Tests.Gameplay.FloodEscape
{
    public class PlayerControllerTests : InputTestFixture
    {
        private const float TestMoveSpeed = 7f;
        
        private Keyboard keyboard;
        private PlayerController player;
        
        private FloodEscapeTestHelper helper;
        
        public override void Setup()
        {
            base.Setup();
            
            keyboard = InputSystem.AddDevice<Keyboard>();
            
            helper = new FloodEscapeTestHelper();

            player = helper.CreatePlayer(TestMoveSpeed);
        }

        [UnityTest]
        public IEnumerator PressW_MovesPlayerUp()
        {
            Press(keyboard.wKey);

            yield return null;
            yield return new WaitForFixedUpdate();

            Rigidbody2D body = player.GetComponent<Rigidbody2D>();

            Assert.Greater(body.linearVelocity.y, 0f);
            Assert.AreEqual(0f, body.linearVelocity.x);
        }
        
        [UnityTest]
        public IEnumerator ReleaseW_StopsPlayer()
        {
            Press(keyboard.wKey);

            yield return null;
            yield return new WaitForFixedUpdate();
            
            //Release(keyboard.wKey);

            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            InputSystem.Update();
            
            yield return null;
            yield return new WaitForFixedUpdate();

            Rigidbody2D body = player.GetComponent<Rigidbody2D>();

            Assert.AreEqual(Vector2.zero, body.linearVelocity);
        }
        
        [UnityTest]
        public IEnumerator PressWAndD_DoesNotIncreaseMovementSpeed()
        {
            Press(keyboard.wKey);
            Press(keyboard.dKey);

            yield return null;
            yield return new WaitForFixedUpdate();

            Rigidbody2D body = player.GetComponent<Rigidbody2D>();

            Assert.AreEqual(TestMoveSpeed, body.linearVelocity.magnitude, 0.01f);
        }
        
        public override void TearDown()
        {
            helper.Dispose();
            base.TearDown();
        }
    }
}