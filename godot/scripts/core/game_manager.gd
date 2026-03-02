extends Node

var state: GameState
var data_factory: DataFactory
var host_ai: HostAIController
var player_ctrl: PlayerController
var skill_tree_mgr: SkillTreeManager
var effect_processor: SkillEffectProcessor
var event_log_gen: EventLogGenerator

var _tick_timer: float = 0.0
var _paused: bool = true
var _speed_multiplier: float = 1.0

var is_paused: bool:
	get: return _paused
var speed_multiplier: float:
	get: return _speed_multiplier


func _ready() -> void:
	data_factory = DataFactory.new()
	add_child(data_factory)
	data_factory.generate()

	host_ai = HostAIController.new()
	player_ctrl = PlayerController.new()
	skill_tree_mgr = SkillTreeManager.new()
	effect_processor = SkillEffectProcessor.new()
	event_log_gen = EventLogGenerator.new()

	# Wire up UI components
	_initialize_ui()


func _initialize_ui() -> void:
	var ui_root := get_node("UIRoot")
	var setup_screen := ui_root.get_node("SetupScreen")
	var dashboard := ui_root.get_node("Dashboard")

	# Setup screen
	setup_screen.initialize(self)

	# Dashboard panels (find by script type via node paths)
	var top_bar := dashboard.get_node("VBoxLayout/TopBar")
	top_bar.initialize(self)

	var speed_controls := dashboard.get_node("VBoxLayout/TopBar/HBox/SpeedControls")
	speed_controls.initialize(self)

	var left_panel := dashboard.get_node("VBoxLayout/MiddleHBox/LeftPanel")
	left_panel.initialize(self)

	var right_panel := dashboard.get_node("VBoxLayout/MiddleHBox/RightPanel")
	right_panel.initialize(self)

	var bottom_bar := dashboard.get_node("VBoxLayout/BottomBar")
	bottom_bar.initialize(self)

	# Game over screen
	var game_over := get_node("GameOverScreen")
	game_over.initialize(self)


func begin_game(host: HostProfile, gestation_class: GestationClassData) -> void:
	state = GameState.new()
	state.selected_host = host
	state.selected_class = gestation_class
	state.initialize_from_selection()

	host_ai.initialize(state)
	player_ctrl.initialize(state)
	skill_tree_mgr.initialize(state, data_factory)
	event_log_gen.initialize(self)

	GameEvents.game_started.emit()
	GameEvents.event_log_entry.emit(
		"Simulation initialized. Host: %s. Payload: %s. Vulnerability: %s." % [
			state.selected_host.host_name,
			state.selected_class.class_name_text,
			Enums.VulnerabilityType.keys()[state.selected_host.vulnerability]
		]
	)

	start_ticking()


func _process(delta: float) -> void:
	if _paused or state == null or state.is_game_over:
		return

	_tick_timer += delta * _speed_multiplier
	if _tick_timer >= GameConstants.SECONDS_PER_TICK:
		_tick_timer -= GameConstants.SECONDS_PER_TICK
		_process_tick()


func start_ticking() -> void:
	_paused = false

func pause() -> void:
	_paused = true

func resume() -> void:
	_paused = false

func set_speed(mult: float) -> void:
	_speed_multiplier = maxf(0.25, mult)


func _process_tick() -> void:
	if state == null or state.is_game_over:
		return

	state.current_tick += 1

	# 1. Recalculate aggregate effects from all purchased skills
	effect_processor.recalculate_effects(state)

	# 2. Generate biomass
	player_ctrl.generate_biomass(state)

	# 3. Advance gestation
	_advance_gestation()

	# 4. Apply per-tick stat damage from skills
	_apply_tick_damage()

	# 5. Run Host AI (state evaluation + task execution)
	host_ai.process_tick(state)

	# 6. Apply natural stat recovery
	_apply_natural_recovery()

	# 7. Check win/lose
	_check_end_conditions()

	# 8. Fire day boundary
	if state.tick_within_day == 0 and state.current_tick > 0:
		GameEvents.day_advanced.emit(state.current_day)
		GameEvents.event_log_entry.emit("--- Day %d ---" % state.current_day)

	GameEvents.tick_end.emit()


func _advance_gestation() -> void:
	var rate := GameConstants.BASE_GESTATION_PER_TICK \
		* state.gestation_speed_mult \
		* (1.0 + state.tick_gestation_speed_bonus)

	state.gestation = minf(state.gestation + rate, state.gestation_cap)
	GameEvents.gestation_changed.emit(state.gestation)


func _apply_tick_damage() -> void:
	var vuln := state.selected_host.vulnerability

	# Discomfort
	var discomfort_delta := state.tick_discomfort
	if vuln == Enums.VulnerabilityType.DISCOMFORT:
		discomfort_delta *= state.selected_host.vulnerability_multiplier
	if discomfort_delta > 0.0:
		state.discomfort = clampf(state.discomfort + discomfort_delta, 0.0, 100.0)
		GameEvents.discomfort_changed.emit(state.discomfort)

	# Humiliation
	var humiliation_delta := state.tick_humiliation
	if vuln == Enums.VulnerabilityType.HUMILIATION:
		humiliation_delta *= state.selected_host.vulnerability_multiplier
	if humiliation_delta > 0.0:
		state.humiliation = clampf(state.humiliation + humiliation_delta, 0.0, 100.0)
		GameEvents.humiliation_changed.emit(state.humiliation)

	# Financial drain
	if state.tick_financial_drain > 0.0:
		state.financial_resources = maxf(0.0, state.financial_resources - state.tick_financial_drain)

	# Mobility
	state.mobility = clampf(1.0 - state.tick_mobility_reduction, 0.0, 1.0)


func _apply_natural_recovery() -> void:
	var phys_recovery := state.physical_resistance * 0.005
	var ment_recovery := state.mental_defense * 0.004

	state.discomfort = maxf(0.0,
		state.discomfort - GameConstants.DISCOMFORT_NATURAL_DECAY * (1.0 + phys_recovery))
	state.humiliation = maxf(0.0,
		state.humiliation - GameConstants.HUMILIATION_NATURAL_DECAY * (1.0 + ment_recovery))

	GameEvents.discomfort_changed.emit(state.discomfort)
	GameEvents.humiliation_changed.emit(state.humiliation)


func _check_end_conditions() -> void:
	if state.gestation >= state.gestation_cap:
		state.is_game_over = true
		state.player_won = true
		pause()
		GameEvents.game_ended.emit(true)
		GameEvents.event_log_entry.emit("=== GESTATION COMPLETE. THE PAYLOAD HAS WON. ===")
	elif state.intervention_meter >= GameConstants.INTERVENTION_LOSE_THRESHOLD:
		state.is_game_over = true
		state.player_won = false
		pause()
		GameEvents.game_ended.emit(false)
		GameEvents.event_log_entry.emit("=== INTERVENTION SUCCESSFUL. THE HOST HAS BEEN CURED. ===")
