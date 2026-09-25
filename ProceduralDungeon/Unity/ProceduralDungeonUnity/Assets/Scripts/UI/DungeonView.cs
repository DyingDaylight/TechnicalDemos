using UnityEngine;

namespace UI
{
    public class DungeonView : MonoBehaviour
    {
        [SerializeField] private RectTransform previewImage;

        public void FitPreview(int dungeonWidth, int dungeonHeight)
        {
            RectTransform view = (RectTransform)transform;

            float scale = Mathf.Min(
                view.rect.width / dungeonWidth,
                view.rect.height / dungeonHeight
            );

            float previewSize = Mathf.Max(dungeonWidth, dungeonHeight) * scale;

            previewImage.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                previewSize
            );

            previewImage.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                previewSize
            );
        }
    }
}
