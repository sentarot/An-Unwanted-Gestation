class_name GameState
extends RefCounted

# --- Setup choices ---
var selected_host: HostProfile
var selected_class: GestationClassData

# --- Time ---
var current_tick: int = 0
var current_day: int:
	get: return current_tick / GameConstants.TICKS_PER_DAY
var tick_within_day: int:
	get: return current_tick % GameConstants.TICKS_PER_DAY

# --- Player resources ---
var biomass: float = 0.0
var gestation: float = 0.0
var gestation_cap: float = GameConstants.DEFAULT_GESTATION_CAP
var gestation_speed_mult: float = 1.0

# --- Host stats (mutable runtime copies) ---
var physical_resistance: float = 0.0
var mental_defense: float = 0.0
var financial_resources: float = 0.0
var social_standing: float = 0.0

# --- Host condition meters ---
var humiliation: float = 0.0
var discomfort: float = 0.0
var intervention_meter: float = 0.0
var mobility: float = 1.0

# --- Host AI ---
var current_host_state: int = Enums.HostState.ACTIVE
var schedule_index: int = 0
var is_mind_controlled: bool = false

# --- Purchased skills ---
var purchased_skills: Array[SkillNodeData] = []

# --- Aggregate per-tick effects (recalculated each tick) ---
var tick_discomfort: float = 0.0
var tick_humiliation: float = 0.0
var tick_mobility_reduction: float = 0.0
var tick_intellect_reduction: float = 0.0
var tick_stamina_reduction: float = 0.0
var tick_financial_drain: float = 0.0
var tick_gestation_speed_bonus: float = 0.0
var tick_biomass_bonus: float = 0.0
var task_failure_chance_bonus: float = 0.0
var cancel_intervention: bool = false

# --- Active visual flags ---
var active_visual_flags: Array[int] = []

# --- Game over ---
var is_game_over: bool = false
var player_won: bool = false


func initialize_from_selection() -> void:
	physical_resistance = selected_host.physical_resistance
	mental_defense = selected_host.mental_defense
	financial_resources = selected_host.financial_resources
	social_standing = selected_host.social_standing
	gestation_cap = selected_class.base_gestation_cap
	gestation_speed_mult = selected_class.gestation_speed_mult


func get_current_phase() -> int:
	if gestation < 15.0:
		return Enums.GestationPhase.EARLY
	if gestation < 41.0:
		return Enums.GestationPhase.MID
	if gestation < 76.0:
		return Enums.GestationPhase.LATE
	return Enums.GestationPhase.TERMINAL


func has_skill(node: SkillNodeData) -> bool:
	return node in purchased_skills


func has_visual_flag(flag: int) -> bool:
	return flag in active_visual_flags
