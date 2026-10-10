using System;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closedSprite;
    
    public event Action Entered;

    private bool isOpen;
    
    public void SetOpen(bool isOpen)
    {
        this.isOpen = isOpen;
        spriteRenderer.sprite = isOpen ? openSprite : closedSprite;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered " + other.name);
        
        if (!isOpen || !other.CompareTag("Player"))
            return;

        Entered?.Invoke();
    }
}
