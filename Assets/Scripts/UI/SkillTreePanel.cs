using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UWG.Data;

namespace UWG.UI
{
    /// <summary>
    /// Right panel: Displays the evolution skill tree. Nodes are
    /// laid out per-branch with purchase buttons. Locked/available/purchased
    /// states are visually distinguished.
    /// </summary>
    public class SkillTreePanel : MonoBehaviour
    {
        [Header("Node Prefab")]
        [SerializeField] private GameObject skillNodePrefab;

        [Header("Branch Containers")]
        [SerializeField] private Transform somaticContainer;
        [SerializeField] private Transform endocrineContainer;
        [SerializeField] private Transform temporalContainer;
        [SerializeField] private Transform classAContainer;
        [SerializeField] private Transform classBContainer;
        [SerializeField] private Transform classCContainer;

        [Header("Skill Trees")]
        [SerializeField] private SkillTreeData somaticTree;
        [SerializeField] private SkillTreeData endocrineTree;
        [SerializeField] private SkillTreeData temporalTree;

        [Header("Detail Tooltip")]
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private TextMeshProUGUI tooltipName;
        [SerializeField] private TextMeshProUGUI tooltipDesc;
        [SerializeField] private TextMeshProUGUI tooltipCost;
        [SerializeField] private Button purchaseButton;

        private SkillNodeData _selectedNode;

        private void OnEnable()
        {
            GameEvents.OnGameStarted += RebuildTree;
            GameEvents.OnSkillPurchased += _ => RefreshAllNodes();
            GameEvents.OnBiomassChanged += _ => RefreshAllNodes();
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= RebuildTree;
        }

        private void RebuildTree()
        {
            BuildBranch(somaticTree, somaticContainer);
            BuildBranch(endocrineTree, endocrineContainer);
            BuildBranch(temporalTree, temporalContainer);

            var cls = GameManager.Instance.State.SelectedClass;
            if (cls.classBranchA != null) BuildBranch(cls.classBranchA, classAContainer);
            if (cls.classBranchB != null) BuildBranch(cls.classBranchB, classBContainer);
            if (cls.classBranchC != null) BuildBranch(cls.classBranchC, classCContainer);

            HideTooltip();
        }

        private void BuildBranch(SkillTreeData tree, Transform container)
        {
            if (tree == null || container == null || skillNodePrefab == null) return;

            // Clear existing
            foreach (Transform child in container)
                Destroy(child.gameObject);

            if (tree.nodes == null) return;

            foreach (var node in tree.nodes)
            {
                var go = Instantiate(skillNodePrefab, container);
                var ui = go.GetComponent<SkillNodeUI>();
                if (ui != null)
                    ui.Setup(node, OnNodeClicked);
            }
        }

        private void OnNodeClicked(SkillNodeData node)
        {
            _selectedNode = node;
            ShowTooltip(node);
        }

        private void ShowTooltip(SkillNodeData node)
        {
            if (tooltipPanel != null) tooltipPanel.SetActive(true);
            if (tooltipName != null) tooltipName.text = node.nodeName;
            if (tooltipDesc != null) tooltipDesc.text = node.description;
            if (tooltipCost != null) tooltipCost.text = $"COST: {node.biomassCost} Biomass";

            if (purchaseButton != null)
            {
                purchaseButton.onClick.RemoveAllListeners();
                purchaseButton.onClick.AddListener(TryPurchaseSelected);
                purchaseButton.interactable = SkillTreeManager.Instance.CanPurchase(node);
            }
        }

        private void HideTooltip()
        {
            if (tooltipPanel != null) tooltipPanel.SetActive(false);
            _selectedNode = null;
        }

        private void TryPurchaseSelected()
        {
            if (_selectedNode == null) return;
            SkillTreeManager.Instance.TryPurchase(_selectedNode);
            RefreshAllNodes();
            ShowTooltip(_selectedNode);
        }

        private void RefreshAllNodes()
        {
            var allUI = GetComponentsInChildren<SkillNodeUI>();
            foreach (var ui in allUI)
                ui.Refresh();
        }
    }
}
