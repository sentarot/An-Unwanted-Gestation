extends CanvasLayer

@onready var panel: PanelContainer = %GameOverPanel
@onready var outcome_label: Label = %OutcomeLabel
@onready var summary_label: RichTextLabel = %SummaryLabel
@onready var restart_button: Button = %RestartButton

var _game_manager: Node


func initialize(game_manager: Node) -> void:
	_game_manager = game_manager
	GameEvents.game_ended.connect(_show_result)
	restart_button.pressed.connect(_restart)
	panel.hide()


func _show_result(player_won: bool) -> void:
	panel.show()

	var state: Variant = _game_manager.state

	outcome_label.text = "GESTATION COMPLETE" if player_won else "INTERVENTION SUCCESSFUL"

	summary_label.text = (
		"Host: %s\n" +
		"Payload: %s\n" +
		"Days Survived: %d\n" +
		"Final Gestation: %.1f%%\n" +
		"Final Intervention: %.1f%%\n" +
		"Skills Purchased: %d\n" +
		"Peak Humiliation: %.0f\n" +
		"Peak Discomfort: %.0f"
	) % [
		String(state.selected_host.host_name),
		String(state.selected_class.class_name_text),
		int(state.current_day),
		float(state.gestation),
		float(state.intervention_meter),
		int(state.purchased_skills.size()),
		float(state.humiliation),
		float(state.discomfort)
	]


func _restart() -> void:
	get_tree().reload_current_scene()
