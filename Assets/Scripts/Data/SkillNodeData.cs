using UnityEngine;

namespace UWG.Data
{
    [CreateAssetMenu(fileName = "NewSkillNode", menuName = "UWG/Skill Node")]
    public class SkillNodeData : ScriptableObject
    {
        [Header("Identity")]
        public string nodeName;
        public int tier;
        [TextArea(2, 4)]
        public string description;

        [Header("Cost")]
        public int biomassCost;

        [Header("Prerequisites")]
        [Tooltip("Nodes that must be unlocked before this one")]
        public SkillNodeData[] prerequisites;

        [Header("Effects")]
        public SkillEffect[] effects;

        [Header("Narrative")]
        [TextArea(2, 4)]
        public string activationNarrative;
        [TextArea(2, 4)]
        public string tickNarrative;
    }

    [System.Serializable]
    public class SkillEffect
    {
        public SkillEffectType effectType;
        public float magnitude;
        [Tooltip("If true, applies every tick. If false, applied once on purchase.")]
        public bool isPerTick;
        [Tooltip("Optional: task type affected by this effect")]
        public TaskType affectedTask;
        [Tooltip("Optional: task category affected by this effect")]
        public TaskCategory affectedCategory;
    }
}
