using UnityEngine;

namespace UWG.Data
{
    [CreateAssetMenu(fileName = "NewHost", menuName = "UWG/Host Profile")]
    public class HostProfile : ScriptableObject
    {
        [Header("Identity")]
        public string hostName;
        public HostArchetype archetype;
        [TextArea(2, 4)]
        public string narrativeBackdrop;

        [Header("Baseline Stats (0-100)")]
        [Range(0, 100)] public int physicalResistance;
        [Range(0, 100)] public int mentalDefense;
        [Range(0, 100)] public int financialResources;
        [Range(0, 100)] public int socialStanding;

        [Header("Schedule")]
        public ScheduleEntry[] dailySchedule;

        [Header("Vulnerability")]
        public VulnerabilityType vulnerability;
        public float vulnerabilityMultiplier = 1.5f;

        [Header("AI Behavior")]
        [Range(0, 100)] public float panicGestationThreshold = 30f;
        [Range(0, 100)] public float hideAttemptHumiliationThreshold = 40f;
        [Tooltip("How aggressively this host seeks intervention once panicked")]
        [Range(0.5f, 3f)] public float interventionDrive = 1f;

        [Header("Visuals")]
        public Sprite portrait;
        public RuntimeAnimatorController animatorController;
    }

    [System.Serializable]
    public class ScheduleEntry
    {
        public string taskLabel;
        public TaskType taskType;
        public TaskCategory category;
        [Range(0f, 1f)] public float baseSuccessChance = 0.85f;
        [Tooltip("Stat that primarily determines success")]
        public StatAffinity primaryStat;
    }

    public enum StatAffinity
    {
        Physical,
        Mental,
        Financial,
        Social
    }
}
