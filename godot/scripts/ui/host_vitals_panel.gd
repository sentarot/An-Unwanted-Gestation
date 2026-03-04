extends PanelContainer

@onready var host_name_label: Label = %VitalsHostName
@onready var archetype_label: Label = %ArchetypeLabel
@onready var current_task_label: Label = %CurrentTaskLabel
@onready var humiliation_bar: ProgressBar = %HumiliationBar
@onready var humiliation_label: Label = %HumiliationLabel
@onready var discomfort_bar: ProgressBar = %DiscomfortBar
@onready var discomfort_label: Label = %DiscomfortLabel
@onready var social_bar: ProgressBar = %SocialBar
@onready var social_label: Label = %SocialLabel
@onready var state_label: Label = %StateLabel

var _game_manager: Node


func initialize(game_manager: Node) -> void:
	_game_manager = game_manager
	GameEvents.game_started.connect(_on_game_started)
	GameEvents.humiliation_changed.connect(_update_humiliation)
	GameEvents.discomfort_changed.connect(_update_discomfort)
	GameEvents.social_standing_changed.connect(_update_social)
	GameEvents.host_task_resolved.connect(_on_task_resolved)
	GameEvents.host_state_changed.connect(_on_state_changed)


func _on_game_started() -> void:
	var state: Variant = _game_manager.state
	host_name_label.text = String(state.selected_host.host_name)
	archetype_label.text = Enums.HostArchetype.keys()[int(state.selected_host.archetype)]
	_update_humiliation(0.0)
	_update_discomfort(0.0)
	_update_social(float(state.social_standing))


func _update_humiliation(value: float) -> void:
	humiliation_bar.value = value
	humiliation_label.text = "HUMILIATION: %.0f" % value


func _update_discomfort(value: float) -> void:
	discomfort_bar.value = value
	discomfort_label.text = "DISCOMFORT: %.0f" % value


func _update_social(value: float) -> void:
	social_bar.value = value
	social_label.text = "SOCIAL: %.0f" % value


func _on_task_resolved(task: int, succeeded: bool) -> void:
	var status := "SUCCESS" if succeeded else "FAILED"
	var task_name: String = Enums.TaskType.keys()[task].replace("_", " ").capitalize()
	current_task_label.text = "%s — %s" % [task_name, status]
	if succeeded:
		current_task_label.modulate = Color(0.3, 0.95, 0.4)
	else:
		current_task_label.modulate = Color(0.95, 0.3, 0.3)


func _on_state_changed(_old_state: int, new_state: int) -> void:
	match new_state:
		Enums.HostState.ACTIVE:
			state_label.text = "STATUS: ACTIVE"
		Enums.HostState.ISOLATED:
			state_label.text = "STATUS: ISOLATED"
		Enums.HostState.BEDRIDDEN:
			state_label.text = "STATUS: BEDRIDDEN"
