using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UWG.UI
{
    /// <summary>
    /// Left panel: Host schedule display, current task status,
    /// and live readouts of Humiliation, Discomfort, Social Standing.
    /// </summary>
    public class HostVitalsPanel : MonoBehaviour
    {
        [Header("Host Identity")]
        [SerializeField] private TextMeshProUGUI hostNameText;
        [SerializeField] private TextMeshProUGUI archetypeText;

        [Header("Current Task")]
        [SerializeField] private TextMeshProUGUI currentTaskText;
        [SerializeField] private Image taskStatusIcon;
        [SerializeField] private Color successColor = Color.green;
        [SerializeField] private Color failColor = Color.red;

        [Header("Stat Bars")]
        [SerializeField] private Slider humiliationBar;
        [SerializeField] private TextMeshProUGUI humiliationText;
        [SerializeField] private Slider discomfortBar;
        [SerializeField] private TextMeshProUGUI discomfortText;
        [SerializeField] private Slider socialBar;
        [SerializeField] private TextMeshProUGUI socialText;

        [Header("Host State")]
        [SerializeField] private TextMeshProUGUI stateText;

        private void OnEnable()
        {
            GameEvents.OnGameStarted += OnGameStarted;
            GameEvents.OnHumiliationChanged += UpdateHumiliation;
            GameEvents.OnDiscomfortChanged += UpdateDiscomfort;
            GameEvents.OnSocialStandingChanged += UpdateSocial;
            GameEvents.OnHostTaskResolved += OnTaskResolved;
            GameEvents.OnHostStateChanged += OnStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= OnGameStarted;
            GameEvents.OnHumiliationChanged -= UpdateHumiliation;
            GameEvents.OnDiscomfortChanged -= UpdateDiscomfort;
            GameEvents.OnSocialStandingChanged -= UpdateSocial;
            GameEvents.OnHostTaskResolved -= OnTaskResolved;
            GameEvents.OnHostStateChanged -= OnStateChanged;
        }

        private void OnGameStarted()
        {
            var state = GameManager.Instance.State;
            if (hostNameText != null)
                hostNameText.text = state.SelectedHost.hostName;
            if (archetypeText != null)
                archetypeText.text = state.SelectedHost.archetype.ToString();

            UpdateHumiliation(0f);
            UpdateDiscomfort(0f);
            UpdateSocial(state.SocialStanding);
        }

        private void UpdateHumiliation(float value)
        {
            if (humiliationBar != null) humiliationBar.value = value / 100f;
            if (humiliationText != null) humiliationText.text = $"HUMILIATION: {value:F0}";
        }

        private void UpdateDiscomfort(float value)
        {
            if (discomfortBar != null) discomfortBar.value = value / 100f;
            if (discomfortText != null) discomfortText.text = $"DISCOMFORT: {value:F0}";
        }

        private void UpdateSocial(float value)
        {
            if (socialBar != null) socialBar.value = value / 100f;
            if (socialText != null) socialText.text = $"SOCIAL: {value:F0}";
        }

        private void OnTaskResolved(TaskType task, bool succeeded)
        {
            if (currentTaskText != null)
            {
                string status = succeeded ? "SUCCESS" : "FAILED";
                currentTaskText.text = $"{task.ToString().Replace("_", " ")} — {status}";
            }
            if (taskStatusIcon != null)
                taskStatusIcon.color = succeeded ? successColor : failColor;
        }

        private void OnStateChanged(HostState oldState, HostState newState)
        {
            if (stateText != null)
            {
                stateText.text = newState switch
                {
                    HostState.Active => "STATUS: ACTIVE",
                    HostState.Isolated => "STATUS: ISOLATED",
                    HostState.Bedridden => "STATUS: BEDRIDDEN",
                    _ => "STATUS: UNKNOWN"
                };
            }
        }
    }
}
