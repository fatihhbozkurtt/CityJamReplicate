using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class ZoomAndDragManager : MonoBehaviour
    {
        [Header("Zoom")] public float zoomSpeed; // Speed of zooming
        public float minDistance; // Minimum distance the camera can move forward
        public float maxDistance; // Maximum distance the camera can move backward
        public Transform camHolderTransform; // The transform of the actual game camera
        public float zoomThreshold;

        [Header("Drag")] public float dragSpeed; // Speed for dragging the camera
        private Vector3 lastTouchPosition; // To store the last touch position for dragging
        private bool isDragging; // Flag to track if dragging is happening

        [Header("Test")] public Button incrementSpeedButton;
        public Button decrementSpeedButton;
        public TextMeshProUGUI speedTxt;
        public Button minDistancePlus;
        public Button minDistanceMinus;
        public TextMeshProUGUI zoomThresholdTxt;

        private void Start()
        {
#if UNITY_EDITOR
            zoomSpeed = 20f;
#endif
            #region MOBILE TEST

            // speedTxt.text = zoomSpeed.ToString();
            // incrementSpeedButton.onClick.AddListener(() =>
            // {
            //     dragSpeed += 0.01f;
            //     speedTxt.text = dragSpeed.ToString();
            // });
            //
            // decrementSpeedButton.onClick.AddListener(() =>
            // {
            //     dragSpeed -= 0.01f;
            //     speedTxt.text = dragSpeed.ToString();
            // });
            //
            // minDistancePlus.onClick.AddListener((() =>
            // {
            //     zoomThreshold += 0.01f;
            //     zoomThresholdTxt.text = zoomThreshold.ToString();
            // }));
            //
            // minDistanceMinus.onClick.AddListener((() =>
            // {
            //     zoomThreshold -= 0.01f;
            //     zoomThresholdTxt.text = zoomThreshold.ToString();
            // }));

            #endregion
        }

        void Update()
        {
#if UNITY_EDITOR
            float scrollData = Input.GetAxis("Mouse ScrollWheel");
            ZoomCamera(scrollData * 10, zoomSpeed); // Apply zoom based on scroll input

            // Handle dragging with the mouse in the editor
            if (Input.GetMouseButtonDown(0)) // Left mouse button press
            {
                StartDragging(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0)) // Mouse drag
            {
                DragCamera(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0)) // Left mouse button release
            {
                EndDragging();
            }
#else
            // On mobile, use pinch-to-zoom and drag logic
            if (Input.touchCount == 2)
            {
                // Pinch-to-zoom logic
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
                Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

                float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
                float touchDeltaMag = (touch1.position - touch2.position).magnitude;

                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                ZoomCamera(deltaMagnitudeDiff, zoomSpeed);
            }
            else if (Input.touchCount == 1) // One-finger drag logic
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    StartDragging(touch.position);
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    DragCamera(touch.position);
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    EndDragging();
                }
            }
#endif
        }

        void ZoomCamera(float deltaMagnitudeDiff, float speed)
        {
            if (Mathf.Abs(deltaMagnitudeDiff) <= zoomThreshold) return;

            // Move the camera along the parent’s forward axis (which accounts for its rotation)
            Vector3 movement = camHolderTransform.forward * (deltaMagnitudeDiff * speed);

            // Calculate the target position by adding the movement to the current position
            Vector3 targetPosition = camHolderTransform.position + movement;

            // Calculate the distance from the camera to the target (assuming target is at (0, 0, 0))
            float distance = Vector3.Distance(targetPosition, Vector3.zero); // Adjust target as needed

            // Clamp the distance to stay within the min and max distance limits
            if (distance > minDistance && distance < maxDistance)
            {
                // Smoothly interpolate between the current position and the target position
                camHolderTransform.position =
                    Vector3.Lerp(camHolderTransform.position, targetPosition, Time.deltaTime * speed);
            }
        }

        void StartDragging(Vector3 touchPosition)
        {
            // Record the initial touch position for dragging
            lastTouchPosition = touchPosition;
            isDragging = true;
        }

        void DragCamera(Vector3 touchPosition)
        {
            if (!isDragging) return;

            // Calculate the touch delta movement
            Vector3 touchDelta = (touchPosition - lastTouchPosition) * dragSpeed;

            // Move the camera holder based on the touch delta
            camHolderTransform.Translate(-touchDelta.x, -touchDelta.y, 0); // Moving along X and Y axes (pan)

            // Update the last touch position
            lastTouchPosition = touchPosition;
        }

        void EndDragging()
        {
            // Stop dragging
            isDragging = false;
        }
    }
}