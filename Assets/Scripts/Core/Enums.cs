namespace UWG
{
    public enum HostState
    {
        Active,
        Isolated,
        Bedridden
    }

    public enum TaskType
    {
        // Physical tasks
        TeamTraining,
        GymSession,
        YogaStudio,
        SpaWellness,
        BusinessTravel,

        // Social tasks
        LectureHall,
        SororityMixer,
        CampusPlaza,
        CharityGala,
        HighEndDining,
        SponsoredPhotoShoot,
        JuiceBar,
        BoardroomMeeting,
        HighStakesNegotiation,
        LateNightWork,

        // Intervention tasks
        SeekDoctor,
        PrivateClinic,
        HidePregnancy,

        // Override tasks (from skill effects)
        RestAndStrokeBelly,
        PatrolForVictims
    }

    public enum TaskCategory
    {
        Physical,
        Social,
        Intellectual,
        Intervention,
        Override
    }

    public enum GestationClassType
    {
        MacrosomicMultiples,
        VanityCurse,
        SymbioticEpidemic
    }

    public enum SkillBranch
    {
        // General tree
        SomaticMalady,
        EndocrineAestheticSabotage,
        TemporalDistortion,

        // Class-specific
        ClassBranchA,
        ClassBranchB,
        ClassBranchC
    }

    public enum HostArchetype
    {
        UniversityElite,
        AdulterousTrophyWife,
        RuthlessExecutive,
        FitnessInfluencer
    }

    public enum VulnerabilityType
    {
        Humiliation,
        Discomfort,
        PregnancyBrain,
        PhysicalNodes
    }

    public enum GestationPhase
    {
        Early,    // 0-15%
        Mid,      // 15-40%
        Late,     // 41-75%
        Terminal  // 76-100%+
    }

    public enum SkillEffectType
    {
        // Stat modifiers
        AddDiscomfort,
        AddHumiliation,
        ReduceMobility,
        ReduceIntellect,
        ReduceStamina,
        DrainFinancial,
        ReduceSocialStanding,

        // Task modifiers
        TaskFailureChance,
        BlockTaskCategory,
        ReplaceTask,

        // Gestation modifiers
        IncreaseGestationSpeed,
        IncreaseGestationDensity,
        RaiseGestationCap,

        // Biomass modifiers
        IncreaseBiomassFromHumiliation,
        IncreaseBiomassFromDiscomfort,
        PassiveBiomassGeneration,

        // Special
        CancelIntervention,
        ConvertDiscomfortToEuphoria,
        InfectionZone,
        DeleteAuthority,

        // Visual flags
        VisualSweat,
        VisualWardrobeFailure,
        VisualAbdominalUndulation,
        VisualGlowingVeins,
        VisualSkinTranslucent,
        VisualPetitification,
        VisualSphericalBelly
    }
}
