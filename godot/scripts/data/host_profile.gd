class_name HostProfile
extends Resource

@export var host_name: String = ""
@export var archetype: int = Enums.HostArchetype.UNIVERSITY_ELITE
@export_multiline var narrative_backdrop: String = ""

# Baseline stats (0-100)
@export var physical_resistance: int = 50
@export var mental_defense: int = 50
@export var financial_resources: int = 50
@export var social_standing: int = 50

# Schedule: Array of Dictionaries
# { "task_label": String, "task_type": TaskType, "category": TaskCategory,
#   "base_success_chance": float, "primary_stat": StatAffinity }
@export var daily_schedule: Array[Dictionary] = []

# Vulnerability
@export var vulnerability: int = Enums.VulnerabilityType.HUMILIATION
@export var vulnerability_multiplier: float = 1.5

# AI Behavior
@export var panic_gestation_threshold: float = 30.0
@export var hide_attempt_humiliation_threshold: float = 40.0
@export var intervention_drive: float = 1.0
