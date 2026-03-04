extends PanelContainer

@onready var biomass_label: Label = %BiomassLabel
@onready var gestation_bar: ProgressBar = %GestationBar
@onready var gestation_label: Label = %GestationLabel
@onready var intervention_bar: ProgressBar = %InterventionBar
@onready var intervention_label: Label = %InterventionLabel
@onready var day_label: Label = %DayLabel

var _game_manager: Node


func initialize(game_manager: Node) -> void:
	_game_manager = game_manager
	GameEvents.biomass_changed.connect(_update_biomass)
	GameEvents.gestation_changed.connect(_update_gestation)
	GameEvents.intervention_changed.connect(_update_intervention)
	GameEvents.round_started.connect(_update_round)
	GameEvents.game_started.connect(_on_game_started)


func _on_game_started() -> void:
	var state: Variant = _game_manager.state
	gestation_bar.max_value = float(state.gestation_cap)
	intervention_bar.max_value = GameConstants.INTERVENTION_LOSE_THRESHOLD
	_update_biomass(0.0)
	_update_gestation(0.0)
	_update_intervention(0.0)
	_update_round(0)


func _update_biomass(value: float) -> void:
	biomass_label.text = "BIOMASS: %.1f" % value


func _update_gestation(value: float) -> void:
	gestation_bar.value = value
	gestation_label.text = "GESTATION: %.1f%%" % value
	var ratio: float = 0.0
	if _game_manager.state:
		ratio = value / float(_game_manager.state.gestation_cap)
	var style: Variant = gestation_bar.get("theme_override_styles/fill")
	if style is StyleBoxFlat:
		(style as StyleBoxFlat).bg_color = Color(0.95, 0.15, 0.15) if ratio > 0.75 else Color(0.75, 0.18, 0.30)


func _update_intervention(value: float) -> void:
	intervention_bar.value = value
	intervention_label.text = "INTERVENTION: %.1f%%" % value


func _update_round(round_number: int) -> void:
	day_label.text = "ROUND %d" % round_number
