using System;

namespace UWG
{
    /// <summary>
    /// Central event bus for decoupled communication between game systems.
    /// UI, audio, and visual systems subscribe; simulation systems publish.
    /// </summary>
    public static class GameEvents
    {
        // --- Tick ---
        public static event Action OnTickStart;
        public static event Action OnTickEnd;
        public static event Action<int> OnDayAdvanced; // day number

        // --- Game flow ---
        public static event Action OnGameStarted;
        public static event Action<bool> OnGameEnded; // true = player wins

        // --- Host ---
        public static event Action<HostState, HostState> OnHostStateChanged; // old, new
        public static event Action<TaskType, bool> OnHostTaskResolved; // task, succeeded
        public static event Action<float> OnHumiliationChanged; // new value
        public static event Action<float> OnDiscomfortChanged;
        public static event Action<float> OnInterventionChanged;
        public static event Action<float> OnSocialStandingChanged;

        // --- Player ---
        public static event Action<float> OnBiomassChanged;
        public static event Action<float> OnGestationChanged;
        public static event Action<Data.SkillNodeData> OnSkillPurchased;

        // --- Narrative ---
        public static event Action<string> OnEventLogEntry; // narrative text

        // Fire helpers
        public static void FireTickStart() => OnTickStart?.Invoke();
        public static void FireTickEnd() => OnTickEnd?.Invoke();
        public static void FireDayAdvanced(int day) => OnDayAdvanced?.Invoke(day);
        public static void FireGameStarted() => OnGameStarted?.Invoke();
        public static void FireGameEnded(bool playerWins) => OnGameEnded?.Invoke(playerWins);

        public static void FireHostStateChanged(HostState oldState, HostState newState)
            => OnHostStateChanged?.Invoke(oldState, newState);
        public static void FireHostTaskResolved(TaskType task, bool succeeded)
            => OnHostTaskResolved?.Invoke(task, succeeded);
        public static void FireHumiliationChanged(float v) => OnHumiliationChanged?.Invoke(v);
        public static void FireDiscomfortChanged(float v) => OnDiscomfortChanged?.Invoke(v);
        public static void FireInterventionChanged(float v) => OnInterventionChanged?.Invoke(v);
        public static void FireSocialStandingChanged(float v) => OnSocialStandingChanged?.Invoke(v);
        public static void FireBiomassChanged(float v) => OnBiomassChanged?.Invoke(v);
        public static void FireGestationChanged(float v) => OnGestationChanged?.Invoke(v);
        public static void FireSkillPurchased(Data.SkillNodeData node) => OnSkillPurchased?.Invoke(node);
        public static void FireEventLogEntry(string text) => OnEventLogEntry?.Invoke(text);
    }
}
