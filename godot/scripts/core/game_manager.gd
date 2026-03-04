extends Node

var state: GameState
var data_factory: DataFactory
var skill_tree_mgr: SkillTreeManager
var card_resolver: CardResolver
var event_log_gen: EventLogGenerator

# AI controllers (used when a side is AI-controlled)
var pregnancy_ai: PregnancyAI
var host_ai: HostAI


func _ready() -> void:
	data_factory = DataFactory.new()
	add_child(data_factory)
	data_factory.generate()

	skill_tree_mgr = SkillTreeManager.new()
	card_resolver = CardResolver.new()
	event_log_gen = EventLogGenerator.new()
	pregnancy_ai = PregnancyAI.new()
	host_ai = HostAI.new()

	_initialize_ui()


func _initialize_ui() -> void:
	var ui_root := get_node("UIRoot")
	var setup_screen := ui_root.get_node("SetupScreen")
	var dashboard := ui_root.get_node("Dashboard")

	setup_screen.initialize(self)

	var top_bar := dashboard.get_node("VBoxLayout/TopBar")
	top_bar.initialize(self)

	var left_panel := dashboard.get_node("VBoxLayout/MiddleHBox/LeftPanel")
	left_panel.initialize(self)

	var right_panel := dashboard.get_node("VBoxLayout/MiddleHBox/RightPanel")
	right_panel.initialize(self)

	var card_hand := dashboard.get_node("VBoxLayout/CardHandPanel")
	card_hand.initialize(self)

	var bottom_bar := dashboard.get_node("VBoxLayout/BottomBar")
	bottom_bar.initialize(self)

	var game_over := get_node("GameOverScreen")
	game_over.initialize(self)


func begin_game(host: HostProfile, gestation_class: GestationClassData,
		pregnancy_player_type: int = Enums.PlayerType.HUMAN,
		host_player_type: int = Enums.PlayerType.AI) -> void:
	state = GameState.new()
	state.selected_host = host
	state.selected_class = gestation_class
	state.pregnancy_player_type = pregnancy_player_type
	state.host_player_type = host_player_type
	state.initialize_from_selection()

	state.biomass = GameConstants.STARTING_BIOMASS
	state.awareness = GameConstants.STARTING_AWARENESS

	skill_tree_mgr.initialize(state, data_factory)
	pregnancy_ai.initialize(state, skill_tree_mgr)
	host_ai.initialize(state, skill_tree_mgr)
	event_log_gen.initialize(self)

	_build_starting_decks()

	GameEvents.game_started.emit()
	GameEvents.event_log_entry.emit(
		"Versus mode initialized. Host: %s. Payload: %s. Vulnerability: %s." % [
			state.selected_host.host_name,
			state.selected_class.class_name_text,
			Enums.VulnerabilityType.keys()[state.selected_host.vulnerability]
		]
	)

	begin_round()


func _build_starting_decks() -> void:
	for card in data_factory.pregnancy_starter_cards:
		state.pregnancy_deck.add_card(card)
	state.pregnancy_deck.shuffle()

	for card in data_factory.host_starter_cards:
		state.host_deck.add_card(card)
	state.host_deck.shuffle()


# =====================================================================
# ROUND FLOW
# =====================================================================

func begin_round() -> void:
	if state == null or state.is_game_over:
		return

	state.current_round += 1
	state.current_phase = Enums.RoundPhase.DRAW

	GameEvents.round_started.emit(state.current_round)
	GameEvents.event_log_entry.emit("--- Round %d ---" % state.current_round)

	var preg_drawn := state.pregnancy_deck.draw(GameConstants.CARDS_TO_DRAW)
	var host_drawn := state.host_deck.draw(GameConstants.CARDS_TO_DRAW)
	GameEvents.cards_drawn.emit(Enums.CardSide.PREGNANCY, preg_drawn.size())
	GameEvents.cards_drawn.emit(Enums.CardSide.HOST, host_drawn.size())

	_start_pregnancy_turn()


func _start_pregnancy_turn() -> void:
	state.current_phase = Enums.RoundPhase.PREGNANCY_TURN
	GameEvents.phase_changed.emit(state.current_phase)

	if state.pregnancy_player_type == Enums.PlayerType.AI:
		var ai_cards := pregnancy_ai.choose_cards(state)
		pregnancy_submit_cards(ai_cards)


func _start_host_turn() -> void:
	state.current_phase = Enums.RoundPhase.HOST_TURN
	GameEvents.phase_changed.emit(state.current_phase)

	if state.host_player_type == Enums.PlayerType.AI:
		var ai_cards := host_ai.choose_cards(state)
		host_submit_cards(ai_cards)


func pregnancy_submit_cards(cards: Array[CardData]) -> void:
	if state.current_phase != Enums.RoundPhase.PREGNANCY_TURN:
		return

	var total_cost := 0.0
	var valid_cards: Array[CardData] = []
	for card in cards:
		if valid_cards.size() >= GameConstants.MAX_CARDS_PER_TURN:
			break
		if state.biomass >= total_cost + card.play_cost:
			valid_cards.append(card)
			total_cost += card.play_cost

	state.biomass -= total_cost
	GameEvents.biomass_changed.emit(state.biomass)

	for card in valid_cards:
		state.pregnancy_deck.play_card(card)
	state.pregnancy_cards_played = valid_cards

	GameEvents.cards_played.emit(Enums.CardSide.PREGNANCY, valid_cards)
	GameEvents.turn_submitted.emit(Enums.PlayerRole.PREGNANCY)

	_start_host_turn()


func host_submit_cards(cards: Array[CardData]) -> void:
	if state.current_phase != Enums.RoundPhase.HOST_TURN:
		return

	var total_cost := 0.0
	var valid_cards: Array[CardData] = []
	for card in cards:
		if valid_cards.size() >= GameConstants.MAX_CARDS_PER_TURN:
			break
		if state.awareness >= total_cost + card.play_cost:
			valid_cards.append(card)
			total_cost += card.play_cost

	state.awareness -= total_cost
	GameEvents.awareness_changed.emit(state.awareness)

	for card in valid_cards:
		state.host_deck.play_card(card)
	state.host_cards_played = valid_cards

	GameEvents.cards_played.emit(Enums.CardSide.HOST, valid_cards)
	GameEvents.turn_submitted.emit(Enums.PlayerRole.HOST)

	_resolve_round()


func _resolve_round() -> void:
	state.current_phase = Enums.RoundPhase.RESOLUTION
	GameEvents.phase_changed.emit(state.current_phase)

	card_resolver.resolve_round(state)
	_check_end_conditions()

	if not state.is_game_over:
		GameEvents.round_ended.emit(state.current_round)
		begin_round()


func _check_end_conditions() -> void:
	if state.gestation >= state.gestation_cap:
		state.is_game_over = true
		state.pregnancy_won = true
		GameEvents.game_ended.emit(true)
		GameEvents.event_log_entry.emit("=== GESTATION COMPLETE. THE PREGNANCY HAS WON. ===")
	elif state.intervention_meter >= GameConstants.INTERVENTION_LOSE_THRESHOLD:
		state.is_game_over = true
		state.pregnancy_won = false
		GameEvents.game_ended.emit(false)
		GameEvents.event_log_entry.emit("=== INTERVENTION SUCCESSFUL. THE HOST HAS BEEN CURED. ===")
