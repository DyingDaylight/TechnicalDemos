using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(Camera))]
    public class CameraFitter : MonoBehaviour
    {
        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }
    
        public void Fit(Vector2 center, Vector2 size)
        {
            _camera.transform.position = new Vector3(
                center.x, center.y, _camera.transform.position.z);
        
            // Half the required world height, since orthographic size defines half the camera height.
            float verticalSize = size.y / 2f;
        
            // Convert the required world width to orthographic size.
            // Orthographic size defines half the camera height, so width must be adjusted by the aspect ratio.
            float horizontalSize = size.x / (2f * _camera.aspect);

            _camera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
        }
    }
}
