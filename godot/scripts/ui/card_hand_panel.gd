extends PanelContainer

@onready var hand_container: HBoxContainer = %HandContainer
@onready var phase_label: Label = %PhaseLabel
@onready var budget_label: Label = %BudgetLabel
@onready var submit_button: Button = %SubmitButton

var _game_manager: Node
var _selected_cards: Array[CardData] = []
var _card_buttons: Array[Button] = []


func initialize(game_manager: Node) -> void:
	_game_manager = game_manager
	submit_button.pressed.connect(_on_submit)
	GameEvents.phase_changed.connect(_on_phase_changed)
	GameEvents.game_started.connect(_on_game_started)
	GameEvents.game_ended.connect(func(_w: bool) -> void: _hide_panel())
	_hide_panel()


func _on_game_started() -> void:
	_hide_panel()


func _on_phase_changed(phase: int) -> void:
	var state: GameState = _game_manager.state
	if state == null:
		_hide_panel()
		return

	if phase == Enums.RoundPhase.PREGNANCY_TURN and state.pregnancy_player_type == Enums.PlayerType.HUMAN:
		_show_hand(Enums.CardSide.PREGNANCY)
	elif phase == Enums.RoundPhase.HOST_TURN and state.host_player_type == Enums.PlayerType.HUMAN:
		_show_hand(Enums.CardSide.HOST)
	else:
		_hide_panel()


func _show_hand(side: int) -> void:
	_selected_cards.clear()
	_card_buttons.clear()

	for child in hand_container.get_children():
		child.queue_free()

	var state: GameState = _game_manager.state
	var deck: Deck
	var budget: float
	var side_label: String

	if side == Enums.CardSide.PREGNANCY:
		deck = state.pregnancy_deck
		budget = state.biomass
		side_label = "GESTATION"
	else:
		deck = state.host_deck
		budget = state.awareness
		side_label = "HOST"

	phase_label.text = "%s TURN — Select up to %d cards" % [side_label, GameConstants.MAX_CARDS_PER_TURN]
	budget_label.text = "BUDGET: %.0f" % budget
	submit_button.text = "SUBMIT TURN"

	for card in deck.hand:
		var btn := Button.new()
		btn.custom_minimum_size = Vector2(150, 80)
		btn.text = "%s\nCost: %.0f" % [card.card_name, card.play_cost]
		btn.tooltip_text = card.description
		btn.toggle_mode = true
		btn.toggled.connect(_on_card_toggled.bind(card, btn))
		hand_container.add_child(btn)
		_card_buttons.append(btn)

	_update_submit_state()
	show()


func _on_card_toggled(toggled_on: bool, card: CardData, btn: Button) -> void:
	if toggled_on:
		if _selected_cards.size() >= GameConstants.MAX_CARDS_PER_TURN:
			btn.button_pressed = false
			return
		_selected_cards.append(card)
	else:
		_selected_cards.erase(card)
	_update_submit_state()


func _update_submit_state() -> void:
	var total_cost := 0.0
	for card in _selected_cards:
		total_cost += card.play_cost

	var state: GameState = _game_manager.state
	var budget := 0.0
	if state.current_phase == Enums.RoundPhase.PREGNANCY_TURN:
		budget = state.biomass
	else:
		budget = state.awareness

	var over_budget := total_cost > budget
	submit_button.disabled = over_budget
	if _selected_cards.is_empty():
		submit_button.text = "PASS TURN"
	elif over_budget:
		submit_button.text = "OVER BUDGET (%.0f / %.0f)" % [total_cost, budget]
	else:
		submit_button.text = "SUBMIT %d CARD(S) — COST: %.0f" % [_selected_cards.size(), total_cost]


func _on_submit() -> void:
	var state: GameState = _game_manager.state
	var cards: Array[CardData] = _selected_cards.duplicate()
	_hide_panel()

	if state.current_phase == Enums.RoundPhase.PREGNANCY_TURN:
		_game_manager.pregnancy_submit_cards(cards)
	elif state.current_phase == Enums.RoundPhase.HOST_TURN:
		_game_manager.host_submit_cards(cards)


func _hide_panel() -> void:
	_selected_cards.clear()
	_card_buttons.clear()
	hide()
