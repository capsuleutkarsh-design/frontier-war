using UnityEngine;
using UnityEngine.UI;

namespace FW.View
{
    /// <summary>
    /// The locked pixel filter (08_Rendering_and_Art §2).
    /// The world camera renders into a small RenderTexture (screen ÷ pixelSize, Point filter).
    /// A full-screen RawImage shows it scaled up with no smoothing. UI canvases with a higher
    /// sorting order draw on top at full resolution, so text stays sharp.
    /// Put this on the world camera, next to RtsCameraController.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [DefaultExecutionOrder(100)] // runs after RtsCameraController.LateUpdate, so it snaps the final position
    public sealed class PixelFilterController : MonoBehaviour
    {
        [Tooltip("Size of one 'big pixel' in screen pixels. Locked default: 4. Setting: 3 / 4 / 5.")]
        [Range(1, 8)] [SerializeField] private int pixelSize = 4;

        [Tooltip("Snap the camera to whole low-res pixels so the image does not shimmer while panning.")]
        [SerializeField] private bool pixelSnap = true;

        public int PixelSize
        {
            get => pixelSize;
            set { pixelSize = Mathf.Clamp(value, 1, 8); Rebuild(); }
        }

        private Camera _worldCam;
        private Camera _presentCam;
        private RenderTexture _rt;
        private RawImage _image;
        private int _builtW, _builtH, _builtPixel;

        private void Awake()
        {
            _worldCam = GetComponent<Camera>();
            CreatePresenter();
            Rebuild();
        }

        private void CreatePresenter()
        {
            // A camera that renders nothing but clears the screen, so the display is never undefined.
            var camGo = new GameObject("PixelFilter_PresentCamera");
            camGo.transform.SetParent(transform, false);
            _presentCam = camGo.AddComponent<Camera>();
            _presentCam.cullingMask = 0;
            _presentCam.clearFlags = CameraClearFlags.SolidColor;
            _presentCam.backgroundColor = Color.black;
            _presentCam.depth = _worldCam.depth + 1;

            // Full-screen image that shows the low-res world.
            var canvasGo = new GameObject("PixelFilter_Canvas");
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = -1000; // HUD canvases use 0 and above, so they draw on top
            var imageGo = new GameObject("PixelFilter_Image");
            imageGo.transform.SetParent(canvasGo.transform, false);
            _image = imageGo.AddComponent<RawImage>();
            _image.raycastTarget = false;
            var rect = _image.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void Rebuild()
        {
            if (_worldCam == null) return;
            int w = Mathf.Max(1, Mathf.CeilToInt(Screen.width / (float)pixelSize));
            int h = Mathf.Max(1, Mathf.CeilToInt(Screen.height / (float)pixelSize));
            if (_rt != null && w == _builtW && h == _builtH && pixelSize == _builtPixel) return;

            if (_rt != null)
            {
                _worldCam.targetTexture = null;
                _rt.Release();
                Destroy(_rt);
            }

            _rt = new RenderTexture(w, h, 24, RenderTextureFormat.ARGB32)
            {
                name = "PixelFilter_RT",
                filterMode = FilterMode.Point,
                antiAliasing = 1,
                useMipMap = false,
            };
            _rt.Create();
            _worldCam.targetTexture = _rt;
            _image.texture = _rt;
            _builtW = w; _builtH = h; _builtPixel = pixelSize;
        }

        private void LateUpdate()
        {
            Rebuild(); // cheap check; recreates only when the screen size or pixel size changed
            if (pixelSnap && _rt != null) SnapToPixelGrid();
        }

        /// <summary>
        /// Move the camera to the nearest whole low-res pixel along its own right/up axes.
        /// ponytail: snaps only (panning steps one big pixel at a time). If panning looks too
        /// steppy, add a sub-pixel uvRect offset on the RawImage with a 1-pixel RT border.
        /// </summary>
        private void SnapToPixelGrid()
        {
            float worldPerPixel = 2f * _worldCam.orthographicSize / _rt.height;
            Transform t = transform;
            Vector3 p = t.position;
            float x = Vector3.Dot(p, t.right);
            float y = Vector3.Dot(p, t.up);
            float z = Vector3.Dot(p, t.forward);
            x = Mathf.Round(x / worldPerPixel) * worldPerPixel;
            y = Mathf.Round(y / worldPerPixel) * worldPerPixel;
            t.position = t.right * x + t.up * y + t.forward * z;
        }

        private void OnDestroy()
        {
            if (_worldCam != null) _worldCam.targetTexture = null;
            if (_rt != null) { _rt.Release(); Destroy(_rt); }
        }
    }
}
