class_name SkillEffectProcessor
extends RefCounted

# NOTE: In the versus/round-based system, per-round effect aggregation is
# handled by CardResolver._apply_skill_effects(). This class is kept for
# any legacy or direct-query use.


func recalculate_effects(state: GameState) -> void:
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

	for node in state.purchased_skills:
		for effect in node.effects:
			_apply_effect(state, effect)


func _apply_effect(state: GameState, effect: Dictionary) -> void:
	if not effect.get("is_per_tick", true):
		return

	var etype: int = effect["type"]
	var mag: float = effect["magnitude"]
	var E := Enums.SkillEffectType

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
		E.RAISE_GESTATION_CAP:
			pass
		E.INCREASE_BIOMASS_FROM_HUMILIATION, E.INCREASE_BIOMASS_FROM_DISCOMFORT, E.PASSIVE_BIOMASS_GENERATION:
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
