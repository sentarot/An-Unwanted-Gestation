extends Node

# --- Tick ---
signal tick_start
signal tick_end
signal day_advanced(day: int)

# --- Game flow ---
signal game_started
signal game_ended(player_wins: bool)

# --- Host ---
signal host_state_changed(old_state: int, new_state: int)
signal host_task_resolved(task: int, succeeded: bool)
signal humiliation_changed(value: float)
signal discomfort_changed(value: float)
signal intervention_changed(value: float)
signal social_standing_changed(value: float)

# --- Player ---
signal biomass_changed(value: float)
signal gestation_changed(value: float)
signal skill_purchased(node: Resource)

# --- Narrative ---
signal event_log_entry(text: String)
