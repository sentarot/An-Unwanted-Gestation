using System.Collections.Generic;
using UWG.Data;

namespace UWG
{
    /// <summary>
    /// Mutable runtime state for the current session.
    /// Owned by GameManager; read by all systems.
    /// </summary>
    public class GameState
    {
        // --- Setup choices ---
        public HostProfile SelectedHost { get; set; }
        public GestationClassData SelectedClass { get; set; }

        // --- Time ---
        public int CurrentTick { get; set; }
        public int CurrentDay => CurrentTick / GameConstants.TICKS_PER_DAY;
        public int TickWithinDay => CurrentTick % GameConstants.TICKS_PER_DAY;

        // --- Player resources ---
        public float Biomass { get; set; }
        public float Gestation { get; set; }
        public float GestationCap { get; set; } = GameConstants.DEFAULT_GESTATION_CAP;
        public float GestationSpeedMult { get; set; } = 1f;

        // --- Host stats (mutable runtime copies) ---
        public float PhysicalResistance { get; set; }
        public float MentalDefense { get; set; }
        public float FinancialResources { get; set; }
        public float SocialStanding { get; set; }

        // --- Host condition meters ---
        public float Humiliation { get; set; }
        public float Discomfort { get; set; }
        public float InterventionMeter { get; set; }
        public float Mobility { get; set; } = 1f;

        // --- Host AI ---
        public HostState CurrentHostState { get; set; } = HostState.Active;
        public int ScheduleIndex { get; set; }
        public bool IsMindControlled { get; set; }

        // --- Purchased skills ---
        public HashSet<SkillNodeData> PurchasedSkills { get; } = new HashSet<SkillNodeData>();

        // --- Aggregate per-tick effects (recalculated each tick from purchased skills) ---
        public float TickDiscomfort { get; set; }
        public float TickHumiliation { get; set; }
        public float TickMobilityReduction { get; set; }
        public float TickIntellectReduction { get; set; }
        public float TickStaminaReduction { get; set; }
        public float TickFinancialDrain { get; set; }
        public float TickGestationSpeedBonus { get; set; }
        public float TickBiomassBonus { get; set; }
        public float TaskFailureChanceBonus { get; set; }
        public bool CancelIntervention { get; set; }

        // --- Active visual flags ---
        public HashSet<SkillEffectType> ActiveVisualFlags { get; } = new HashSet<SkillEffectType>();

        // --- Game over ---
        public bool IsGameOver { get; set; }
        public bool PlayerWon { get; set; }

        public void InitializeFromSelection()
        {
            PhysicalResistance = SelectedHost.physicalResistance;
            MentalDefense = SelectedHost.mentalDefense;
            FinancialResources = SelectedHost.financialResources;
            SocialStanding = SelectedHost.socialStanding;
            GestationCap = SelectedClass.baseGestationCap;
            GestationSpeedMult = SelectedClass.gestationSpeedMult;
        }

        public GestationPhase GetCurrentPhase()
        {
            if (Gestation < 15f) return GestationPhase.Early;
            if (Gestation < 41f) return GestationPhase.Mid;
            if (Gestation < 76f) return GestationPhase.Late;
            return GestationPhase.Terminal;
        }
    }
}
