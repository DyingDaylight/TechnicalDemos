using System;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closedSprite;
    
    public event Action Entered;

    private bool isOpen;
    
    public void SetOpen(bool isOpen)
    {
        this.isOpen = isOpen;
        spriteRenderer.sprite = isOpen ? openSprite : closedSprite;
        doorCollider.isTrigger = isOpen;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOpen || !other.CompareTag("Player"))
            return;

        Entered?.Invoke();
    }
}
