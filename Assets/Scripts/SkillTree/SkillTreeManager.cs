using UnityEngine;
using UWG.Data;

namespace UWG
{
    /// <summary>
    /// Handles skill node purchase logic: cost validation, prerequisite
    /// checks, one-time effect application, and visual flag activation.
    /// </summary>
    public class SkillTreeManager : MonoBehaviour
    {
        public static SkillTreeManager Instance { get; private set; }

        [Header("General Skill Trees")]
        [SerializeField] private SkillTreeData somaticMalady;
        [SerializeField] private SkillTreeData endocrineSabotage;
        [SerializeField] private SkillTreeData temporalDistortion;

        private GameState State => GameManager.Instance.State;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public bool CanPurchase(SkillNodeData node)
        {
            if (State == null || State.IsGameOver) return false;
            if (State.PurchasedSkills.Contains(node)) return false;
            if (State.Biomass < node.biomassCost) return false;

            // Check prerequisites
            if (node.prerequisites != null)
            {
                foreach (var prereq in node.prerequisites)
                {
                    if (!State.PurchasedSkills.Contains(prereq))
                        return false;
                }
            }

            return true;
        }

        public bool TryPurchase(SkillNodeData node)
        {
            if (!CanPurchase(node)) return false;

            State.Biomass -= node.biomassCost;
            State.PurchasedSkills.Add(node);

            // Apply one-time effects
            foreach (var effect in node.effects)
            {
                if (!effect.isPerTick)
                    ApplyOneTimeEffect(effect);
            }

            GameEvents.FireBiomassChanged(State.Biomass);
            GameEvents.FireSkillPurchased(node);

            if (!string.IsNullOrEmpty(node.activationNarrative))
                GameEvents.FireEventLogEntry(node.activationNarrative);

            return true;
        }

        private void ApplyOneTimeEffect(SkillEffect effect)
        {
            switch (effect.effectType)
            {
                case SkillEffectType.RaiseGestationCap:
                    State.GestationCap = Mathf.Max(State.GestationCap, effect.magnitude);
                    break;
                case SkillEffectType.ReduceIntellect:
                    State.MentalDefense = Mathf.Max(0f,
                        State.MentalDefense * (1f - effect.magnitude));
                    break;
                case SkillEffectType.ReduceSocialStanding:
                    State.SocialStanding = Mathf.Max(0f,
                        State.SocialStanding - effect.magnitude);
                    GameEvents.FireSocialStandingChanged(State.SocialStanding);
                    break;
                case SkillEffectType.DeleteAuthority:
                    State.MentalDefense *= 0.1f;
                    State.SocialStanding *= 0.5f;
                    break;
                case SkillEffectType.ReplaceTask:
                    State.IsMindControlled = true;
                    break;
            }
        }

        public SkillNodeData[] GetAvailableNodes()
        {
            var available = new System.Collections.Generic.List<SkillNodeData>();

            CollectAvailable(somaticMalady, available);
            CollectAvailable(endocrineSabotage, available);
            CollectAvailable(temporalDistortion, available);

            // Class-specific trees
            var cls = State.SelectedClass;
            if (cls.classBranchA != null) CollectAvailable(cls.classBranchA, available);
            if (cls.classBranchB != null) CollectAvailable(cls.classBranchB, available);
            if (cls.classBranchC != null) CollectAvailable(cls.classBranchC, available);

            return available.ToArray();
        }

        private void CollectAvailable(SkillTreeData tree,
            System.Collections.Generic.List<SkillNodeData> list)
        {
            if (tree == null || tree.nodes == null) return;
            foreach (var node in tree.nodes)
            {
                if (CanPurchase(node))
                    list.Add(node);
            }
        }
    }
}
