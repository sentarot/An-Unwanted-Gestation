using UnityEngine;

namespace UWG
{
    /// <summary>
    /// Drives the simulation clock. Each tick triggers the core loop
    /// in GameManager. Supports pause and speed controls.
    /// </summary>
    public class TickSystem : MonoBehaviour
    {
        [SerializeField] private float secondsPerTick = GameConstants.SECONDS_PER_TICK;

        private float _timer;
        private bool _paused = true;
        private float _speedMultiplier = 1f;

        public bool IsPaused => _paused;
        public float SpeedMultiplier => _speedMultiplier;

        public void StartTicking() => _paused = false;
        public void Pause() => _paused = true;
        public void Resume() => _paused = false;
        public void SetSpeed(float mult) => _speedMultiplier = Mathf.Max(0.25f, mult);

        private void Update()
        {
            if (_paused) return;

            _timer += Time.deltaTime * _speedMultiplier;
            if (_timer >= secondsPerTick)
            {
                _timer -= secondsPerTick;
                GameEvents.FireTickStart();
            }
        }
    }
}
