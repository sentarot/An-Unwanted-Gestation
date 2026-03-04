extends Control

@onready var host_button_container: VBoxContainer = %HostButtonContainer
@onready var host_name_label: Label = %HostNameLabel
@onready var host_backdrop_label: RichTextLabel = %HostBackdropLabel
@onready var host_stats_label: Label = %HostStatsLabel

@onready var class_button_container: VBoxContainer = %ClassButtonContainer
@onready var class_name_label: Label = %ClassNameLabel
@onready var class_concept_label: RichTextLabel = %ClassConceptLabel

@onready var pregnancy_type_button: Button = %PregnancyTypeButton
@onready var host_type_button: Button = %HostTypeButton

@onready var start_button: Button = %StartButton

var _selected_host: HostProfile
var _selected_class: GestationClassData
var _game_manager: Node

var _pregnancy_player_type: int = Enums.PlayerType.HUMAN
var _host_player_type: int = Enums.PlayerType.AI


func initialize(game_manager: Node) -> void:
	_game_manager = game_manager
	_build_host_buttons()
	_build_class_buttons()
	start_button.pressed.connect(_on_start_clicked)
	start_button.disabled = true

	pregnancy_type_button.pressed.connect(_toggle_pregnancy_type)
	host_type_button.pressed.connect(_toggle_host_type)
	_update_type_buttons()


func _build_host_buttons() -> void:
	for host in _game_manager.data_factory.hosts:
		var btn := Button.new()
		btn.text = host.host_name
		btn.custom_minimum_size = Vector2(320, 54)
		btn.size_flags_horizontal = Control.SIZE_EXPAND_FILL
		btn.pressed.connect(_select_host.bind(host))
		host_button_container.add_child(btn)


func _build_class_buttons() -> void:
	for cls in _game_manager.data_factory.gestation_classes:
		var btn := Button.new()
		btn.text = cls.class_name_text
		btn.custom_minimum_size = Vector2(320, 54)
		btn.size_flags_horizontal = Control.SIZE_EXPAND_FILL
		btn.pressed.connect(_select_class.bind(cls))
		class_button_container.add_child(btn)


func _select_host(host: HostProfile) -> void:
	_selected_host = host
	host_name_label.text = host.host_name
	host_backdrop_label.text = host.narrative_backdrop
	host_stats_label.text = "PHYS: %d  MENT: %d\nFIN: %d  SOC: %d\nVULNERABILITY: %s" % [
		host.physical_resistance, host.mental_defense,
		host.financial_resources, host.social_standing,
		Enums.VulnerabilityType.keys()[host.vulnerability]
	]
	_update_start_button()


func _select_class(cls: GestationClassData) -> void:
	_selected_class = cls
	class_name_label.text = cls.class_name_text
	class_concept_label.text = "%s\n\n%s" % [cls.concept, cls.physicality]
	_update_start_button()


func _update_start_button() -> void:
	start_button.disabled = _selected_host == null or _selected_class == null


func _toggle_pregnancy_type() -> void:
	if _pregnancy_player_type == Enums.PlayerType.HUMAN:
		_pregnancy_player_type = Enums.PlayerType.AI
	else:
		_pregnancy_player_type = Enums.PlayerType.HUMAN
	_update_type_buttons()


func _toggle_host_type() -> void:
	if _host_player_type == Enums.PlayerType.HUMAN:
		_host_player_type = Enums.PlayerType.AI
	else:
		_host_player_type = Enums.PlayerType.HUMAN
	_update_type_buttons()


func _update_type_buttons() -> void:
	var preg_label := "HUMAN" if _pregnancy_player_type == Enums.PlayerType.HUMAN else "AI"
	pregnancy_type_button.text = "GESTATION: %s" % preg_label

	var host_label := "HUMAN" if _host_player_type == Enums.PlayerType.HUMAN else "AI"
	host_type_button.text = "HOST: %s" % host_label


func _on_start_clicked() -> void:
	if _selected_host == null or _selected_class == null:
		return
	hide()
	get_parent().get_node("Dashboard").show()
	_game_manager.begin_game(_selected_host, _selected_class,
		_pregnancy_player_type, _host_player_type)
