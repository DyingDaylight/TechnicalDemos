using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace Tests.Gameplay.FloodEscape
{
    public class PlayerControllerTests : InputTestFixture
    {
        private const float TestMoveSpeed = 7f;
        
        private Keyboard keyboard;
        private GameObject player;
        
        public override void Setup()
        {
            base.Setup();
            
            keyboard = InputSystem.AddDevice<Keyboard>();

            player = new GameObject("Player");
            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            
            PlayerController controller = player.AddComponent<PlayerController>();
            SerializedObject serializedController = new SerializedObject(controller);
            serializedController.FindProperty("moveSpeed").floatValue = TestMoveSpeed;
            serializedController.ApplyModifiedProperties();
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

            Release(keyboard.wKey);

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
            Object.DestroyImmediate(player);

            base.TearDown();
        }
    }
}