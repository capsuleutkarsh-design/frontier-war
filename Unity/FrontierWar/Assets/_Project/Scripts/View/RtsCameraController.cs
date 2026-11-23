using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace FW.View
{
    /// <summary>
    /// Fixed-angle orthographic RTS camera (07_Input_Camera_UI §1).
    /// Touch: one-finger drag pans, pinch zooms. PC testing: right/middle mouse drag pans, wheel zooms.
    /// The camera never rotates. It looks at a "focus" point on the ground (y = 0).
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class RtsCameraController : MonoBehaviour
    {
        [Header("Angle (locked look)")]
        [SerializeField] private float pitch = 50f;
        [SerializeField] private float yaw = 45f;
        [SerializeField] private float distance = 600f;

        [Header("Zoom = orthographic size (half the visible height in metres)")]
        [SerializeField] private float zoom = 60f;
        [SerializeField] private float minZoom = 20f;
        [SerializeField] private float maxZoom = 400f;
        [SerializeField] private float wheelZoomStep = 0.12f;

        [Header("Map limits (metres, ground plane x/z)")]
        [SerializeField] private Vector2 mapMin = new Vector2(-250f, -250f);
        [SerializeField] private Vector2 mapMax = new Vector2(250f, 250f);

        [Header("Pan feel")]
        [SerializeField] private float inertiaDamping = 6f;

        public Vector3 Focus { get; private set; }
        public float Zoom => zoom;

        private Camera _cam;
        private Vector3 _velocity;
        private float _lastPinchDistance;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _cam.orthographic = true;
            _cam.nearClipPlane = 1f;
            _cam.farClipPlane = distance * 2.5f;
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            Focus = new Vector3(transform.position.x, 0f, transform.position.z);
        }

        private void OnEnable() => EnhancedTouchSupport.Enable();
        private void OnDisable() => EnhancedTouchSupport.Disable();

        /// <summary>Jump the camera to a ground point (minimap taps, alerts).</summary>
        public void JumpTo(Vector3 groundPoint)
        {
            Focus = Clamp(new Vector3(groundPoint.x, 0f, groundPoint.z));
            _velocity = Vector3.zero;
        }

        private void LateUpdate()
        {
            float dt = Time.unscaledDeltaTime; // camera works while the game is paused
            Vector3 panDelta = Vector3.zero;
            bool dragging = false;

            var touches = Touch.activeTouches;
            if (touches.Count == 1)
            {
                panDelta = ScreenDeltaToGround(touches[0].delta);
                dragging = true;
                _lastPinchDistance = 0f;
            }
            else if (touches.Count >= 2)
            {
                float d = Vector2.Distance(touches[0].screenPosition, touches[1].screenPosition);
                if (_lastPinchDistance > 0f && d > 1f) zoom *= _lastPinchDistance / d;
                _lastPinchDistance = d;
                panDelta = ScreenDeltaToGround((touches[0].delta + touches[1].delta) * 0.5f);
                dragging = true;
            }
            else
            {
                _lastPinchDistance = 0f;
                var mouse = Mouse.current;
                if (mouse != null)
                {
                    if (mouse.rightButton.isPressed || mouse.middleButton.isPressed)
                    {
                        panDelta = ScreenDeltaToGround(mouse.delta.ReadValue());
                        dragging = true;
                    }
                    float wheel = mouse.scroll.ReadValue().y;
                    if (Mathf.Abs(wheel) > 0.01f) zoom *= 1f - Mathf.Sign(wheel) * wheelZoomStep;
                }
            }

            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);

            if (dragging)
            {
                Focus += panDelta;
                _velocity = dt > 0f ? panDelta / dt : Vector3.zero;
            }
            else
            {
                Focus += _velocity * dt;
                _velocity = Vector3.Lerp(_velocity, Vector3.zero, inertiaDamping * dt);
            }

            Focus = Clamp(Focus);
            _cam.orthographicSize = zoom;
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            transform.position = Focus - transform.forward * distance;
        }

        /// <summary>Convert a finger/mouse movement in pixels into a ground movement (drag the map with the finger).</summary>
        private Vector3 ScreenDeltaToGround(Vector2 screenDelta)
        {
            float metresPerPixel = 2f * zoom / Mathf.Max(1, Screen.height);
            Vector3 right = transform.right; right.y = 0f; right.Normalize();
            Vector3 forward = transform.forward; forward.y = 0f; forward.Normalize();
            // Vertical screen movement covers more ground because the camera looks down at an angle.
            float groundStretch = 1f / Mathf.Max(0.1f, Mathf.Sin(pitch * Mathf.Deg2Rad));
            return -(right * screenDelta.x + forward * screenDelta.y * groundStretch) * metresPerPixel;
        }

        private Vector3 Clamp(Vector3 p) =>
            new Vector3(Mathf.Clamp(p.x, mapMin.x, mapMax.x), 0f, Mathf.Clamp(p.z, mapMin.y, mapMax.y));
    }
}
