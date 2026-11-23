using UnityEngine;

namespace FW.UI
{
    /// <summary>
    /// Small on-screen debug text: FPS, sim tick, zoom. Grows into the full debug overlay (11_Performance §5).
    /// Uses OnGUI on purpose: debug only, never shipped in the real HUD.
    /// </summary>
    public sealed class DebugOverlay : MonoBehaviour
    {
        public System.Func<string> ExtraInfo;

        private float _fps;

        private void Update()
        {
            float current = 1f / Mathf.Max(Time.unscaledDeltaTime, 0.0001f);
            _fps = Mathf.Lerp(_fps, current, 0.1f);
        }

        private void OnGUI()
        {
            var style = new GUIStyle(GUI.skin.label) { fontSize = Mathf.Max(14, Screen.height / 45) };
            string text = $"FPS {_fps:0}";
            if (ExtraInfo != null) text += "\n" + ExtraInfo();
            GUI.Label(new Rect(10, 10, Screen.width, Screen.height), text, style);
        }
    }
}
