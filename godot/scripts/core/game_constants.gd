extends Node

# --- Round-based timing (replaces tick system) ---
const SECONDS_PER_ROUND := 0.0  # Rounds are turn-based, no timer

# --- Win / Lose thresholds ---
const GESTATION_WIN_THRESHOLD := 100.0
const INTERVENTION_LOSE_THRESHOLD := 100.0
const DEFAULT_GESTATION_CAP := 100.0

# --- Starting resources ---
const STARTING_BIOMASS := 15.0
const STARTING_AWARENESS := 10.0

# --- Per-round income ---
const BASE_BIOMASS_PER_ROUND := 5.0
const BASE_AWARENESS_PER_ROUND := 4.0
const HUMILIATION_BIOMASS_MULT := 0.15
const DISCOMFORT_BIOMASS_MULT := 0.10

# --- Gestation progression ---
const BASE_GESTATION_PER_ROUND := 2.0

# --- Card system ---
const CARDS_TO_DRAW := 3
const MAX_HAND_SIZE := 7
const MAX_CARDS_PER_TURN := 3
const DECK_SHUFFLE_ON_EMPTY := true

# --- Host AI thresholds ---
const BEDRIDDEN_DISCOMFORT_THRESHOLD := 85.0
const ISOLATED_HUMILIATION_THRESHOLD := 75.0
const PANIC_GESTATION_THRESHOLD := 30.0

# --- Host stat decay/recovery per round ---
const DISCOMFORT_NATURAL_DECAY := 2.0
const HUMILIATION_NATURAL_DECAY := 1.5
const INTERVENTION_PER_DOCTOR_VISIT := 12.0

# --- Task success base chances ---
const BASE_TASK_SUCCESS_CHANCE := 0.85
const DISCOMFORT_TASK_PENALTY_MULT := 0.005
const HUMILIATION_TASK_PENALTY_MULT := 0.004

# --- Awareness thresholds ---
const PEAK_AWARENESS_BONUS_THRESHOLD := 50.0
