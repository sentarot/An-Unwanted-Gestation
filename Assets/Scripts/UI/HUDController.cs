using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UWG.UI
{
    /// <summary>
    /// Top bar HUD: Biomass counter, Gestation progress (win bar),
    /// and Intervention progress (lose bar). Updates reactively via events.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Biomass")]
        [SerializeField] private TextMeshProUGUI biomassText;

        [Header("Gestation (Win Bar)")]
        [SerializeField] private Slider gestationSlider;
        [SerializeField] private TextMeshProUGUI gestationLabel;
        [SerializeField] private Image gestationFill;

        [Header("Intervention (Lose Bar)")]
        [SerializeField] private Slider interventionSlider;
        [SerializeField] private TextMeshProUGUI interventionLabel;
        [SerializeField] private Image interventionFill;

        [Header("Day Counter")]
        [SerializeField] private TextMeshProUGUI dayText;

        [Header("Colors")]
        [SerializeField] private Color gestationColor = new Color(0.8f, 0.2f, 0.4f);
        [SerializeField] private Color interventionColor = new Color(0.2f, 0.6f, 0.9f);
        [SerializeField] private Color dangerColor = new Color(0.9f, 0.1f, 0.1f);

        private void OnEnable()
        {
            GameEvents.OnBiomassChanged += UpdateBiomass;
            GameEvents.OnGestationChanged += UpdateGestation;
            GameEvents.OnInterventionChanged += UpdateIntervention;
            GameEvents.OnDayAdvanced += UpdateDay;
            GameEvents.OnGameStarted += OnGameStarted;
        }

        private void OnDisable()
        {
            GameEvents.OnBiomassChanged -= UpdateBiomass;
            GameEvents.OnGestationChanged -= UpdateGestation;
            GameEvents.OnInterventionChanged -= UpdateIntervention;
            GameEvents.OnDayAdvanced -= UpdateDay;
            GameEvents.OnGameStarted -= OnGameStarted;
        }

        private void OnGameStarted()
        {
            var state = GameManager.Instance.State;
            if (gestationSlider != null)
                gestationSlider.maxValue = state.GestationCap;
            if (interventionSlider != null)
                interventionSlider.maxValue = GameConstants.INTERVENTION_LOSE_THRESHOLD;

            UpdateBiomass(0f);
            UpdateGestation(0f);
            UpdateIntervention(0f);
            UpdateDay(0);
        }

        private void UpdateBiomass(float value)
        {
            if (biomassText != null)
                biomassText.text = $"BIOMASS: {value:F1}";
        }

        private void UpdateGestation(float value)
        {
            if (gestationSlider != null)
                gestationSlider.value = value;
            if (gestationLabel != null)
                gestationLabel.text = $"GESTATION: {value:F1}%";
            if (gestationFill != null)
            {
                float ratio = value / GameManager.Instance.State.GestationCap;
                gestationFill.color = ratio > 0.75f ? dangerColor : gestationColor;
            }
        }

        private void UpdateIntervention(float value)
        {
            if (interventionSlider != null)
                interventionSlider.value = value;
            if (interventionLabel != null)
                interventionLabel.text = $"INTERVENTION: {value:F1}%";
            if (interventionFill != null)
            {
                float ratio = value / GameConstants.INTERVENTION_LOSE_THRESHOLD;
                interventionFill.color = ratio > 0.6f ? dangerColor : interventionColor;
            }
        }

        private void UpdateDay(int day)
        {
            if (dayText != null)
                dayText.text = $"DAY {day}";
        }
    }
}
