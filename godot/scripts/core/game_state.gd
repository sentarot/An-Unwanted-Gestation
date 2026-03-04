class_name GameState
extends RefCounted

# --- Setup choices ---
var selected_host: HostProfile
var selected_class: GestationClassData
var pregnancy_player_type: int = Enums.PlayerType.HUMAN
var host_player_type: int = Enums.PlayerType.AI

# --- Round tracking ---
var current_round: int = 0
var current_phase: int = Enums.RoundPhase.DRAW

# --- Pregnancy resources ---
var biomass: float = 0.0
var gestation: float = 0.0
var gestation_cap: float = GameConstants.DEFAULT_GESTATION_CAP
var gestation_speed_mult: float = 1.0

# --- Host resources ---
var awareness: float = 0.0
var peak_awareness: float = 0.0

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

# --- Purchased skills (both sides) ---
var purchased_skills: Array[SkillNodeData] = []
var purchased_host_skills: Array[SkillNodeData] = []

# --- Decks ---
var pregnancy_deck: Deck = Deck.new()
var host_deck: Deck = Deck.new()

# --- Cards played this round ---
var pregnancy_cards_played: Array[CardData] = []
var host_cards_played: Array[CardData] = []

# --- Per-round aggregate effects (recalculated each resolution) ---
var round_discomfort: float = 0.0
var round_humiliation: float = 0.0
var round_mobility_reduction: float = 0.0
var round_intellect_reduction: float = 0.0
var round_stamina_reduction: float = 0.0
var round_financial_drain: float = 0.0
var round_gestation_speed_bonus: float = 0.0
var round_biomass_bonus: float = 0.0
var round_awareness_bonus: float = 0.0
var round_intervention_bonus: float = 0.0
var round_gestation_reduction: float = 0.0
var task_failure_chance_bonus: float = 0.0
var cancel_intervention: bool = false

# --- Active visual flags ---
var active_visual_flags: Array[int] = []

# --- Game over ---
var is_game_over: bool = false
var pregnancy_won: bool = false


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
	return node in purchased_skills or node in purchased_host_skills


func has_visual_flag(flag: int) -> bool:
	return flag in active_visual_flags
