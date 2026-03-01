using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UWG.UI
{
    /// <summary>
    /// Displayed when the game ends (win or lose).
    /// Shows outcome summary and stats.
    /// </summary>
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI outcomeText;
        [SerializeField] private TextMeshProUGUI summaryText;
        [SerializeField] private Button restartButton;

        private void OnEnable()
        {
            GameEvents.OnGameEnded += ShowResult;
        }

        private void OnDisable()
        {
            GameEvents.OnGameEnded -= ShowResult;
        }

        private void Start()
        {
            if (panel != null) panel.SetActive(false);
            if (restartButton != null)
                restartButton.onClick.AddListener(Restart);
        }

        private void ShowResult(bool playerWon)
        {
            if (panel != null) panel.SetActive(true);

            var state = GameManager.Instance.State;

            if (outcomeText != null)
            {
                outcomeText.text = playerWon
                    ? "GESTATION COMPLETE"
                    : "INTERVENTION SUCCESSFUL";
            }

            if (summaryText != null)
            {
                summaryText.text =
                    $"Host: {state.SelectedHost.hostName}\n" +
                    $"Payload: {state.SelectedClass.className}\n" +
                    $"Days Survived: {state.CurrentDay}\n" +
                    $"Final Gestation: {state.Gestation:F1}%\n" +
                    $"Final Intervention: {state.InterventionMeter:F1}%\n" +
                    $"Skills Purchased: {state.PurchasedSkills.Count}\n" +
                    $"Peak Humiliation: {state.Humiliation:F0}\n" +
                    $"Peak Discomfort: {state.Discomfort:F0}";
            }
        }

        private void Restart()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }
}
