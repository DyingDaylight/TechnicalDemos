using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(Camera))]
    public class CameraFitter : MonoBehaviour
    {
        [SerializeField] private float padding = 0f;
        
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = GetComponent<Camera>();
        }
    
        public virtual void Fit(Vector2 center, Vector2 size)
        {
            mainCamera.transform.position = new Vector3(
                center.x, center.y, mainCamera.transform.position.z);
        
            // Half the required world height, since orthographic size defines half the camera height.
            float verticalSize = size.y * 0.5f + padding;
        
            // Convert the required world width to orthographic size.
            // Orthographic size defines half the camera height, so width must be adjusted by the aspect ratio.
            float horizontalSize = size.x * 0.5f / mainCamera.aspect + padding;

            mainCamera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
        }
    }
}
