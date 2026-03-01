using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UWG.UI
{
    /// <summary>
    /// Pause / 1x / 2x / 4x speed controls for the simulation tick.
    /// </summary>
    public class SpeedControls : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button speed1Button;
        [SerializeField] private Button speed2Button;
        [SerializeField] private Button speed4Button;
        [SerializeField] private TextMeshProUGUI speedLabel;

        private void Start()
        {
            if (pauseButton != null) pauseButton.onClick.AddListener(OnPause);
            if (speed1Button != null) speed1Button.onClick.AddListener(() => SetSpeed(1f));
            if (speed2Button != null) speed2Button.onClick.AddListener(() => SetSpeed(2f));
            if (speed4Button != null) speed4Button.onClick.AddListener(() => SetSpeed(4f));
        }

        private void OnPause()
        {
            var tick = GameManager.Instance?.Tick;
            if (tick == null) return;

            if (tick.IsPaused)
            {
                tick.Resume();
                UpdateLabel(tick.SpeedMultiplier);
            }
            else
            {
                tick.Pause();
                UpdateLabel(0f);
            }
        }

        private void SetSpeed(float mult)
        {
            var tick = GameManager.Instance?.Tick;
            if (tick == null) return;

            tick.SetSpeed(mult);
            if (tick.IsPaused) tick.Resume();
            UpdateLabel(mult);
        }

        private void UpdateLabel(float mult)
        {
            if (speedLabel == null) return;
            speedLabel.text = mult <= 0f ? "PAUSED" : $"{mult:F0}x";
        }
    }
}
