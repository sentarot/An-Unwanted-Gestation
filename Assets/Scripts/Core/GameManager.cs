using UnityEngine;
using UWG.Data;

namespace UWG
{
    /// <summary>
    /// Top-level orchestrator. Wires together Host AI, Player systems,
    /// and the skill-effect pipeline. Listens to TickStart and drives
    /// the full simulation step.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Setup (assign via Inspector or SetupScreen)")]
        [SerializeField] private HostProfile startingHost;
        [SerializeField] private GestationClassData startingClass;

        [Header("Skill Trees (General)")]
        [SerializeField] private SkillTreeData generalSomatic;
        [SerializeField] private SkillTreeData generalEndocrine;
        [SerializeField] private SkillTreeData generalTemporal;

        public GameState State { get; private set; }
        public TickSystem Tick { get; private set; }

        private HostAIController _hostAI;
        private PlayerController _player;
        private SkillEffectProcessor _effectProcessor;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            Tick = GetComponent<TickSystem>();
            if (Tick == null) Tick = gameObject.AddComponent<TickSystem>();

            _hostAI = GetComponent<HostAIController>();
            if (_hostAI == null) _hostAI = gameObject.AddComponent<HostAIController>();

            _player = GetComponent<PlayerController>();
            if (_player == null) _player = gameObject.AddComponent<PlayerController>();

            _effectProcessor = new SkillEffectProcessor();
        }

        private void OnEnable() => GameEvents.OnTickStart += ProcessTick;
        private void OnDisable() => GameEvents.OnTickStart -= ProcessTick;

        /// <summary>
        /// Called from the setup screen or directly if startingHost/Class are assigned.
        /// </summary>
        public void BeginGame(HostProfile host = null, GestationClassData gestationClass = null)
        {
            State = new GameState
            {
                SelectedHost = host ?? startingHost,
                SelectedClass = gestationClass ?? startingClass
            };
            State.InitializeFromSelection();

            _hostAI.Initialize(State);
            _player.Initialize(State);

            GameEvents.FireGameStarted();
            GameEvents.FireEventLogEntry(
                $"Simulation initialized. Host: {State.SelectedHost.hostName}. " +
                $"Payload: {State.SelectedClass.className}. " +
                $"Vulnerability: {State.SelectedHost.vulnerability}."
            );

            Tick.StartTicking();
        }

        private void ProcessTick()
        {
            if (State == null || State.IsGameOver) return;

            State.CurrentTick++;

            // 1. Recalculate aggregate effects from all purchased skills
            _effectProcessor.RecalculateEffects(State);

            // 2. Generate biomass
            _player.GenerateBiomass(State);

            // 3. Advance gestation
            AdvanceGestation();

            // 4. Apply per-tick stat damage from skills
            ApplyTickDamage();

            // 5. Run Host AI (state evaluation + task execution)
            _hostAI.ProcessTick(State);

            // 6. Apply natural stat recovery
            ApplyNaturalRecovery();

            // 7. Check win/lose
            CheckEndConditions();

            // 8. Fire day boundary
            if (State.TickWithinDay == 0 && State.CurrentTick > 0)
            {
                GameEvents.FireDayAdvanced(State.CurrentDay);
                GameEvents.FireEventLogEntry($"--- Day {State.CurrentDay} ---");
            }

            GameEvents.FireTickEnd();
        }

        private void AdvanceGestation()
        {
            float rate = GameConstants.BASE_GESTATION_PER_TICK
                       * State.GestationSpeedMult
                       * (1f + State.TickGestationSpeedBonus);

            State.Gestation = Mathf.Min(State.Gestation + rate, State.GestationCap);
            GameEvents.FireGestationChanged(State.Gestation);
        }

        private void ApplyTickDamage()
        {
            float vulnMult = GameConstants.VULNERABILITY_MULTIPLIER_STANDARD;
            var vuln = State.SelectedHost.vulnerability;

            // Discomfort
            float discomfortDelta = State.TickDiscomfort;
            if (vuln == VulnerabilityType.Discomfort)
                discomfortDelta *= State.SelectedHost.vulnerabilityMultiplier;
            if (discomfortDelta > 0f)
            {
                State.Discomfort = Mathf.Clamp(State.Discomfort + discomfortDelta, 0f, 100f);
                GameEvents.FireDiscomfortChanged(State.Discomfort);
            }

            // Humiliation
            float humiliationDelta = State.TickHumiliation;
            if (vuln == VulnerabilityType.Humiliation)
                humiliationDelta *= State.SelectedHost.vulnerabilityMultiplier;
            if (humiliationDelta > 0f)
            {
                State.Humiliation = Mathf.Clamp(State.Humiliation + humiliationDelta, 0f, 100f);
                GameEvents.FireHumiliationChanged(State.Humiliation);
            }

            // Financial drain
            if (State.TickFinancialDrain > 0f)
            {
                State.FinancialResources = Mathf.Max(0f, State.FinancialResources - State.TickFinancialDrain);
            }

            // Mobility
            State.Mobility = Mathf.Clamp01(1f - State.TickMobilityReduction);
        }

        private void ApplyNaturalRecovery()
        {
            float physRecovery = State.PhysicalResistance * 0.005f;
            float mentRecovery = State.MentalDefense * 0.004f;

            State.Discomfort = Mathf.Max(0f,
                State.Discomfort - GameConstants.DISCOMFORT_NATURAL_DECAY * (1f + physRecovery));
            State.Humiliation = Mathf.Max(0f,
                State.Humiliation - GameConstants.HUMILIATION_NATURAL_DECAY * (1f + mentRecovery));

            GameEvents.FireDiscomfortChanged(State.Discomfort);
            GameEvents.FireHumiliationChanged(State.Humiliation);
        }

        private void CheckEndConditions()
        {
            if (State.Gestation >= State.GestationCap)
            {
                State.IsGameOver = true;
                State.PlayerWon = true;
                Tick.Pause();
                GameEvents.FireGameEnded(true);
                GameEvents.FireEventLogEntry("=== GESTATION COMPLETE. THE PAYLOAD HAS WON. ===");
            }
            else if (State.InterventionMeter >= GameConstants.INTERVENTION_LOSE_THRESHOLD)
            {
                State.IsGameOver = true;
                State.PlayerWon = false;
                Tick.Pause();
                GameEvents.FireGameEnded(false);
                GameEvents.FireEventLogEntry("=== INTERVENTION SUCCESSFUL. THE HOST HAS BEEN CURED. ===");
            }
        }
    }
}
