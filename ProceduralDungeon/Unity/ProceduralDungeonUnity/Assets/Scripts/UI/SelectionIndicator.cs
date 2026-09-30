using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionIndicator : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private GameObject focusBorder;

    public void OnSelect(BaseEventData eventData)
    {
        focusBorder.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        focusBorder.SetActive(false);
    }
}
