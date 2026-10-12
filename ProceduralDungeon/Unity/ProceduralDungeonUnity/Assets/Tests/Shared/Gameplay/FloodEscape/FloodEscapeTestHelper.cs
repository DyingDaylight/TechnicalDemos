using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tests.Shared.Gameplay.FloodEscape
{
    public class FloodEscapeTestHelper : IDisposable
    {
        private readonly List<GameObject> createdObjects = new();

        public DoorController CreateDoor(Sprite openSprite = null, Sprite closedSprite = null)
        {
            GameObject doorObject = new GameObject("TestDoor");
            createdObjects.Add(doorObject);

            SpriteRenderer renderer = doorObject.AddComponent<SpriteRenderer>();

            BoxCollider2D collider = doorObject.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;

            DoorController door = doorObject.AddComponent<DoorController>();
            
            SerializedObject serializedDoor = new SerializedObject(door);
            serializedDoor.FindProperty("spriteRenderer").objectReferenceValue = renderer;
            serializedDoor.FindProperty("doorCollider").objectReferenceValue = collider;
            serializedDoor.FindProperty("openSprite").objectReferenceValue = openSprite;
            serializedDoor.FindProperty("closedSprite").objectReferenceValue = closedSprite;
            serializedDoor.ApplyModifiedPropertiesWithoutUndo();

            door.SetOpen(false);
            
            return door;
        }

        public PlayerController CreatePlayer(float? moveSpeed = null)
        {
            GameObject playerObject = new GameObject("TestPlayer");
            createdObjects.Add(playerObject);
            
            playerObject.tag = "Player";
            
            Rigidbody2D playerBody = playerObject.AddComponent<Rigidbody2D>();
            playerBody.gravityScale = 0f;

            BoxCollider2D collider = playerObject.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;

            PlayerController player = playerObject.AddComponent<PlayerController>();
            
            if (moveSpeed.HasValue)
            {
                SerializedObject serializedPlayer = new SerializedObject(player);
                serializedPlayer.FindProperty("moveSpeed").floatValue = moveSpeed.Value;
                serializedPlayer.ApplyModifiedPropertiesWithoutUndo();
            }

            return player;
        }
        
        public DiamondPickup CreateDiamond()
        {
            GameObject diamondObject = new GameObject("TestDiamond");
            createdObjects.Add(diamondObject);

            CircleCollider2D collider = diamondObject.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            collider.isTrigger = true;

            return diamondObject.AddComponent<DiamondPickup>();
        }
        
        public void Dispose()
        {
            foreach (GameObject obj in createdObjects)
            {
                if (obj != null)
                    UnityEngine.Object.DestroyImmediate(obj);
            }

            createdObjects.Clear();
        }
    }
}