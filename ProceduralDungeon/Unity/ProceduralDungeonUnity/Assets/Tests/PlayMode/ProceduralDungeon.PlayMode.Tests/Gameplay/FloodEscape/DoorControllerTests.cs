using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Gameplay.FloodEscape
{
    public class DoorControllerTests
    {
        [UnityTest]
        public IEnumerator OpenDoor_WhenPlayerEnters_RaisesEntered()
        {
            GameObject doorObject = new GameObject("Door");
            GameObject playerObject = new GameObject("Player");
            playerObject.tag = "Player";

            try
            {
                SpriteRenderer spriteRenderer = doorObject.AddComponent<SpriteRenderer>();
                BoxCollider2D doorCollider = doorObject.AddComponent<BoxCollider2D>();
                doorCollider.size = Vector2.one;
                doorCollider.isTrigger = true;

                DoorController door = doorObject.AddComponent<DoorController>();

                SerializedObject serializedDoor = new SerializedObject(door);
                serializedDoor.FindProperty("spriteRenderer").objectReferenceValue = spriteRenderer;
                serializedDoor.ApplyModifiedPropertiesWithoutUndo();

                Rigidbody2D playerBody = playerObject.AddComponent<Rigidbody2D>();
                playerBody.gravityScale = 0f;

                playerObject.AddComponent<BoxCollider2D>();
                PlayerController playerController =
                    playerObject.AddComponent<PlayerController>();

                playerBody.position = new Vector2(-3f, 0f);

                bool entered = false;
                door.Entered += () => entered = true;

                door.SetOpen(true);

                yield return new WaitForFixedUpdate();

                playerObject.transform.position = doorObject.transform.position;

                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();

                Assert.IsTrue(entered);
            }
            finally
            {
                Object.DestroyImmediate(doorObject);
                Object.DestroyImmediate(playerObject);
            }
        }
    }
}