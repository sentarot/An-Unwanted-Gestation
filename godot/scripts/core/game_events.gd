extends Node

# --- Round flow ---
@warning_ignore("unused_signal")
signal round_started(round_number: int)
@warning_ignore("unused_signal")
signal round_ended(round_number: int)
@warning_ignore("unused_signal")
signal phase_changed(phase: int)

# --- Game flow ---
@warning_ignore("unused_signal")
signal game_started
@warning_ignore("unused_signal")
signal game_ended(pregnancy_wins: bool)

# --- Turn management ---
@warning_ignore("unused_signal")
signal turn_submitted(player_role: int)
@warning_ignore("unused_signal")
signal turn_passed_to(player_role: int)

# --- Cards ---
@warning_ignore("unused_signal")
signal cards_drawn(side: int, count: int)
@warning_ignore("unused_signal")
signal cards_played(side: int, cards: Array)
@warning_ignore("unused_signal")
signal card_effect_resolved(card_name: String, effect_description: String)

# --- Host ---
@warning_ignore("unused_signal")
signal host_state_changed(old_state: int, new_state: int)
@warning_ignore("unused_signal")
signal host_task_resolved(task: int, succeeded: bool)
@warning_ignore("unused_signal")
signal humiliation_changed(value: float)
@warning_ignore("unused_signal")
signal discomfort_changed(value: float)
@warning_ignore("unused_signal")
signal intervention_changed(value: float)
@warning_ignore("unused_signal")
signal social_standing_changed(value: float)

# --- Player resources ---
@warning_ignore("unused_signal")
signal biomass_changed(value: float)
@warning_ignore("unused_signal")
signal awareness_changed(value: float)
@warning_ignore("unused_signal")
signal gestation_changed(value: float)
@warning_ignore("unused_signal")
signal skill_purchased(node: Resource)

# --- Narrative ---
@warning_ignore("unused_signal")
signal event_log_entry(text: String)
