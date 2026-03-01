using UnityEngine;

namespace UWG.Data
{
    [CreateAssetMenu(fileName = "NewGestationClass", menuName = "UWG/Gestation Class")]
    public class GestationClassData : ScriptableObject
    {
        [Header("Identity")]
        public string className;
        public GestationClassType classType;
        [TextArea(2, 4)]
        public string concept;
        [TextArea(2, 4)]
        public string physicality;

        [Header("Base Modifiers")]
        [Tooltip("Multiplier to base gestation speed")]
        public float gestationSpeedMult = 1f;
        [Tooltip("Starting gestation cap before Temporal Distortion upgrades")]
        public float baseGestationCap = 100f;

        [Header("Skill Trees")]
        public SkillTreeData classBranchA;
        public SkillTreeData classBranchB;
        public SkillTreeData classBranchC;

        [Header("Visual Descriptors per Phase")]
        public PhaseVisualDescriptor earlyPhase;
        public PhaseVisualDescriptor midPhase;
        public PhaseVisualDescriptor latePhase;
        public PhaseVisualDescriptor terminalPhase;
    }

    [System.Serializable]
    public class PhaseVisualDescriptor
    {
        public GestationPhase phase;
        [Range(0f, 300f)] public float gestationMin;
        [Range(0f, 300f)] public float gestationMax;
        [TextArea(2, 5)]
        public string visualDescription;
        [Tooltip("Abdominal scale multiplier at this phase")]
        public float bellyScaleMult = 1f;
        [Tooltip("Posture curvature (0 = upright, 1 = full swayback)")]
        [Range(0f, 1f)] public float postureSwayback;
        [Tooltip("Movement speed multiplier (1 = normal)")]
        [Range(0.1f, 1f)] public float moveSpeedMult = 1f;
    }
}
