using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closedSprite;

    public void SetOpen(bool isOpen)
    {
        spriteRenderer.sprite = isOpen ? openSprite : closedSprite;
    }
}
