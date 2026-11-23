using FW.Core;
using FW.Sim;
using FW.UI;
using FW.View;
using UnityEngine;

namespace FW.Game
{
    /// <summary>
    /// The only MonoBehaviour that drives the game (00_Architecture §2).
    /// Runs the simulation at a fixed 20 ticks per second, whatever the frame rate.
    /// </summary>
    public sealed class GameRunner : MonoBehaviour
    {
        [Tooltip("0 = paused, 0.5, 1, 1.5, 2")]
        [SerializeField] private float gameSpeed = 1f;

        [Tooltip("Safety: never run more than this many ticks in one frame (stops a spiral after a hitch).")]
        [SerializeField] private int maxTicksPerFrame = 5;

        [SerializeField] private RtsCameraController cameraController;

        public Simulation Sim { get; private set; }

        /// <summary>0..1, how far we are between the last tick and the next. Views use it to interpolate.</summary>
        public float Alpha { get; private set; }

        public float GameSpeed
        {
            get => gameSpeed;
            set => gameSpeed = Mathf.Clamp(value, 0f, 4f);
        }

        private float _accumulator;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Sim = new Simulation();
            // Systems are added here in the fixed order from 00_Architecture §2 as they are built (M1+).

            var overlay = gameObject.AddComponent<DebugOverlay>();
            overlay.ExtraInfo = () =>
                $"Tick {Sim.State.Tick}  Speed x{gameSpeed:0.#}" +
                (cameraController != null ? $"  Zoom {cameraController.Zoom:0}" : "");
        }

        private void Update()
        {
            _accumulator += Time.deltaTime * gameSpeed;
            int ticks = 0;
            while (_accumulator >= SimConstants.TickDt && ticks < maxTicksPerFrame)
            {
                Sim.Tick();
                _accumulator -= SimConstants.TickDt;
                ticks++;
            }
            if (ticks == maxTicksPerFrame) _accumulator = 0f; // drop the backlog instead of freezing
            Alpha = _accumulator / SimConstants.TickDt;
        }
    }
}
