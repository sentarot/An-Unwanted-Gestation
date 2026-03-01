using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UWG.UI
{
    /// <summary>
    /// Scrolling event log at the bottom of the dashboard.
    /// Receives narrative text from GameEvents and displays
    /// with timestamped entries.
    /// </summary>
    public class EventLogPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI logText;
        [SerializeField] private int maxLines = 50;
        [SerializeField] private UnityEngine.UI.ScrollRect scrollRect;

        private readonly LinkedList<string> _lines = new LinkedList<string>();

        private void OnEnable()
        {
            GameEvents.OnEventLogEntry += AddEntry;
        }

        private void OnDisable()
        {
            GameEvents.OnEventLogEntry -= AddEntry;
        }

        public void AddEntry(string text)
        {
            string timestamp = "";
            if (GameManager.Instance?.State != null)
            {
                var state = GameManager.Instance.State;
                timestamp = $"[D{state.CurrentDay}:{state.TickWithinDay:D2}] ";
            }

            _lines.AddLast(timestamp + text);
            while (_lines.Count > maxLines)
                _lines.RemoveFirst();

            if (logText != null)
            {
                logText.text = string.Join("\n", _lines);
            }

            // Auto-scroll to bottom
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        public void Clear()
        {
            _lines.Clear();
            if (logText != null) logText.text = "";
        }
    }
}
