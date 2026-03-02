class_name PlayerController
extends RefCounted

var _state: GameState


func initialize(state: GameState) -> void:
	_state = state


func generate_biomass(state: GameState) -> void:
	_state = state

	var social_mult := state.social_standing / 100.0

	var from_humiliation := state.humiliation \
		* GameConstants.HUMILIATION_BIOMASS_MULT \
		* social_mult

	var from_discomfort := state.discomfort \
		* GameConstants.DISCOMFORT_BIOMASS_MULT

	var total := GameConstants.BASE_BIOMASS_PER_TICK \
		+ from_humiliation \
		+ from_discomfort \
		+ state.tick_biomass_bonus

	state.biomass += total
	GameEvents.biomass_changed.emit(state.biomass)
