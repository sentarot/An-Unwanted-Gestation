using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UWG.Data;

namespace UWG.UI
{
    /// <summary>
    /// Individual skill node button in the skill tree panel.
    /// Displays name, cost, and visual state (locked/available/purchased).
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class SkillNodeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;

        [Header("State Colors")]
        [SerializeField] private Color lockedColor = new Color(0.3f, 0.3f, 0.3f);
        [SerializeField] private Color availableColor = new Color(0.2f, 0.7f, 0.3f);
        [SerializeField] private Color purchasedColor = new Color(0.8f, 0.2f, 0.4f);
        [SerializeField] private Color cantAffordColor = new Color(0.6f, 0.5f, 0.1f);

        private SkillNodeData _data;
        private Button _button;
        private System.Action<SkillNodeData> _onClick;

        public SkillNodeData Data => _data;

        public void Setup(SkillNodeData data, System.Action<SkillNodeData> onClick)
        {
            _data = data;
            _onClick = onClick;
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() => _onClick?.Invoke(_data));

            if (nameText != null) nameText.text = data.nodeName;
            if (costText != null) costText.text = $"{data.biomassCost}";

            Refresh();
        }

        public void Refresh()
        {
            if (_data == null) return;
            var state = GameManager.Instance?.State;
            if (state == null) return;

            bool purchased = state.PurchasedSkills.Contains(_data);
            bool canPurchase = SkillTreeManager.Instance.CanPurchase(_data);
            bool prerequisitesMet = ArePrerequisitesMet(state);

            if (purchased)
            {
                SetVisualState(purchasedColor, false);
            }
            else if (canPurchase)
            {
                SetVisualState(availableColor, true);
            }
            else if (prerequisitesMet && state.Biomass < _data.biomassCost)
            {
                SetVisualState(cantAffordColor, false);
            }
            else
            {
                SetVisualState(lockedColor, false);
            }
        }

        private bool ArePrerequisitesMet(GameState state)
        {
            if (_data.prerequisites == null || _data.prerequisites.Length == 0)
                return true;
            foreach (var prereq in _data.prerequisites)
            {
                if (!state.PurchasedSkills.Contains(prereq))
                    return false;
            }
            return true;
        }

        private void SetVisualState(Color color, bool interactable)
        {
            if (backgroundImage != null) backgroundImage.color = color;
            if (_button != null) _button.interactable = interactable;
        }
    }
}
