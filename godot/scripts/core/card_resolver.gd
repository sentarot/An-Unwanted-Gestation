class_name CardResolver
extends RefCounted


func resolve_round(state: GameState) -> void:
	_reset_round_aggregates(state)

	# 1. Resolve pregnancy cards
	for card in state.pregnancy_cards_played:
		_resolve_pregnancy_card(state, card)

	# 2. Resolve host cards
	for card in state.host_cards_played:
		_resolve_host_card(state, card)

	# 3. Apply persistent skill effects from both sides
	_apply_skill_effects(state)

	# 4. Apply net round results
	_apply_round_results(state)

	# 5. Generate per-round income
	_generate_income(state)

	# 6. Natural decay
	_apply_decay(state)

	# 7. Clear played cards
	state.pregnancy_cards_played.clear()
	state.host_cards_played.clear()


func _reset_round_aggregates(state: GameState) -> void:
	state.round_discomfort = 0.0
	state.round_humiliation = 0.0
	state.round_mobility_reduction = 0.0
	state.round_intellect_reduction = 0.0
	state.round_stamina_reduction = 0.0
	state.round_financial_drain = 0.0
	state.round_gestation_speed_bonus = 0.0
	state.round_biomass_bonus = 0.0
	state.round_awareness_bonus = 0.0
	state.round_intervention_bonus = 0.0
	state.round_gestation_reduction = 0.0
	state.task_failure_chance_bonus = 0.0
	state.cancel_intervention = false
	state.active_visual_flags.clear()


func _resolve_pregnancy_card(state: GameState, card: CardData) -> void:
	var E := Enums.SkillEffectType
	for effect in card.effects:
		var etype: int = effect["type"]
		var mag: float = effect["magnitude"]
		match etype:
			E.ADD_DISCOMFORT:
				state.round_discomfort += mag
			E.ADD_HUMILIATION:
				state.round_humiliation += mag
			E.REDUCE_MOBILITY:
				state.round_mobility_reduction += mag
			E.REDUCE_INTELLECT:
				state.round_intellect_reduction += mag
			E.REDUCE_STAMINA:
				state.round_stamina_reduction += mag
			E.DRAIN_FINANCIAL:
				state.round_financial_drain += mag
			E.INCREASE_GESTATION_SPEED:
				state.round_gestation_speed_bonus += mag
			E.INCREASE_GESTATION_DENSITY:
				state.round_discomfort += mag * 0.5
			E.PASSIVE_BIOMASS_GENERATION, E.INCREASE_BIOMASS_FROM_HUMILIATION, E.INCREASE_BIOMASS_FROM_DISCOMFORT:
				state.round_biomass_bonus += mag
			E.TASK_FAILURE_CHANCE:
				state.task_failure_chance_bonus += mag
			E.CANCEL_INTERVENTION:
				state.cancel_intervention = true
			E.CONVERT_DISCOMFORT_TO_EUPHORIA:
				state.round_discomfort *= (1.0 - mag)
			E.VISUAL_SWEAT, E.VISUAL_WARDROBE_FAILURE, E.VISUAL_ABDOMINAL_UNDULATION, \
			E.VISUAL_GLOWING_VEINS, E.VISUAL_SKIN_TRANSLUCENT, E.VISUAL_PETITIFICATION, \
			E.VISUAL_SPHERICAL_BELLY:
				if etype not in state.active_visual_flags:
					state.active_visual_flags.append(etype)
			E.RAISE_AWARENESS:
				state.round_awareness_bonus += mag

	if card.play_narrative != "":
		GameEvents.event_log_entry.emit(card.play_narrative)
	GameEvents.card_effect_resolved.emit(card.card_name, card.description)


func _resolve_host_card(state: GameState, card: CardData) -> void:
	var E := Enums.SkillEffectType
	for effect in card.effects:
		var etype: int = effect["type"]
		var mag: float = effect["magnitude"]
		match etype:
			E.RAISE_AWARENESS:
				state.round_awareness_bonus += mag
			E.REDUCE_GESTATION:
				state.round_gestation_reduction += mag
			E.BOOST_INTERVENTION:
				state.round_intervention_bonus += mag
			E.REDUCE_BIOMASS:
				state.biomass = maxf(0.0, state.biomass - mag)
				GameEvents.biomass_changed.emit(state.biomass)
			E.SHIELD_AWARENESS:
				state.round_awareness_bonus += mag
			E.ADD_DISCOMFORT:
				state.round_discomfort -= mag
			E.ADD_HUMILIATION:
				state.round_humiliation -= mag

	if card.play_narrative != "":
		GameEvents.event_log_entry.emit(card.play_narrative)
	GameEvents.card_effect_resolved.emit(card.card_name, card.description)


func _apply_skill_effects(state: GameState) -> void:
	var E := Enums.SkillEffectType

	for node in state.purchased_skills:
		for effect in node.effects:
			if not effect.get("is_per_tick", true):
				continue
			var etype: int = effect["type"]
			var mag: float = effect["magnitude"]
			match etype:
				E.ADD_DISCOMFORT:
					state.round_discomfort += mag
				E.ADD_HUMILIATION:
					state.round_humiliation += mag
				E.REDUCE_MOBILITY:
					state.round_mobility_reduction += mag
				E.REDUCE_STAMINA:
					state.round_stamina_reduction += mag
				E.DRAIN_FINANCIAL:
					state.round_financial_drain += mag
				E.INCREASE_GESTATION_SPEED:
					state.round_gestation_speed_bonus += mag
				E.INCREASE_GESTATION_DENSITY:
					state.round_discomfort += mag * 0.5
				E.PASSIVE_BIOMASS_GENERATION, E.INCREASE_BIOMASS_FROM_HUMILIATION, E.INCREASE_BIOMASS_FROM_DISCOMFORT:
					state.round_biomass_bonus += mag
				E.TASK_FAILURE_CHANCE:
					state.task_failure_chance_bonus += mag
				E.CANCEL_INTERVENTION:
					state.cancel_intervention = true
				E.CONVERT_DISCOMFORT_TO_EUPHORIA:
					state.round_discomfort *= (1.0 - mag)
				E.VISUAL_SWEAT, E.VISUAL_WARDROBE_FAILURE, E.VISUAL_ABDOMINAL_UNDULATION, \
				E.VISUAL_GLOWING_VEINS, E.VISUAL_SKIN_TRANSLUCENT, E.VISUAL_PETITIFICATION, \
				E.VISUAL_SPHERICAL_BELLY:
					if etype not in state.active_visual_flags:
						state.active_visual_flags.append(etype)

	for node in state.purchased_host_skills:
		for effect in node.effects:
			if not effect.get("is_per_tick", true):
				continue
			var etype: int = effect["type"]
			var mag: float = effect["magnitude"]
			match etype:
				E.RAISE_AWARENESS:
					state.round_awareness_bonus += mag
				E.BOOST_INTERVENTION:
					state.round_intervention_bonus += mag
				E.REDUCE_GESTATION:
					state.round_gestation_reduction += mag


func _apply_round_results(state: GameState) -> void:
	state.discomfort = clampf(state.discomfort + state.round_discomfort, 0.0, 100.0)
	GameEvents.discomfort_changed.emit(state.discomfort)

	state.humiliation = clampf(state.humiliation + state.round_humiliation, 0.0, 100.0)
	GameEvents.humiliation_changed.emit(state.humiliation)

	state.mobility = clampf(1.0 - state.round_mobility_reduction, 0.0, 1.0)

	state.financial_resources = maxf(0.0, state.financial_resources - state.round_financial_drain)

	if state.round_humiliation > 5.0:
		state.social_standing = maxf(0.0, state.social_standing - state.round_humiliation * 0.1)
		GameEvents.social_standing_changed.emit(state.social_standing)

	var gestation_gain := (GameConstants.BASE_GESTATION_PER_ROUND + state.round_gestation_speed_bonus) \
		* state.gestation_speed_mult \
		- state.round_gestation_reduction
	gestation_gain = maxf(0.0, gestation_gain)
	state.gestation = minf(state.gestation_cap, state.gestation + gestation_gain)
	GameEvents.gestation_changed.emit(state.gestation)

	if not state.cancel_intervention and state.round_intervention_bonus > 0.0:
		state.intervention_meter = minf(100.0, state.intervention_meter + state.round_intervention_bonus)
		GameEvents.intervention_changed.emit(state.intervention_meter)

	state.awareness = maxf(0.0, state.awareness + state.round_awareness_bonus)
	state.peak_awareness = maxf(state.peak_awareness, state.awareness)
	GameEvents.awareness_changed.emit(state.awareness)


func _generate_income(state: GameState) -> void:
	var social_mult := state.social_standing / 100.0
	var from_humiliation := state.humiliation * GameConstants.HUMILIATION_BIOMASS_MULT * social_mult
	var from_discomfort := state.discomfort * GameConstants.DISCOMFORT_BIOMASS_MULT
	var biomass_income := GameConstants.BASE_BIOMASS_PER_ROUND + from_humiliation + from_discomfort + state.round_biomass_bonus
	state.biomass += biomass_income
	GameEvents.biomass_changed.emit(state.biomass)

	var awareness_income := GameConstants.BASE_AWARENESS_PER_ROUND
	state.awareness += awareness_income
	state.peak_awareness = maxf(state.peak_awareness, state.awareness)
	GameEvents.awareness_changed.emit(state.awareness)


func _apply_decay(state: GameState) -> void:
	state.discomfort = maxf(0.0, state.discomfort - GameConstants.DISCOMFORT_NATURAL_DECAY)
	state.humiliation = maxf(0.0, state.humiliation - GameConstants.HUMILIATION_NATURAL_DECAY)
	GameEvents.discomfort_changed.emit(state.discomfort)
	GameEvents.humiliation_changed.emit(state.humiliation)
