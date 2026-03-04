extends Node

# --- Round flow ---
signal round_started(round_number: int)
signal round_ended(round_number: int)
signal phase_changed(phase: int)

# --- Game flow ---
signal game_started
signal game_ended(pregnancy_wins: bool)

# --- Turn management ---
signal turn_submitted(player_role: int)
signal turn_passed_to(player_role: int)

# --- Cards ---
signal cards_drawn(side: int, count: int)
signal cards_played(side: int, cards: Array)
signal card_effect_resolved(card_name: String, effect_description: String)

# --- Host ---
signal host_state_changed(old_state: int, new_state: int)
signal host_task_resolved(task: int, succeeded: bool)
signal humiliation_changed(value: float)
signal discomfort_changed(value: float)
signal intervention_changed(value: float)
signal social_standing_changed(value: float)

# --- Player resources ---
signal biomass_changed(value: float)
signal awareness_changed(value: float)
signal gestation_changed(value: float)
signal skill_purchased(node: Resource)

# --- Narrative ---
signal event_log_entry(text: String)
