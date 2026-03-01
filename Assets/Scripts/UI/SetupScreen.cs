using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UWG.Data;

namespace UWG.UI
{
    /// <summary>
    /// Pre-game selection screen. Player chooses a Host archetype
    /// and a Gestation Class, then starts the simulation.
    /// </summary>
    public class SetupScreen : MonoBehaviour
    {
        [Header("Host Selection")]
        [SerializeField] private HostProfile[] availableHosts;
        [SerializeField] private Transform hostButtonContainer;
        [SerializeField] private GameObject hostButtonPrefab;
        [SerializeField] private Image hostPortrait;
        [SerializeField] private TextMeshProUGUI hostNameText;
        [SerializeField] private TextMeshProUGUI hostBackdropText;
        [SerializeField] private TextMeshProUGUI hostStatsText;

        [Header("Class Selection")]
        [SerializeField] private GestationClassData[] availableClasses;
        [SerializeField] private Transform classButtonContainer;
        [SerializeField] private GameObject classButtonPrefab;
        [SerializeField] private TextMeshProUGUI classNameText;
        [SerializeField] private TextMeshProUGUI classConceptText;

        [Header("Start")]
        [SerializeField] private Button startButton;
        [SerializeField] private GameObject setupPanel;
        [SerializeField] private GameObject dashboardPanel;

        private HostProfile _selectedHost;
        private GestationClassData _selectedClass;

        private void Start()
        {
            BuildHostButtons();
            BuildClassButtons();

            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartClicked);
                startButton.interactable = false;
            }

            if (dashboardPanel != null) dashboardPanel.SetActive(false);
            if (setupPanel != null) setupPanel.SetActive(true);
        }

        private void BuildHostButtons()
        {
            if (hostButtonContainer == null || hostButtonPrefab == null) return;
            foreach (var host in availableHosts)
            {
                var go = Instantiate(hostButtonPrefab, hostButtonContainer);
                var text = go.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null) text.text = host.hostName;

                var btn = go.GetComponent<Button>();
                if (btn != null)
                {
                    var captured = host;
                    btn.onClick.AddListener(() => SelectHost(captured));
                }
            }
        }

        private void BuildClassButtons()
        {
            if (classButtonContainer == null || classButtonPrefab == null) return;
            foreach (var cls in availableClasses)
            {
                var go = Instantiate(classButtonPrefab, classButtonContainer);
                var text = go.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null) text.text = cls.className;

                var btn = go.GetComponent<Button>();
                if (btn != null)
                {
                    var captured = cls;
                    btn.onClick.AddListener(() => SelectClass(captured));
                }
            }
        }

        private void SelectHost(HostProfile host)
        {
            _selectedHost = host;
            if (hostPortrait != null && host.portrait != null)
                hostPortrait.sprite = host.portrait;
            if (hostNameText != null)
                hostNameText.text = host.hostName;
            if (hostBackdropText != null)
                hostBackdropText.text = host.narrativeBackdrop;
            if (hostStatsText != null)
            {
                hostStatsText.text =
                    $"PHYS: {host.physicalResistance}  " +
                    $"MENT: {host.mentalDefense}\n" +
                    $"FIN: {host.financialResources}  " +
                    $"SOC: {host.socialStanding}\n" +
                    $"VULNERABILITY: {host.vulnerability}";
            }
            UpdateStartButton();
        }

        private void SelectClass(GestationClassData cls)
        {
            _selectedClass = cls;
            if (classNameText != null)
                classNameText.text = cls.className;
            if (classConceptText != null)
                classConceptText.text = $"{cls.concept}\n\n{cls.physicality}";
            UpdateStartButton();
        }

        private void UpdateStartButton()
        {
            if (startButton != null)
                startButton.interactable = _selectedHost != null && _selectedClass != null;
        }

        private void OnStartClicked()
        {
            if (_selectedHost == null || _selectedClass == null) return;

            if (setupPanel != null) setupPanel.SetActive(false);
            if (dashboardPanel != null) dashboardPanel.SetActive(true);

            GameManager.Instance.BeginGame(_selectedHost, _selectedClass);
        }
    }
}
