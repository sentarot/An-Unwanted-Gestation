namespace UWG
{
    public static class GameConstants
    {
        // Tick timing
        public const float SECONDS_PER_TICK = 2.0f;
        public const int TICKS_PER_DAY = 12;

        // Win / Lose thresholds
        public const float GESTATION_WIN_THRESHOLD = 100f;
        public const float INTERVENTION_LOSE_THRESHOLD = 100f;

        // Host AI thresholds
        public const float BEDRIDDEN_DISCOMFORT_THRESHOLD = 85f;
        public const float ISOLATED_HUMILIATION_THRESHOLD = 75f;
        public const float PANIC_GESTATION_THRESHOLD = 30f;

        // Biomass generation
        public const float BASE_BIOMASS_PER_TICK = 1.0f;
        public const float HUMILIATION_BIOMASS_MULT = 0.15f;
        public const float DISCOMFORT_BIOMASS_MULT = 0.10f;

        // Gestation progression
        public const float BASE_GESTATION_PER_TICK = 0.15f;
        public const float DEFAULT_GESTATION_CAP = 100f;

        // Host stat decay/recovery per tick
        public const float DISCOMFORT_NATURAL_DECAY = 0.5f;
        public const float HUMILIATION_NATURAL_DECAY = 0.3f;
        public const float INTERVENTION_PER_DOCTOR_VISIT = 12f;

        // Task success base chances
        public const float BASE_TASK_SUCCESS_CHANCE = 0.85f;
        public const float DISCOMFORT_TASK_PENALTY_MULT = 0.005f;
        public const float HUMILIATION_TASK_PENALTY_MULT = 0.004f;
    }
}
